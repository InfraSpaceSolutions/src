/*****************************************************************************
 * Filename: TstView.aspx.cs
 * Project:  unitethiscity.com Administration
 * 
 * Description:
 * View testimonial Forms.
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

public partial class admin_TstView : System.Web.UI.Page
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
            // Get target testimonial Form record
            TblTestimonials rs = db.TblTestimonials.SingleOrDefault( target => target.TstID == id );

            // Verify target record exists
            if (rs == null)
            {
                throw new WebException(RC.TargetDNE);
            }

            // Populate page
            TstIDLabel.Text = rs.TstID.ToString( );
            TstFNameLabel.Text = rs.TstFName;
            TstLNameLabel.Text = rs.TstLName;
            TstEMailHyperLink.Text = rs.TstEMail;
            TstEMailHyperLink.NavigateUrl = "mailto:" + rs.TstEMail;
            TstMemberLabel.Text = ( rs.TstMember == true ) ? "Yes" : "No";
            TstPartnerLabel.Text = ( rs.TstPartner == true ) ? "Yes" : "No";
            TstTestimonialLiteral.Text = WebConvert.PreserveBreaks( rs.TstTestimonial );
            TstTimestampLabel.Text = rs.TstTimestamp.ToString( );
        }
    }

    void DeleteButton_Click(object sender, EventArgs e)
    {
        // Get the target testimonial Form record
        TblTestimonials rsTst = db.TblTestimonials.SingleOrDefault( target => target.TstID == id );

        // Verify target record exists
        if( rsTst == null )
        {
            throw new WebException(RC.TargetDNE);
        }

        // Get name for delete message
        string name = rsTst.TstFName + " " + rsTst.TstLName;

        // Delete the record
        db.TblTestimonials.DeleteOnSubmit( rsTst );
        db.SubmitChanges();

        // Redirect to list page
        Response.Redirect( "TstList.aspx?Name=" + name.ToString( ) );
    }
}