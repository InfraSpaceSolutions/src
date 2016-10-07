/******************************************************************************
 * Filename: Period.cs
 * Project:  UTC WebAPI
 * 
 * Description:
 * Utility class for calculating periods from dates and timestamps
******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.unitethiscity.api.Models
{
    public class Period
    {
        /// <summary>
        /// Looks up a period and gets the perID if it exists, 0 if it does not
        /// </summary>
        /// <param name="dt">datetime within period</param>
        /// <returns></returns>
        public static int IdentifyPeriod(DateTime dt)
        {
            int ret = 0;
            WebDBContext db = new WebDBContext();

            // back up the end date by 24 hours so that we get an inclusive comparison
            DateTime enddt = dt.AddDays(-1);
            TblPeriods rs = db.TblPeriods.SingleOrDefault(target => target.PerStartDate <= dt && target.PerEndDate > enddt);
            if (rs != null)
            {
                ret = rs.PerID;
            }
            return ret;
        }
    }
}