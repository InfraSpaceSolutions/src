/******************************************************************************
 * Filename: FreeSignUp3.cs
 * Project:  UTC WebAPI
 * 
 * Description:
 * POCO for device-based sign up of free accounts - version 3 - with facebook
 * identifier added 
******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.unitethiscity.api.Models
{
    public class FreeSignUp3
    {
        // Account information
        public string AccFName { get; set; }
        public string AccLName { get; set; }
        public string AccEMail { get; set; }

        /// <summary>
        /// New free account demographics
        /// </summary>
        public string AccGender { get; set; }
        public string AccBirthdate { get; set; }

        public string AccZip { get; set; }

        // promotion code or referral code
        public string PromoCode { get; set; }

        // optional facebook identifier
        public string AccFacebookIdentifier { get; set; }
    }
}