using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.unitethiscity.api.Models
{
    public class SummaryCheckIn
    {
        public int AccID { get; set; }
        public int BusID { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
    }
}
