/******************************************************************************
 * Filename: FavoriteController.cs
 * Project:  UTC WebAPI
 * 
 * Description:
 * Controller for enumerating and modifying member favorite locations
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
    /// Enumerate and modify member favorites
    /// </summary>
    public class FavoriteController : ApiController
    {
        /// <summary>
        /// Retrieve a sorted list of favorite location identifers for a member
        /// </summary>
        /// <param name="token">user token</param>
        /// <returns>List of location identifiers</returns>
        public IEnumerable<int> GetFavorites(Guid token)
        {
            WebDBContext db = new WebDBContext();
            int accID = APIToken.IdentifyAccount(db, token);
            if (accID == 0)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Access invalid.  Please login again."));
            }
            Logger.LogAction("Favorites-List", accID);
            return db.TblFavorites.Where(target => target.AccID == accID).OrderBy(target => target.LocID).Select(target => target.LocID);
        }

        /// <summary>
        /// Create a member favorite for a location
        /// </summary>
        /// <param name="token">user token</param>
        /// <param name="id">location identifier</param>
        public void Post(Guid token, int id)
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

            // if it is already a favorite - don't do anything
            if (db.TblFavorites.Count(target => target.LocID == id && target.AccID == accID) > 0)
            {
                return;
            }

            // create a favorite record for this location and user pair
            TblFavorites rs = new TblFavorites();
            rs.AccID = accID;
            rs.LocID = id;
            rs.FavTS = DateTime.Now;
            db.TblFavorites.InsertOnSubmit(rs);
            db.SubmitChanges();
            Logger.LogActionByLocation("Favorites-Add", accID, id);
        }

        /// <summary>
        /// Delete a favorite record for this location and user pair
        /// </summary>
        /// <param name="token">user token</param>
        /// <param name="id">location identifier</param>
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

            // if it is not already a favorite - don't do anything
            if (db.TblFavorites.Count(target => target.LocID == id && target.AccID == accID) <= 0)
            {
                return;
            }

            // delete the favorite record
            db.TblFavorites.DeleteOnSubmit(db.TblFavorites.Single(target => target.LocID == id && target.AccID == accID));
            db.SubmitChanges();
            Logger.LogActionByLocation("Favorites-Remove", accID, id);
        }
    }
}
