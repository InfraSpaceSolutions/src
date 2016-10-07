/******************************************************************************
 * Filename: WalletContext.cs
 * Project:  UTC WebAPI
 * 
 * Description:
 * POCO for representing model of a member's wallet
******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.unitethiscity.api.Models
{
    public class WalletContext
    {
        public int AccId { get; set; }
        public int PerId { get; set; }
        public string PerName { get; set; }
        public DateTime PerStartDate { get; set; }
        public DateTime PerEndDate { get; set; }
        public decimal CashAvailable { get; set; }
        public decimal CashRedeemed { get; set; }
        public decimal NumCheckins { get; set; }
        public decimal Points { get; set; }

        // fields added in version 2.0 of the application
        public decimal CashRedeemedAllTime { get; set; }
        public decimal NumCheckinsAllTime { get; set; }
        public decimal PointsAllTime { get; set; }
    }
}