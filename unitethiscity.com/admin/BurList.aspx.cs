/******************************************************************************
 * Filename: BurList.aspx.cs
 * Project:  unitethiscity.com Administration
 * 
 * Description:
 * Display a database-driven listview of business registration Forms.
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

public partial class admin_BurList : System.Web.UI.Page
{
    WebDBContext db = new WebDBContext();

    protected void Page_Load(object sender, EventArgs e)
    {
        // If navigated to from view page delete function
        // Get name of registration form that has been deleted
        string burName = WebConvert.ToString(Request.QueryString["Name"], "");

        // Verify there is a contact form name being passed on the query string
        if (!burName.Equals(""))
        {
            // Show deleted message
            MessagePanel.Visible = true;
            DeleteMessageLabel.Text = "'" + burName + "' has been deleted.";
        }

        // Bind table data to gridview
        ListGridView.DataSource = db.TblBusinessRegistrations;
        ListGridView.DataBind();

        if (!Page.IsPostBack)
        {
            // Grid default size and sort
            ListGridView.SettingsPager.PageSize = WebConvert.ToInt32(SiteSettings.GetValue("PageSize"), 20);
            ListGridView.SortBy( ListGridView.Columns["BurTimestamp"], DevExpress.Data.ColumnSortOrder.Descending );
        }
    }
}