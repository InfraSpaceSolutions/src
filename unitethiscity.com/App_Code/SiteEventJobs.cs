/******************************************************************************
 * Filename: SiteEventJobs.cs
 * Project:  unitethiscity.com
 * 
 * Description:
 * Summary info class for an event job
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
/// Class for managing events through the site
/// </summary>
public class SiteEventJob
{
    protected WebDBContext db;
    /// <summary>
    /// Properties
    /// </summary>
    #region Properties
    public int EvjID { get; set; }
    public int BusID { get; set; }
    public int EttID { get; set; }
    public int EjtID { get; set; }
    public int EvjInterval { get; set; }
    public string EvjName { get; set; }
    public bool EvjEnabled { get; set; }
    public DateTime EvjBeginDate { get; set; }
    public DateTime EvjStopDate { get; set; }
    public int EvjDuration { get; set; }
    public int EvjDaysPublished { get; set; }
    public string EvjSummary { get; set; }
    public string EvjBody { get; set; }
    public string BusName { get; set; }
    public string EttName { get; set; }
    public string EjtName { get; set; }
    #endregion

    public SiteEventJob()
    {
        db = new WebDBContext();
    }

    public SiteEventJob(int id)
    {
        db = new WebDBContext();
        VwEventJobs rsEvj = db.VwEventJobs.Single(target => target.EvjID == id);
        Load(rsEvj);
    }

    public SiteEventJob(VwEventJobs rs)
    {
        db = new WebDBContext();
        Load(rs);
    }

    /// <summary>
    /// Load the object from a database record
    /// </summary>
    /// <param name="rs"></param>
    public void Load(VwEventJobs rs)
    {
        EvjID = rs.EvjID;
        BusID = rs.BusID;
        EttID = rs.EttID;
        EjtID = rs.EjtID;
        EvjInterval = rs.EvjInterval;
        EvjName = rs.EvjName;
        EvjEnabled = rs.EvjEnabled;
        EvjBeginDate = rs.EvjBeginDate;
        EvjStopDate = rs.EvjStopDate;
        EvjDuration = rs.EvjDuration;
        EvjDaysPublished = rs.EvjDaysPublished;
        EvjSummary = rs.EvjSummary;
        EvjBody = rs.EvjBody;
        BusName = rs.BusName;
        EttName = rs.EttName;
        EjtName = rs.EjtName;
    }

    /// <summary>
    /// Save the changes to the object to the database
    /// </summary>
    public void SaveChanges()
    {
        // get an existing tip from the database
        WebDBContext db = new WebDBContext();
        TblEventJobs rs = db.TblEventJobs.SingleOrDefault(target=>target.EvjID == EvjID && target.BusID == BusID);
        if (rs == null)
        {
            rs = new TblEventJobs();
            rs.BusID = BusID;
            db.TblEventJobs.InsertOnSubmit(rs);
        }

        rs.EttID = EttID;
        rs.EjtID = EjtID;
        rs.EvjInterval = EvjInterval;
        rs.EvjName = EvjName;
        rs.EvjEnabled = EvjEnabled;
        rs.EvjBeginDate = EvjBeginDate;
        rs.EvjStopDate = EvjStopDate;
        rs.EvjDuration = EvjDuration;
        rs.EvjDaysPublished = EvjDaysPublished;
        rs.EvjSummary = EvjSummary;
        rs.EvjBody = EvjBody;
        db.SubmitChanges();
    }

    /// <summary>
    /// Get a string version of the event date range.  Just the start date for single day events and
    /// start date - end date for multi-day events
    /// </summary>
    /// <returns>string of event date range</returns>
    public string EventDateToString()
    {
        string eventDateString;

        eventDateString = EvjBeginDate.ToShortDateString();
        if (EvjBeginDate< EvjStopDate)
        {
            eventDateString += " - " + EvjStopDate.ToShortDateString();
        }
        return eventDateString;
    }
}
