/******************************************************************************
 * Filename: UnifiedActionResults.cs
 * Project:  UTC WebAPI
 * 
 * Description:
 * POCO for representing model of the results of a unified action
******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.unitethiscity.api.Models
{
    public class UnifiedActionResults
    {
        public int AccId { get; set; }
        public int BusId { get; set; }
        public int LocId { get; set; }
        public int DelId { get; set; }
        public double DealAmount { get; set; }
        public string BusName { get; set; }
        public string LocAddress { get; set; }
        public bool Redeemed { get; set; }
        public bool CheckedIn { get; set; }
    }
}