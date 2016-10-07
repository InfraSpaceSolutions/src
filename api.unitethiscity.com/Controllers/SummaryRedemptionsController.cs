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
    public class SummaryRedemptionsController : ApiController
    {
        protected WebDBContext db;

        /// <summary>
        /// Get the summary redemptions for a business and timeframe
        /// </summary>
        /// <param name="token">user token</param>
        /// <param name="id">business</param>
        /// <param name="scopeid">1=today, 2=past week, 3=this period, 4=last period, 5=all-time; defaults to all-time</param>
        /// <returns></returns>
        public IEnumerable<SummaryRedemption> Get(Guid token, int id, int scopeid)
        {
            db = new WebDBContext();

            int accID = APIToken.IdentifyAccount(db, token);
            if (accID == 0)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Access invalid.  Please login again."));
            }


            DateTime startTS = DateTime.Now;
            DateTime endTS = DateTime.Now;
            StatUtility.ScopeTimestamps(scopeid, out startTS, out endTS);

            List<SummaryRedemption> results = new List<SummaryRedemption>();
            IQueryable<VwRedemptions> rs = db.VwRedemptions.Where(target => target.BusID == id && target.RedTS >= startTS && target.RedTS < endTS);
            IQueryable<VwRedemptions> accounts =  rs.GroupBy(target => target.AccID).Select(grp => grp.First()).OrderBy(target=>target.AccFName).ThenBy(target=>target.AccLName);
            foreach (VwRedemptions acc in accounts)
            {
                SummaryRedemption obj = new SummaryRedemption();
                obj.AccID = acc.AccID;
                obj.BusID = id;
                IQueryable<VwRedemptions> rsSummary = rs.Where(target => target.AccID == acc.AccID);
                obj.Name = StatUtility.FormatMemberName(acc.AccFName, acc.AccLName);
                obj.Count = rsSummary.Count();
                obj.Sum = rsSummary.Sum(target => target.DelAmount);
                results.Add(obj);
            }
            db.Dispose();
            db = null;

            Logger.LogAction("Summary-Redemptions", accID);

            return results;
        }
    }
}
