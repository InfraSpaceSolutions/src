/******************************************************************************
 * Filename: SiteMessageJob.cs
 * Project:  unitethiscity.com
 * 
 * Description:
 * Summary info class for a message job
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
public class SiteMessageJob
{
    protected WebDBContext db;
    /// <summary>
    /// Properties
    /// </summary>
    #region Properties
    public int MsjID { get; set; }
    public int MsgID { get; set; }
    public int MjsID { get; set; }
    public int RolID { get; set; }
    public int BusID { get; set; }
    public DateTime MsjSendTS { get; set; }
    public string MjsName { get; set; }
    public int MsgFromID { get; set; }
    public string MsgFromName { get; set; }
    public string MsgSummary { get; set; }
    public string MsgBody { get; set; }
    public DateTime MsgTS { get; set; }
    public DateTime MsgExpires { get; set; }
    public string RolName { get; set; }
    public Guid BusGuid { get; set; }
    public string BusName { get; set; }
    public string BusFormalName { get; set; }
    #endregion

    public SiteMessageJob()
    {
        db = new WebDBContext();
    }

    public SiteMessageJob(int id)
    {
        db = new WebDBContext();
        VwMessageJobs rsMsj = db.VwMessageJobs.Single(target => target.MsjID == id);
        Load(rsMsj);
    }

    public SiteMessageJob(VwMessageJobs rs)
    {
        db = new WebDBContext();
        Load(rs);
    }

    /// <summary>
    /// Load the object from a database record
    /// </summary>
    /// <param name="rs"></param>
    public void Load(VwMessageJobs rs)
    {
        MsjID = rs.MsjID;
        MsgID = rs.MsgID;
        MjsID = rs.MjsID;
        RolID = rs.RolID;
        BusID = rs.BusID;
        MsjSendTS = rs.MsjSendTS;
        MjsName = rs.MjsName;
        MsgFromID = rs.MsgFromID;
        MsgFromName = rs.MsgFromName;
        MsgSummary = rs.MsgSummary;
        MsgBody = rs.MsgBody;
        MsgTS = rs.MsgTS;
        MsgExpires = rs.MsgExpires;
        RolName = rs.RolName;
        BusGuid = rs.BusGuid ?? Guid.Empty;
        BusName = rs.BusName;
        BusFormalName = rs.BusFormalName;
    }

    /// <summary>
    /// Save the changes to the object to the database
    /// </summary>
    public void SaveChanges()
    {
        // get an existing record from the database
        WebDBContext db = new WebDBContext();
        TblMessageJobs rsMsj = db.TblMessageJobs.SingleOrDefault(target=>target.MsjID == MsjID && target.BusID == BusID);
        if (rsMsj == null)
        {
            // we don't have a message yet - create an empty one
            TblMessages rsNewMsg = new TblMessages();
            rsNewMsg.MsgFromID = 0;
            rsNewMsg.MsgFromName = "";
            rsNewMsg.MsgSummary = "<template message>";
            rsNewMsg.MsgBody = "";
            rsNewMsg.MsgTS = DateTime.Now;
            rsNewMsg.MsgExpires = DateTime.Now;
            db.TblMessages.InsertOnSubmit(rsNewMsg);
            db.SubmitChanges();

            // capture the id of our template message into the object
            MsgID = rsNewMsg.MsgID;

            // create the new message job for insertion
            rsMsj = new TblMessageJobs();
            db.TblMessageJobs.InsertOnSubmit(rsMsj);
        }

        // update the associated message fields
        TblMessages rsMsg = db.TblMessages.Single(target => target.MsgID == MsgID);
        rsMsg.MsgFromID = MsgFromID;
        rsMsg.MsgFromName = MsgFromName;
        rsMsg.MsgSummary = MsgSummary;
        rsMsg.MsgBody = MsgBody;
        rsMsg.MsgTS = MsgTS;
        rsMsg.MsgExpires = MsgExpires;

        // update the message job itself
        rsMsj.MsgID = MsgID;
        rsMsj.MjsID = MjsID;
        rsMsj.RolID = RolID;
        rsMsj.BusID = BusID;
        rsMsj.MsjSendTS = MsjSendTS;

        // save the changes to the job and the messasge
        db.SubmitChanges();
        MsjID = rsMsj.MsjID;
    }

    /// <summary>
    /// See if the user can edit the messsage - can't do it if already sent
    /// </summary>
    /// <returns>true - allow user to edit</returns>
    public bool CanUserEdit()
    {
        return (MjsID == (int)MessageJobStates.Definition);
    }

    /// <summary>
    /// See if the user can send the messsage - can't do it if already sent
    /// </summary>
    /// <returns>true - allow user to edit</returns>
    public bool CanUserSend()
    {
        return (MjsID == (int)MessageJobStates.Definition);
    }

    /// <summary>
    /// See if the user can cancel the messsage - can't do it if already sent
    /// </summary>
    /// <returns>true - allow user to edit</returns>
    public bool CanUserCancel()
    {
        return (MjsID == (int)MessageJobStates.Queued);
    }

    /// <summary>
    /// Queue the message for sending
    /// </summary>
    public void QueueForSend()
    {
        MjsID = (int)MessageJobStates.Queued;
    }

    /// <summary>
    /// Dequeue the message back to definition
    /// </summary>
    public void CancelSend()
    {
        MjsID = (int)MessageJobStates.Definition;
    }

}
