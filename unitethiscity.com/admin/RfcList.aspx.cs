/******************************************************************************
 * Filename: RfcList.aspx.cs
 * Project:  unitethiscity.com Administration
 * 
 * Description:
 * Display a database-driven listview of referral codes.
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

public partial class admin_RfcList : System.Web.UI.Page
{
	WebDBContext db = new WebDBContext();

	protected void Page_Load( object sender, EventArgs e )
	{
		// If navigated to from view page delete function
		// Get administrator account that has been deleted
		string rfcCode = WebConvert.ToString( Request.QueryString["Code"], "" );

		// Verify there is an administrator account name being passed on query string
		if ( !rfcCode.Equals( "" ) )
		{
			// Show deleted message
			MessagePanel.Visible = true;
			DeleteMessageLabel.Text = "'" + rfcCode + "' has been deleted.";
		}

		// Bind table data to gridview
		ListGridView.DataSource = db.VwReferralCodes;
		ListGridView.DataBind();

		if ( !Page.IsPostBack )
		{
			// Grid default size and sort
			ListGridView.SettingsPager.PageSize = WebConvert.ToInt32( SiteSettings.GetValue("PageSize"), 20 );
			ListGridView.SortBy( ListGridView.Columns["RfcCode"], DevExpress.Data.ColumnSortOrder.Ascending );
		}
	}
}