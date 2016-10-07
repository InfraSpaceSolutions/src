/******************************************************************************
 * Filename: ResList.aspx.cs
 * Project:  unitethiscity.com Administration
 * 
 * Description:
 * Display a file system-based list view of the resources.
 * 
******************************************************************************/
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class admin_ResList : System.Web.UI.Page
{
    private ResourceFileManager Manager = new ResourceFileManager( );

    protected void Page_Load( object sender, EventArgs e )
    {
        // Wire the events
        UploadButton.Click += UploadButton_Click;

        // Deleted filename check
        string deleted = WebConvert.ToString( Request.QueryString["deleted"], "" );
        if ( !deleted.Equals( "" ) )
        {
            // Show deleted message
            MessagePanel.Visible = true;
            MessageLabel.Text = "The file '" + deleted + "' has been deleted.";
        }

        // Uploaded filename check
        string uploaded = WebConvert.ToString( Request.QueryString["uploaded"], "" );
        if ( !uploaded.Equals( "" ) )
        {
            // Show uploaded message
            MessagePanel.Visible = true;
            MessageLabel.Text = "The file '" + uploaded + "' has been uploaded.";
        }

        // Bind table data to gridview
        ListGridView.DataSource = this.Manager.GetFiles( );
        ListGridView.DataBind( );

        if ( !Page.IsPostBack )
        {
            // Grid default size and sort
            ListGridView.SettingsPager.PageSize = WebConvert.ToInt32( SiteSettings.GetValue( "PageSize" ), 20 );
            ListGridView.SortBy( ListGridView.Columns["Filename"], DevExpress.Data.ColumnSortOrder.Ascending );
        }
    }

    void UploadButton_Click( object sender, EventArgs e )
    {
        // Validate the file
        if ( !ResourceFileUpload.HasFile )
        {
            return;
        }

        string filename;
        string error;
        HttpPostedFile postedFile = ResourceFileUpload.PostedFile;
        if ( this.Manager.Upload( postedFile, out filename, out error ) )
        {
            Response.Redirect( String.Format( "/admin/ResList.aspx?uploaded={0}", filename ) );
            return;
        }

        // Set/display the error
        ErrorLabel.Text = error;
        ErrorPanel.Visible = true;
    }
}