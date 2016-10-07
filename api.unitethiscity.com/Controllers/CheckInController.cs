/******************************************************************************
 * Filename: CheckInController.cs
 * Project:  UTC WebAPI
 * 
 * Description:
 * Controller for performing checkins
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
    /// Processing of member checkin requests
    /// </summary>
    public class CheckInController: ApiController
    {
        WebDBContext db;

        /// <summary>
        /// Verify the checkin credentials and attempt a checking.  Throws an 
        /// error with message if checkin fails; returns 200/204 on success
        /// </summary>
        /// <param name="token">api token identifying caller</param>
        /// <param name="cc">checkin credentials model</param>
        public void Post(Guid token, CheckInCredentials cc)
        {
            db = new WebDBContext();
            int accID = APIToken.IdentifyAccount(db, token);
            if (accID == 0)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Access invalid.  Please login again."));
            }

            // confirm that the location exists (HARD ERROR)
            if (db.TblLocations.Count(target => target.LocID == cc.LocId) <= 0)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Location not found"));
            }

            // make sure that the requesting account can redeem this business/member combination (HARD ERROR)
            if (accID != cc.AccId)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Requesting account mismatch"));
            }

            // make sure that the member account is the same as the requesting account (HARD ERROR)
            // Member scanned the code and submitted - make sure accid is the member id
            if ((accID != cc.MemberAccId) && (cc.RolId == (int)Roles.Member))
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Requesting member mismatch"));
            }

            // if this is a business member attempting to check in - update the credentials by parsing the context
            if (cc.RolId == (int)Roles.Business)
            {
                // get the member by parsing the qr code
                cc.MemberAccId = IdentifyMember(cc);
            }

            // see if the member has already checked in today
            DateTime sod = DateTime.Now.Date;
            DateTime eod = sod.AddDays(1);
            if ( db.TblCheckIns.Count(target => target.AccID == cc.MemberAccId && target.LocID == cc.LocId && target.ChkTS >= sod && target.ChkTS < eod) > 0)
            {
                TblCheckIns rsPrev = db.TblCheckIns.First(target => target.AccID == cc.MemberAccId && target.LocID == cc.LocId && target.ChkTS >= sod && target.ChkTS < eod);
                string message = String.Format("Already checked in today at {0}", rsPrev.ChkTS.ToShortTimeString());
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, message));
            }

            // check the contents of the QURL to confirm that the scan is valid (SOFT)
            switch ((Roles)cc.RolId)
            {
                case Roles.Business:
                    // if the business scanned - check the QURL to identify the member
                    if (!CheckQRForMember(cc))
                    {
                        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid Member QR code"));
                    }
                    break;
                case Roles.Member:
                    // if the member submitted the code, we just accept it as valid if in range
                    if (!ProximityInRange(cc))
                    {
                        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.Forbidden, "Out of Proximity Range"));
                    }
                    break;
                default:
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid QR Role"));
            }

            // create the new check in record
            TblCheckIns rsNew = new TblCheckIns();
            rsNew.AccID = cc.MemberAccId;
            rsNew.LocID = cc.LocId;
            rsNew.PerID = Period.IdentifyPeriod(DateTime.Now);
            rsNew.ChkTS= DateTime.Now;
            db.TblCheckIns.InsertOnSubmit(rsNew);
            db.SubmitChanges();

            Logger.LogActionByLocation("Loyalty", accID, cc.LocId);

        }

        /// <summary>
        /// Check the qr code supplied in the credentials to see if it represents a valid member
        /// </summary>
        /// <param name="rc">credentials</param>
        /// <returns>TRUE if ok for checkin</returns>
        protected bool CheckQRForMember(CheckInCredentials rc)
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
        /// Check the qr code supplied in the credentials to see if it represents a valid business
        /// </summary>
        /// <param name="rc">credentials</param>
        /// <returns>TRUE if ok for checkin</returns>
        protected bool CheckQRForBusiness(CheckInCredentials rc)
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
        /// <param name="cs">credentials</param>
        /// <returns>TRUE if ok for checkin</returns>
        protected bool CheckQRForBusinessReferral(CheckInCredentials cs)
        {
            // get the query string and convert to a name value collection
            int iqs = cs.Qurl.IndexOf('?');
            String qs = (iqs >= 0) ? cs.Qurl.Substring(iqs + 1) : String.Empty;
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
            VwLocations rsLoc = db.VwLocations.Single(target => target.LocID == cs.LocId);
            int busid = rsLoc.BusID;

            // get a matching referral code and business identifier
            return (db.TblReferralCodes.Count(target => target.RfcCode == rfccode && target.BusID == busid && target.RfcAllowCheckin == true) > 0);
        }

        /// <summary>
        /// identify the member from the supplied qr code
        /// </summary>
        /// <param name="cc">credentials supplied for checkin</param>
        /// <returns>account id of member</returns>
        protected int IdentifyMember(CheckInCredentials cc)
        {
            // get the query string and convert to a name value collection
            int iqs = cc.Qurl.IndexOf('?');
            String qs = (iqs >= 0) ? cc.Qurl.Substring(iqs + 1) : String.Empty;
            NameValueCollection qurlqs = HttpUtility.ParseQueryString(qs);

            return Convert.ToInt32(qurlqs.Get("a"));
        }

        /// <summary>
        /// Check to see if the action is within the accepted range for the business
        /// </summary>
        /// <param name="cc">credentials</param>
        /// <returns>true if within proximity range for the location</returns>
        protected bool ProximityInRange(CheckInCredentials cc)
        {
            VwLocations rsLoc = db.VwLocations.Single(target => target.LocID == cc.LocId);
            double distance = Distance.Between(cc.Latitude, cc.Longitude, rsLoc.LocLatitude, rsLoc.LocLongitude);
            Logger.LogAction(String.Format("Distance {0}", distance), 0, 0);
            return (distance < rsLoc.BusProximityRange);
        }
    }
}
