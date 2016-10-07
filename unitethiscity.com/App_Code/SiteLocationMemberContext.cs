/******************************************************************************
 * Filename: SiteLocationMemberContext.cs
 * Project:  unitethiscity.com
 * 
 * Description:
 * Gets the context for a member at a location for the current period.
******************************************************************************/
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Security.Cryptography;
using Sancsoft.Web;
using System.Text;

/// <summary>
/// Summary info class for lists of locations
/// </summary>
public class SiteLocationContext
{
    protected WebDBContext db;

    /// <summary>
    /// Properties
    /// </summary>
    #region Properties
    protected int accID;
    public int AccID
    {
        get
        {
            return accID;
        }
    }
    protected int locID;
    public int LocID
    {
        get
        {
            return locID;
        }
    }
    protected int busID;
    public int BusID
    {
        get
        {
            return busID;
        }
    }

    protected int perID;
    public int PerID
    {
        get
        {
            return perID;
        }
    }

    protected bool isRedeemed;
    public bool IsRedeemed
    {
        get
        {
            return isRedeemed;
        }
    }

    protected decimal redeemedThisMonth;
    public decimal RedeemedThisMonth
    {
        get
        {
            return redeemedThisMonth;
        }
    }

    protected decimal redeemedAllTime;
    public decimal RedeemedAllTime
    {
        get
        {
            return redeemedAllTime;
        }
    }

    protected DateTime? lastRedeemed;
    public DateTime? LastRedeemed
    {
        get
        {
            return lastRedeemed;
        }
    }

    protected decimal checkInsThisMonth;
    public decimal CheckInsThisMonth
    {
        get
        {
            return checkInsThisMonth;
        }
    }

    protected decimal checkInsAllTime;
    public decimal CheckInsAllTime
    {
        get
        {
            return checkInsAllTime;
        }
    }

    protected DateTime? lastCheckedIn;
    public DateTime? LastCheckedIn
    {
        get
        {
            return lastCheckedIn;
        }
    }

    public string LastCheckedInAsString
    {
        get
        {
            return (lastCheckedIn == null) ? "N/A" : ((DateTime)lastCheckedIn).ToString("d");
        }
    }

    public string LastRedeemedAsString
    {
        get
        {
            return (lastRedeemed == null) ? "N/A" : ((DateTime)lastRedeemed).ToString("d");
        }
    }
    #endregion Properties

    /// <summary>
    /// Create a location context for a member account and location
    /// </summary>
    public SiteLocationContext(int account, int location)
    {
        db = new WebDBContext();

        accID = account;
        locID = location;
        perID = Period.IdentifyPeriod(DateTime.Now);

        // load the properties from the database object
        LoadContext();
    }

    /// <summary>
    /// Load the context from the database using the configured account, location and the
    /// active period
    /// </summary>
    protected void LoadContext()
    {
        TblLocations rsLoc = db.TblLocations.Single(target=>target.LocID == locID);
        busID = rsLoc.BusID;
        // get the current deal for this period
        TblDeals rsDeal = db.TblDeals.SingleOrDefault(target => target.BusID == busID && target.PerID == perID);
        if (rsDeal == null)
        {
            // no deal available, no money available
            isRedeemed = false;      
        }
        else
        {
            // there is a deal defined, see if this account has already redeemed it
            isRedeemed = (db.TblRedemptions.Count(target => target.AccID == AccID && target.DelID == rsDeal.DelID) != 0);
        }

        // get the latest check in
        TblCheckIns rsLastCheckIn = db.TblCheckIns.Where(target=>target.AccID == accID && target.LocID == locID).OrderByDescending(target=>target.ChkTS).FirstOrDefault();
        if (rsLastCheckIn != null)
        {
            lastCheckedIn = rsLastCheckIn.ChkTS;
        }
        else
        {
            lastCheckedIn = null;
        }

        // calculate the number of checkins this month
        checkInsThisMonth = db.TblCheckIns.Count(target =>target.AccID == accID && target.LocID == locID && target.PerID == perID);
        // calculate the number of checkins all time
        checkInsAllTime = db.TblCheckIns.Count(target => target.AccID == accID && target.LocID == locID);

        // get the latest redemption
        VwRedemptions rsLastRedemption = db.VwRedemptions.Where(target => target.AccID == accID && target.BusID== busID).OrderByDescending(target => target.RedTS).FirstOrDefault();
        if (rsLastRedemption != null)
        {
            lastRedeemed = rsLastRedemption.RedTS;
        }
        else
        {
            lastRedeemed = null;
        }
    
        // calculate the value of redemptions this month
        IEnumerable<VwRedemptions> rsRedeemedThisMonth = db.VwRedemptions.Where(target => target.AccID == accID && target.BusID == busID && target.PerID == perID);
        redeemedThisMonth = (rsRedeemedThisMonth != null) ? rsRedeemedThisMonth.Sum(target=>target.DelAmount) : 0;

        // calculate the value of redemptions all time
        IEnumerable<VwRedemptions> rsRedeemedAllTime = db.VwRedemptions.Where(target => target.AccID == accID && target.BusID == busID);
        redeemedAllTime = (rsRedeemedAllTime != null) ? rsRedeemedAllTime.Sum(target => target.DelAmount) : 0;
    }
}
