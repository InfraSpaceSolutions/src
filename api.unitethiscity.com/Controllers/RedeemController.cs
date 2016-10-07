/******************************************************************************
 * Filename: RedeemController.cs
 * Project:  UTC WebAPI
 * 
 * Description:
 * Controller for support of account redeem actions
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
    public class RedeemController : ApiController
    {
        WebDBContext db;

        /// <summary>
        /// Process the request to redeem a deal
        /// </summary>
        /// <param name="token">identify account</param>
        /// <param name="rc">credentials for redemption</param>
        public void Post(Guid token, RedeemCredentials rc)
        {
            int pinid = 0; // assume we don't have an associated pin number

            db = new WebDBContext();
            int accID = APIToken.IdentifyAccount(db, token);
            if (accID == 0)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Access invalid.  Please login again."));
            }

            // confirm that the location exits (HARD ERROR)
            if (db.TblLocations.Count(target => target.LocID == rc.LocId) <= 0) 
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Location not found"));
            }

            // make sure that the requesting account can redeem this business/member combination (HARD ERROR)
            if (accID != rc.AccId)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Requesting account mismatch"));
            }

            // make sure that the member account is the same as the requesting account (HARD ERROR)
            // Member scanned the code and submitted - make sure accid is the member id
            if ((accID != rc.MemberAccId) && (rc.RolId == (int)Roles.Member))
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Requesting member mismatch"));
            }

            // if this is a business member attempting to redeem - update the credentials by parsing the context
            if (rc.RolId == (int)Roles.Business)
            {
                // get the active deal if one exists
                rc.DealId = IdentifyActiveDeal(rc.LocId);
                // get the member by parsing the qr code
                rc.MemberAccId = IdentifyMember(rc);
            }

            // make sure that the deal exists (HARD)
            VwDeals rsDeal = db.VwDeals.SingleOrDefault(target => target.DelID == rc.DealId);
            if ( rsDeal == null )
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Deal not found"));
            }

            // see if the member has already redeemed the deal (SOFT)
            TblRedemptions rsPrev = db.TblRedemptions.SingleOrDefault(target => target.AccID == rc.MemberAccId && target.DelID == rc.DealId);
            if (rsPrev != null)
            {
                string message = String.Format("Already redeemed on {0} at {1}", rsPrev.RedTS.ToShortDateString(), rsPrev.RedTS.ToShortTimeString()); 
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, message));
            }

            // make sure that the deal is available in the current period (SOFT)
            int perID = Period.IdentifyPeriod(DateTime.Now);
            if (rsDeal.PerID != perID)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Deal has expired"));
            }

            // check the supplied PIN if it is required (SOFT)
            if ( rsDeal.BusRequirePin == true )
            {
                TblPins rsPin = db.TblPins.SingleOrDefault(target => target.PinNumber == rc.PinNumber && target.BusID == rsDeal.BusID);
                if (rc.PinNumber == null)
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
                // capture the pin identifier
                pinid = rsPin.PinID;
            }

            // check the contents of the QURL to confirm that the scan is valid (SOFT)

            // if the business scanned - check the QURL to identify the member
            switch ((Roles)rc.RolId)
            {
                case Roles.Business:
                    if (!CheckQRForMember(rc))
                    {
                        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid Member QR code"));
                    }
                    break;
                case Roles.Member:
                    Logger.LogActionByLocation("Trace-check-proximity", accID, rc.LocId);
                    // if the member submitted the code, we just accept it as valid if in range
                    if (!ProximityInRange(rc))
                    {
                        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.Forbidden, "Out of Proximity Range"));
                    }
                    break;
                default:
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid QR Role"));
            }

            // create the new redemption record
            TblRedemptions rsNew = new TblRedemptions();
            rsNew.AccID = rc.MemberAccId;
            rsNew.DelID = rc.DealId;
            rsNew.RedTS = DateTime.Now;
            rsNew.PinID = pinid;
            db.TblRedemptions.InsertOnSubmit(rsNew);
            db.SubmitChanges();

            Logger.LogActionByLocation("Social-Redeem", accID, rc.LocId);
        }

        /// <summary>
        /// Check the qr code for identification of the member in the credentials
        /// </summary>
        /// <param name="rc">credentials supplied for redemption</param>
        /// <returns>true if qr code is valid for redeeming per credentials</returns>
        protected bool CheckQRForMember(RedeemCredentials rc)
        {
            // get the query string and convert to a name value collection
            int iqs = rc.Qurl.IndexOf('?');
            String qs = (iqs >= 0) ? rc.Qurl.Substring(iqs + 1) : String.Empty;
            NameValueCollection qurlqs = HttpUtility.ParseQueryString(qs);

            // confirm that the account in the qurl matches the target member identifier
            if (Convert.ToInt32(qurlqs.Get("a")) != rc.MemberAccId)
            {
                return false;
            }
            // calculate the member identifier hash and check against the qurl provided hash
            if (Encryption.CalculateMemberHash(rc.MemberAccId) != qurlqs.Get("h"))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Chec the qr code for identification of the busines in the redemption request
        /// </summary>
        /// <param name="rc">credentials supplied for the redemption</param>
        /// <returns>true if qr code is valid for redeeming per credentials</returns>
        protected bool CheckQRForBusiness(RedeemCredentials rc)
        {
            // get the query string and convert to a name value collection
            int iqs = rc.Qurl.IndexOf('?');
            String qs = (iqs >= 0) ? rc.Qurl.Substring(iqs + 1) : String.Empty;
            NameValueCollection qurlqs = HttpUtility.ParseQueryString(qs);

            // get the business id
            VwLocations rsLoc = db.VwLocations.Single(target => target.LocID == rc.LocId);
            int busid = rsLoc.BusID;
            Guid busguid = rsLoc.BusGuid;

            // confirm that the account in the qurl matches the target business identifier
            if (Convert.ToInt32(qurlqs.Get("b")) != busid)
            {
                return false;
            }

            // calculate the business identifier hash and check against the qurl provided hash
            if (Encryption.CalculateBusinessHash(rc.LocId) != qurlqs.Get("h"))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Check the qr code supplied in the credentials to see if it represents a referral code
        /// that is configured as valid for business operations
        /// </summary>
        /// <param name="rc">credentials</param>
        /// <returns>TRUE if ok for checkin</returns>
        protected bool CheckQRForBusinessReferral(RedeemCredentials rc)
        {
            // get the query string and convert to a name value collection
            int iqs = rc.Qurl.IndexOf('?');
            String qs = (iqs >= 0) ? rc.Qurl.Substring(iqs + 1) : String.Empty;
            NameValueCollection qurlqs = HttpUtility.ParseQueryString(qs);
            // get the referral code from the query string
            string rfccode = qurlqs.Get("code") ?? "";
            rfccode = rfccode.ToLower();
            // if we couldnt get a referral code, give up now
            if (rfccode == "")
            {
                return false;
            }

            // get the business id
            VwLocations rsLoc = db.VwLocations.Single(target => target.LocID == rc.LocId);
            int busid = rsLoc.BusID;

            // get a matching referral code and business identifier
            return (db.TblReferralCodes.Count(target => target.RfcCode == rfccode && target.BusID == busid && target.RfcAllowRedeem == true) > 0);
        }

        /// <summary>
        /// Get the id of the active deal for the specified business
        /// </summary>
        /// <param name="locid">target location</param>
        /// <returns>deal id; 0= no deal</returns>
        protected int IdentifyActiveDeal(int locid)
        {
            int perid = Period.IdentifyPeriod(DateTime.Now);
            TblLocations rsLoc = db.TblLocations.Single(target => target.LocID == locid);
            TblDeals rs = db.TblDeals.SingleOrDefault(target => target.BusID == rsLoc.BusID && target.PerID == perid);
            return (rs != null) ? rs.DelID : 0;
        }

        /// <summary>
        /// identify the member from the supplied qr code
        /// </summary>
        /// <param name="rc">credentials supplied for redemption</param>
        /// <returns>account id of member</returns>
        protected int IdentifyMember(RedeemCredentials rc)
        {
            // get the query string and convert to a name value collection
            int iqs = rc.Qurl.IndexOf('?');
            String qs = (iqs >= 0) ? rc.Qurl.Substring(iqs + 1) : String.Empty;
            NameValueCollection qurlqs = HttpUtility.ParseQueryString(qs);

            return Convert.ToInt32(qurlqs.Get("a"));
        }

        /// <summary>
        /// Check to see if the action is within the accepted range for the business
        /// </summary>
        /// <param name="rc">position in redeem credentials</param>
        /// <returns>true if within proximity range for the location</returns>
        protected bool ProximityInRange(RedeemCredentials rc)
        {
            VwLocations rsLoc = db.VwLocations.Single(target => target.LocID == rc.LocId);
            double distance = Distance.Between(rc.Latitude, rc.Longitude, rsLoc.LocLatitude, rsLoc.LocLongitude);
            return (distance < rsLoc.BusProximityRange);
        }
    }
}
