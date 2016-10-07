/******************************************************************************
 * Filename: InboxReadController.cs
 * Project:  UTC WebAPI
 * 
 * Description:
 * Controller for enumerating and retrieving inbox messages for an account
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
    public class InboxReadController : ApiController
    {
        public void Post(Guid token, int id)
        {
            WebDBContext db = new WebDBContext();
            int accID = APIToken.IdentifyAccount(db, token);
            if (accID == 0)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Access invalid.  Please login again."));
            }

            // get the message
            TblInboxMessages rs = db.TblInboxMessages.SingleOrDefault(target => target.InbID == id);
            if (rs == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Message not found"));
            }
            // make sure it is our message
            if (rs.InbToAccID != accID)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Message access denied"));
            }

            // mark the message as read
            rs.InbRead = true;
            db.SubmitChanges();
            Logger.LogAction("Inbox-Read", accID);
        }
    }
}
