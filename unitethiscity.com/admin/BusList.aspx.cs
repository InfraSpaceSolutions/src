/******************************************************************************
 * Filename: BusList.aspx.cs
 * Project:  unitethiscity.com Administration
 * 
 * Description:
 * Display a database-driven listview of businesses.
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

public partial class admin_BusList : System.Web.UI.Page
{
	WebDBContext db = new WebDBContext();

	protected void Page_Load( object sender, EventArgs e )
	{
		// Wire events
		ListGridView.HeaderFilterFillItems += new DevExpress.Web.ASPxGridView.ASPxGridViewHeaderFilterEventHandler( ListGridView_HeaderFilterFillItems );
		
		// If navigated to from view page delete function
		// Get the business name that has been deleted
        string busName = WebConvert.ToString(Request.QueryString["Name"], "");

		// Verify there is an administrator account name being passed on query string
        if (!busName.Equals(""))
		{
			// Show deleted message
			MessagePanel.Visible = true;
            DeleteMessageLabel.Text = "'" + busName + "' has been deleted.";
		}

		// Bind table data to gridview
		ListGridView.DataSource = db.VwBusinesses;
		ListGridView.DataBind();

		if ( !Page.IsPostBack )
		{
			// Grid default size and sort
			ListGridView.SettingsPager.PageSize = WebConvert.ToInt32( SiteSettings.GetValue("PageSize"), 20 );
			ListGridView.SortBy( ListGridView.Columns["BusName"], DevExpress.Data.ColumnSortOrder.Ascending );
		}
	}

	void ListGridView_HeaderFilterFillItems( object sender, DevExpress.Web.ASPxGridView.ASPxGridViewHeaderFilterEventArgs e )
	{
		if ( e.Column.FieldName == "BusEnabled" )
		{
			e.Values.Clear();
			e.AddShowAll();
			e.AddValue( "Yes", "true" );
			e.AddValue( "No", "false" );
		}
	}
}