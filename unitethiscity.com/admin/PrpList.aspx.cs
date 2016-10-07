/******************************************************************************
 * Filename: PrpList.aspx.cs
 * Project:  unitethiscity.com Administration
 * 
 * Description:
 * Display a database-driven listview of properties.
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

public partial class admin_PrpList : System.Web.UI.Page
{
	WebDBContext db = new WebDBContext();

	protected void Page_Load( object sender, EventArgs e )
	{
		// If navigated to from view page delete function
		// Get property name that has been deleted
        string prpName = WebConvert.ToString(Request.QueryString["Name"], "");

		// Verify there is an administrator account name being passed on query string
		if ( !prpName.Equals( "" ) )
		{
			// Show deleted message
			MessagePanel.Visible = true;
			DeleteMessageLabel.Text = "'" + prpName + "' has been deleted.";
		}

		// Bind table data to gridview
		ListGridView.DataSource = db.TblProperties;
		ListGridView.DataBind();

		if ( !Page.IsPostBack )
		{
			// Grid default size and sort
			ListGridView.SettingsPager.PageSize = WebConvert.ToInt32( SiteSettings.GetValue("PageSize"), 20 );
            ListGridView.SortBy(ListGridView.Columns["PrpName"], DevExpress.Data.ColumnSortOrder.Ascending);
		}
	}
}