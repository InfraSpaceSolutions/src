/******************************************************************************
 * Filename: FacebookLoginCredentials.cs
 * Project:  UTC WebAPI
 * 
 * Description:
 * POCO for representing model of login credentials used to authenticate
 * and retrieve an access token with a Facebook login
******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.unitethiscity.api.Models
{
    public class FacebookLoginCredentials
    {
        /// <summary>
        /// Email address associated with the account
        /// </summary>
        public string Account { get; set; }
        /// <summary>
        /// Shared passcode for facebook logins
        /// </summary>
        public string Passcode { get; set; }
        /// <summary>
        /// Facebook account identifier
        /// </summary>
        public string FacebookId { get; set; }

    }
}