/******************************************************************************
 * Filename: LoginController.cs
 * Project:  UTC WebAPI
 * 
 * Description:
 * Controller for authenticating the user account
******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Security.Cryptography;
using System.Text;
using com.unitethiscity.api.Models;

namespace com.unitethiscity.api.Controllers
{
    /// <summary>
    /// Controller for authenticating the user account
    /// </summary>
    public class LoginController : ApiController
    {
        /// <summary>
        /// Process login credentials and generate the account context and access token
        /// </summary>
        /// <param name="lc">account login credentials</param>
        /// <returns>api token and account context model</returns>
        public APIToken Post(LoginCredentials lc)
        {
            WebDBContext db = new WebDBContext();

            // look up the account we are trying to login as by email address
            TblAccounts rs = db.TblAccounts.SingleOrDefault(target => target.AccEMail == lc.Account);
            if (rs == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Account not found"));
            }

            // confirm that the account is enabled
            if (!rs.AccEnabled)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Account disabled"));
            }

            // check the supplied password - check both encrypted and unencrypted varieties
            // this is to support testing as well as encrypted passwords in the database
            String hashword = Encryption.CalculatePasswordHash(rs.AccPassword);
            if ((hashword != lc.Password) && (rs.AccPassword != lc.Password))
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Bad password"));
            }

            // create a new authentication token record and pass the token back to the caller
            TblAPITokens rsTok = new TblAPITokens();
            rsTok.AccID = rs.AccID;
            rsTok.TokGuid = Guid.NewGuid();
            rsTok.TokTS = DateTime.Now;
            db.TblAPITokens.InsertOnSubmit(rsTok);
            db.SubmitChanges();
            APIToken ret = new APIToken() 
            { 
                AccId = rsTok.AccID, 
                CitId  = rs.CitID, 
                AccGuid = rs.AccGuid, 
                Token = rsTok.TokGuid, 
                AccEMail = rs.AccEMail, 
                AccFName = rs.AccFName,
                AccLName = rs.AccLName,
                BusinessRoles = new List<int>(),
                CharityRoles = new List<int>(),
                AssociateRoles = new List<int>()
            };
            // add the role information to enable features on the client side
            ret.IsAdmin = (db.TblAccountRoles.Count(target => target.AccID == ret.AccId && target.RolID == (int)Roles.Administrator) > 0);
            ret.IsSalesRep = (db.TblAccountRoles.Count(target => target.AccID == ret.AccId && target.RolID == (int)Roles.SalesRep) > 0);
            ret.IsMember = (db.TblAccountRoles.Count(target => target.AccID == ret.AccId && target.RolID == (int)Roles.Member) > 0);
            IEnumerable<TblAccountRoles> rsBus = db.TblAccountRoles.Where(target => target.AccID == ret.AccId && target.RolID == (int)Roles.Business);
            foreach (TblAccountRoles row in rsBus)
            {
                ret.BusinessRoles.Add(row.BusID);
            }
            // TODO - add charity roles

            Logger.LogAction("Login", ret.AccId);

            return ret;
        }
    }
}
