/******************************************************************************
 * Filename: MsjList.aspx.cs
 * Project:  unitethiscity.com Administration
 * 
 * Description:
 * Display a database-driven listview of message jobs
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

public partial class admin_MsjList : System.Web.UI.Page
{
    WebDBContext db = new WebDBContext();

    protected void Page_Load(object sender, EventArgs e)
    {
        // Bind table data to gridview
        ListGridView.DataSource = db.VwMessageJobs;
        ListGridView.DataBind();

        if (!Page.IsPostBack)
        {
            // Grid default size and sort
            ListGridView.SettingsPager.PageSize = WebConvert.ToInt32(SiteSettings.GetValue("PageSize"), 20);
            ListGridView.SortBy(ListGridView.Columns["MsjID"], DevExpress.Data.ColumnSortOrder.Ascending);
        }
    }
}