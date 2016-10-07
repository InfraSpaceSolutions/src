/******************************************************************************
 * Filename: ChaList.aspx.cs
 * Project:  unitethiscity.com Administration
 * 
 * Description:
 * Display a database-driven listview of charity registration Forms.
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

public partial class admin_ChaList : System.Web.UI.Page
{
    WebDBContext db = new WebDBContext();

    protected void Page_Load(object sender, EventArgs e)
    {
        // If navigated to from view page delete function
        // Get name of registration form that has been deleted
        string chaName = WebConvert.ToString(Request.QueryString["Name"], "");

        // Verify there is a contact form name being passed on the query string
        if (!chaName.Equals(""))
        {
            // Show deleted message
            MessagePanel.Visible = true;
            DeleteMessageLabel.Text = "'" + chaName + "' has been deleted.";
        }

        // Bind table data to gridview
        ListGridView.DataSource = db.TblCharityRegistrations;
        ListGridView.DataBind();

        if (!Page.IsPostBack)
        {
            // Grid default size and sort
            ListGridView.SettingsPager.PageSize = WebConvert.ToInt32(SiteSettings.GetValue("PageSize"), 20);
            ListGridView.SortBy(ListGridView.Columns["ChaTimestamp"], DevExpress.Data.ColumnSortOrder.Descending);
        }
    }
}