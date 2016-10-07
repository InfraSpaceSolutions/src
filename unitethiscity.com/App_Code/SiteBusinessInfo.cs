/******************************************************************************
 * Filename: SiteBusinessInfo.cs
 * Project:  unitethiscity.com
 * 
 * Description:
 * Summary info class for a business.  Used to provide lists of businesses
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
/// Summary info class for lists of businesses
/// </summary>
public class SiteBusinessInfo
{
    protected WebDBContext db;

    /// <summary>
    /// Properties
    /// </summary>
    #region Properties
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

    protected bool busEnabled;
    public bool BusEnabled
    {
        get
        {
            return busEnabled;
        }
    }

    protected float busRating;
    public float BusRating
    {
        get
        {
            return busRating;
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

    protected string busWebsite;
    public string BusWebsite
    {
        get
        {
            return busWebsite;
        }
    }

    protected string busFacebookLink;
    public string BusFacebookLink
    {
        get
        {
            return busFacebookLink;
        }
    }

    protected string busRepEMail;
    public string BusRepEMail
    {
        get
        {
            return busRepEMail;
        }
    }

    protected string busRepFName;
    public string BusRepFName
    {
        get
        {
            return busRepFName;
        }
    }

    protected string busRepLName;
    public string BusRepLName
    {
        get
        {
            return busRepLName;
        }
    }

    protected string busRepPhone;
    public string BusRepPhone
    {
        get
        {
            return busRepPhone;
        }
    }

    protected string busFacebookID;
    public string BusFacebookID
    {
        get
        {
            return busFacebookID;
        }
    }
    #endregion Properties

    /// <summary>
    /// Get the image url for a business guid
    /// </summary>
    /// <param name="busGuid">target business</param>
    /// <param name="big">size - true = 200x200, false = 100x100</param>
    /// <returns></returns>
    public static string GetImageUrl(Guid busGuid, bool big = false)
    {
        string sizeSuffix = (big) ? "@2x" : "";
        string filePath = SiteSettings.GetValue("BusinessImagesPath") + busGuid.ToString() + sizeSuffix + ".png";
        string url = "/BusinessImages/" + busGuid.ToString() + sizeSuffix + ".png";
        if ( !System.IO.File.Exists(filePath) )
        {
            url = "/BusinessImages/default" + sizeSuffix + ".png";
        }
        return url;
    }


    /// <summary>
    /// Create an empty business info object
    /// </summary>
    public SiteBusinessInfo()
    {
        db = new WebDBContext();

        // load the properties from the database object
        busID = 0;
        busGuid = Guid.Empty;
        catID = 0;
        catName = "";
        citID = 0;
        citName = "";
        busName = "";
        busFormalName = "";
        busEnabled = false;
        busRating = 0;
        busSummary = "";
        busWebsite = "";
        busFacebookLink = "";
        busRepEMail = SiteSettings.GetValue("SupportEMail");
        busRepFName = "UTC";
        busRepLName = "Administrator";
        busRepPhone = "";
        busFacebookID = "";
    }

    /// <summary>
    /// Create a business info object from an id
    /// </summary>
    public SiteBusinessInfo(int id)
    {
        db = new WebDBContext();

        VwBusinesses rs = db.VwBusinesses.Single(target => target.BusID == id);

        // load the properties from the database object
        busID = id;
        busGuid = rs.BusGuid;
        catID = rs.CatID;
        catName = WebConvert.ToString(rs.CatName, "");
        citID = rs.CitID;
        citName = rs.CitName;
        busName = rs.BusName;
        busFormalName = rs.BusFormalName;
        busEnabled = rs.BusEnabled;
        busRating = (float)rs.BusRating;
        busSummary = rs.BusSummary;
        busWebsite = rs.BusWebsite;
        busFacebookLink = rs.BusFacebookLink;
        busRepEMail = rs.BusRepEMail;
        busRepFName = rs.BusRepFName;
        busRepLName = rs.BusRepLName;
        busRepPhone = rs.BusRepPhone;
        busFacebookID = rs.BusFacebookID;
    }

    /// <summary>
    /// Create a business info object from an accountbusiness view record
    /// </summary>
    public SiteBusinessInfo(VwAccountBusinesses rs)
    {
        db = new WebDBContext();

        // load the properties from the database object
        busID = rs.BusID;
        busGuid = rs.BusGuid;
        catID = rs.CatID;
        catName = WebConvert.ToString(rs.CatName, "");
        citID = rs.CitID;
        citName = rs.CitName;
        busName = rs.BusName;
        busFormalName = rs.BusFormalName;
        busEnabled = rs.BusEnabled;
        busRating = (float)rs.BusRating;
        busSummary = rs.BusSummary;
        busWebsite = rs.BusWebsite;
        busFacebookLink = rs.BusFacebookLink;
        busRepEMail = rs.BusRepEMail;
        busRepFName = rs.BusRepFName;
        busRepLName = rs.BusRepLName;
        busRepPhone = rs.BusRepPhone;
        busFacebookID = rs.BusFacebookID;
    }

    /// <summary>
    /// Get the image url for a business
    /// </summary>
    /// <param name="big">if true, use the big @2x image</param>
    /// <returns>image url for business image</returns>
    public string ImageUrl(bool big = false)
    {
        return GetImageUrl(BusGuid, big);
    }


    /// <summary>
    /// Count the number of redemptions in a time window
    /// </summary>
    /// <param name="startTS">start time (inclusive)</param>
    /// <param name="endTS">end time (not inclusive)</param>
    /// <returns></returns>
    protected int CountRedemptions(DateTime startTS, DateTime endTS)
    {
        return db.VwRedemptions.Count(target => target.BusID == busID && target.RedTS >= startTS && target.RedTS < endTS);
    }

    /// <summary>
    /// Count the number of checkins in a time window
    /// </summary>
    /// <param name="startTS">start time (inclusive)</param>
    /// <param name="endTS">end time (not inclusive)</param>
    /// <returns></returns>
    protected int CountCheckins(DateTime startTS, DateTime endTS)
    {
        return db.VwCheckIns.Count(target => target.BusID == busID && target.ChkTS >= startTS && target.ChkTS < endTS);
    }

    /// <summary>
    /// Count the number of favorites in a time window
    /// </summary>
    /// <param name="startTS">start time (inclusive)</param>
    /// <param name="endTS">end time (not inclusive)</param>
    /// <returns></returns>
    protected int CountFavorites(DateTime startTS, DateTime endTS)
    {
        return db.VwFavorites.Count(target => target.BusID == busID && target.FavTS >= startTS && target.FavTS < endTS);
    }

    /// <summary>
    /// Count the number of ratings in a time window
    /// </summary>
    /// <param name="startTS">start time (inclusive)</param>
    /// <param name="endTS">end time (not inclusive)</param>
    /// <returns></returns>
    protected int CountRatings(DateTime startTS, DateTime endTS)
    {
        return db.VwRatings.Count(target => target.BusID == busID && target.RatTS >= startTS && target.RatTS < endTS);
    }

    /// <summary>
    /// Count the number of tips in a time window
    /// </summary>
    /// <param name="startTS">start time (inclusive)</param>
    /// <param name="endTS">end time (not inclusive)</param>
    /// <returns></returns>
    protected int CountTips(DateTime startTS, DateTime endTS)
    {
        return db.VwTips.Count(target => target.BusID == busID && target.TipTS >= startTS && target.TipTS < endTS);
    }

    /// <summary>
    /// Count the number of social posts in a time window
    /// </summary>
    /// <param name="startTS">start time (inclusive)</param>
    /// <param name="endTS">end time (not inclusive)</param>
    /// <returns></returns>
    protected int CountSocial(DateTime startTS, DateTime endTS)
    {
        return db.VwSocialPosts.Count(target => target.BusID == busID && target.SopTS >= startTS && target.SopTS < endTS);
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

        // redemptions
        calc = new SummaryStats("Social Deals");
        calc.Link = "Redemptions";
        calc.Today = CountRedemptions(startOfToday, endOfToday);
        calc.PastWeek = CountRedemptions(startOfPastWeek, endOfPastWeek);
        calc.ThisPeriod = CountRedemptions(startOfThisMonth, endOfThisMonth);
        calc.LastPeriod = CountRedemptions(startOfLastMonth, endOfLastMonth);
        calc.AllTime = CountRedemptions(startOfTime, endOfTime);
        stats.Add(calc);

        // checkins
        calc = new SummaryStats("Loyalty Points");
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
        calc = new SummaryStats("Reviews");
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
