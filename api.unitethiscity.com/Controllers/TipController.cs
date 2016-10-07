/******************************************************************************
 * Filename: TipController.cs
 * Project:  UTC WebAPI
 * 
 * Description:
 * Controller for support of reading and updating tips
******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using com.unitethiscity.api.Models;

namespace com.unitethiscity.api.Controllers
{
    /// <summary>
    /// Controller for supporting of reading and updating of tips
    /// </summary>
    public class TipController : ApiController
    {
        /// <summary>
        /// Retrieve an account's tip for a location; if no tip is found, a blank one
        /// is returned
        /// </summary>
        /// <param name="token">identify account</param>
        /// <param name="id">identify location</param>
        /// <returns>account's tip for location</returns>
        public LocationTip Get(Guid token, int id)
        {
            WebDBContext db = new WebDBContext();
            int accID = APIToken.IdentifyAccount(db, token);
            if (accID == 0)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Access invalid.  Please login again."));
            }

            // confirm that the location exits
            if (db.TblLocations.Count(target => target.LocID == id) <= 0)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Location not found"));
            }

            // make an empty tip, fill it with any tip the member has already submitted
            LocationTip lt = new LocationTip() { AccId = accID, LocId = id, Signature = "", Text = "", Timestamp = DateTime.Now };
            VwTips rsTip = db.VwTips.SingleOrDefault(target => target.AccID == accID && target.LocID == id);
            if (rsTip != null)
            {
                lt.Text = rsTip.TipText;
                lt.Timestamp = rsTip.TipTS;
                lt.TimestampAsStr = rsTip.TipTS.ToUniversalTime().ToString("yyyy-MM-ddHH:mm:ss");
                lt.Signature = rsTip.AccFName + " " + rsTip.AccLName.Substring(0, 1) + ".";
            }

            Logger.LogActionByLocation("Review-Read", accID, id);

            return lt;

        }

        /// <summary>
        /// Store the account's tip for a location
        /// </summary>
        /// <param name="token">identify account</param>
        /// <param name="lt">tip model</param>
        public void Post(Guid token, LocationTip lt)
        {
            WebDBContext db = new WebDBContext();
            int accID = APIToken.IdentifyAccount(db, token);
            if (accID == 0)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Access invalid.  Please login again."));
            }

            // confirm that the location exits
            if (db.TblLocations.Count(target => target.LocID == lt.LocId) <= 0)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Location not found"));
            }

            // we don't want to deal with null tips
            string tipText = lt.Text ?? "";

            // update an existing tip record if found, or create a new tip
            TblTips rs = db.TblTips.SingleOrDefault(target => target.AccID == accID && target.LocID == lt.LocId);


            // if we got a blank tip - delete it if it exists, don't save one if one doesn't exist
            if (tipText.Length == 0)
            {
                if (rs != null)
                {
                    db.TblTips.DeleteOnSubmit(db.TblTips.Single(target => target.LocID == lt.LocId && target.AccID == accID));
                    db.SubmitChanges();
                }
                return;
            }

            // if a tip doesn't exist, create one
            if (rs == null)
            {
                rs = new TblTips();
                rs.AccID = accID;
                rs.LocID = lt.LocId;
                db.TblTips.InsertOnSubmit(rs);
            }
            // tip always gets the latest text and updated timestamp
            rs.TipText = tipText;
            rs.TipTS = DateTime.Now;
            db.SubmitChanges();
            Logger.LogActionByLocation("Review-Update", accID, lt.LocId);

        }

        /// <summary>
        /// Remove the account's tip for a location.  If no tip is found, just returns, does not throw error.
        /// </summary>
        /// <param name="token">identify account</param>
        /// <param name="id">identify location</param>
        public void Delete(Guid token, int id)
        {
            WebDBContext db = new WebDBContext();
            int accID = APIToken.IdentifyAccount(db, token);
            if (accID == 0)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Access invalid.  Please login again."));
            }

            // confirm that the location exits
            if (db.TblLocations.Count(target => target.LocID == id) <= 0)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Location not found"));
            }

            // only remove if a record exists
            if (db.TblTips.Count(target => target.LocID == id && target.AccID == accID) <= 0)
            {
                return;
            }

            // delete the record
            db.TblTips.DeleteOnSubmit(db.TblTips.Single(target => target.LocID == id && target.AccID == accID));
            db.SubmitChanges();

            Logger.LogActionByLocation("Review-Delete", accID, id);
        }
    }
}
