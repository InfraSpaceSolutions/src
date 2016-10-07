/******************************************************************************
 * Filename: RatingController.cs
 * Project:  UTC WebAPI
 * 
 * Description:
 * Controller for reading and writing location ratings
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
    /// Controller for reading/writing ratings
    /// </summary>
    public class RatingController : ApiController
    {
        /// <summary>
        /// Get the account's rating for a location
        /// </summary>
        /// <param name="token">identify account</param>
        /// <param name="id">identify location</param>
        /// <returns>Rating 0-5, 0-Unrated, 1-5=location rating for account</returns>
        public int Get(Guid token, int id)
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
            int rating = 0;
            TblRatings rsRat = db.TblRatings.SingleOrDefault(target => target.AccID == accID && target.LocID == id);
            if (rsRat != null)
            {
                rating = rsRat.RatRating;
            }
            Logger.LogActionByLocation("Rating-Read", accID, id);

            return rating;
        }

        /// <summary>
        /// Set the account's rating for a location
        /// </summary>
        /// <param name="token">identify account</param>
        /// <param name="id">identify location</param>
        /// <param name="rating">rating to assign</param>
        public void Post(Guid token, int id, int rating)
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
            // update an existing record if found, or create a new record
            TblRatings rs = db.TblRatings.SingleOrDefault(target => target.AccID == accID && target.LocID == id);
            if (rs == null)
            {
                rs = new TblRatings();
                rs.AccID = accID;
                rs.LocID = id;
                db.TblRatings.InsertOnSubmit(rs);
            }
            // update the record
            rs.RatRating = rating;
            rs.RatTS = DateTime.Now;
            db.SubmitChanges();

            // update the aggregate ratings
            Recalculate(id);

            Logger.LogActionByLocation("Rating-Update", accID, id);
        }

        /// <summary>
        /// Remove an account's rating for a location
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
            if (db.TblRatings.Count(target => target.LocID == id && target.AccID == accID) <= 0)
            {
                return;
            }

            // delete the record
            db.TblRatings.DeleteOnSubmit(db.TblRatings.Single(target => target.LocID == id && target.AccID == accID));
            db.SubmitChanges();

            // update the aggregate ratings
            Recalculate(id);
            Logger.LogActionByLocation("Rating-Delete", accID, id);

        }

        /// <summary>
        /// Recalculate the overall ratings at the business and location levels when a rating is changed
        /// </summary>
        /// <param name="locID">identify location</param>
        [NonAction]
        protected static void Recalculate(int locID)
        {
            WebDBContext db = new WebDBContext();

            // get the target records
            TblLocations rsLoc = db.TblLocations.Single(target => target.LocID == locID);
            TblBusinesses rsBus = db.TblBusinesses.Single(target => target.BusID == rsLoc.BusID);

            // calculate the new values
            int locCount = db.TblRatings.Where(target => target.LocID == locID).Count();
            int busCount = db.VwBusinessRatings.Where(target => target.BusID == rsLoc.BusID).Count();

            double locSum = (locCount > 0) ? db.TblRatings.Where(target => target.LocID == locID).Sum(target => target.RatRating) : 0;
            double busSum = (busCount > 0) ? db.VwBusinessRatings.Where(target => target.BusID == rsLoc.BusID).Sum(target => target.RatRating) : 0;

            // update the ratings and store to database
            rsLoc.LocRating = (locCount > 0) ? locSum / (double)locCount : 0;
            rsBus.BusRating = (busCount > 0) ? busSum / (double)busCount : 0;
            db.SubmitChanges();
            // update the revision for cache support
            DataRevision.Bump(Revisioned.LocationInfo);
        }

    }
}
