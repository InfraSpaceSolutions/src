/******************************************************************************
 * Filename: LoginCredentials.cs
 * Project:  UTC WebAPI
 * 
 * Description:
 * POCO for representing model of login credentials used to authenticate
 * and retrieve an access token
******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.unitethiscity.api.Models
{
    public class LoginCredentials
    {
        /// <summary>
        /// Email address associated with the account
        /// </summary>
        public string Account{ get; set; }
        /// <summary>
        /// Can be supplied unencrypted (clear text) or MD5 hashed per established
        /// properties
        /// </summary>
        public string Password { get; set; }
    }
}