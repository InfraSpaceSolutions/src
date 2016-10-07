/******************************************************************************
 * Filename: MsjView.aspx.cs
 * Project:  unitethiscity.com Administration
 * 
 * Description:
 * View a record
 * 
 * Revision History:
 * $Log: $
******************************************************************************/
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sancsoft.Web;

public partial class admin_MsjView : System.Web.UI.Page
{
    int id;
    WebDBContext db = new WebDBContext();

    protected void Page_Load(object sender, EventArgs e)
    {
        // Wire events
        EditButton.Click += new EventHandler(EditButton_Click);
        DeleteButton.Click += new EventHandler(DeleteButton_Click);

        // Get the target id
        id = WebConvert.ToInt32(Request.QueryString["ID"], 0);

        // Verify id exists
        if (id == 0)
        {
            throw new WebException(RC.DataIncomplete);
        }

        if (!Page.IsPostBack)
        {
            // Get the record
            VwMessageJobs rs = db.VwMessageJobs.SingleOrDefault(target => target.MsjID == id);

            // Verify target record exists
            if (rs == null)
            {
                throw new WebException(RC.TargetDNE);
            }

            // Populate the page
            MsjIDLiteral.Text = rs.MsjID.ToString();
            AccNameLiteral.Text = rs.AccEMail;
            MjsNameLiteral.Text = rs.MjsName;
            MsjSendTSLiteral.Text = rs.MsjSendTS.ToString();
            MsjToLiteral.Text = rs.RolName + " " + rs.BusName;
            MsgFromNameLiteral.Text = rs.MsgFromName;
            MsgSummaryLiteral.Text = rs.MsgSummary;
            MsgBodyLiteral.Text = WebConvert.PreserveBreaks(rs.MsgBody);
            MsgTSLiteral.Text = rs.MsgTS.ToString();
            MsgExpiresLiteral.Text = rs.MsgExpires.ToString();
        }
    }

    void DeleteButton_Click(object sender, EventArgs e)
    {
        // Get the record
        TblMessageJobs rs = db.TblMessageJobs.Single(Target => Target.MsjID == id);

        // Delete the record
        db.TblMessageJobs.DeleteOnSubmit(rs);

        // sync to database
        db.SubmitChanges();

        // Redirect to list page
        Response.Redirect("MsjList.aspx");
    }

    void EditButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("MsjEdit.aspx?ID=" + id.ToString());
    }
}