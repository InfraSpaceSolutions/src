/******************************************************************************
 * Filename: LocationTip.cs
 * Project:  UTC WebAPI
 * 
 * Description:
 * POCO for representing model of a single location tip
******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.unitethiscity.api.Models
{
    public class LocationTip
    {
        public int LocId { get; set; }
        public int AccId { get; set; }
        public string Text { get; set; }
        public string Signature { get; set; }
        public DateTime Timestamp { get; set; }
        /// <summary>
        /// String version of timestamp for mobile interface since IOS
        /// can't handle .NET webapi timestamps
        /// </summary>
        public string TimestampAsStr { get; set; }
    }
}