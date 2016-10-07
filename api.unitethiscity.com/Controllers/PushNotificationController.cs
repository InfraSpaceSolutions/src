/******************************************************************************
 * Filename: PushNotification.cs
 * Project:  UTC WebAPI
 * 
 * Description:
 * Controller for creating push notifications.
******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using com.unitethiscity.api.Models;
using System.Text.RegularExpressions;

namespace com.unitethiscity.api.Controllers
{
    public class PushNotificationController : ApiController
    {
        WebDBContext db;

        /// <summary>
        /// Process the request to create a push notification
        /// </summary>
        /// <param name="token">identify account</param>
        /// <param name="pn">push notification</param>
        public void Post(Guid token, PushNotification pn)
        {
            db = new WebDBContext();
            int accID = APIToken.IdentifyAccount(db, token);
            if (accID == 0)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Access invalid.  Please login again."));
            }

            // match the account token to the account in token definition
            if (accID != pn.AccId)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Account mismatch"));
            }

            // normalize the push token for string comparisons and consistent representation
            string tokenRaw = pn.PutToken;
            string tokenNorm = PushTokenController.NormalizeDeviceID(pn.PutToken);

            // acquire the device identifier for sending the push notification - start with raw token
            TblPushTokens rsPut;
            rsPut = db.TblPushTokens.SingleOrDefault(target => target.PutToken == tokenRaw && target.AccID == pn.AccId);
            if (rsPut == null)
            {
                // try it with the normalized token
                rsPut = db.TblPushTokens.SingleOrDefault(target => target.PutToken == tokenNorm && target.AccID == pn.AccId);
                if (rsPut == null)
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid device identifier"));
                }
                else
                {
                    // use the normalized token
                    pn.PutToken = tokenNorm;
                }
            }

            // create a new push notification queued for processing and delivery
            TblPushNotifications rsPun = new TblPushNotifications();
            rsPun.PnsID = (int)PushNotificationStates.Queued;
            rsPun.PutID = rsPut.PutID;
            rsPun.MsgID = 0; // this doesn't correspond to a UTC message
            rsPun.PunAlert = pn.PunAlert;
            rsPun.PunBadgeID = pn.PunBadgeId;
            rsPun.PunCreateTS = DateTime.Now;
            db.TblPushNotifications.InsertOnSubmit(rsPun);
            db.SubmitChanges();

            Logger.LogAction("PushNotification-Post", accID);
        }
    }
}
