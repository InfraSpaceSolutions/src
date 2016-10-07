/******************************************************************************
 * Filename: InboxMessageController.cs
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
    public class InboxMessageController : ApiController
    {
        /// <summary>
        /// Get all messages in the account inbox
        /// </summary>
        /// <param name="token">identify account</param>
        /// <returns>array of inbox message objects</returns>
        public IEnumerable<InboxMessage> GetAllMessages(Guid token)
        {
            WebDBContext db = new WebDBContext();
            int accID = APIToken.IdentifyAccount(db, token);
            if (accID == 0)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Access invalid.  Please login again."));
            }

            List<InboxMessage> inbox = new List<InboxMessage>();

            IEnumerable<VwInboxMessages> rs = db.VwInboxMessages.Where(target => target.InbToAccID == accID).OrderByDescending(target => target.MsgTS);
            foreach (VwInboxMessages row in rs)
            {
                inbox.Add(Factory(row, 140));
            }
            Logger.LogAction("Inbox-List", accID);

            return inbox;
        }

        /// <summary>
        /// Get a single message in the account inbox
        /// </summary>
        /// <param name="token">identify account</param>
        /// <param name="id">target message identifier</param>
        /// <returns></returns>
        public InboxMessage GetMessage(Guid token, int id)
        {
            WebDBContext db = new WebDBContext();
            int accID = APIToken.IdentifyAccount(db, token);
            if (accID == 0)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Access invalid.  Please login again."));
            }
            VwInboxMessages rs = db.VwInboxMessages.SingleOrDefault(target => target.InbID == id);
            if (rs == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            if (rs.InbToAccID != accID)
            {
                throw new HttpResponseException(HttpStatusCode.Forbidden);
            }
            Logger.LogAction("Inbox-View", accID);

            return Factory(rs, 0);
        }

        [NonAction]
        protected InboxMessage Factory(VwInboxMessages rs, int bodyLimit)
        {
            InboxMessage ret = new InboxMessage();
            ret.Id = rs.InbID;
            ret.MsgId = rs.MsgID;
            ret.ToAccId = rs.InbToAccID;
            ret.InbRead = rs.InbRead;
            ret.FromAccID = rs.MsgFromID;
            ret.FromName = rs.MsgFromName;
            ret.Summary = rs.MsgSummary;
            // limit the message body as specified
            if (( bodyLimit > 0 ) && (bodyLimit < rs.MsgBody.Length))
            {
                ret.Body = rs.MsgBody.Substring( 0, bodyLimit);
            }
            else
            {
                ret.Body = rs.MsgBody;
            }
            ret.MsgTS = rs.MsgTS;
            // generate a string version of the timestamp that is suitable for IOS parsing
            ret.MsgTSAsStr = rs.MsgTS.ToUniversalTime().ToString("yyyy-MM-ddHH:mm:ss");
            ret.MsgExpires = rs.MsgExpires;
            // generate a string version of the timestamp that is suitable for IOS parsing
            ret.MsgExpiresAsStr = rs.MsgExpires.ToUniversalTime().ToString("yyyy-MM-ddHH:mm:ss");

            // assign a business guid if applicable
            ret.BusGuid = rs.BusGuid ?? Guid.Empty;

            return ret;
        }
    }
}
