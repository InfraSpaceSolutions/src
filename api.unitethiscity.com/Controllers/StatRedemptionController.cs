/******************************************************************************
 * Filename: StatRedemptionController.cs
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
    /// Get the redemptions for a business
    /// </summary>
    public class StatRedemptionController : ApiController
    {
        /// <summary>
        /// Get the redemptions for a business
        /// </summary>
        /// <param name="token">access token</param>
        /// <param name="id">business</param>
        /// <returns></returns>
        public IEnumerable<StatRedemption> Get(Guid token, int id)
        {
            WebDBContext db = new WebDBContext();
            List<StatRedemption> redemptions = new List<StatRedemption>();

            int accID = APIToken.IdentifyAccount(db, token);
            if (accID == 0)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Access invalid.  Please login again."));
            }

            IEnumerable<VwRedemptions> rs = db.VwRedemptions.Where(target => target.BusID == id).OrderByDescending(target=>target.RedTS);
            foreach (VwRedemptions row in rs)
            {
                redemptions.Add(Factory(row));
            }

            Logger.LogAction("Stats-SocialRedemptions", accID, 0);

            return redemptions;
        }

        /// <summary>
        /// Get the redemptions in a scoped timeframe
        /// </summary>
        /// <param name="token">access</param>
        /// <param name="id">business</param>
        /// <param name="scopeid">1=today, 2=past week, 3=this period, 4=last period, 5=all-time; defaults to all-time</param>
        /// <returns></returns>
        public IEnumerable<StatRedemption> GetScopedRedemptions(Guid token, int id, int scopeid)
        {
            WebDBContext db = new WebDBContext();
            List<StatRedemption> redemptions = new List<StatRedemption>();

            int accID = APIToken.IdentifyAccount(db, token);
            if (accID == 0)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Access invalid.  Please login again."));
            }

            DateTime startTS = DateTime.Now;
            DateTime endTS = DateTime.Now;
            StatUtility.ScopeTimestamps(scopeid, out startTS, out endTS);

            IEnumerable<VwRedemptions> rs = db.VwRedemptions.Where(target => target.BusID == id && target.RedTS >= startTS && target.RedTS < endTS).OrderByDescending(target => target.RedTS);
            foreach (VwRedemptions row in rs)
            {
                redemptions.Add(Factory(row));
            }

            Logger.LogAction("Stats-ScopedRedemptions", accID, id);

            return redemptions;
        }

        /// <summary>
        /// Get the redemptions in a scoped timeframe for a single account
        /// </summary>
        /// <param name="token">access</param>
        /// <param name="id">business</param>
        /// <param name="accID">account</param>
        /// <param name="scopeid">1=today, 2=past week, 3=this period, 4=last period, 5=all-time; defaults to all-time</param>
        /// <returns></returns>
        public IEnumerable<StatRedemption> GetUserScopedRedemptions(Guid token, int id, int accID, int scopeid)
        {
            WebDBContext db = new WebDBContext();
            List<StatRedemption> redemptions = new List<StatRedemption>();

            int usrID = APIToken.IdentifyAccount(db, token);
            if (usrID == 0)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Access invalid.  Please login again."));
            }

            DateTime startTS = DateTime.Now;
            DateTime endTS = DateTime.Now;
            StatUtility.ScopeTimestamps(scopeid, out startTS, out endTS);

            IEnumerable<VwRedemptions> rs = db.VwRedemptions.Where(target => target.BusID == id && target.AccID == accID && target.RedTS >= startTS && target.RedTS < endTS).OrderByDescending(target => target.RedTS);
            foreach (VwRedemptions row in rs)
            {
                redemptions.Add(Factory(row));
            }

            Logger.LogAction("Stats-UserScopedRedemptions", usrID, id);

            return redemptions;
        }

        [NonAction]
        protected StatRedemption Factory(VwRedemptions rs)
        {
            StatRedemption obj = new StatRedemption();
            obj.Id = rs.RedID;
            obj.BusId = rs.BusID;
            obj.BusGuid = rs.BusGuid ?? Guid.Empty;
            obj.Name = StatUtility.FormatMemberName(rs.AccFName, rs.AccLName);
            obj.Period = rs.PerName;
            obj.Timestamp = rs.RedTS;
            obj.TimestampAsString = StatUtility.FormatDisplayTimestamp(rs.RedTS);
            obj.TimestampSortable = StatUtility.FormatSortableTimestamp(rs.RedTS);
            obj.Pin = rs.PinName ?? "N/A";
            obj.Deal = rs.DelName;
            obj.Amount = rs.DelAmount;
            return obj;
        }
    }
}
