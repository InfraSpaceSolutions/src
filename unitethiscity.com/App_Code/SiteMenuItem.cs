/******************************************************************************
 * Filename: SiteMenuItem.cs
 * Project:  unitethiscity.com
 * 
 * Description:
 * Summary info class for a menu item.
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
/// Class for managing menu items through the site
/// </summary>
public class SiteMenuItem
{
    protected WebDBContext db;
    /// <summary>
    /// Properties
    /// </summary>
    #region Properties
    public int MenID { get; set; }
    public int BusID { get; set; }
    public int MenSeq { get; set; }
    public string MenName { get; set; }
    public decimal MenPrice { get; set; }
    #endregion

    public SiteMenuItem()
    {
        db = new WebDBContext();
    }

    public SiteMenuItem(int id)
    {
        db = new WebDBContext();
        TblMenuItems rsMen = db.TblMenuItems.Single(target => target.MenID == id);
        Load(rsMen);
    }

    public SiteMenuItem(TblMenuItems rs)
    {
        db = new WebDBContext();
        Load(rs);
    }

    /// <summary>
    /// Load the object from a database record
    /// </summary>
    /// <param name="rs"></param>
    public void Load(TblMenuItems rs)
    {
        MenID = rs.MenID;
        BusID = rs.BusID;
        MenSeq = rs.MenSeq;
        MenName = rs.MenName;
        MenPrice = rs.MenPrice;
    }

    /// <summary>
    /// Save the changes to the object to the database
    /// </summary>
    public void SaveChanges()
    {
        // get an existing tip from the database
        WebDBContext db = new WebDBContext();
        TblMenuItems rs = db.TblMenuItems.SingleOrDefault(target=>target.MenID == MenID && target.BusID == BusID);
        if (rs == null)
        {
            // Get the next sequence
            var maxSeq = db.TblMenuItems.Where( target => target.BusID == BusID );
            int menSeq = 1;
            if ( maxSeq.Any() )
            {
                menSeq += maxSeq.Max( target => target.MenSeq );
            }

            rs = new TblMenuItems();
            rs.BusID = BusID;
            rs.MenSeq = menSeq;

            db.TblMenuItems.InsertOnSubmit(rs);
        }

        rs.MenName = MenName;
        rs.MenPrice = MenPrice;
        db.SubmitChanges();
    }
}
