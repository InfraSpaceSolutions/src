/******************************************************************************
 * Filename: EventContext.cs
 * Project:  UTC WebAPI
 * 
 * Description:
 * POCO for representing model of user credentials combined with an event
 * to support the detail view
******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.unitethiscity.api.Models
{
    public class EventContext
    {
        // eventinfo components
        public int Id { get; set; }
        public int EttId { get; set; }
        public int BusId { get; set; }
        public Guid BusGuid { get; set; }
        public int CitId { get; set; }
        public string BusName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string DateAsString { get; set; }
        public string SortableDate { get; set; }
        public string Summary { get; set; }
        public string EventType { get; set; }
        public int CatId { get; set; }
        public string CatName { get; set; }
        public List<string> Properties;

        // additional event fields in context
        public string Body { get; set; }

        // business details
        public string BusinessSummary { get; set; }
        public string Website { get; set; }
        public string FacebookLink { get; set; }
        public bool RequiresPIN { get; set; }
        public List<int> Locations;

        // active deal information
        public int DealId { get; set; }
        public decimal DealAmount { get; set; }
        public string CustomTerms { get; set; }

        // member context
        public int AccId { get; set; }
        public bool MyIsFavorite { get; set; }
        public bool MyIsRedeemed { get; set; }
    }
}