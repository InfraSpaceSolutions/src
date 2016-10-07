/******************************************************************************
 * Filename: ProximityActionController.cs
 * Project:  UTC WebAPI
 * 
 * Description:
 * Controller for support of proximity actions - checkin/redeem; if no action
 * is specified, it defaults to check-in; uses the location provided and 
 * attempts to do a QR code optional action if allowed
******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using com.unitethiscity.api.Models;
using System.Web;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Text;

namespace com.unitethiscity.api.Controllers
{
    /// <summary>
    /// Controller for support of proximity-based actions
    /// </summary>
    public class ProximityActionController : ApiController
    {
        WebDBContext db;
        ProximityActionResults par = new ProximityActionResults();

        /// <summary>
        /// Process the proximity action request
        /// </summary>
        /// <param name="token">identify account</param>
        /// <param name="pac">credentials for unified action</param>
        public ProximityActionResults Post(Guid token, ProximityActionCredentials pac)
        {
            db = new WebDBContext();

            bool checkedIn = false;
            bool redeemed = false;

            int accID = APIToken.IdentifyAccount(db, token);
            if (accID == 0)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Access invalid.  Please login again."));
            }

            // make sure that the requesting account can redeem this business/member combination (HARD ERROR)
            if (accID != pac.AccId)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Requesting account mismatch"));
            }

            // make sure that the member account is the same as the requesting account (HARD ERROR)
            // Member scanned the code and submitted - make sure accid is the member id
            if ((accID != pac.MemberAccId) && (pac.RolId == (int)Roles.Member))
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Requesting member mismatch"));
            }

            // confirm that the location exits (HARD ERROR)
            VwLocations rsLoc = db.VwLocations.SingleOrDefault(target=>target.LocID == pac.LocId);
            if (rsLoc == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Location not found"));
            }

            // grab the target business to support redemption
            int busID = rsLoc.BusID;

            // identify the deal from the business - use the currently active deal
            int delID = IdentifyActiveDeal(busID);

            // handle explicit request to redeem - with a soft error
            if (pac.RequestRedeem != 0)
            {
                redeemed = PerformRedeem(pac, busID, delID);
            }

            // try to check in as well - with a soft error
            if ((pac.RequestCheckin != 0) && (!redeemed))
            {
                checkedIn = PerformCheckin(pac, busID, pac.LocId);
            }

            // update the results - build all complect fields
            par.AccId = accID;
            par.BusId = busID;
            par.LocId = pac.LocId;
            par.DelId = delID;
            // we must have a location or this will fail
            par.BusName = rsLoc.BusName;
            par.LocAddress = rsLoc.LocAddress + ", " + rsLoc.LocCity + ", " + rsLoc.LocState + " " + rsLoc.LocZip;
            // if we have a deal - send it back - whether redeemed or not
            VwDeals rsDel = db.VwDeals.SingleOrDefault(target => target.DelID == delID);
            par.DealAmount = (rsDel == null) ? 0 : (double)rsDel.DelAmount;

            // send back the flags for what we accomplished
            par.CheckedIn = checkedIn;
            par.Redeemed = redeemed;

            Logger.LogAction("Proximity-Action", accID, busID);

            return par;
        }

        /// <summary>
        /// Get the id of the active deal for the specified business
        /// </summary>
        /// <param name="busid">target business</param>
        /// <returns>deal id; 0= no deal</returns>
        protected int IdentifyActiveDeal(int busid)
        {
            int perid = Period.IdentifyPeriod(DateTime.Now);
            TblDeals rs = db.TblDeals.SingleOrDefault(target => target.BusID == busid && target.PerID == perid);
            return (rs != null) ? rs.DelID : 0;
        }

        /// <summary>
        /// Check the qr code for identification of the busines in the request
        /// </summary>
        /// <param name="busid">business identifier</param>
        /// <param name="pac">credentials supplied for the redemption</param>
        /// <returns>true if qr code is valid for redeeming per credentials</returns>
        protected bool CheckQRForBusiness(int busid, ProximityActionCredentials pac)
        {
            // get the query string and convert to a name value collection
            int iqs = pac.Qurl.IndexOf('?');
            String qs = (iqs >= 0) ? pac.Qurl.Substring(iqs + 1) : String.Empty;
            NameValueCollection qurlqs = HttpUtility.ParseQueryString(qs);

            // get the business information
            TblBusinesses rsBus = db.TblBusinesses.Single(target => target.BusID == busid);
            Guid busguid = rsBus.BusGuid;

            // confirm that the account in the qurl matches the target business identifier
            if (Convert.ToInt32(qurlqs.Get("b")) != busid)
            {
                return false;
            }

            // calculate the business identifier hash and check against the qurl provided hash
            if (Encryption.CalculateBusinessHashByBusID(busid) != qurlqs.Get("h"))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Check the qr code supplied in the credentials to see if it represents a referral code
        /// that is configured as valid for business operations
        /// </summary>
        /// <param name="busid">business identifier</param>
        /// <param name="pac">credentials</param>
        /// <param name="allowCheckin">if valid, can it be used to redeem</param>
        /// <param name="allowRedeem">if valid, can it be used to redeem</param>
        /// <returns>TRUE if ok for checkin</returns>
        protected bool CheckQRForBusinessReferral(int busid, ProximityActionCredentials pac, out bool allowCheckin, out bool allowRedeem)
        {
            // assume that the code isn't valid
            bool valid = false;

            // assume that we can't checkin with this code
            allowCheckin = false;

            // assume that we can't redeem with this code
            allowRedeem = false;

            // get the query string and convert to a name value collection
            int iqs = pac.Qurl.IndexOf('?');
            String qs = (iqs >= 0) ? pac.Qurl.Substring(iqs + 1) : String.Empty;
            NameValueCollection qurlqs = HttpUtility.ParseQueryString(qs);
            // get the referral code from the query string
            string rfccode = qurlqs.Get("code") ?? "";
            rfccode = rfccode.ToLower();
            // if we couldnt get a referral code, give up now
            if (rfccode != "")
            {
                TblReferralCodes rs = db.TblReferralCodes.SingleOrDefault(target => target.RfcCode == rfccode && target.BusID == busid);
                if (rs != null)
                {
                    valid = true;
                    allowCheckin = rs.RfcAllowCheckin;
                    allowRedeem = rs.RfcAllowRedeem;
                }
            }
            return valid;
        }

        /// <summary>
        /// Perform a check-in action; throws error if it fails
        /// </summary>
        /// <param name="pac">credentials</param>
        /// <param name="busID">business</param>
        /// <param name="locID">location</param>
        /// <returns>true if checked in</returns>
        protected bool PerformCheckin(ProximityActionCredentials pac, int busID, int locID)
        {
            // see if the member has already checked in today
            DateTime sod = DateTime.Now.Date;
            DateTime eod = sod.AddDays(1);
            if (db.TblCheckIns.Count(target => target.AccID == pac.MemberAccId && target.LocID == locID && target.ChkTS >= sod && target.ChkTS < eod) > 0)
            {
                TblCheckIns rsPrev = db.TblCheckIns.First(target => target.AccID == pac.MemberAccId && target.LocID == locID && target.ChkTS >= sod && target.ChkTS < eod);
                string message = String.Format("Already checked in today at {0}", rsPrev.ChkTS.ToShortTimeString());
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, message));
            }

            // create the new checkin record
            TblCheckIns rsChk = new TblCheckIns();
            rsChk.AccID = pac.MemberAccId;
            rsChk.LocID = locID;
            rsChk.PerID = Period.IdentifyPeriod(DateTime.Now);
            rsChk.ChkTS = DateTime.Now;
            db.TblCheckIns.InsertOnSubmit(rsChk);
            db.SubmitChanges();

            return true;
        }

        /// <summary>
        /// Perform a redeem action; throws errors if it fails
        /// </summary>
        /// <param name="pac">credentials</param>
        /// <param name="busID">identify business</param>
        /// <param name="delID">identify deal</param>
        /// <returns>true if redeemed</returns>
        protected bool PerformRedeem(ProximityActionCredentials pac, int busID, int delID)
        {
            // need a deal to redeem
            if (delID == 0)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "No active deal to redeem"));
            }

            // see if we have already redeemed the deal
            TblRedemptions rsPrev = db.TblRedemptions.SingleOrDefault(target => target.AccID == pac.MemberAccId && target.DelID == delID);
            if (rsPrev != null)
            {
                string message = String.Format("Already redeemed on {0} at {1}", rsPrev.RedTS.ToShortDateString(), rsPrev.RedTS.ToShortTimeString());
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, message));
            }

            // get the deal record for the redemption
            VwDeals rsDeal = db.VwDeals.Single(target => target.DelID == delID);

            // check the supplied PIN if it is required (SOFT)
            if (rsDeal.BusRequirePin == true)
            {
                TblPins rsPin = db.TblPins.SingleOrDefault(target => target.PinNumber == pac.PinNumber && target.BusID == busID);
                if (pac.PinNumber == null)
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.Forbidden, "PIN required"));
                }
                else if (rsPin == null)
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.Forbidden, "PIN invalid"));
                }
                else if (rsPin.PinEnabled == false)
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.Forbidden, "PIN disabled"));
                }
            }
            // create the new redemption record
            TblRedemptions rsRed = new TblRedemptions();
            rsRed.AccID = pac.MemberAccId;
            rsRed.DelID = delID;
            rsRed.RedTS = DateTime.Now;
            db.TblRedemptions.InsertOnSubmit(rsRed);
            db.SubmitChanges();

            return true;
        }
    }
}
