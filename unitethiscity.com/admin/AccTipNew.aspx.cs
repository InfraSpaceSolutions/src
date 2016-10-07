/******************************************************************************
 * Filename: AccTipNew.aspx.cs
 * Project:  acrtinc.com Administration
 * 
 * Description:
 * Create a new tip for an account/location.
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
using Sancsoft.Web;

public partial class admin_AccTipNew : System.Web.UI.Page
{
    int id;
	WebDBContext db = new WebDBContext();

	protected void Page_Load( object sender, EventArgs e )
	{
        // Wire events
        SubmitButton.Click += new EventHandler(SubmitButton_Click);

        // Get the target id
        id = WebConvert.ToInt32(Request.QueryString["ID"], 0);

        if (!Page.IsPostBack)
        {
            // Get the record
            VwAccounts rs = db.VwAccounts.SingleOrDefault(target => target.AccID == id);

            // Verify target record exists
            if (rs == null)
            {
                throw new WebException(RC.TargetDNE);
            }

            // Populate the page
            AccIDLiteral.Text = rs.AccID.ToString();
            AccGuidLiteral.Text = rs.AccGuid.ToString();
            AccFNameLiteral.Text = rs.AccFName;
            AccLNameLiteral.Text = rs.AccLName;
            AccEMailHyperLink.NavigateUrl = "mailto:" + rs.AccEMail;
            AccEMailHyperLink.Text = rs.AccEMail;
            AccEMailHyperLink.ToolTip = "Send E-mail";

            // Load the roles dropdown list
            List<TblLocations> rsLoc =
                (from loc in db.TblLocations
                 where !(from tip in db.TblTips where tip.AccID == id select tip.LocID).Contains(loc.LocID)
                 orderby loc.LocName
                 select loc).ToList();

            LocIDDropDownList.DataSource = rsLoc;
            LocIDDropDownList.DataBind();
        }
	}

	void SubmitButton_Click( object sender, EventArgs e )
	{
		// Validate page
		if ( !Page.IsValid )
		{
			return;
		}

		// Create the record
		TblTips rs = new TblTips();

        // Populate fields
        rs.AccID = id;
        rs.LocID = WebConvert.ToInt32(LocIDDropDownList.SelectedValue, 0);
        rs.TipText = WebConvert.Truncate(TipTextTextBox.Text.Trim(), 255);
        rs.TipTS = DateTime.Now;

		// Submit to the db
		db.TblTips.InsertOnSubmit( rs );
		db.SubmitChanges();

		// Redirect to the view page
        Response.Redirect( "AccView.aspx?ID=" + rs.AccID.ToString( ) );
	}
}