/******************************************************************************
 * Filename: EventInfo.cs
 * Project:  UTC WebAPI
 * 
 * Description:
 * POCO for representing model of the general information for an eventto
 * support list views.
******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.unitethiscity.api.Models
{
    public class EventInfo
    {
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
        public string CatName{ get; set; }
        public string EventLink { get; set; }

        public List<string> Properties;

        /// <summary>
        /// Format the date as a sortable string
        /// </summary>
        /// <param name="dt">date</param>
        /// <returns>string version of date</returns>
        public static string FormatSortableDate(DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd");
        }

    }
}