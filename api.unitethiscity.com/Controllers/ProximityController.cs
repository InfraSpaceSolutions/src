/******************************************************************************
 * Filename: PushTokenController.cs
 * Project:  UTC WebAPI
 * 
 * Description:
 * Controller for support of Push Token management - device identifiers and
 * association to UTC accountss
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
    public class ProximityController : ApiController
    {
        WebDBContext db;

        /// <summary>
        /// Update the location of a device in order to support delivery of 
        /// proximity notifications
        /// </summary>
        /// <param name="token">identify account</param>
        /// <param name="prox">identify device by token and specify location</param>
        public void Post(Guid token, Proximity prox)
        {
            db = new WebDBContext();
            int accID = APIToken.IdentifyAccount(db, token);
            if (accID == 0)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Access invalid.  Please login again."));
            }

            string normToken = NormalizeDeviceID(prox.PutToken);

            // update the location of the device by storing the token and the location
            TblProximities rsPrx = db.TblProximities.SingleOrDefault(target=>target.PutToken == normToken);
            if (rsPrx == null)
            {
                rsPrx = new TblProximities();
                rsPrx.AccID = accID;
                rsPrx.PutToken = normToken;
                db.TblProximities.InsertOnSubmit(rsPrx);
            }
            rsPrx.PrxLatitude = prox.Latitude;
            rsPrx.PrxLongitude = prox.Longitude;
            rsPrx.PrxModifyTS = DateTime.Now;

            // add an entry to the proximity log to support debugging and analysis
            TblProximityLog rsPxl = new TblProximityLog();
            rsPxl.AccID = accID;
            rsPxl.PutToken = normToken;
            rsPxl.PxlLatitude = prox.Latitude;
            rsPxl.PxlLongitude = prox.Longitude;
            rsPxl.PxlTS = DateTime.Now;
            db.TblProximityLog.InsertOnSubmit(rsPxl);

            // save upserted location and add a corresponding log entry
            db.SubmitChanges();

            Logger.LogAction("Proximity-Update", accID);
        }

        /// <summary>
        /// Normalize a push token device identifier string - uppercase hex digits only
        /// </summary>
        /// <param name="raw">raw device identifier</param>
        /// <returns>normalized device identifier</returns>
        [NonAction]
        public static string NormalizeDeviceID(string raw)
        {
            // if the token is wrapped in < > then we need to normalize it for IOS
            // Google tokens can't be normalized like this
            // this is a miserable hack to cover up a bug in the api based on not identifying the device type when pushing the token up
            if (raw.Length > 2)
            {
                if (raw.Substring(0, 1) == "<")
                {
                    return Regex.Replace(raw.ToLower(), @"[^0-9a-f]", "");
                }
            }
            return raw;
        }
    }
}
