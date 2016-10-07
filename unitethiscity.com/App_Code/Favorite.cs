/******************************************************************************
 * Filename: Favorite.cs
 * Project:  unitethiscity.com
 * 
 * Description:
 * Handle changes to the favorite locations for a member
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
/// Utility class for handling favorites
/// </summary>
public static class Favorite
{
    /// <summary>
    /// Check to see if a specified location has been favorited by a specified member
    /// </summary>
    /// <param name="accID">member</param>
    /// <param name="locID">location</param>
    /// <returns></returns>
    public static bool IsFavorite(int accID, int locID)
    {
        WebDBContext db = new WebDBContext();
        return (db.TblFavorites.Count(target => target.LocID == locID && target.AccID == accID) > 0);
    }

    /// <summary>
    /// Add a favorite record for the account/location pair
    /// </summary>
    /// <param name="accID">member</param>
    /// <param name="locID">location</param>
    public static void Add(int accID, int locID)
    {
        WebDBContext db = new WebDBContext();

        // if it is already a favorite - don't do anything
        if (db.TblFavorites.Count(target => target.LocID == locID && target.AccID == accID) > 0)
        {
            return;
        }

        // create a favorite record for this location and user pair
        TblFavorites rs = new TblFavorites();
        rs.AccID = accID;
        rs.LocID = locID;
        rs.FavTS = DateTime.Now;
        db.TblFavorites.InsertOnSubmit(rs);
        db.SubmitChanges();
    }

    /// <summary>
    /// Remove a favorite record for an account/location pair
    /// </summary>
    /// <param name="accID">member</param>
    /// <param name="locID">location</param>
    public static void Remove(int accID, int locID)
    {
        WebDBContext db = new WebDBContext();

        // if it is not already a favorite - don't do anything
        if (db.TblFavorites.Count(target => target.LocID == locID && target.AccID == accID) <= 0)
        {
            return;
        }

        // delete the favorite record
        db.TblFavorites.DeleteOnSubmit(db.TblFavorites.Single(target => target.LocID == locID && target.AccID == accID));
        db.SubmitChanges();
    }
}
