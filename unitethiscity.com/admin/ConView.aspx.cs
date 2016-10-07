/*****************************************************************************
 * Filename: ConView.aspx.cs
 * Project:  unitethiscity.com Administration
 * 
 * Description:
 * View Contact Forms.
 * 
 * Revision History:
 * $Log: $
******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using System.Configuration;
using System.IO;
using System.Data;
using Sancsoft.Web;

public partial class admin_ConView : System.Web.UI.Page
{
    int id;
    WebDBContext db = new WebDBContext();

    protected void Page_Load(object sender, EventArgs e)
    {
        // Wire events
        DeleteButton.Click += new EventHandler(DeleteButton_Click);

        // Get target id
        id = WebConvert.ToInt32(Request.QueryString["ID"], 0);

        // Verify id exists
        if (id == 0)
        {
            throw new WebException(RC.DataIncomplete);
        }

        if (!Page.IsPostBack)
        {
            // Get target Contact Form record
            TblContactUs rs = db.TblContactUs.SingleOrDefault(target => target.ConID == id);

            // Verify target record exists
            if (rs == null)
            {
                throw new WebException(RC.TargetDNE);
            }

            // Populate page
            ConIDLabel.Text = rs.ConID.ToString( );
            ConFNameLabel.Text = rs.ConFName;
            ConLNameLabel.Text = rs.ConLName;
            ConEMailHyperLink.Text = rs.ConEMail;
            ConEMailHyperLink.NavigateUrl = "mailto:" + rs.ConEMail;
            ConMemberLabel.Text = ( rs.ConMember == true ) ? "Yes" : "No";
            ConPartnerLabel.Text = ( rs.ConPartner == true ) ? "Yes" : "No";
            ConCommentsLiteral.Text = WebConvert.PreserveBreaks(rs.ConComments);
            ConTimestampLabel.Text = rs.ConTimestamp.ToString();
        }
    }

    void DeleteButton_Click(object sender, EventArgs e)
    {
        // Get the target Contact Form record
        TblContactUs rsCon = db.TblContactUs.SingleOrDefault(target => target.ConID == id);

        // Verify target record exists
        if (rsCon == null)
        {
            throw new WebException(RC.TargetDNE);
        }

        // Get name for delete message
        string name = rsCon.ConFName + " " + rsCon.ConLName;

        // Delete the record
        db.TblContactUs.DeleteOnSubmit(rsCon);
        db.SubmitChanges();

        // Redirect to list page
        Response.Redirect("ConList.aspx?Name=" + name.ToString());
    }
}