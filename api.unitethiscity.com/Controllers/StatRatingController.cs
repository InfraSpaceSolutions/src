/******************************************************************************
 * Filename: StatRatingController.cs
 * Project:  UTC WebAPI
 * 
 * Description:
 * Get statistics for a business
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
    public class StatRatingController : ApiController
    {
        /// <summary>
        /// Get the location summary for all of teh locations.  Only returns
        /// locations for enabled businesses
        /// </summary>
        /// <returns>list of location information</returns>
        public IEnumerable<StatRating> Get(Guid token, int id)
        {
            WebDBContext db = new WebDBContext();
            List<StatRating> ratings = new List<StatRating>();

            int accID = APIToken.IdentifyAccount(db, token);
            if (accID == 0)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Access invalid.  Please login again."));
            }

            IEnumerable<VwRatings> rs = db.VwRatings.Where(target => target.BusID == id).OrderByDescending(target => target.RatTS);
            foreach (VwRatings row in rs)
            {
                ratings.Add(Factory(row));
            }

            Logger.LogAction("Stats-Ratings", accID, 0);

            return ratings;
        }


        [NonAction]
        protected StatRating Factory(VwRatings rs)
        {
            StatRating obj = new StatRating();
            obj.Id = rs.RatID;
            obj.BusId = rs.BusID;
            // need to get the business guid for showing the icon
            obj.BusGuid = Guid.Empty;
            obj.Name = StatUtility.FormatMemberName(rs.AccFName, rs.AccLName);
            obj.Timestamp = rs.RatTS;
            obj.TimestampAsString = StatUtility.FormatDisplayTimestamp(rs.RatTS);
            obj.TimestampSortable = StatUtility.FormatSortableTimestamp(rs.RatTS);
            obj.Rating = rs.RatRating;
            obj.LocationName = rs.LocName;
            return obj;
        }
    }
}
