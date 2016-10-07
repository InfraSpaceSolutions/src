/******************************************************************************
 * Filename: JoinNow.cs
 * Project:  UTC WebAPI
 * 
 * Description:
 * POCO for device-based sign up
******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.unitethiscity.api.Models
{
    public class JoinNow
    {
        // Account information
        public string AccFName { get; set; }
        public string AccLName { get; set; }
        public string AccEMail { get; set; }
        public string AccPhone { get; set; }

        // Billing address
        public string BillFName { get; set; }
        public string BillLName { get; set; }
        public string BillAddress { get; set; }
        public string BillCity { get; set; }
        public string BillState { get; set; }
        public string BillZip { get; set; }

        // Credit card information
        public string CardNumber { get; set; }
        public int CardCode { get; set; }
        public int CardExpMonth { get; set; }
        public int CardExpYear { get; set; }

        // promotion code or referral code
        public string PromoCode { get; set; }

        // product for join
        public int PrdID { get; set; }

    }
}