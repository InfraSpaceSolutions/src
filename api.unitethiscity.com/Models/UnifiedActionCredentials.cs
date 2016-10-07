/******************************************************************************
 * Filename: UnifiedActionCredentials.cs
 * Project:  UTC WebAPI
 * 
 * Description:
 * POCO for representing model of user credentials and actions for the unified
 * action interface - redeem if available, fall back to checkin, identify the
 * business from the QURL and use the closest location
******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.unitethiscity.api.Models
{
    public class UnifiedActionCredentials
    {
        public int AccId { get; set; }
        public int RolId { get; set; }
        public int MemberAccId { get; set; }
        public string Qurl { get; set; }
        public string PinNumber { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int RequestCheckin { get; set; }
        public int RequestRedeem { get; set; } 
    }
}