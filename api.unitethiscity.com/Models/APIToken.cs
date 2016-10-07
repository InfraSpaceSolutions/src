/******************************************************************************
 * Filename: APIToken.cs
 * Project:  UTC WebAPI
 * 
 * Description:
 * POCO for representing model of user credentials and actions that includes
 * the API token for subsequent requests
******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web;
using System.Web.Http.Description;


namespace com.unitethiscity.api.Models
{
    public class APIToken
    {
        public int AccId { get; set; }
        public int CitId { get; set; }
        public Guid AccGuid { get; set; }
        public Guid Token { get; set; }
        public string AccEMail { get; set; }
        public string AccFName { get; set; }
        public string AccLName { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsSalesRep { get; set; }
        public bool IsMember { get; set; }
        public List<int> BusinessRoles;
        public List<int> CharityRoles;
        public List<int> AssociateRoles;
        /// <summary>
        /// Identify the account from the supplied api token
        /// </summary>
        /// <param name="db">database context</param>
        /// <param name="token">guid for access</param>
        /// <returns>account id</returns>
        public static int IdentifyAccount(WebDBContext db, Guid token)
        {
            TblAPITokens tok = db.TblAPITokens.SingleOrDefault(target => target.TokGuid == token);
            return (tok != null) ? tok.AccID : 0;
        }
    }
}