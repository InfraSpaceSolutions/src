/******************************************************************************
 * Filename: CatList.aspx.cs
 * Project:  unitethiscity.com Administration
 * 
 * Description:
 * Display a database-driven listview of categories.
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

public partial class admin_CatList : System.Web.UI.Page
{
	WebDBContext db = new WebDBContext();

	protected void Page_Load( object sender, EventArgs e )
	{
		// If navigated to from view page delete function
		// Get administrator account that has been deleted
        string catName = WebConvert.ToString(Request.QueryString["Name"], "");

		// Verify there is an administrator account name being passed on query string
		if ( !catName.Equals( "" ) )
		{
			// Show deleted message
			MessagePanel.Visible = true;
			DeleteMessageLabel.Text = "'" + catName + "' has been deleted.";
		}

		// Bind table data to gridview
		ListGridView.DataSource = db.VwCategories;
		ListGridView.DataBind();

		if ( !Page.IsPostBack )
		{
			// Grid default size and sort
			ListGridView.SettingsPager.PageSize = WebConvert.ToInt32( SiteSettings.GetValue("PageSize"), 20 );
            ListGridView.SortBy(ListGridView.Columns["CatName"], DevExpress.Data.ColumnSortOrder.Ascending);
		}
	}
}