/******************************************************************************
 * Filename: Tip.cs
 * Project:  unitethiscity.com
 * 
 * Description:
 * Handle changes to the tips on locations
 * 
 * Note - the exists flag indicates if a corresponding tip is stored in the
 * database. If false, you get an empty tip to work with.
 * 
 * TODO - Figure out where to process HTML encoding - Tips must be plain text
 * for support across platforms.
 * 
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
/// Utility class for handling tips
/// </summary>
public class Tip
{
    /// <summary>
    /// Properties
    /// </summary>
    #region Properties
    private bool exists;
    public bool Exists
    {
        get
        {
            return exists;
        }
    }

    private int accID;
    public int AccID
    {
        get
        {
            return accID;
        }
    }
    private int locID;
    public int LocID
    {
        get
        {
            return locID;
        }
    }
    public string TipText;
    private DateTime tipTS;
    public DateTime TipTS
    {
        get
        {
            return tipTS;
        }
    }
    #endregion

    /// <summary>
    /// Create a tip object for an account and location - uses existing tip if one is found.
    /// </summary>
    /// <param name="accountID"></param>
    /// <param name="locationID"></param>
    public Tip(int accountID, int locationID)
    {
        accID = accountID;
        locID = locationID;
        TipText = "";
        tipTS = DateTime.Now;
        exists = false;

        // get an existing tip from the database
        WebDBContext db = new WebDBContext();
        TblTips rs = db.TblTips.SingleOrDefault(target => target.AccID == accID && target.LocID == locID);
        if (rs != null)
        {
            TipText = rs.TipText;
            tipTS = rs.TipTS;
            // an existing tip has been loaded
            exists = true;
        }
    }

    /// <summary>
    /// Save the changes to a tip
    /// </summary>
    public void SaveChanges()
    {
        // get an existing tip from the database
        WebDBContext db = new WebDBContext();
        TblTips rs = db.TblTips.SingleOrDefault(target => target.AccID == accID && target.LocID == locID);
        if (rs == null)
        {
            rs = new TblTips();
            rs.AccID = accID;
            rs.LocID = locID;
            db.TblTips.InsertOnSubmit(rs);
        }
        rs.TipText = TipText;
        rs.TipTS = DateTime.Now;
        db.SubmitChanges();
        // mark the tip as existing in the database
        exists = true;
    }
}
