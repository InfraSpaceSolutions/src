/******************************************************************************
 * Filename: StatTipController.cs
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
    public class StatTipController : ApiController
    {
        /// <summary>
        /// Get the location summary for all of teh locations.  Only returns
        /// locations for enabled businesses
        /// </summary>
        /// <returns>list of location information</returns>
        public IEnumerable<StatTip> Get(Guid token, int id)
        {
            WebDBContext db = new WebDBContext();
            List<StatTip> tips = new List<StatTip>();

            int accID = APIToken.IdentifyAccount(db, token);
            if (accID == 0)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Access invalid.  Please login again."));
            }

            IEnumerable<VwTips> rs = db.VwTips.Where(target => target.BusID == id).OrderByDescending(target => target.TipTS);
            foreach (VwTips row in rs)
            {
                tips.Add(Factory(row));
            }

            Logger.LogAction("Stats-Reviews", accID, 0);
            return tips;
        }


        [NonAction]
        protected StatTip Factory(VwTips rs)
        {
            StatTip obj = new StatTip();
            obj.Id = rs.TipID;
            obj.BusId = rs.BusID;
            // need to get the business guid for showing the icon
            obj.BusGuid = Guid.Empty;
            obj.Name = StatUtility.FormatMemberName(rs.AccFName, rs.AccLName);
            obj.Timestamp = rs.TipTS;
            obj.TimestampAsString = StatUtility.FormatDisplayTimestamp(rs.TipTS);
            obj.TimestampSortable = StatUtility.FormatSortableTimestamp(rs.TipTS);
            obj.TipText = rs.TipText;
            obj.LocationName = rs.LocName;
            return obj;
        }
    }
}
