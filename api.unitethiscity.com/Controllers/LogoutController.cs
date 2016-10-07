/******************************************************************************
 * Filename: LogoutController.cs
 * Project:  UTC WebAPI
 * 
 * Description:
 * Controller for disabling an authentication token
******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using com.unitethiscity.api.Models;

namespace com.unitethiscity.api.Controllers
{
    /// <summary>
    /// Controller for disabling authentication tokens
    /// </summary>
    public class LogoutController : ApiController
    {
        /// <summary>
        /// Invalidate the supplied API token.  Needs to have email, account id and token in token object.
        /// </summary>
        /// <param name="tok">token object</param>
        public void Post(APIToken tok)
        {
            WebDBContext db = new WebDBContext();

            // look up the account that goes with the supplied email address
            TblAccounts rsAcc = db.TblAccounts.SingleOrDefault(target => target.AccEMail == tok.AccEMail);
            if (rsAcc == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid account"));
            }

            // make sure we have the right account identifier
            if (rsAcc.AccID != tok.AccId)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid account identification"));
            }

            // confirm that a login token exists that matches the supplied information
            if (db.TblAPITokens.Count( target => target.TokGuid == tok.Token && target.AccID == rsAcc.AccID ) <= 0)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid authorization"));
            }

            // delete the token
            db.TblAPITokens.DeleteOnSubmit(db.TblAPITokens.Single(target => target.TokGuid == tok.Token));
            db.SubmitChanges();

            Logger.LogAction("Logout", tok.AccId);

        }
    }
}
