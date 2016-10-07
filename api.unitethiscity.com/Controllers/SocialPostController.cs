/******************************************************************************
 * Filename: SocialPostController.cs
 * Project:  UTC WebAPI
 * 
 * Description:
 * Controller for support of storing social post references
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
    public class SocialPostController : ApiController
    {
        WebDBContext db;

        /// <summary>
        /// Process the request to create a social post reference
        /// </summary>
        /// <param name="token">identify account</param>
        /// <param name="sop">definition of social post</param>
        public void Post(Guid token, SocialPost sop)
        {
            db = new WebDBContext();
            int accID = APIToken.IdentifyAccount(db, token);
            if (accID == 0)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Access invalid.  Please login again."));
            }

            // match the account token to the account in token definition
            if (accID != sop.AccId)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Account mismatch"));
            }

            // force invalid social post types to be facebook posts
            if (sop.SptId == 0)
            {
                sop.SptId = 2;
            }

            DateTime now = DateTime.Now;
            DateTime sod = DateTime.Today;
            DateTime eod = sod.AddDays(1);
            // see if we have already credited the user for a matching post
            if (db.TblSocialPosts.Count(target => target.AccID == sop.AccId && target.SptID == sop.SptId && 
                target.BusID == sop.BusId && target.SopTS >= sod && target.SopTS < eod) > 0)
            {
                // already have a matching post, don't create a new one
                return;
            }

            // create a new record as credit for the post
            TblSocialPosts rsSop = new TblSocialPosts();
            rsSop.AccID = sop.AccId;
            rsSop.SptID = sop.SptId;
            rsSop.PerID = Period.IdentifyPeriod(now);
            rsSop.BusID = sop.BusId;
            rsSop.SopTS = now;
            db.TblSocialPosts.InsertOnSubmit(rsSop);
            db.SubmitChanges();
            Logger.LogAction("Social-Post", accID, sop.BusId);
        }
    }
}
