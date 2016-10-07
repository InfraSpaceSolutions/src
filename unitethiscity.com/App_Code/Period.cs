/******************************************************************************
 * Filename: Period.cs
 * Project:  unitethiscity.com
 * 
 * Description:
 * Identify periods for dates and retrieve period names.
 * Utility class
******************************************************************************/
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Text;
using Sancsoft.Web;

/// <summary>
/// Utility class for handling periods
/// </summary>
public class Period
{
    /// <summary>
    /// Properties
    /// </summary>
    #region Properties
    private int perID;
    public int PerID
    {
        get
        {
            return perID;
        }
    }
    private string perName;
    public string PerName
    {
        get
        {
            return perName;
        }
    }
    private DateTime perStartDate;
    public DateTime PerStartDate
    {
        get
        {
            return perStartDate;
        }
    }
    private DateTime perEndDate;
    public DateTime PerEndDate
    {
        get
        {
            return perEndDate;
        }
    }
    #endregion

    /// <summary>
    /// Returns true if there is a period that corresponds to the supplied datetime
    /// </summary>
    /// <param name="dt">datetime to check for period</param>
    /// <returns></returns>
    public static bool Defined(DateTime dt)
    {
        return (IdentifyPeriod(dt) != 0);
    }

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

    /// <summary>
    /// Create a period object for the current datetime
    /// Throws an exception if no period exists
    /// </summary>
    public Period()
    {
        LoadPeriod(DateTime.Now);
    }

    /// <summary>
    /// Create a period object for a specific date time
    /// Throws an exception if no period exists
    /// </summary>
    /// <param name="dat"></param>
    public Period(DateTime dt)
    {
        LoadPeriod(dt);
    }

    /// <summary>
    /// Load the period from the database using the supplied datetime
    /// </summary>
    /// <param name="dt">datetime to look up</param>
    protected void LoadPeriod(DateTime dt)
    {
        WebDBContext db = new WebDBContext();
        // back up the end date by 24 hours so that we get an inclusive comparison
        DateTime enddt = dt.AddDays(-1);
        TblPeriods rs = db.TblPeriods.Single(target => target.PerStartDate <= dt && target.PerEndDate > enddt);
        perID = rs.PerID;
        perName = rs.PerName;
        perStartDate = rs.PerStartDate;
        perEndDate = rs.PerEndDate;
    }
}
