/******************************************************************************
 * Filename: LocationSummaryController.cs
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
    public class LocationSummaryController : ApiController
    {
        /// <summary>
        /// Get the location summary for all of teh locations.  Only returns
        /// locations for enabled businesses
        /// </summary>
        /// <returns>list of location information</returns>
        public IEnumerable<LocationSummary> GetAllLocationSummaries(Guid token)
        {
            WebDBContext db = new WebDBContext();

            // working list of location summaries to return 
            List<LocationSummary> locations = new List<LocationSummary>();

            // identify the user
            TblAPITokens rsTok = db.TblAPITokens.SingleOrDefault(target => target.TokGuid == token);
            int accID = (rsTok != null) ? rsTok.AccID : 0;

            // identify the period
            int perID = Period.IdentifyPeriod(DateTime.Now);

            // NOTE - we make lists out of the context related information so that we can process it quickly; each list
            // should be relatively small and hold only the information that we end up needing; converting from raw 
            // enumerables using where clauses to lists speeds it up by a factor of 3 working with production data;
            // this is still pretty slow, but avoids schema changes and supports backwards compatibility on the API

            // get the deals for the current period
            IEnumerable<VwDeals> rsDel = db.VwDeals.Where(target => target.PerID == perID).ToList();

            // get the redemptions for this period and user
            IEnumerable<VwRedemptions> rsRed = db.VwRedemptions.Where(target => target.AccID == accID && target.PerID == perID).ToList();

            // get the check ins for today and this user
            DateTime sod = DateTime.Now.Date;
            DateTime eod = sod.AddDays(1);
            IEnumerable<VwCheckIns> rsChk = db.VwCheckIns.Where(target => target.AccID == accID && target.ChkTS >= sod && target.ChkTS < eod).ToList();

            // iterate the active locations
            IEnumerable<VwLocations> rs = db.VwLocations.Where(target => target.BusEnabled == true);
            foreach (VwLocations row in rs)
            {
                VwDeals del = rsDel.SingleOrDefault(target => target.BusID == row.BusID);
                VwRedemptions red = rsRed.SingleOrDefault(target => target.BusID == row.BusID);
                VwCheckIns chk = rsChk.SingleOrDefault(target => target.BusID == row.BusID);
                locations.Add(Factory(row, del, red, chk));
            }

            Logger.LogAction("Location-Summary-List", accID);

            return locations;
        }

        public IEnumerable<LocationSummary> GetAllLocationSummaries()
        {
            return GetAllLocationSummaries(Guid.Empty);
        }

        /// <summary>
        /// Get the location summary with the context information for an associated account
        /// </summary>
        /// <param name="token">identify user</param>
        /// <param name="id">identify location</param>
        /// <returns></returns>
        public LocationSummary Get(Guid token, int id)
        {
            WebDBContext db = new WebDBContext();

            // get the target location
            VwLocations rs = db.VwLocations.SingleOrDefault(target => target.LocID == id);
            if (rs == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            // identify the user
            TblAPITokens rsTok = db.TblAPITokens.SingleOrDefault(target => target.TokGuid == token);
            int accID = (rsTok != null) ? rsTok.AccID : 0;

            // identify the period
            int perID = Period.IdentifyPeriod(DateTime.Now);

            // get the deals
            VwDeals del = db.VwDeals.SingleOrDefault(target => target.BusID == rs.BusID && target.PerID == perID);

            // get the redemptions for this period
            VwRedemptions red = null;
            if (del != null)
            {
                red = db.VwRedemptions.SingleOrDefault(target => target.AccID == accID && target.DelID == del.DelID);
            }

            // get the check ins for today
            DateTime sod = DateTime.Now.Date;
            DateTime eod = sod.AddDays(1);
            VwCheckIns chk = db.VwCheckIns.SingleOrDefault(target => target.AccID == accID && target.LocID == id && target.ChkTS >= sod && target.ChkTS < eod);

            LocationSummary loc = Factory(rs, del, red, chk);

            Logger.LogAction("Location-Summary", accID, loc.BusId);

            return loc;

        }

        /// <summary>
        /// Get the location summary without a member account for context
        /// </summary>
        /// <param name="id"></param>
        /// <returns>location with summary info for context</returns>
        public LocationSummary Get(int id)
        {
            return Get(Guid.Empty, id);
        }

        /// <summary>
        /// Create a location summary from the component database records
        /// </summary>
        /// <param name="rs">location </param>
        /// <param name="del">deal - may be null</param>
        /// <param name="red">redemption - may be null</param>
        /// <param name="chk">check in - may be null</param>
        /// <returns></returns>
        [NonAction]
        protected LocationSummary Factory(VwLocations rs, VwDeals del, VwRedemptions red, VwCheckIns chk)
        {
            LocationSummary loc = new LocationSummary();

            // construct the location info elements
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

            // add the deal information - we have a recordset of all businesses
            if (del != null)
            {
                loc.DealId = del.DelID;
                loc.DealAmount = del.DelAmount;
            }
            else
            {
                loc.DealId = 0;
                loc.DealAmount = 0;
            }

            // add the redemption information
            if (red != null)
            {
                loc.MyIsRedeemed = true;
                loc.MyRedeemDate = red.RedTS.ToShortDateString();
            }
            else
            {
                // the deal has not been redeeemed
                loc.MyIsRedeemed = false;
                loc.MyRedeemDate = "";
            }

            // see if the member has already checked in today
            if (chk != null)
            {
                loc.MyIsCheckedIn = true;
                loc.MyCheckInTime = chk.ChkTS.ToShortTimeString();
            }
            else
            {
                loc.MyIsCheckedIn = false;
                loc.MyCheckInTime = "";
            }

            return loc;
        }

        /// <summary>
        /// Get the properties for a location as an array of strings
        /// </summary>
        /// <param name="loc">identify location</param>
        /// <returns>array of string of properties</returns>
        [NonAction]
        protected List<string> GetProperties(LocationSummary loc)
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
            loc.Entertainer = (db.TblEntertainers.Count(target => target.BusID == loc.BusId) > 0);

            return ret;
        }
    }
}
