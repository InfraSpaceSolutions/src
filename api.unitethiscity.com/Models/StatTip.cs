using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.unitethiscity.api.Models
{
    public class StatTip
    {
        public int Id { get; set; }
        public int BusId { get; set; }
        public Guid BusGuid { get; set; }
        public string Name { get; set; }
        public DateTime Timestamp { get; set; }
        public string TimestampAsString { get; set; }
        public string TimestampSortable { get; set; }
        public string TipText { get; set; }
        public string LocationName { get; set; }
    }
}