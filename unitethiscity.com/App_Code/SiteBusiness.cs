/******************************************************************************
 * Filename: SiteBusiness.cs
 * Project:  unitethiscity.com
 * 
 * Description:
 * Class for managing a business object.  This can be instantiated from the
 * active session variable or against a specific id/guid.
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
/// Manage a business through the web interface
/// </summary>
public class SiteBusiness
{
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
            return BusGuid;
        }
    }

    public int CatID
    {
        get;
        set;
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
    public string BusName
    {
        get;
        set;
    }

    public string BusFormalName
    {
        get;
        set;
    }
    private bool busEnabled;
    public bool BusEnabled
    {
        get 
        {
            return busEnabled;
        }
    }
    private double busRating;
    public double BusRating
    {
        get
        {
            return busRating;
        }
    }

    public string BusWebsite
    {
        get;
        set;
    }

    public string BusFacebookLink
    {
        get;
        set;
    }

    public int BusAssignedDldID
    {
        get;
        set;
    }

    public bool BusRequirePin
    {
        get;
        set;
    }

    public string BusFacebookID
    {
        get;
        set;
    }
    #endregion Properties

    /// <summary>
    /// Create a business object using the current session variable; throw exception if not found
    /// </summary>
    public SiteBusiness()
    {
        busID = WebConvert.ToInt32(HttpContext.Current.Session["BUSINESS_ID"], 0);
        if (busID == 0)
        {
            throw new WebException(RC.InternalError);
        }
        Load(busID);
    }

    /// <summary>
    /// Create a business object using the busid value
    /// </summary>
    public SiteBusiness(int id)
    {
        Load(id);
    }

    /// <summary>
    /// Load the object from the database
    /// </summary>
    /// <param name="id">busid of the target</param>
    protected void Load(int id)
    {
        busID = id;
        WebDBContext db = new WebDBContext();
        VwBusinesses rs = db.VwBusinesses.Single(target => target.BusID == busID);
        busGuid = rs.BusGuid;
        CatID = rs.CatID;
        catName = WebConvert.ToString(rs.CatName, "");
        citID = rs.CitID;
        citName = rs.CitName;
        BusName = rs.BusName;
        BusFormalName = rs.BusFormalName;
        busEnabled = rs.BusEnabled;
        busRating = rs.BusRating;
        BusWebsite = rs.BusWebsite;
        BusFacebookLink = rs.BusFacebookLink;
        BusAssignedDldID = rs.BusAssignedDldID;
        BusRequirePin = rs.BusRequirePin;
        BusFacebookID = rs.BusFacebookID;
    }

    /// <summary>
    /// Save changes to the account information
    /// </summary>
    /// <returns>true if changes saved</returns>
    public bool SaveChanges()
    {
        WebDBContext db = new WebDBContext();

        // TODO - figure out how we should handle undefined category identifiers

        // save the changes to the account object
        TblBusinesses rs = db.TblBusinesses.Single(target => target.BusID == busID);
        rs.BusName = WebConvert.Truncate(BusName, 50);
        rs.BusFormalName = WebConvert.Truncate(BusFormalName, 128);
        rs.BusWebsite = WebConvert.Truncate(BusWebsite, 255);
        rs.BusFacebookLink= WebConvert.Truncate(BusFacebookLink, 255);
        rs.BusAssignedDldID = BusAssignedDldID;
        rs.BusRequirePin = BusRequirePin;
        rs.BusFacebookID = WebConvert.Truncate(BusFacebookID, 50);
        rs.CatID = CatID;

        db.SubmitChanges();

        // update the revision level
        DataRevision.Bump(Revisioned.LocationInfo);

        return true;
    }

    /// <summary>
    /// Format a member name as first name last initial
    /// </summary>
    /// <param name="fname">nullable first name</param>
    /// <param name="lname">nullable last name</param>
    /// <returns>Firstname L.</returns>
    public static string FormatMemberName(string fname, string lname)
    {
        string firstname = fname ?? "Guest";
        string lastname = lname ?? "User";

        return firstname + " " + lastname.Substring(0, 1) + ".";
    }
}
