/*****************************************************************************
 * Filename: BurView.aspx.cs
 * Project:  unitethiscity.com Administration
 * 
 * Description:
 * View business registration Forms.
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

public partial class admin_BurView : System.Web.UI.Page
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
            // Get target business registration Form record
            TblBusinessRegistrations rs = db.TblBusinessRegistrations.SingleOrDefault( target => target.BurID == id );

            // Verify target record exists
            if (rs == null)
            {
                throw new WebException(RC.TargetDNE);
            }

            // Populate page
            BurIDLabel.Text = rs.BurID.ToString( );
            BurFNameLabel.Text = rs.BurFName;
            BurLNameLabel.Text = rs.BurLName;
            BurBusinessNameLabel.Text = rs.BurBusinessName;
            BurCategoryLabel.Text = rs.BurCategory;
            BurEMailHyperLink.Text = rs.BurEMail;
            BurEMailHyperLink.NavigateUrl = "mailto:" + rs.BurEMail;
            BurPhoneLabel.Text = rs.BurPhone;
            BurAdditionalInfoLiteral.Text = WebConvert.PreserveBreaks( rs.BurAdditionalInfo );
            BurTimestampLabel.Text = rs.BurTimestamp.ToString( );
        }
    }

    void DeleteButton_Click(object sender, EventArgs e)
    {
        // Get the target business registration Form record
        TblBusinessRegistrations rsBur = db.TblBusinessRegistrations.SingleOrDefault( target => target.BurID == id );

        // Verify target record exists
        if( rsBur == null )
        {
            throw new WebException(RC.TargetDNE);
        }

        // Get name for delete message
        string name = rsBur.BurFName + " " + rsBur.BurLName;

        // Delete the record
        db.TblBusinessRegistrations.DeleteOnSubmit( rsBur );
        db.SubmitChanges();

        // Redirect to list page
        Response.Redirect( "BurList.aspx?Name=" + name.ToString( ) );
    }
}