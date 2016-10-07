/******************************************************************************
 * Filename: LocationContextController.cs
 * Project:  UTC WebAPI
 * 
 * Description:
 * Controller for retrieving the account's context as a member for a specified
 * location
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
    /// Controller for retrieving the account's context as a member for a specified location
    /// </summary>
    public class LocationContextController : ApiController
    {

        /// <summary>
        /// Get the member context for a location
        /// </summary>
        /// <param name="token">identify account</param>
        /// <param name="id">identify location</param>
        /// <returns>location context for specified user</returns>
        public LocationContext Get(Guid token, int id)
        {
            WebDBContext db = new WebDBContext();
            VwLocations rs = db.VwLocations.SingleOrDefault(target => target.LocID == id);
            if (rs == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            LocationContext loc = new LocationContext();

            // set the location info properties
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

            // set the location/business details
            loc.Summary = rs.BusSummary;
            loc.Phone = Phone.Format(rs.LocPhone);
            loc.Website = rs.BusWebsite;
            // restored the facebook link - wonky in IOS v1.1
            loc.FacebookLink = rs.BusFacebookLink;
            loc.FacebookId = rs.BusFacebookID;
            loc.RequiresPIN = rs.BusRequirePin;

            // get the data from the deal for the current period
            int perID = Period.IdentifyPeriod(DateTime.Now);
            TblDeals rsDel = db.TblDeals.SingleOrDefault(target => target.BusID == loc.BusId && target.PerID == perID);
            if (rsDel != null)
            {
                loc.DealId = rsDel.DelID;
                loc.DealAmount = rsDel.DelAmount;
                loc.CustomTerms = rsDel.DelCustomTerms;
                loc.DealDescription = rsDel.DelDescription ?? "";
                loc.DealName = rsDel.DelName ?? "";
            }
            else
            {
                loc.DealId = 0;
                loc.DealAmount = 0;
                loc.CustomTerms = "";
                loc.DealDescription = "";
                loc.DealName = "";
            }

            // member context specific information
            TblAPITokens rsTok = db.TblAPITokens.SingleOrDefault(target => target.TokGuid == token);
            loc.AccId = (rsTok != null) ? rsTok.AccID : 0;
            // just the tip (text)
            TblTips rsTip = db.TblTips.SingleOrDefault(target => target.AccID == loc.AccId && target.LocID == loc.Id);
            loc.MyTip = (rsTip != null ) ? rsTip.TipText : "";
            // this member's rating if exists
            TblRatings rsRat = db.TblRatings.SingleOrDefault(target => target.AccID == loc.AccId && target.LocID == loc.Id);
            loc.MyRating = (rsRat != null ) ? rsRat.RatRating : 0;
            // indicate if this is a member's favorite
            loc.MyIsFavorite = (db.TblFavorites.Count(target => target.AccID == loc.AccId && target.LocID == loc.Id) > 0);

            TblRedemptions rsRed = db.TblRedemptions.SingleOrDefault(target => target.AccID == loc.AccId && target.DelID == loc.DealId);
            if (rsRed != null)
            {
                loc.MyIsRedeemed = true;
                loc.MyRedeemDate = rsRed.RedTS.ToShortDateString() + " " + rsRed.RedTS.ToShortTimeString();
            }
            else
            {
                // the deal has not been redeeemed
                loc.MyIsRedeemed = false;
                loc.MyRedeemDate = "";
            }
            // see if the member has already checked in today
            DateTime sod = DateTime.Now.Date;
            DateTime eod = sod.AddDays(1);
            TblCheckIns rsPrev = db.TblCheckIns.SingleOrDefault(target => target.AccID == loc.AccId && target.LocID == loc.Id && target.ChkTS >= sod && target.ChkTS < eod);
            if (rsPrev != null)
            {
                loc.MyIsCheckedIn = true;
                loc.MyCheckInTime = rsPrev.ChkTS.ToShortTimeString();
            }
            else
            {
                loc.MyIsCheckedIn = false;
                loc.MyCheckInTime = "";
            }

            // add a count of the more info fields so we can determine what to activate
            loc.NumMenuItems = db.TblMenuItems.Count(target => target.BusID == loc.BusId);
            loc.NumGalleryItems = db.TblGalleryItems.Count(target => target.BusID == loc.BusId);
            loc.NumEvents = db.TblEvents.Count(target => target.BusID == loc.BusId && target.EvtEndDate >= DateTime.Today);
            TblMenus rsMenu = db.TblMenus.SingleOrDefault(target => target.BusID == loc.BusId);
            loc.MenuLink = (rsMenu == null) ? "" : rsMenu.MenLink;

            // get the loyalty deal information
            loc.LoyaltySummary = SiteSettings.GetValue("LoyaltyDefaultReward");
            loc.PointsNeeded = WebConvert.ToInt32(SiteSettings.GetValue("LoyaltyDefaultPoints"), 50);
            loc.PointsCollected = db.TblCheckIns.Count(target => target.AccID == loc.AccId && target.LocID == loc.Id);
            TblLoyaltyDeals rsLoyaltyDeal = db.TblLoyaltyDeals.SingleOrDefault(target => target.LocID == loc.Id);
            if (rsLoyaltyDeal != null)
            {
                loc.LoyaltySummary = rsLoyaltyDeal.LoySummary;
                loc.PointsNeeded = rsLoyaltyDeal.LoyPoints;
            }
            Logger.LogAction("Location-Context", loc.AccId, loc.BusId);
            return loc;
        }

        /// <summary>
        /// Get the location context without an associated member account for context
        /// </summary>
        /// <param name="id">identify location</param>
        /// <returns>location details with context of guest user</returns>
        public LocationContext Get(int id)
        {
            return Get(Guid.Empty, id);
        }

        /// <summary>
        /// Get the properties for a location context as an array of strings
        /// </summary>
        /// <param name="loc">location context for properties</param>
        /// <returns>array of strings for properties</returns>
        [NonAction]
        protected List<string> GetProperties(LocationContext loc)
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
