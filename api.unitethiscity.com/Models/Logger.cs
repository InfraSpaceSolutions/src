/******************************************************************************
 * Filename: Logger.cs
 * Project:  api.unitethiscity.com
 * 
 * Description:
 * Class to log activity to the database
******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;


namespace com.unitethiscity.api.Models
{
    public static class Logger
    {
        /// <summary>
        /// Log an action
        /// </summary>
        /// <param name="action">String name of action performed</param>
        /// <param name="accID">associated account or 0 for no account</param>
        /// <param name="busID">associated business or 0 for no business</param>
        public static void LogAction(string action, int accID = 0, int busID = 0)
        {
            WebDBContext db = new WebDBContext();
            TblLogs log = new TblLogs();
            log.LogAction = action;
            log.AccID = accID;
            log.BusID = busID;
            log.LogAgent = WebConvert.Truncate(WebConvert.ToString(HttpContext.Current.Request.UserAgent, "User-Agent-Not-Supplied"), 255);
            log.LogDevice = DeviceTypeFromUserAgent(log.LogAgent);
            log.LogIPAddress = HttpContext.Current.Request.UserHostAddress.ToString();
            log.LogTS = DateTime.Now;
            db.TblLogs.InsertOnSubmit(log);
            db.SubmitChanges();
            db.Dispose();
        }

        public static void LogActionByLocation(string action, int accID, int locID)
        {
            WebDBContext db = new WebDBContext();
            TblLocations rsLoc = db.TblLocations.SingleOrDefault(target => target.LocID == locID);
            int busID = (rsLoc != null) ? rsLoc.BusID : 0;
            Logger.LogAction(action, accID, busID);
            db.Dispose();
        }

        /// <summary>
        /// Determine the device type from the user agent string.  Very hackish
        /// </summary>
        /// <param name="agent">user agent provided by browser</param>
        /// <returns>device type string (IOS|ANDROID|?)</returns>
        public static string DeviceTypeFromUserAgent(string agent)
        {
            agent = agent.ToUpper();
            if (agent.Contains("IPHONE"))
            {
                return "IOS";
            }
            if (agent.Contains("IPAD"))
            {
                return "IOS";
            }
            if (agent.Contains("ANDROID"))
            {
                return "ANDROID";
            }
            if (agent.Contains("USER-AGENT-NOT-SUPPLIED"))
            {
                return "ANDROID";
            }

            return "?";
        }
    }
}