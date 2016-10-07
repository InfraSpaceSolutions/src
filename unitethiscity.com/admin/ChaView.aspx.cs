/*****************************************************************************
 * Filename: ChaView.aspx.cs
 * Project:  unitethiscity.com Administration
 * 
 * Description:
 * View charity registration Forms.
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

public partial class admin_ChaView : System.Web.UI.Page
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
            // Get target record
            TblCharityRegistrations rs = db.TblCharityRegistrations.SingleOrDefault( target => target.ChaID == id );

            // Verify target record exists
            if (rs == null)
            {
                throw new WebException(RC.TargetDNE);
            }

            // Populate page
            ChaIDLabel.Text = rs.ChaID.ToString();
            ChaFNameLabel.Text = rs.ChaFName;
            ChaLNameLabel.Text = rs.ChaLName;
            ChaCharityNameLabel.Text = rs.ChaCharityName;
            ChaEMailHyperLink.Text = rs.ChaEMail;
            ChaEMailHyperLink.NavigateUrl = "mailto:" + rs.ChaEMail;
            ChaPhoneLabel.Text = rs.ChaPhone;
            ChaAdditionalInfoLiteral.Text = WebConvert.PreserveBreaks(rs.ChaAdditionalInfo);
            ChaTimestampLabel.Text = rs.ChaTimestamp.ToString();
        }
    }

    void DeleteButton_Click(object sender, EventArgs e)
    {
        // Get the target record
        TblCharityRegistrations rsCha = db.TblCharityRegistrations.SingleOrDefault(target => target.ChaID == id);

        // Verify target record exists
        if (rsCha == null)
        {
            throw new WebException(RC.TargetDNE);
        }

        // Get name for delete message
        string name = rsCha.ChaFName + " " + rsCha.ChaLName;

        // Delete the record
        db.TblCharityRegistrations.DeleteOnSubmit(rsCha);
        db.SubmitChanges();

        // Redirect to list page
        Response.Redirect("ChaList.aspx?Name=" + name.ToString());
    }
}