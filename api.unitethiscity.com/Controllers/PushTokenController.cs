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
    public class PushTokenController : ApiController
    {
        WebDBContext db;

        /// <summary>
        /// Process the request to create a push token
        /// </summary>
        /// <param name="token">identify account</param>
        /// <param name="pt">definition of push token to create</param>
        public void Post(Guid token, PushToken pt)
        {
            db = new WebDBContext();
            int accID = APIToken.IdentifyAccount(db, token);
            if (accID == 0)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Access invalid.  Please login again."));
            }
            
            // match the account token to the account in token definition
            if (accID != pt.AccId)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Account mismatch"));
            }

            // normalize UIS push tokens for string comparisons and consistent representation
            if (pt.PdtId == 1)
            {
                pt.PutToken = NormalizeDeviceID(pt.PutToken);
            }

            TblPushTokens rsPut = db.TblPushTokens.SingleOrDefault(target => target.PutToken == pt.PutToken);
            if (rsPut == null)
            {
                // create a new record
                rsPut = new TblPushTokens();
                rsPut.PutToken = pt.PutToken;
                rsPut.PutCreateTS = DateTime.Now;
                db.TblPushTokens.InsertOnSubmit(rsPut);
            }
            rsPut.AccID = pt.AccId;
            rsPut.PdtID = pt.PdtId;
            rsPut.PutEnabled = pt.PutEnabled;
            rsPut.PutModifyTS = DateTime.Now;
            db.SubmitChanges();

            Logger.LogAction("PushToken-Create", accID);
        }

        /// <summary>
        /// Return all of the push tokens associated with the authenticated user
        /// </summary>
        /// <param name="token">identify account</param>
        /// <returns>List of Push Tokens</returns>
        public List<PushToken> GetAll(Guid token)
        {
            db = new WebDBContext();
            List<PushToken> list = new List<PushToken>();
            int accID = APIToken.IdentifyAccount(db, token);
            if (accID == 0)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Access invalid.  Please login again."));
            }

            IEnumerable<TblPushTokens> rs = db.TblPushTokens.Where(target => target.AccID == accID);
            foreach (TblPushTokens row in rs)
            {
                PushToken pt = Factory(row);
                list.Add(pt);
            }
            Logger.LogAction("PushToken-List", accID);

            return list;
        }

        /// <summary>
        /// Return a specific push token by idany user validated user can retrieve any
        /// defined token - not sure if that is a good idea but we need to be able to check the
        /// availability since devices could be shared and they will only have one push token 
        /// assigned by push provider
        /// </summary>
        /// <param name="token">identify account</param>
        /// <param name="id">identify token</param>
        /// <returns>Push Tokens</returns>
        public PushToken Get(Guid token, int id)
        {
            db = new WebDBContext();
            int accID = APIToken.IdentifyAccount(db, token);
            if (accID == 0)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Access invalid.  Please login again."));
            }
            TblPushTokens rs = db.TblPushTokens.SingleOrDefault(target => target.PutID == id);
            if (rs == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            return Factory(rs);
        }

        /// <summary>
        /// Return a specific push token by token; any user validated user can retrieve any
        /// defined token - not sure if that is a good idea but we need to be able to check the
        /// availability since devices could be shared and they will only have one push token 
        /// assigned by push provider
        /// </summary>
        /// <param name="token">identify account</param>
        /// <param name="pt">push token string</param>
        /// <returns>Push Tokens</returns>
        public PushToken Get(Guid token, string pt)
        {
            db = new WebDBContext();
            int accID = APIToken.IdentifyAccount(db, token);
            if (accID == 0)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Access invalid.  Please login again."));
            }
            pt = NormalizeDeviceID(pt);
            TblPushTokens rs = db.TblPushTokens.SingleOrDefault(target => target.PutToken == pt);
            if (rs == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            Logger.LogAction("PushToken-Read", accID);
            return Factory(rs);
        }

        /// <summary>
        /// Delete a push token.  Token must be associated with this account
        /// </summary>
        /// <param name="token">identify account</param>
        /// <param name="id">identify token</param>
        public void Delete(Guid token, int id)
        {
            WebDBContext db = new WebDBContext();
            int accID = APIToken.IdentifyAccount(db, token);
            if (accID == 0)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Access invalid.  Please login again."));
            }

            // confirm that the target exits and is accessible
            if (db.TblPushTokens.Count(target => target.PutID == id && target.AccID == accID) <= 0)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Push token not found"));
            }

            // delete the record
            db.TblPushTokens.DeleteOnSubmit(db.TblPushTokens.Single(target => target.PutID == id && target.AccID == accID));
            db.SubmitChanges();
            Logger.LogAction("PushToken-Delete", accID);
        }

        /// <summary>
        /// Update a push token
        /// </summary>
        /// <param name="token">identify account</param>
        /// <param name="pt">definition of push token to update</param>
        public void Put(Guid token, PushToken pt)
        {
            db = new WebDBContext();
            int accID = APIToken.IdentifyAccount(db, token);
            if (accID == 0)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Access invalid.  Please login again."));
            }

            // match the account token to the account in token definition
            if (accID != pt.AccId)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Account mismatch"));
            }

            // normalize the push token for string comparisons and consistent representation
            pt.PutToken = NormalizeDeviceID(pt.PutToken);

            TblPushTokens rsPut = db.TblPushTokens.SingleOrDefault(target => target.PutID == pt.PutId);
            if (rsPut == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Target not found"));
            }
            rsPut.AccID = pt.AccId;
            rsPut.PdtID = pt.PdtId;
            rsPut.PutToken = pt.PutToken;
            rsPut.PutEnabled = pt.PutEnabled;
            rsPut.PutModifyTS = DateTime.Now;
            db.SubmitChanges();
            Logger.LogAction("PushToken-Update", accID);
        }

        /// <summary>
        /// Create a push token object from a push token record
        /// </summary>
        /// <param name="rs">push token record</param>
        /// <returns>push token object</returns>
        [NonAction]
        protected PushToken Factory(TblPushTokens rs)
        {
            PushToken put = new PushToken();
            put.PutId = rs.PutID;
            put.AccId = rs.AccID;
            put.PdtId = rs.PdtID;
            put.PutToken = rs.PutToken;
            put.PutEnabled = rs.PutEnabled;
            return put;
        }

        /// <summary>
        /// Normalize a push token device identifier string - uppercase hex digits only
        /// </summary>
        /// <param name="raw">raw device identifier</param>
        /// <returns>normalized device identifier</returns>
        [NonAction]
        public static string NormalizeDeviceID(string raw)
        {
            return Regex.Replace(raw.ToLower(), @"[^0-9a-f]", "");
        }
    }
}
