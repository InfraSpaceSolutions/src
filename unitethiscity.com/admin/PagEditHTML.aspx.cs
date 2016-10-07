/****************************************************************************
 * Filename: PagEditHTML.aspx.cs
 * Project:  unitethiscity.com Administration
 * 
 * Description:
 * Edit the page body using an HTML editor.
 * 
 * Revision History:
 * $Log: $
******************************************************************************/
using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sancsoft.Web;

public partial class admin_PagEditHTML : System.Web.UI.Page
{
	int id;
	WebDBContext db = new WebDBContext();

	protected void Page_Load( object sender, EventArgs e )
	{
		// Wire events
		SubmitButton.Click += new EventHandler( SubmitButton_Click );

		// Get the target id
		id = WebConvert.ToInt32( Request.QueryString["ID"], 0 );

		// Verify id exists
		if ( id == 0 )
		{
			throw new WebException( RC.DataIncomplete );
		}

		if ( !Page.IsPostBack )
		{
			// Get the record
			VwPages rs = db.VwPages.SingleOrDefault( target => target.PagID == id );

			// Verify record exists
			if ( rs == null )
			{
				throw new WebException( RC.TargetDNE );
			}

			// Populate the page
			PagIDLiteral.Text = rs.PagID.ToString();
			PagSequenceLiteral.Text = rs.PagSequence.ToString();
			PagNameLiteral.Text = rs.PagName;
			PagTitleLiteral.Text = rs.PagTitle;
            PagBodyEditor.Text = rs.PagBody;
            PagFilenameHyperlink.ToolTip = SiteSettings.GetValue( "RootUrl" ) + "/" + rs.PagFilename;
			PagFilenameHyperlink.NavigateUrl = SiteSettings.GetValue("RootUrl") + "/" + rs.PagFilename;
			PagFilenameHyperlink.Text = rs.PagFilename;
	
			// Configure the Cute Editor
			CuteEditorConfig.SetToolbarDefaultAdmin( PagBodyEditor );
		}
	}

	void SubmitButton_Click( object sender, EventArgs e )
	{
		// Get target page record
		TblPages rs = db.TblPages.SingleOrDefault( target => target.PagID == id );

		// Verify record exists
		if ( rs == null )
		{
			throw new WebException( RC.TargetDNE );
		}

		// Update page body
		rs.PagBody = PagBodyEditor.Text;

		// Sync to database
		db.SubmitChanges();

		// Redirect to target view page
		Response.Redirect( "PagView.aspx?ID=" + id );
	}

}