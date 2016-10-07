/******************************************************************************
 * Filename: AccFavNew.aspx.cs
 * Project:  acrtinc.com Administration
 * 
 * Description:
 * Create a new favorite location for an account.
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

public partial class admin_AccFavNew : System.Web.UI.Page
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
                 where !(from fav in db.TblFavorites where fav.AccID == id select fav.LocID).Contains(loc.LocID)
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
		TblFavorites rs = new TblFavorites();

        // Populate fields
        rs.AccID = id;
        rs.LocID = WebConvert.ToInt32(LocIDDropDownList.SelectedValue, 0);
        rs.FavTS = DateTime.Now;

		// Submit to the db
		db.TblFavorites.InsertOnSubmit( rs );
		db.SubmitChanges();

		// Redirect to the view page
        Response.Redirect( "AccView.aspx?ID=" + rs.AccID.ToString( ) );
	}
}