/******************************************************************************
 * Filename: EventContextController.cs
 * Project:  UTC WebAPI
 * 
 * Description:
 * Controller for retrieving the account's context as a member for a specified
 * event
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
    public class EventContextController : ApiController
    {

        /// <summary>
        /// Get the member context for an event
        /// </summary>
        /// <param name="token">identify account</param>
        /// <param name="id">identify event</param>
        /// <returns>event context for specified user</returns>
        public EventContext Get(Guid token, int id)
        {
            WebDBContext db = new WebDBContext();
            VwEvents rs = db.VwEvents.SingleOrDefault(target => target.EvtID == id);
            if (rs == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            EventContext evt = new EventContext();

            // set the event info properties
            evt.Id = rs.EvtID;
            evt.EttId = rs.EttID;
            evt.BusId = rs.BusID;
            evt.BusGuid = rs.BusGuid;
            evt.CitId = rs.CitID;
            evt.BusName = rs.BusName;
            evt.StartDate = rs.EvtStartDate;
            evt.EndDate = rs.EvtEndDate;
            evt.DateAsString = evt.StartDate.ToShortDateString();
            evt.SortableDate = EventInfo.FormatSortableDate(evt.StartDate);
            evt.Summary = rs.EvtSummary;
            evt.EventType = rs.EttName;
            evt.CatId = rs.CatID;
            evt.CatName = rs.CatName;

            // create the properties array
            IEnumerable<string> rsBusProp = db.VwBusinessProperties.Where(target => target.BusID == evt.BusId).OrderBy(target => target.PrpName).Select(target=>target.PrpName);
            evt.Properties = rsBusProp.ToList();

            // add on the additional event information not provided in the summary info
            evt.Body = rs.EvtBody;

            // add on the detailed business information
            VwBusinesses rsBus = db.VwBusinesses.SingleOrDefault(target => target.BusID == evt.BusId);
            if (rsBus != null)
            {
                evt.BusinessSummary = rsBus.BusSummary;
                evt.Website = rsBus.BusWebsite;
                evt.FacebookLink = rsBus.BusFacebookLink;
                evt.RequiresPIN = rsBus.BusRequirePin;
            }
            else
            {
                evt.BusinessSummary = "";
                evt.Website = "";
                evt.FacebookLink = "";
                evt.RequiresPIN = false;
            }

            // add on the location identifiers
            IEnumerable<int> rsBusLoc = db.TblLocations.Where(target => target.BusID == evt.BusId).OrderBy(target => target.LocID).Select(target => target.LocID);
            evt.Locations = rsBusLoc.ToList();

            // add on any active deal information
            int perID = Period.IdentifyPeriod(DateTime.Now);
            TblDeals rsDel = db.TblDeals.SingleOrDefault(target => target.BusID == evt.BusId && target.PerID == perID);
            if (rsDel != null)
            {
                evt.DealId = rsDel.DelID;
                evt.DealAmount = rsDel.DelAmount;
                evt.CustomTerms = rsDel.DelCustomTerms;
            }
            else
            {
                evt.DealId = 0;
                evt.DealAmount = 0;
                evt.CustomTerms = "";
            }

            // member context specific information
            TblAPITokens rsTok = db.TblAPITokens.SingleOrDefault(target => target.TokGuid == token);
            evt.AccId = (rsTok != null) ? rsTok.AccID : 0;
            // indicate if this is a member's favorite - any location
            evt.MyIsFavorite = (db.VwFavorites.Count(target => target.AccID == evt.AccId && target.BusID == evt.BusId) > 0);
            // indicate if this offer has been redeemed by the member
            evt.MyIsRedeemed = (db.TblRedemptions.Count(target => target.AccID == evt.AccId && target.DelID == evt.DealId) > 0);

            Logger.LogAction("Event-Context", evt.AccId, evt.BusId);

            return evt;
        }

        /// <summary>
        /// Get the event context without an associated member account for context
        /// </summary>
        /// <param name="id">identify event</param>
        /// <returns>event details with context of guest user</returns>
        public EventContext Get(int id)
        {
            return Get(Guid.Empty, id);
        }
    }
}
