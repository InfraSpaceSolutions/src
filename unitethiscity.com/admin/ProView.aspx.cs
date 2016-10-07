/******************************************************************************
 * Filename: ProView.aspx.cs
 * Project:  unitethiscity.com Administration
 * 
 * Description:
 * View an account.
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

public partial class admin_ProView : System.Web.UI.Page
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
            TblPromotions rs = db.TblPromotions.SingleOrDefault(target => target.ProID == id);

            // Verify target record exists
            if (rs == null)
            {
                throw new WebException(RC.TargetDNE);
            }

            // Populate the page
            ProIDLiteral.Text = rs.ProID.ToString();
            ProNameLiteral.Text = rs.ProName;
            ProTitleLiteral.Text = rs.ProTitle;
            ProTextLiteral.Text = rs.ProText;
            ProEnabledLiteral.Text = (rs.ProEnabled) ? "Yes" : "No";
        }
    }

    void DeleteButton_Click(object sender, EventArgs e)
    {
        // check for dependencies
        if ( db.TblReferralCodes.Count(target=>target.ProID == id) > 0 )
        {
            throw new Sancsoft.Web.WebException(Sancsoft.Web.RC.Dependencies);
        }

        // Get the record
        TblPromotions rs = db.TblPromotions.Single(Target => Target.ProID == id);

        // Get account
        string name = rs.ProName;

        // Delete the record
        db.TblPromotions.DeleteOnSubmit(rs);

        // sync to database
        db.SubmitChanges();

        // Redirect to list page
        Response.Redirect("ProList.aspx?Name=" + name);
    }

    void EditButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("ProEdit.aspx?ID=" + id.ToString());
    }
}