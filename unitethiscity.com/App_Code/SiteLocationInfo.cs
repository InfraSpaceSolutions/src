/******************************************************************************
 * Filename: SiteLocationInfo.cs
 * Project:  unitethiscity.com
 * 
 * Description:
 * Summary info class for a location.  Used to provide lists of locations
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
public class SiteLocationInfo
{
    protected WebDBContext db;

    /// <summary>
    /// Properties
    /// </summary>
    #region Properties
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
    protected Guid busGuid;
    public Guid BusGuid
    {
        get
        {
            return busGuid;
        }
    }
    protected int catID;
    public int CatID
    {
        get
        {
            return catID;
        }
    }

    protected string catName;
    public string CatName
    {
        get
        {
            return catName;
        }
    }

    protected int citID;
    public int CitID
    {
        get
        {
            return citID;
        }
    }

    protected string citName;
    public string CitName
    {
        get
        {
            return citName;
        }
    }

    protected string locName;
    public string LocName
    {
        get
        {
            return locName;
        }
    }

    protected string locAddress;
    public string LocAddress
    {
        get
        {
            return locAddress;
        }
    }

    protected string locCity;
    public string LocCity
    {
        get 
        {
            return locCity;
        }
    }

    protected string locState;
    public string LocState
    {
        get
        {
            return locState;
        }
    }

    protected string locZip;
    public string LocZip
    {
        get
        {
            return locZip;
        }
    }

    protected string locPhone;
    public string LocPhone
    {
        get
        {
            return locPhone;
        }
    }

    protected double locRating;
    public double LocRating
    {
        get
        {
            return locRating;
        }
    }

    protected double locLatitude;
    public double LocLatitude
    {
        get
        {
            return locLatitude;
        }
    }

    protected double locLongitude;
    public double LocLongitude
    {
        get
        {
            return locLongitude;
        }
    }

    public string FullAddress
    {
        get
        {
            return locAddress + ", " + locCity + ", " + locState + " " + locZip;
        }
    }

    protected string busName;
    public string BusName
    {
        get
        {
            return busName;
        }

    }

    protected string busFormalName;
    public string BusFormalName
    {
        get
        {
            return busFormalName;
        }

    }

    protected string busSummary;
    public string BusSummary
    {
        get
        {
            return busSummary;
        }
    }

    protected int delID;
    public int DelID
    {
        get
        {
            return delID;
        }
    }

    protected decimal delAmount;
    public decimal DelAmount
    {
        get
        {
            return delAmount;
        }
    }

    protected string delCustomTerms;
    public string DelCustomTerms
    {
        get
        {
            return delCustomTerms;
        }
    }

    #endregion Properties

    /// <summary>
    /// A formatted list of location properties
    /// </summary>
    protected List<string> locProperties;

    /// <summary>
    /// Create an empty business info object
    /// </summary>
    public SiteLocationInfo()
    {
        db = new WebDBContext();

        // load the properties from the database object
        busID = 0;
        busGuid = Guid.Empty;
        catID = 0;
        catName = "";
        citID = 0;
        citName = "";
        locName = "";
        locAddress = "";
        locCity = "";
        locState = "";
        locZip = "";
        locRating = 0;
        locLatitude = 0;
        locLongitude = 0;

        busName = "";
        busFormalName = "";
        busSummary = "";

        delID = 0;
        delAmount = 0;
        delCustomTerms = "";

        // null location - no properties
        locProperties = new List<string>();
    }

    /// <summary>
    /// Create a business info object from an id
    /// </summary>
    public SiteLocationInfo(int id)
    {
        db = new WebDBContext();
        VwLocations rs = db.VwLocations.Single(target => target.LocID == id);
        LoadLocation(rs);
    }

    /// <summary>
    /// Create a business info object from an accountbusiness view record
    /// </summary>
    public SiteLocationInfo(VwLocations rs)
    {
        db = new WebDBContext();
        LoadLocation(rs);
    }

    /// <summary>
    /// Load the location for a database record
    /// </summary>
    /// <param name="rs">location database record</param>
    protected void LoadLocation(VwLocations rs)
    {
        // load the properties from the database object
        locID = rs.LocID;
        busID = rs.BusID;
        busGuid = rs.BusGuid;
        catID = rs.CatID;
        catName = rs.CatName;
        citID = rs.CitID;
        citName = rs.CitName;
        locName = rs.LocName;
        locAddress = rs.LocAddress;
        locCity = rs.LocCity;
        locState = rs.LocState;
        locZip = rs.LocZip;
        locPhone = rs.LocPhone;
        locRating = rs.LocRating;
        locLatitude = rs.LocLatitude;
        locLongitude = rs.LocLongitude;

        busName = rs.BusName;
        busFormalName = rs.BusFormalName;
        busSummary = rs.BusSummary;

        // load a list of the properties for the location
        LoadLocationProperties();

        // load the current deal
        LoadCurrentDeal();
    }

    /// <summary>
    /// Load all of the properties for the location 
    /// </summary>
    protected void LoadLocationProperties()
    {
        locProperties = new List<string>();

        // get the business properties and add to the tag string
        var rsBusProp = from prp in db.VwBusinessProperties
                        where prp.BusID == busID
                        orderby prp.PrpName
                        select prp.PrpName;
        locProperties.Concat(rsBusProp);
        // get the location properties and add to the tag string
        var rsLocProp = from prp in db.VwLocationProperties
                        where prp.LocID == locID
                        orderby prp.PrpName
                        select prp.PrpName;
        locProperties.Concat(rsLocProp);
    }

    /// <summary>
    /// Load the current deal for the location
    /// </summary>
    protected void LoadCurrentDeal()
    {
        // identify the period based on the current time
        int perid = Period.IdentifyPeriod(DateTime.Now);
        // get the deal that applies for the current period
        TblDeals rsDeal = db.TblDeals.SingleOrDefault(target => target.BusID == busID && target.PerID == perid);
        if (rsDeal == null)
        {
            // no deal - default out the values
            delID = 0;
            delAmount = 0;
            delCustomTerms = "";
        }
        else
        {
            // copy the deal into the object
            delID = rsDeal.DelID;
            delAmount = rsDeal.DelAmount;
            delCustomTerms = rsDeal.DelCustomTerms;
        }
    }

    /// <summary>
    /// Get the properties of the location as a comma separated string list
    /// </summary>
    /// <returns></returns>
    public string LocationProperties()
    {
        StringBuilder sb = new StringBuilder();
        foreach (string prp in locProperties)
        {
            if (sb.Length > 0)
            {
                sb.Append(", ");
            }
            sb.Append(prp);
        }
        return sb.ToString();
    }

    /// <summary>
    /// Generate the tags for the location - union of category and properties
    /// </summary>
    /// <returns></returns>
    public string LocationTags()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(CatName);
        string prop = LocationProperties();
        if (prop.Length > 0)
        {
            sb.Append(", ");
            sb.Append(prop);
        }
        return sb.ToString();
    }

    /// <summary>
    /// Return the deal amount as a formatted string if a deal is defined.  Returns N/A if no deal
    /// is defined
    /// </summary>
    /// <returns>string format of deal amount</returns>
    public string LocationDealValue()
    {
        return SiteLocationInfo.FormatDealValue(delAmount, (delID != 0));
    }

    /// <summary>
    /// Perform standard formatting of a deal value
    /// </summary>
    /// <param name="delAmount">Amount of deal</param>
    /// <param name="valid">true-> use deal amount</param>
    /// <returns>formatted deal value</returns>
    public static string FormatDealValue(decimal delAmount, bool valid = true)
    {
        string dealDisplay = "N/A";

        if (valid && delAmount > 0)
        {
            if (delAmount != (int)delAmount)
            {
                dealDisplay = delAmount.ToString("C2");
            }
            else
            {
                dealDisplay = delAmount.ToString("C0");
            }
        }
        return dealDisplay;
    }

    /// <summary>
    /// Get the image url for a business
    /// </summary>
    /// <param name="big">if true, use the big @2x image</param>
    /// <returns>image url for business image</returns>
    public string ImageUrl(bool big = false)
    {
        string sizeSuffix = (big) ? "@2x" : "";
        string filePath = SiteSettings.GetValue("BusinessImagesPath") + BusGuid.ToString() + sizeSuffix + ".png";
        string url = "/BusinessImages/" + BusGuid.ToString() + sizeSuffix + ".png";
        if (!System.IO.File.Exists(filePath))
        {
            url = "/BusinessImages/default" + sizeSuffix + ".png";
        }
        return url;
    }


    /// <summary>
    /// Count the number of checkins in a time window
    /// </summary>
    /// <param name="startTS">start time (inclusive)</param>
    /// <param name="endTS">end time (not inclusive)</param>
    /// <returns></returns>
    protected int CountCheckins(DateTime startTS, DateTime endTS)
    {
        return db.VwCheckIns.Count(target => target.LocID == locID && target.ChkTS >= startTS && target.ChkTS < endTS);
    }

    /// <summary>
    /// Count the number of favorites in a time window
    /// </summary>
    /// <param name="startTS">start time (inclusive)</param>
    /// <param name="endTS">end time (not inclusive)</param>
    /// <returns></returns>
    protected int CountFavorites(DateTime startTS, DateTime endTS)
    {
        return db.VwFavorites.Count(target => target.LocID == locID && target.FavTS >= startTS && target.FavTS < endTS);
    }

    /// <summary>
    /// Count the number of ratings in a time window
    /// </summary>
    /// <param name="startTS">start time (inclusive)</param>
    /// <param name="endTS">end time (not inclusive)</param>
    /// <returns></returns>
    protected int CountRatings(DateTime startTS, DateTime endTS)
    {
        return db.VwRatings.Count(target => target.LocID == locID && target.RatTS >= startTS && target.RatTS < endTS);
    }

    /// <summary>
    /// Count the number of tips in a time window
    /// </summary>
    /// <param name="startTS">start time (inclusive)</param>
    /// <param name="endTS">end time (not inclusive)</param>
    /// <returns></returns>
    protected int CountTips(DateTime startTS, DateTime endTS)
    {
        return db.VwTips.Count(target => target.LocID == locID && target.TipTS >= startTS && target.TipTS < endTS);
    }

    /// <summary>
    /// Generate the summary stats for the business
    /// </summary>
    /// <returns>List of summary stats</returns>
    public List<SummaryStats> CalculateSummaryStats()
    {
        WebDBContext db = new WebDBContext();

        List<SummaryStats> stats = new List<SummaryStats>();

        SummaryStats calc;

        // create the dates used for queries
        DateTime startOfToday = DateTime.Today;
        DateTime endOfToday = startOfToday.AddDays(1);
        DateTime startOfPastWeek = endOfToday.AddDays(-7);
        DateTime endOfPastWeek = endOfToday;
        DateTime startOfThisMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        DateTime endOfThisMonth = startOfThisMonth.AddMonths(1);
        DateTime startOfLastMonth = startOfThisMonth.AddMonths(-1);
        DateTime endOfLastMonth = startOfLastMonth.AddMonths(1);
        DateTime startOfTime = new DateTime(2000, 1, 1);
        DateTime endOfTime = new DateTime(2100, 1, 1);

        // checkins
        calc = new SummaryStats("Check-Ins");
        calc.Link = "CheckIns";
        calc.Today = CountCheckins(startOfToday, endOfToday);
        calc.PastWeek = CountCheckins(startOfPastWeek, endOfPastWeek);
        calc.ThisPeriod = CountCheckins(startOfThisMonth, endOfThisMonth);
        calc.LastPeriod = CountCheckins(startOfLastMonth, endOfLastMonth);
        calc.AllTime = CountCheckins(startOfTime, endOfTime);
        stats.Add(calc);

        // favorites
        calc = new SummaryStats("Favorites");
        calc.Link = "Favorites";
        calc.Today = CountFavorites(startOfToday, endOfToday);
        calc.PastWeek = CountFavorites(startOfPastWeek, endOfPastWeek);
        calc.ThisPeriod = CountFavorites(startOfThisMonth, endOfThisMonth);
        calc.LastPeriod = CountFavorites(startOfLastMonth, endOfLastMonth);
        calc.AllTime = CountFavorites(startOfTime, endOfTime);
        stats.Add(calc);

        // ratings
        calc = new SummaryStats("Ratings");
        calc.Link = "Ratings";
        calc.Today = CountRatings(startOfToday, endOfToday);
        calc.PastWeek = CountRatings(startOfPastWeek, endOfPastWeek);
        calc.ThisPeriod = CountRatings(startOfThisMonth, endOfThisMonth);
        calc.LastPeriod = CountRatings(startOfLastMonth, endOfLastMonth);
        calc.AllTime = CountRatings(startOfTime, endOfTime);
        stats.Add(calc);

        // tips
        calc = new SummaryStats("Tips");
        calc.Link = "Tips";
        calc.Today = CountTips(startOfToday, endOfToday);
        calc.PastWeek = CountTips(startOfPastWeek, endOfPastWeek);
        calc.ThisPeriod = CountTips(startOfThisMonth, endOfThisMonth);
        calc.LastPeriod = CountTips(startOfLastMonth, endOfLastMonth);
        calc.AllTime = CountTips(startOfTime, endOfTime);
        stats.Add(calc);

        return stats;
    }
}
