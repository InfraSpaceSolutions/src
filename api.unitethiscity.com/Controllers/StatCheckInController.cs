/******************************************************************************
 * Filename: StatCheckIncontroller.cs
 * Project:  UTC WebAPI
 * 
 * Description:
 * Get the statistics
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
    public class StatCheckInController : ApiController
    {
        /// <summary>
        /// Get the location summary for all of teh locations.  Only returns
        /// locations for enabled businesses
        /// </summary>
        /// <returns>list of location information</returns>
        public IEnumerable<StatCheckIn> Get(Guid token, int id)
        {
            WebDBContext db = new WebDBContext();
            List<StatCheckIn> checkIns = new List<StatCheckIn>();

            int accID = APIToken.IdentifyAccount(db, token);
            if (accID == 0)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Access invalid.  Please login again."));
            }

            IEnumerable<VwCheckIns> rs = db.VwCheckIns.Where(target => target.BusID == id).OrderByDescending(target => target.ChkTS);
            foreach (VwCheckIns row in rs)
            {
                checkIns.Add(Factory(row));
            }

            Logger.LogAction("Stats-Loyalty", accID, 0);

            return checkIns;
        }

        /// <summary>
        /// Get the checkins in a scoped timeframe
        /// </summary>
        /// <param name="token">access</param>
        /// <param name="id">business</param>
        /// <param name="scopeid">1=today, 2=past week, 3=this period, 4=last period, 5=all-time; defaults to all-time</param>
        /// <returns></returns>
        public IEnumerable<StatCheckIn> GetScopedCheckIns(Guid token, int id, int scopeid)
        {
            WebDBContext db = new WebDBContext();
            List<StatCheckIn> results = new List<StatCheckIn>();

            int accID = APIToken.IdentifyAccount(db, token);
            if (accID == 0)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Access invalid.  Please login again."));
            }

            DateTime startTS = DateTime.Now;
            DateTime endTS = DateTime.Now;
            StatUtility.ScopeTimestamps(scopeid, out startTS, out endTS);

            IEnumerable<VwCheckIns> rs = db.VwCheckIns.Where(target => target.BusID == id && target.ChkTS >= startTS && target.ChkTS < endTS).OrderByDescending(target => target.ChkTS);
            foreach (VwCheckIns row in rs)
            {
                results.Add(Factory(row));
            }

            Logger.LogAction("Stats-ScopedCheckIns", accID, id);

            return results;
        }

        /// <summary>
        /// Get the redemptions in a scoped timeframe for a single account
        /// </summary>
        /// <param name="token">access</param>
        /// <param name="id">business</param>
        /// <param name="accID">account</param>
        /// <param name="scopeid">1=today, 2=past week, 3=this period, 4=last period, 5=all-time; defaults to all-time</param>
        /// <returns></returns>
        public IEnumerable<StatCheckIn> GetUserScopedRedemptions(Guid token, int id, int accID, int scopeid)
        {
            WebDBContext db = new WebDBContext();
            List<StatCheckIn> results = new List<StatCheckIn>();

            int usrID = APIToken.IdentifyAccount(db, token);
            if (usrID == 0)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Access invalid.  Please login again."));
            }

            DateTime startTS = DateTime.Now;
            DateTime endTS = DateTime.Now;
            StatUtility.ScopeTimestamps(scopeid, out startTS, out endTS);

            IEnumerable<VwCheckIns> rs = db.VwCheckIns.Where(target => target.BusID == id && target.AccID == accID && target.ChkTS >= startTS && target.ChkTS < endTS).OrderByDescending(target => target.ChkTS);
            foreach (VwCheckIns row in rs)
            {
                results.Add(Factory(row));
            }

            Logger.LogAction("Stats-UserScopedCheckIns", usrID, id);

            return results;
        }

        [NonAction]
        protected StatCheckIn Factory(VwCheckIns rs)
        {
            StatCheckIn obj = new StatCheckIn();
            obj.Id = rs.ChkID;
            obj.BusId = rs.BusID;
            obj.Name = StatUtility.FormatMemberName(rs.AccFName, rs.AccLName);
            obj.Period = rs.PerName;
            obj.Timestamp = rs.ChkTS;
            obj.TimestampAsString = StatUtility.FormatDisplayTimestamp(rs.ChkTS);
            obj.TimestampSortable = StatUtility.FormatSortableTimestamp(rs.ChkTS);
            obj.LocationName = rs.LocName;
            return obj;
        }
    }
}
