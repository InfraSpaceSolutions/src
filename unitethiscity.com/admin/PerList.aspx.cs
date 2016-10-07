/******************************************************************************
 * Filename: PerList.aspx.cs
 * Project:  unitethiscity.com Administration
 * 
 * Description:
 * Display a database-driven listview of periods.
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

public partial class admin_PerList : System.Web.UI.Page
{
	WebDBContext db = new WebDBContext();

	protected void Page_Load( object sender, EventArgs e )
	{
		// If navigated to from view page delete function
		// Get period name that has been deleted
        string perName = WebConvert.ToString(Request.QueryString["Name"], "");

		// Verify there is an name being passed on query string
        if (!perName.Equals(""))
		{
			// Show deleted message
			MessagePanel.Visible = true;
			DeleteMessageLabel.Text = "'" + perName + "' has been deleted.";
		}

		// Bind table data to gridview
		ListGridView.DataSource = db.TblPeriods;
		ListGridView.DataBind();

		if ( !Page.IsPostBack )
		{
			// Grid default size and sort
			ListGridView.SettingsPager.PageSize = WebConvert.ToInt32( SiteSettings.GetValue("PageSize"), 20 );
            ListGridView.SortBy(ListGridView.Columns["PerStartDate"], DevExpress.Data.ColumnSortOrder.Descending);
		}
	}
}