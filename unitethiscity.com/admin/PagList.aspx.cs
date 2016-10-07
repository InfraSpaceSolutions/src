/******************************************************************************
 * Filename: PagList.aspx.cs
 * Project:  thebonnotco.com Administration
 * 
 * Description:
 * Display a database-driven listview of the pages.
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
using System.Configuration;

public partial class admin_PagList : System.Web.UI.Page
{
	WebDBContext db = new WebDBContext();

	protected void Page_Load( object sender, EventArgs e )
	{
		// If navigated to from view page delete function
		// Get page name that has been deleted
		string pagName = WebConvert.ToString( Request.QueryString["Name"], "" );

		// Verify there is a page title being passed on query string
		if ( !pagName.Equals( "" ) )
		{
			// Show deleted message
			MessagePanel.Visible = true;
			DeleteMessageLabel.Text = "'" + pagName + "' has been deleted.";
		}

		// Bind table data to gridview
		ListGridView.DataSource = db.VwPages;
		ListGridView.DataBind();

		if ( !Page.IsPostBack )
		{
			// Grid default size and sort
			ListGridView.SettingsPager.PageSize = WebConvert.ToInt32( SiteSettings.GetValue("PageSize"), 20 );
			ListGridView.SortBy( ListGridView.Columns["PagName"], DevExpress.Data.ColumnSortOrder.Ascending );
		}
	}
}