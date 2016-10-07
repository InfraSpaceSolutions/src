/******************************************************************************
 * Filename: Rating.cs
 * Project:  unitethiscity.com
 * 
 * Description:
 * Handle changes to the ratings of locations and indirectly, businesses.
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
/// Utility class for handling ratings
/// </summary>
public static class Rating
{
    /// <summary>
    /// Get the rating that a member has assigned a location.  If no rating has
    /// been assigned - you get zero.
    /// </summary>
    /// <param name="accID">member</param>
    /// <param name="locID">location</param>
    /// <returns>rating 1-5 or 0 if no rating</returns>
    public static int Get(int accID, int locID)
    {
        int rating = 0;
        WebDBContext db = new WebDBContext();

        // get an existing rating
        TblRatings rs = db.TblRatings.SingleOrDefault(target => target.AccID == accID && target.LocID == locID);
        if (rs != null)
        {
            rating = rs.RatRating;
        }

        return rating;
    }

    /// <summary>
    /// Convert a rating to an image number from 0-10 which corresponds to 0-5 with
    /// half stars.
    /// </summary>
    /// <param name="rating">rating as double</param>
    /// <returns>rating as integer 0-10</returns>
    public static int ImageNumber(double rating)
    {
        rating = (rating * 2) + 0.5;
        rating = Math.Min(10, rating);
        rating = Math.Max(0, rating);
        return (int)(rating);
    }

    /// <summary>
    /// Assign a rating to a location.  This will update an existing rating
    /// or create a new rating if one doesn't exist.
    /// </summary>
    /// <param name="accID">account</param>
    /// <param name="locID">location</param>
    /// <param name="rating">rating (1-5)</param>
    public static void Assign(int accID, int locID, int rating)
    {
        WebDBContext db = new WebDBContext();

        // limit the rating value from 1 to 5
        rating = Math.Min(5, rating);
        rating = Math.Max(1, rating);

        // get the existing rating or create a new record
        TblRatings rs = db.TblRatings.SingleOrDefault(target => target.AccID == accID && target.LocID == locID);
        if (rs == null)
        {
            rs = new TblRatings();
            rs.AccID = accID;
            rs.LocID = locID;
            db.TblRatings.InsertOnSubmit(rs);
        }
        // include the update/or new data
        rs.RatRating = rating;
        rs.RatTS = DateTime.Now;
        db.SubmitChanges();

        // recalculate the corresponding location and business
        Recalculate(locID);
    }

    /// <summary>
    /// Remove a rating of a location by a member.  If a rating is found and
    /// removed, the sums are updated
    /// </summary>
    /// <param name="accID">account</param>
    /// <param name="locID">location</param>
    public static void Remove(int accID, int locID)
    {
        WebDBContext db = new WebDBContext();

        // if there is no rating, do nothing.
        if (db.TblRatings.Count(target => target.LocID == locID && target.AccID == accID) <= 0)
        {
            return;
        }

        // delete the rating record
        db.TblRatings.DeleteOnSubmit(db.TblRatings.Single(target => target.LocID == locID && target.AccID == accID));
        db.SubmitChanges();

        // recalculate the corresponding location and business
        Recalculate(locID);
    }

    /// <summary>
    /// Recalculate the location rating and the business rating
    /// </summary>
    /// <param name="locID">location</param>
    public static void Recalculate(int locID)
    {
        WebDBContext db = new WebDBContext();
        
        // get the target records
        TblLocations rsLoc = db.TblLocations.Single( target => target.LocID == locID);
        TblBusinesses rsBus = db.TblBusinesses.Single( target => target.BusID == rsLoc.BusID);

        // calculate the new values
        double locSum = db.TblRatings.Where(target => target.LocID == locID).Sum(target => target.RatRating);
        double busSum = db.VwBusinessRatings.Where(target => target.BusID == rsLoc.BusID).Sum(target => target.RatRating);
        int locCount = db.TblRatings.Where(target => target.LocID == locID).Count();
        int busCount = db.VwBusinessRatings.Where(target => target.BusID == rsLoc.BusID).Count();

        // update the ratings and store to database
        rsLoc.LocRating = (locCount > 0) ? locSum / (double)locCount : 0;
        rsBus.BusRating = (busCount > 0) ? busSum / (double)busCount : 0;
        db.SubmitChanges();

        // update the revision level
        DataRevision.Bump(Revisioned.LocationInfo);
    }

}
