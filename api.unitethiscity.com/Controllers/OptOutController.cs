/******************************************************************************
 * Filename: OptOutController.cs
 * Project:  UTC WebAPI
 * 
 * Description:
 * Opt out of messages from a specific business
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
    public class OptOutController : ApiController
    {
        public void Post(Guid token, int id)
        {
            WebDBContext db = new WebDBContext();
            int accID = APIToken.IdentifyAccount(db, token);
            if (accID == 0)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Access invalid.  Please login again."));
            }

            // get the inbox message
            VwInboxMessages rsInbox = db.VwInboxMessages.SingleOrDefault(target => target.InbID == id);
            if (rsInbox == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Message not found"));
            }

            // only allow for opting out of businesses
            if (rsInbox.BusID != null)
            {
                TblOptOuts rs = db.TblOptOuts.SingleOrDefault(target => target.AccID == accID && target.BusID == rsInbox.BusID);
                if (rs == null)
                {
                    rs = new TblOptOuts();
                    rs.BusID = rsInbox.BusID ?? 0;
                    rs.AccID = accID;
                    db.TblOptOuts.InsertOnSubmit(rs);
                }
                rs.OptTS = DateTime.Now;
                db.SubmitChanges();
            }

            Logger.LogAction("OptOut", accID, id);
        }
    }
}
