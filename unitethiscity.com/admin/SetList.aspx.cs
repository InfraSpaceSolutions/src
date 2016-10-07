/******************************************************************************
 * Filename: SetList.aspx.cs
 * Project:  unitethiscity.com Administration
 * 
 * Description:
 * Display a database-driven listview of website settings.
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

public partial class admin_SetList : System.Web.UI.Page
{
	WebDBContext db = new WebDBContext();

    protected void Page_Load(object sender, EventArgs e)
    {
		// Bind table data to gridview
		ListGridView.DataSource = db.TblSettings;
		ListGridView.DataBind();

		if ( !Page.IsPostBack )
		{
			// Grid default size and sort
			ListGridView.SettingsPager.PageSize = WebConvert.ToInt32( SiteSettings.GetValue( "PageSize" ), 20 );
			ListGridView.SortBy( ListGridView.Columns["SetName"], DevExpress.Data.ColumnSortOrder.Ascending );
		}
    }
}