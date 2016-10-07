/******************************************************************************
 * Filename: StatFavoriteController.cs
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
    public class StatFavoriteController : ApiController
    {
        /// <summary>
        /// Get the location summary for all of teh locations.  Only returns
        /// locations for enabled businesses
        /// </summary>
        /// <returns>list of location information</returns>
        public IEnumerable<StatFavorite> Get(Guid token, int id)
        {
            WebDBContext db = new WebDBContext();
            List<StatFavorite> favorites = new List<StatFavorite>();

            int accID = APIToken.IdentifyAccount(db, token);
            if (accID == 0)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Access invalid.  Please login again."));
            }

            IEnumerable<VwFavorites> rs = db.VwFavorites.Where(target => target.BusID == id).OrderByDescending(target => target.FavTS);
            foreach (VwFavorites row in rs)
            {
                favorites.Add(Factory(row));
            }

            Logger.LogAction("Stats-Favorites", accID, 0);

            return favorites;
        }


        [NonAction]
        protected StatFavorite Factory(VwFavorites rs)
        {
            StatFavorite obj = new StatFavorite();
            obj.Id = rs.FavID;
            obj.BusId = rs.BusID;
            // need to get the business guid for showing the icon
            obj.BusGuid = Guid.Empty;
            obj.Name = StatUtility.FormatMemberName(rs.AccFName, rs.AccLName);
            obj.Timestamp = rs.FavTS;
            obj.TimestampAsString = StatUtility.FormatDisplayTimestamp(rs.FavTS);
            obj.TimestampSortable = StatUtility.FormatSortableTimestamp(rs.FavTS);
            obj.LocationName = rs.LocName;
            return obj;
        }
    }
}
