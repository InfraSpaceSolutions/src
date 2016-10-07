/******************************************************************************
 * Filename: CheckInCredentials.cs
 * Project:  UTC WebAPI
 * 
 * Description:
 * POCO for representing model of an account request for a checkin.
******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.unitethiscity.api.Models
{
    public class CheckInCredentials
    {
        /// <summary>
        /// Identifies the account making the request
        /// </summary>
        public int AccId { get; set; }
        /// <summary>
        /// Role of account making the request. Ex. members can request a checkin, businesses can check members in
        /// </summary>
        public int RolId { get; set; }
        /// <summary>
        /// Specifies the location for the checkin
        /// </summary>
        public int LocId { get; set; }
        /// <summary>
        /// Account id of the member to check in, which matches the accid for member self-checkins or is the target member for business check ins
        /// </summary>
        public int MemberAccId { get; set; }
        /// <summary>
        /// The complete url found in the qr code that was scanned for check in, if applicable.
        /// </summary>
        public string Qurl { get; set; }
        /// <summary>
        /// Physical location of the check in for validation if required based on business/location
        /// </summary>
        public double Latitude { get; set; }
        /// <summary>
        /// Physical location of the check in for validation if required based on business/location
        /// </summary>
        public double Longitude { get; set; }
    }
}