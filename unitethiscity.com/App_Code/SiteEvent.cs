/******************************************************************************
 * Filename: SiteEvent.cs
 * Project:  unitethiscity.com
 * 
 * Description:
 * Summary info class for an event.
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
public class SiteEvent
{
    protected WebDBContext db;
    /// <summary>
    /// Properties
    /// </summary>
    #region Properties
    public int EvtID { get; set; }
    public int EvjID { get; set; }
    public int EttID { get; set; }
    public int BusID { get; set; }
    public DateTime EvtStartDate { get; set; }
    public DateTime EvtEndDate { get; set; }
    public string EvtSummary { get; set; }
    public string EvtBody { get; set; }
    public string EttName { get; set; }
    public string BusName { get; set; }
    public string BusFormalName { get; set; }
    public string EvjName { get; set; }
    public string EvtLink { get; set; }
    #endregion

    public SiteEvent()
    {
        db = new WebDBContext();
        EvtLink = "";
    }

    public SiteEvent(int id)
    {
        db = new WebDBContext();
        VwEvents rsEvt = db.VwEvents.Single(target => target.EvtID == id);
        Load(rsEvt);
    }

    public SiteEvent(VwEvents rs)
    {
        db = new WebDBContext();
        Load(rs);
    }

    /// <summary>
    /// Load the object from a database record
    /// </summary>
    /// <param name="rs"></param>
    public void Load(VwEvents rs)
    {
        EvtID = rs.EvtID;
        EvjID = rs.EvjID;
        EttID = rs.EttID;
        BusID = rs.BusID;
        EvtStartDate = rs.EvtStartDate;
        EvtEndDate = rs.EvtEndDate;
        EvtSummary = rs.EvtSummary;
        EvtBody = rs.EvtBody;
        EttName = rs.EttName;
        BusName = rs.BusName;
        BusFormalName = rs.BusFormalName;
        EvjName = rs.EvjName;

        TblEventLinks rsEventLink = db.TblEventLinks.SingleOrDefault(target => target.EvtID == rs.EvtID);
        EvtLink = (rsEventLink != null) ? rsEventLink.EvtLinkName : "";
    }

    /// <summary>
    /// Save the changes to the object to the database
    /// </summary>
    public void SaveChanges()
    {
        // get an existing tip from the database
        WebDBContext db = new WebDBContext();
        TblEvents rs = db.TblEvents.SingleOrDefault(target => target.EvtID == EvtID && target.BusID == BusID);
        if (rs == null)
        {
            rs = new TblEvents();
            rs.EvjID = 0;
            rs.BusID = BusID;
            db.TblEvents.InsertOnSubmit(rs);
        }

        rs.EttID = EttID;
        rs.EvtStartDate = EvtStartDate;
        rs.EvtEndDate = EvtEndDate;
        rs.EvtSummary = EvtSummary;
        rs.EvtBody = EvtBody;
        db.SubmitChanges();

        // upsert or delete the event link
        TblEventLinks rsEventLink = db.TblEventLinks.SingleOrDefault(target => target.EvtID == rs.EvtID);
        if (EvtLink.Length > 0)
        {
            if (rsEventLink == null)
            {
                rsEventLink = new TblEventLinks();
                rsEventLink.EvtID = rs.EvtID;
                db.TblEventLinks.InsertOnSubmit(rsEventLink);
            }
            rsEventLink.EvtLinkName = EvtLink;
            db.SubmitChanges();
        }
        else
        {
            if (rsEventLink != null)
            {
                db.TblEventLinks.DeleteOnSubmit(rsEventLink);
                db.SubmitChanges();
            }
        }
    }

    /// <summary>
    /// Get a string version of the event date range.  Just the start date for single day events and
    /// start date - end date for multi-day events
    /// </summary>
    /// <returns>string of event date range</returns>
    public string EventDateToString()
    {
        string eventDateString;

        eventDateString = EvtStartDate.ToShortDateString();
        if (EvtStartDate < EvtEndDate)
        {
            eventDateString += " - " + EvtEndDate.ToShortDateString();
        }
        return eventDateString;
    }

    /// <summary>
    /// Get a string version of the event date range.  Just the start date for single day events and
    /// start date - end date for multi-day events
    /// </summary>
    /// <param name="start">start date</param>
    /// <param name="end">end date</param>
    /// <returns>string of event date range</returns>
    public static string EventDateToString(DateTime start, DateTime end)
    {
        string eventDateString;

        eventDateString = start.ToShortDateString();
        if (start < end)
        {
            eventDateString += " - " + end.ToShortDateString();
        }
        return eventDateString;
    }

}
