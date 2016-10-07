/******************************************************************************
 * Filename: ProximityActionCredentials.cs
 * Project:  UTC WebAPI
 * 
 * Description:
 * POCO for representing model of user credentials and actions for the proximity
 * action interface - redeem if available, fall back to checkin, using the 
 * supplied location id - confirm that we are in range
******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.unitethiscity.api.Models
{
    public class ProximityActionCredentials
    {
        public int AccId { get; set; }
        public int RolId { get; set; }
        public int MemberAccId { get; set; }
        public int LocId { get; set; }
        public string Qurl { get; set; }
        public string PinNumber { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int RequestCheckin { get; set; }
        public int RequestRedeem { get; set; }
    }
}