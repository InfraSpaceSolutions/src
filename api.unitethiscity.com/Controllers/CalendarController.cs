/******************************************************************************
 * Filename: CalendarController.cs
 * Project:  UTC WebAPI
 * 
 * Description:
 * Get the summary information for one or more events for a specific business
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
    /// Get the summary information for one or more events
    /// </summary>
    public class CalendarController : ApiController
    {
        /// <summary>
        /// Get the event summary for all of the upcoming events.  Only returns
        /// events for enabled businesses
        /// </summary>
        /// <returns>list of location information</returns>
        public IEnumerable<EventInfo> Get(int id)
        {
            WebDBContext db = new WebDBContext();
            List<EventInfo> events = new List<EventInfo>();

            IEnumerable<VwEventsWithLinks> rs = db.VwEventsWithLinks.Where(target => target.BusEnabled == true && target.EvtEndDate >= DateTime.Today && target.BusID == id).OrderBy( target => target.EvtStartDate);
            foreach (VwEventsWithLinks row in rs)
            {
                events.Add(Factory(row));
            }
            Logger.LogAction("Calendar", 0, id);
            return events;
        }

        /// <summary>
        /// Create an event info object from the database record for the location
        /// </summary>
        /// <param name="rs">event record</param>
        /// <returns>event info object</returns>
        [NonAction]
        protected EventInfo Factory(VwEventsWithLinks rs)
        {
            EventInfo evt = new EventInfo();
            evt.Id = rs.EvtID;
            evt.EttId = rs.EttID;
            evt.BusId = rs.BusID;
            evt.CitId = rs.CitID;
            evt.BusName = rs.BusName;
            evt.BusGuid = rs.BusGuid;
            evt.StartDate = rs.EvtStartDate;
            evt.EndDate = rs.EvtEndDate;
            evt.SortableDate = EventInfo.FormatSortableDate(rs.EvtStartDate);
            evt.DateAsString = rs.EvtStartDate.ToShortDateString();
            if (rs.EvtEndDate > rs.EvtStartDate)
            {
                evt.DateAsString = rs.EvtStartDate.ToString("MM/dd-") + rs.EvtEndDate.ToShortDateString();
            }
            evt.Summary = rs.EvtSummary;
            evt.EventType = rs.EttName;
            evt.CatId = rs.CatID;
            evt.CatName = rs.CatName;
            evt.EventLink = rs.EvtLinkName ?? "";
            evt.Properties = GetProperties(evt);

            return evt;
        }

        /// <summary>
        /// Get the properties for an event as an array of strings
        /// </summary>
        /// <param name="evt">event</param>
        /// <returns>array of string of properties</returns>
        [NonAction]
        protected List<string> GetProperties(EventInfo evt)
        {
            List<string> ret = new List<string>();

            WebDBContext db = new WebDBContext();

            // get the business properties and add to the tag string
            var rsBusProp = from prp in db.VwBusinessProperties
                            where prp.BusID == evt.BusId
                            orderby prp.PrpName
                            select prp.PrpName;
            ret.AddRange(rsBusProp);

            return ret;
        }
    }
}
