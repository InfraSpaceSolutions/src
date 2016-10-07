/******************************************************************************
 * Filename: LocationInfoController.cs
 * Project:  UTC WebAPI
 * 
 * Description:
 * Get the summary information for one or more locations
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
    /// Get the summary information for one or more locations
    /// </summary>
    public class LocationInfoController : ApiController
    {
        /// <summary>
        /// Get the location summary for all of teh locations.  Only returns
        /// locations for enabled businesses
        /// </summary>
        /// <returns>list of location information</returns>
        public IEnumerable<LocationInfo> GetAllLocationInfos()
        {
            WebDBContext db = new WebDBContext();
            List<LocationInfo> locations = new List<LocationInfo>();

            IEnumerable<VwLocations> rs = db.VwLocations.Where(target=>target.BusEnabled == true);
            foreach (VwLocations row in rs)
            {
                locations.Add(Factory(row));
            }
            Logger.LogAction("Location-List");
            return locations;
        }

        /// <summary>
        /// Get the summary for a specific location.  Works if the location is enabled or not
        /// </summary>
        /// <param name="id">identify location</param>
        /// <returns>location summary</returns>
        public LocationInfo GetLocationInfo(int id)
        {
            WebDBContext db = new WebDBContext();
            VwLocations rs = db.VwLocations.SingleOrDefault(target => target.LocID == id);
            if (rs == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            LocationInfo loc = Factory(rs);

            return Factory(rs);
        }

        /// <summary>
        /// Create a location info object from the database record for the location
        /// </summary>
        /// <param name="rs">locations view record</param>
        /// <returns>location info object</returns>
        [NonAction]
        protected LocationInfo Factory(VwLocations rs)
        {
            LocationInfo loc = new LocationInfo();
            loc.Id = rs.LocID;
            loc.BusId = rs.BusID;
            loc.BusGuid = rs.BusGuid;
            loc.CitId = rs.CitID;
            loc.Name = rs.BusName;
            loc.Address = rs.LocAddress + ", " + rs.LocCity + ", " + rs.LocState + " " + rs.LocZip;
            loc.Rating = rs.LocRating;
            loc.Latitude = rs.LocLatitude;
            loc.Longitude = rs.LocLongitude;
            loc.CatId = rs.CatID;
            loc.CatName = rs.CatName;
            loc.Properties = GetProperties(loc);
            return loc;
        }

        /// <summary>
        /// Get the properties for a location as an array of strings
        /// </summary>
        /// <param name="loc">identify location</param>
        /// <returns>array of string of properties</returns>
        [NonAction]
        protected List<string> GetProperties(LocationInfo loc)
        {
            List<string> ret = new List<string>();

            WebDBContext db = new WebDBContext();

            // get the business properties and add to the tag string
            var rsBusProp = from prp in db.VwBusinessProperties
                            where prp.BusID == loc.BusId
                            orderby prp.PrpName
                            select prp.PrpName;
            ret.AddRange(rsBusProp);
            // get the location properties and add to the tag string
            var rsLocProp = from prp in db.VwLocationProperties
                            where prp.LocID == loc.Id
                            orderby prp.PrpName
                            select prp.PrpName;
            ret.AddRange(rsLocProp);

            // identify this location as an entertainer
            loc.Entertainer = (db.TblEntertainers.Count( target => target.BusID == loc.BusId) > 0);

            return ret;
        }
    }
}
