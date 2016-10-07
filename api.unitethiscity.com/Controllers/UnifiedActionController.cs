/******************************************************************************
 * Filename: UnifiedActionController.cs
 * Project:  UTC WebAPI
 * 
 * Description:
 * Controller for support of unified actions - checkin/redeem; if no action
 * is specified, it defaults to check-in; uses the provided QR code to 
 * identify the target business; selects the closest location
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
    /// Controller for support of account redeem actions
    /// </summary>
    public class UnifiedActionController : ApiController
    {
        WebDBContext db;
        UnifiedActionResults uar = new UnifiedActionResults();

        /// <summary>
        /// Process the unified action request
        /// </summary>
        /// <param name="token">identify account</param>
        /// <param name="uac">credentials for unified action</param>
        public UnifiedActionResults Post(Guid token, UnifiedActionCredentials uac)
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
            if (accID != uac.AccId)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Requesting account mismatch"));
            }

            // make sure that the member account is the same as the requesting account (HARD ERROR)
            // Member scanned the code and submitted - make sure accid is the member id
            if ((accID != uac.MemberAccId) && (uac.RolId == (int)Roles.Member))
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Requesting member mismatch"));
            }

            // identify the business from the QURL
            int busID = IdentifyBusinessFromQR(uac);
            // if we couldn't identify the business - we should just fail now
            if (busID == 0)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid Business QR code"));
            }

            // identify the deal from the business - use the currently active deal
            int delID = IdentifyActiveDeal(busID, uac);

            // find the closest location to the user and apply to it
            int locID = FindBestLocation(busID, uac);
            // if we can't find a location - throw an error - not a valid state
            if (locID == 0)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Unable to localize business"));
            }

            // handle explicit request to redeem
            if (uac.RequestRedeem != 0)
            {
                redeemed = PerformRedeem(uac, busID, delID);
            }
            else // default to attempting a check-in
            {
                checkedIn = PerformCheckin(uac, busID, locID);
            }

            // update the results - build all complect fields
            uar.AccId = accID;
            uar.BusId = busID;
            uar.LocId = locID;
            uar.DelId = delID;
            // we must have a location or this will fail
            VwLocations rsLoc = db.VwLocations.Single(target => target.LocID == locID);
            uar.BusName = rsLoc.BusName;
            uar.LocAddress = rsLoc.LocAddress + ", " + rsLoc.LocCity + ", " + rsLoc.LocState + " " + rsLoc.LocZip;
            // if we have a deal - send it back - whether redeemed or not
            VwDeals rsDel = db.VwDeals.SingleOrDefault(target => target.DelID == delID);
            uar.DealAmount = (rsDel == null) ? 0 : (double)rsDel.DelAmount;

            // send back the flags for what we accomplished
            uar.CheckedIn = checkedIn;
            uar.Redeemed = redeemed;

            Logger.LogAction("Unified-Action", accID, busID);

            return uar;
        }

        /// <summary>
        /// Identify the target business from the QR code that was scanned
        /// </summary>
        /// <param name="uac">unified action credentials</param>
        /// <returns>business id or 0 if not identified</returns>
        protected int IdentifyBusinessFromQR(UnifiedActionCredentials uac)
        {
            // get the query string and convert to a name value collection
            int iqs = uac.Qurl.IndexOf('?');
            String qs = (iqs >= 0) ? uac.Qurl.Substring(iqs + 1) : String.Empty;
            NameValueCollection qurlqs = HttpUtility.ParseQueryString(qs);

            // try using the scan as a business qr code

            // get the business from the query string
            string b = qurlqs.Get("b") ?? "0";
            int busid = Convert.ToInt32(b);
            if (busid == 0)
            {
                // try using the scan as a referral qr code

                // get the referral code from the query string
                string rfccode = qurlqs.Get("code") ?? "";
                rfccode = rfccode.ToLower();
                TblReferralCodes rsRef = db.TblReferralCodes.SingleOrDefault(target => target.RfcCode == rfccode);
                if (rsRef != null)
                {
                    busid = rsRef.BusID;
                }
            }

            return busid;
        }

        /// <summary>
        /// Select a location for the business based on the credentials;
        /// This just finds the nearest location.
        /// </summary>
        /// <param name="busid">target business</param>
        /// <param name="uac">unified action credentials</param>
        /// <returns>location id or 0 if not identified</returns>
        protected int FindBestLocation(int busid, UnifiedActionCredentials uac)
        {
            int locID = 0;
            double distance = Double.MaxValue;

            IEnumerable<TblLocations> rs = db.TblLocations.Where(target => target.BusID == busid);
            foreach (TblLocations row in rs)
            {
                double thisDistance = Distance.Between(uac.Latitude, uac.Longitude, row.LocLatitude, row.LocLongitude);
                if (thisDistance < distance)
                {
                    locID = row.LocID;
                    distance = thisDistance;
                }
            }
            return locID;
        }

        /// <summary>
        /// Get the id of the active deal for the specified business
        /// </summary>
        /// <param name="busid">target business</param>
        /// <param name="uac">unified action credentials</param>
        /// <returns>location id or 0 if not identified</returns>
        protected int IdentifyActiveDeal(int busid, UnifiedActionCredentials uac)
        {
            int perid = Period.IdentifyPeriod(DateTime.Now);
            TblDeals rs = db.TblDeals.SingleOrDefault(target => target.BusID == busid && target.PerID == perid);
            return (rs != null) ? rs.DelID : 0;
        }

        /// <summary>
        /// Check the qr code for identification of the busines in the request
        /// </summary>
        /// <param name="busid">business identifier</param>
        /// <param name="uac">credentials supplied for the redemption</param>
        /// <returns>true if qr code is valid for redeeming per credentials</returns>
        protected bool CheckQRForBusiness(int busid, UnifiedActionCredentials uac)
        {
            // get the query string and convert to a name value collection
            int iqs = uac.Qurl.IndexOf('?');
            String qs = (iqs >= 0) ? uac.Qurl.Substring(iqs + 1) : String.Empty;
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
        /// <param name="uac">credentials</param>
        /// <param name="allowCheckin">if valid, can it be used to redeem</param>
        /// <param name="allowRedeem">if valid, can it be used to redeem</param>
        /// <returns>TRUE if ok for checkin</returns>
        protected bool CheckQRForBusinessReferral(int busid, UnifiedActionCredentials uac, out bool allowCheckin, out bool allowRedeem )
        {
            // assume that the code isn't valid
            bool valid = false;

            // assume that we can't checkin with this code
            allowCheckin = false;

            // assume that we can't redeem with this code
            allowRedeem = false;

            // get the query string and convert to a name value collection
            int iqs = uac.Qurl.IndexOf('?');
            String qs = (iqs >= 0) ? uac.Qurl.Substring(iqs + 1) : String.Empty;
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
        /// <param name="uac">credentials</param>
        /// <param name="busID">business</param>
        /// <param name="locID">location</param>
        /// <returns>true if checked in</returns>
        protected bool PerformCheckin(UnifiedActionCredentials uac, int busID, int locID)
        {
            // make sure that the qr code is valid for check-in
            if (!CheckQRForBusiness(busID, uac))
            {
                bool allowCheckin = false;
                bool allowRedeem = false;
                if (!CheckQRForBusinessReferral(busID, uac, out allowCheckin, out allowRedeem))
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid Business QR code"));
                }
                else
                {
                    if (!allowCheckin)
                    {
                        // if we can't check in with the code - we cant use the code
                        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "QR code not valid for Check-Ins"));
                    }
                }
            }

            // see if the member has already checked in today
            DateTime sod = DateTime.Now.Date;
            DateTime eod = sod.AddDays(1);
            if (db.TblCheckIns.Count(target => target.AccID == uac.MemberAccId && target.LocID == locID && target.ChkTS >= sod && target.ChkTS < eod) > 0)
            {
                TblCheckIns rsPrev = db.TblCheckIns.First(target => target.AccID == uac.MemberAccId && target.LocID == locID && target.ChkTS >= sod && target.ChkTS < eod);
                string message = String.Format("Already checked in today at {0}", rsPrev.ChkTS.ToShortTimeString());
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, message));
            }

            // create the new checkin record
            TblCheckIns rsChk = new TblCheckIns();
            rsChk.AccID = uac.MemberAccId;
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
        /// <param name="uac">credentials</param>
        /// <param name="busID">identify business</param>
        /// <param name="delID">identify deal</param>
        /// <returns>true if redeemed</returns>
        protected bool PerformRedeem(UnifiedActionCredentials uac, int busID, int delID)
        {
            // need a deal to redeem
            if (delID == 0)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "No active deal to redeem"));
            }

            // see if we have already redeemed the deal
            TblRedemptions rsPrev = db.TblRedemptions.SingleOrDefault(target => target.AccID == uac.MemberAccId && target.DelID == delID);
            if (rsPrev != null)
            {
                string message = String.Format("Already redeemed on {0} at {1}", rsPrev.RedTS.ToShortDateString(), rsPrev.RedTS.ToShortTimeString());
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, message));
            }

            // get the deal record for the redemption
            VwDeals rsDeal = db.VwDeals.Single(target => target.DelID == delID);

            // make sure that we can redeem with the QR code
            if (!CheckQRForBusiness(busID, uac))
            {
                bool allowCheckin = false;
                bool allowRedeem = false;
                if (!CheckQRForBusinessReferral(busID, uac, out allowCheckin, out allowRedeem))
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid Business QR code"));
                }
                else
                {
                    if (!allowRedeem)
                    {
                        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "QR code not valid for redeem"));
                    }
                }
            }

            // check the supplied PIN if it is required (SOFT)
            if (rsDeal.BusRequirePin == true)
            {
                TblPins rsPin = db.TblPins.SingleOrDefault(target => target.PinNumber == uac.PinNumber && target.BusID == busID);
                if (uac.PinNumber == null)
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
            rsRed.AccID = uac.MemberAccId;
            rsRed.DelID = delID;
            rsRed.RedTS = DateTime.Now;
            db.TblRedemptions.InsertOnSubmit(rsRed);
            db.SubmitChanges();

            return true;
        }
    }
}
