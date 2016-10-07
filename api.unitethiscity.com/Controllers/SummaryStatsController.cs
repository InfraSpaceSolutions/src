/******************************************************************************
 * Filename: SummaryStatsController.cs
 * Project:  UTC WebAPI
 * 
 * Description:
 * Get the information for a business image gallery
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
    public class SummaryStatsController : ApiController
    {
        protected WebDBContext db;
        protected int busID;

        /// <summary>
        /// Get the summary statistics for a business
        /// </summary>
        /// <param name="token">user token</param>
        /// <param name="id">business identifier</param>
        /// <returns></returns>
        public IEnumerable<SummaryStats> Get(Guid token, int id)
        {
            db = new WebDBContext();
            busID = id;

            int accID = APIToken.IdentifyAccount(db, token);
            if (accID == 0)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Access invalid.  Please login again."));
            }

            IEnumerable<SummaryStats> stats = CalculateSummaryStats();
            db.Dispose();
            db = null;

            Logger.LogAction("Stats-Summary", accID, busID);

            return stats;
        }


        /// <summary>
        /// Count the number of redemptions in a time window
        /// </summary>
        /// <param name="startTS">start time (inclusive)</param>
        /// <param name="endTS">end time (not inclusive)</param>
        /// <returns></returns>
        [NonAction]
        protected int CountRedemptions(DateTime startTS, DateTime endTS)
        {
            return db.VwRedemptions.Count(target => target.BusID == busID && target.RedTS >= startTS && target.RedTS < endTS);
        }

        /// <summary>
        /// Count the number of checkins in a time window
        /// </summary>
        /// <param name="startTS">start time (inclusive)</param>
        /// <param name="endTS">end time (not inclusive)</param>
        /// <returns></returns>
        [NonAction]
        protected int CountCheckins(DateTime startTS, DateTime endTS)
        {
            return db.VwCheckIns.Count(target => target.BusID == busID && target.ChkTS >= startTS && target.ChkTS < endTS);
        }

        /// <summary>
        /// Count the number of favorites in a time window
        /// </summary>
        /// <param name="startTS">start time (inclusive)</param>
        /// <param name="endTS">end time (not inclusive)</param>
        /// <returns></returns>
        [NonAction]
        protected int CountFavorites(DateTime startTS, DateTime endTS)
        {
            return db.VwFavorites.Count(target => target.BusID == busID && target.FavTS >= startTS && target.FavTS < endTS);
        }

        /// <summary>
        /// Count the number of ratings in a time window
        /// </summary>
        /// <param name="startTS">start time (inclusive)</param>
        /// <param name="endTS">end time (not inclusive)</param>
        /// <returns></returns>
        [NonAction]
        protected int CountRatings(DateTime startTS, DateTime endTS)
        {
            return db.VwRatings.Count(target => target.BusID == busID && target.RatTS >= startTS && target.RatTS < endTS);
        }

        /// <summary>
        /// Count the number of tips in a time window
        /// </summary>
        /// <param name="startTS">start time (inclusive)</param>
        /// <param name="endTS">end time (not inclusive)</param>
        /// <returns></returns>
        [NonAction]
        protected int CountTips(DateTime startTS, DateTime endTS)
        {
            return db.VwTips.Count(target => target.BusID == busID && target.TipTS >= startTS && target.TipTS < endTS);
        }

        /// <summary>
        /// Count the number of social posts in a time window
        /// </summary>
        /// <param name="startTS">start time (inclusive)</param>
        /// <param name="endTS">end time (not inclusive)</param>
        /// <returns></returns>
        [NonAction]
        protected int CountSocial(DateTime startTS, DateTime endTS)
        {
            return db.VwSocialPosts.Count(target => target.BusID == busID && target.SopTS >= startTS && target.SopTS < endTS);
        }


        /// <summary>
        /// Generate the summary stats for the business
        /// </summary>
        /// <returns>List of summary stats</returns>
        [NonAction]
        public IEnumerable<SummaryStats> CalculateSummaryStats()
        {

            List<SummaryStats> stats = new List<SummaryStats>();

            SummaryStats calc;

            // create the dates used for queries
            DateTime startOfToday = DateTime.Today;
            DateTime endOfToday = startOfToday.AddDays(1);
            DateTime startOfPastWeek = endOfToday.AddDays(-7);
            DateTime endOfPastWeek = endOfToday;
            DateTime startOfThisMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            DateTime endOfThisMonth = startOfThisMonth.AddMonths(1);
            DateTime startOfLastMonth = startOfThisMonth.AddMonths(-1);
            DateTime endOfLastMonth = startOfLastMonth.AddMonths(1);
            DateTime startOfTime = new DateTime(2000, 1, 1);
            DateTime endOfTime = new DateTime(2100, 1, 1);

            // redemptions
            calc = new SummaryStats();
            calc.Name = "Redemptions";
            calc.Link = "Redemptions";
            calc.Today = CountRedemptions(startOfToday, endOfToday);
            calc.PastWeek = CountRedemptions(startOfPastWeek, endOfPastWeek);
            calc.ThisPeriod = CountRedemptions(startOfThisMonth, endOfThisMonth);
            calc.LastPeriod = CountRedemptions(startOfLastMonth, endOfLastMonth);
            calc.AllTime = CountRedemptions(startOfTime, endOfTime);
            stats.Add(calc);

            // checkins
            calc = new SummaryStats();
            calc.Name = "Check-Ins";
            calc.Link = "CheckIns";
            calc.Today = CountCheckins(startOfToday, endOfToday);
            calc.PastWeek = CountCheckins(startOfPastWeek, endOfPastWeek);
            calc.ThisPeriod = CountCheckins(startOfThisMonth, endOfThisMonth);
            calc.LastPeriod = CountCheckins(startOfLastMonth, endOfLastMonth);
            calc.AllTime = CountCheckins(startOfTime, endOfTime);
            stats.Add(calc);

            // favorites
            calc = new SummaryStats();
            calc.Name = "Favorites";
            calc.Link = "Favorites";
            calc.Today = CountFavorites(startOfToday, endOfToday);
            calc.PastWeek = CountFavorites(startOfPastWeek, endOfPastWeek);
            calc.ThisPeriod = CountFavorites(startOfThisMonth, endOfThisMonth);
            calc.LastPeriod = CountFavorites(startOfLastMonth, endOfLastMonth);
            calc.AllTime = CountFavorites(startOfTime, endOfTime);
            stats.Add(calc);

            // ratings
            calc = new SummaryStats();
            calc.Name = "Ratings";
            calc.Link = "Ratings";
            calc.Today = CountRatings(startOfToday, endOfToday);
            calc.PastWeek = CountRatings(startOfPastWeek, endOfPastWeek);
            calc.ThisPeriod = CountRatings(startOfThisMonth, endOfThisMonth);
            calc.LastPeriod = CountRatings(startOfLastMonth, endOfLastMonth);
            calc.AllTime = CountRatings(startOfTime, endOfTime);
            stats.Add(calc);

            // tips
            calc = new SummaryStats();
            calc.Name = "Tips";
            calc.Link = "Tips";
            calc.Today = CountTips(startOfToday, endOfToday);
            calc.PastWeek = CountTips(startOfPastWeek, endOfPastWeek);
            calc.ThisPeriod = CountTips(startOfThisMonth, endOfThisMonth);
            calc.LastPeriod = CountTips(startOfLastMonth, endOfLastMonth);
            calc.AllTime = CountTips(startOfTime, endOfTime);
            stats.Add(calc);

            // social posts
            calc = new SummaryStats();
            calc.Name = "Social";
            calc.Link = "Social";
            calc.Today = CountSocial(startOfToday, endOfToday);
            calc.PastWeek = CountSocial(startOfPastWeek, endOfPastWeek);
            calc.ThisPeriod = CountSocial(startOfThisMonth, endOfThisMonth);
            calc.LastPeriod = CountSocial(startOfLastMonth, endOfLastMonth);
            calc.AllTime = CountSocial(startOfTime, endOfTime);
            stats.Add(calc);

            return stats;
        }
    }
}
