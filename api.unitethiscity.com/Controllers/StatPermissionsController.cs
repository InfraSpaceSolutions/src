/******************************************************************************
 * Filename: MenuController.cs
 * Project:  UTC WebAPI
 * 
 * Description:
 * Get the information for a business image gallery
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
    public class StatPermissionsController : ApiController
    {
        /// <summary>
        /// Get the menu items for a business
        /// </summary>
        /// <param name="token">user identifier</param>
        /// <returns>array of menu items in defined sequence</returns>
        public StatPermissions Get(Guid token)
        {
            WebDBContext db = new WebDBContext();

            // member context specific information
            TblAPITokens rsTok = db.TblAPITokens.SingleOrDefault(target => target.TokGuid == token);
            if (rsTok == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            StatPermissions ret = new StatPermissions();
            ret.AccId = rsTok.AccID;
            TblAccountAnalytics rs = db.TblAccountAnalytics.SingleOrDefault(target => target.AccID == rsTok.AccID);
            // if we dont have explicit permissions, we don't have any permissions
            if (rs != null)
            {
                ret.HasGlobalStatistics = rs.AcaGlobalStats;
                ret.HasGlobalAnalytics = rs.AcaGlobalAnalytics;
                ret.HasBusinessStatistics = rs.AcaBusinessStats;
                ret.HasBusinessAnalytics = rs.AcaBusinessAnalytics;
            }
            Logger.LogAction("StatPermissions", rsTok.AccID, 0);
            return ret;
        }
    }
}
