/******************************************************************************
 * Filename: FreeSignUp.cs
 * Project:  UTC WebAPI
 * 
 * Description:
 * POCO for device-based sign up of free accounts
******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.unitethiscity.api.Models
{
    public class FreeSignUp
    {
        // Account information
        public string AccFName { get; set; }
        public string AccLName { get; set; }
        public string AccEMail { get; set; }
        public string AccPhone { get; set; }

        /// <summary>
        /// New free account demographics
        /// </summary>
        public string AccGender { get; set; }
        public string AccBirthdate { get; set; }

        // promotion code or referral code
        public string PromoCode { get; set; }

    }
}