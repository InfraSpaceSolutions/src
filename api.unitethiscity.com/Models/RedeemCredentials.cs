/******************************************************************************
 * Filename: RedeemCredentials.cs
 * Project:  UTC WebAPI
 * 
 * Description:
 * POCO for representing model of user credentials and actions that includes
 * the API token for subsequent requests
******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.unitethiscity.api.Models
{
    public class RedeemCredentials
    {
        public int AccId { get; set; }
        public int RolId { get; set; }
        public int LocId { get; set; }
        public int DealId { get; set; }
        public int MemberAccId { get; set; }
        public string Qurl { get; set; }
        public string PinNumber { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}