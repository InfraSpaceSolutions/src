/******************************************************************************
 * Filename: WalletController.cs
 * Project:  UTC WebAPI
 * 
 * Description:
 * Controller for accessing account wallet information
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
    /// Controller for accessing account wallet information
    /// </summary>
    public class WalletController : ApiController
    {
        /// <summary>
        /// Read the wallet for a specific account - must match the requesting
        /// account
        /// </summary>
        /// <param name="token">identify account</param>
        /// <returns>wallet context - current status of points and redeemable offers</returns>
        public WalletContext Get(Guid token)
        {
            WebDBContext db = new WebDBContext();

            WalletContext wal = new WalletContext();

            // member context specific information
            TblAPITokens rsTok = db.TblAPITokens.SingleOrDefault(target => target.TokGuid == token);
            wal.AccId = (rsTok != null) ? rsTok.AccID : 0;

            // if there is no member (accid=0), the rest of the code should still calculate properly
            // this assumes that there aren't any bogus records in the database with 0 accid values

            // report based on current period - the period MUST be defined in the database or we reject the request
            if ((wal.PerId = Period.IdentifyPeriod(DateTime.Now)) == 0)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid or undefined period"));

            }
            TblPeriods rsPer = db.TblPeriods.Single(target => target.PerID == wal.PerId);
            wal.PerName = rsPer.PerName;
            wal.PerStartDate = rsPer.PerStartDate;
            wal.PerEndDate = rsPer.PerEndDate;

            // count up the available offers for this period and sum them up
            wal.CashAvailable = 0;
            if (db.TblDeals.Where(target => target.PerID == wal.PerId).Count() > 0)
            {
                wal.CashAvailable = db.TblDeals.Where(target => target.PerID == wal.PerId).Sum(target => target.DelAmount);
            }

            // count up the member activity and sum up redemptions

            // cash redeemed in this period
            wal.CashRedeemed = 0;
            if (db.VwRedemptions.Where(target => target.PerID == wal.PerId && target.AccID == wal.AccId).Count() > 0)
            {
                wal.CashRedeemed = db.VwRedemptions.Where(target => target.PerID == wal.PerId && target.AccID == wal.AccId).Sum(target => target.DelAmount);
            }

            // cash redeemed all-time (new in version 2.0); added in API version 1.8
            wal.CashRedeemedAllTime = 0;
            if (db.VwRedemptions.Where(target => target.AccID == wal.AccId).Count() > 0)
            {
                wal.CashRedeemedAllTime = db.VwRedemptions.Where(target => target.AccID == wal.AccId).Sum(target => target.DelAmount);
            }

            // number of check-ins in the period
            wal.NumCheckins = db.TblCheckIns.Where(target=>target.PerID == wal.PerId && target.AccID == wal.AccId).Count();
            // users get a point for each check-in
            wal.Points = wal.NumCheckins;

            // count up the applicable social networking posts and add as points
            int socialPoints = db.TblSocialPosts.Count(target => target.AccID == wal.AccId && target.PerID == wal.PerId);
            wal.Points += socialPoints;

            // points and check-ins all time - addded in API version 1.8
            int socialPostsAllTime = db.TblSocialPosts.Count(target => target.AccID == wal.AccId);
            wal.NumCheckinsAllTime = db.TblCheckIns.Where(target => target.AccID == wal.AccId).Count();
            wal.PointsAllTime = wal.NumCheckinsAllTime + socialPostsAllTime;

            Logger.LogAction("Wallet-View", wal.AccId);

            return wal;
        }

        /// <summary>
        /// Get the wallet context for the default/guest user - this is used to find out what they could get if they were a member
        /// </summary>
        /// <returns>wallet context for default/guest user</returns>
        public WalletContext Get()
        {
            return Get(Guid.Empty);
        }
    }
}
