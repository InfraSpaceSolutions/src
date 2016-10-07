/******************************************************************************
 * Filename: AccList.aspx.cs
 * Project:  unitethiscity.com Administration
 * 
 * Description:
 * Display a database-driven listview of the accounts.
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

public partial class admin_AccList : System.Web.UI.Page
{
	WebDBContext db = new WebDBContext();

	protected void Page_Load( object sender, EventArgs e )
	{
		// Wire events
		ListGridView.HeaderFilterFillItems += new DevExpress.Web.ASPxGridView.ASPxGridViewHeaderFilterEventHandler( ListGridView_HeaderFilterFillItems );
		
		// If navigated to from view page delete function
		// Get administrator account that has been deleted
		string accAccount = WebConvert.ToString( Request.QueryString["Account"], "" );

		// Verify there is an administrator account name being passed on query string
		if ( !accAccount.Equals( "" ) )
		{
			// Show deleted message
			MessagePanel.Visible = true;
			DeleteMessageLabel.Text = "'" + accAccount + "' has been deleted.";
		}

		// Bind table data to gridview
		ListGridView.DataSource = db.TblAccounts;
		ListGridView.DataBind();

		if ( !Page.IsPostBack )
		{
			// Grid default size and sort
			ListGridView.SettingsPager.PageSize = WebConvert.ToInt32( SiteSettings.GetValue("PageSize"), 20 );
			ListGridView.SortBy( ListGridView.Columns["AccEMail"], DevExpress.Data.ColumnSortOrder.Ascending );
		}
	}

	void ListGridView_HeaderFilterFillItems( object sender, DevExpress.Web.ASPxGridView.ASPxGridViewHeaderFilterEventArgs e )
	{
		if ( e.Column.FieldName == "AccEnabled" )
		{
			e.Values.Clear();
			e.AddShowAll();
			e.AddValue( "Yes", "true" );
			e.AddValue( "No", "false" );
		}
	}
}