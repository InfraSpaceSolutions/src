/******************************************************************************
 * Filename: SiteMember.cs
 * Project:  unitethiscity.com
 * 
 * Description:
 * Class for managing the membership of the authenticated user.
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
/// User interface to membership 
/// </summary>
public class SiteMember
{
    WebDBContext db;

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
    protected Guid accGuid;
    public Guid AccGuid
    {
        get
        {
            return accGuid;
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
    protected string accEMail;
    public string AccEMail
    {
        get
        {
            return accEMail;
        }
    }
    protected string accFName;
    public string AccFName
    {
        get
        {
            return accFName;
        }
    }
    protected string accLName;
    public string AccLName
    {
        get
        {
            return accLName;
        }
    }

    protected string accPhone;
    public string AccPhone
    {
        get
        {
            return accPhone;
        }
    }
    #endregion Properties

    /// <summary>
    /// Create a membership object for the authenticated user
    /// </summary>
    public SiteMember()
    {
        db = new WebDBContext();
        accID = WebConvert.ToInt32(HttpContext.Current.Session["ACCOUNT_ID"], 0);
        if (accID == 0)
        {
            throw new WebException(RC.InternalError);
        }

        TblAccounts rs = db.TblAccounts.Single(target => target.AccID == accID);

        // make sure that the account is enabled
        if (!rs.AccEnabled)
        {
            throw new WebException(RC.AccountDisabled);
        }

        // make sure that the account is a member
        if (db.TblAccountRoles.Count(target => (target.RolID == (int)Roles.Member) && (target.AccID == AccID)) == 0)
        {
            throw new WebException(RC.AccessDenied);
        }

        // load the object properties
        accGuid = rs.AccGuid;
        citID = rs.CitID;
        accEMail = rs.AccEMail;
        accFName = rs.AccFName;
        accLName = rs.AccLName;
        accPhone = rs.AccPhone;
    }

    /// <summary>
    /// Get a list of locations that are the member's favorites
    /// </summary>
    /// <returns>List of location information</returns>
    public List<SiteLocationInfo> MemberFavorites()
    {
        // get this user's favorites as a list of location identifiers
        var rsFav = from fav in db.TblFavorites
                    where fav.AccID == accID
                    select fav.LocID;
        List<int> favList = rsFav.ToList();

        // get the locations that are in the list of favorites
        var rsLoc = from loc in db.VwLocations
                    where favList.Contains(loc.LocID)
                    orderby loc.LocName
                    select loc;

        // build a list of objects representing the favorites
        List<SiteLocationInfo> locList = new List<SiteLocationInfo>();
        foreach (VwLocations loc in rsLoc)
        {
            locList.Add(new SiteLocationInfo(loc));
        }

        return locList;
    }

    /// <summary>
    /// Add a location to the member's favorites
    /// </summary>
    /// <param name="locID">id of location to favorite</param>
    public void AddFavorite(int locID)
    {
        Favorite.Add(accID, locID);
    }

    /// <summary>
    /// Remove a location from the member's favorites
    /// </summary>
    /// <param name="locID">id of location to un-favorite</param>
    public void RemoveFavorite(int locID)
    {
        Favorite.Remove(accID, locID);
    }

    /// <summary>
    /// Submit a rating by this member against a location
    /// </summary>
    /// <param name="locID">location</param>
    /// <param name="rating">rating 1-5</param>
    public void SubmitRating(int locID, int rating)
    {
        Rating.Assign(accID, locID, rating);
    }

    /// <summary>
    /// Gets the tip object for a member for the 
    /// </summary>
    /// <param name="locID"></param>
    /// <returns></returns>
    public Tip GetTip(int locID)
    {
        return new Tip(accID, locID);
    }

    /// <summary>
    /// Count the number of redemptions in a time window
    /// </summary>
    /// <param name="startTS">start time (inclusive)</param>
    /// <param name="endTS">end time (not inclusive)</param>
    /// <returns></returns>
    protected int CountRedemptions(DateTime startTS, DateTime endTS)
    {
        return db.VwRedemptions.Count(target => target.AccID == AccID && target.RedTS >= startTS && target.RedTS < endTS);
    }

    /// <summary>
    /// Count the number of checkins in a time window
    /// </summary>
    /// <param name="startTS">start time (inclusive)</param>
    /// <param name="endTS">end time (not inclusive)</param>
    /// <returns></returns>
    protected int CountCheckins(DateTime startTS, DateTime endTS)
    {
        return db.VwCheckIns.Count(target => target.AccID == AccID && target.ChkTS >= startTS && target.ChkTS < endTS);
    }

    /// <summary>
    /// Count the number of favorites in a time window
    /// </summary>
    /// <param name="startTS">start time (inclusive)</param>
    /// <param name="endTS">end time (not inclusive)</param>
    /// <returns></returns>
    protected int CountFavorites(DateTime startTS, DateTime endTS)
    {
        return db.VwFavorites.Count(target => target.AccID == AccID && target.FavTS >= startTS && target.FavTS < endTS);
    }

    /// <summary>
    /// Count the number of ratings in a time window
    /// </summary>
    /// <param name="startTS">start time (inclusive)</param>
    /// <param name="endTS">end time (not inclusive)</param>
    /// <returns></returns>
    protected int CountRatings(DateTime startTS, DateTime endTS)
    {
        return db.VwRatings.Count(target => target.AccID == AccID && target.RatTS >= startTS && target.RatTS < endTS);
    }

    /// <summary>
    /// Count the number of tips in a time window
    /// </summary>
    /// <param name="startTS">start time (inclusive)</param>
    /// <param name="endTS">end time (not inclusive)</param>
    /// <returns></returns>
    protected int CountTips(DateTime startTS, DateTime endTS)
    {
        return db.VwTips.Count(target => target.AccID == AccID && target.TipTS >= startTS && target.TipTS < endTS);
    }

    /// <summary>
    /// Count the number of social posts in a time window
    /// </summary>
    /// <param name="startTS">start time (inclusive)</param>
    /// <param name="endTS">end time (not inclusive)</param>
    /// <returns></returns>
    protected int CountSocial(DateTime startTS, DateTime endTS)
    {
        return db.VwSocialPosts.Count(target => target.AccID == accID && target.SopTS >= startTS && target.SopTS < endTS);
    }

    /// <summary>
    /// Generate the summary stats for the member
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

