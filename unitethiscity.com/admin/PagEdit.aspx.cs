/****************************************************************************
 * Filename: PagEdit.aspx.cs
 * Project:  unitethiscity.com Administration
 * 
 * Description:
 * Edit an existing page.
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
using Sancsoft.Web;

public partial class admin_PagEdit : System.Web.UI.Page
{
	int id;
	WebDBContext db = new WebDBContext();

    protected void Page_Load(object sender, EventArgs e)
    {
		// Wire events
		SubmitButton.Click += new EventHandler( SubmitButton_Click );

		// Get the target id
		id = WebConvert.ToInt32( Request.QueryString["ID"], 0 );

		// Verify target id exists
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
			PagFilenameLiteral.Text = rs.PagFilename;
			PagNavNameTextBox.Text = rs.PagNavName;
			PagSequenceTextBox.Text = rs.PagSequence.ToString();
			PagParentNameLiteral.Text = WebConvert.ToString( rs.PagParentName, "None" );
			PagNameTextBox.Text = rs.PagName;
			PagHeadingTextBox.Text = rs.PagHeading;
			PagTitleTextBox.Text = rs.PagTitle;
			PagKeywordsTextBox.Text = rs.PagKeywords;
			PagDescriptionTextBox.Text = rs.PagDescription;
			PagBodyTextBox.Text = rs.PagBody;

			// Show sequence # textbox if this page has no parent
			if ( rs.PagParentID == 0 )
			{
				PagSequenceTextBox.Visible = true;
			}
			else
			{ 
				// Show the label and link to the child sequence page if this page has a parent
				PagSequenceNote.Visible = true;
				PagSequenceRequired.Enabled = false;
				PagSequenceHyperLink.Text = "here";
				PagSequenceHyperLink.NavigateUrl = "PagChildSequence.aspx?ID=" + rs.PagParentID.ToString();
			}
		}
    }

	void SubmitButton_Click( object sender, EventArgs e )
	{
		// Grab page record to update
		TblPages rs = db.TblPages.SingleOrDefault( target => target.PagID == id );

		// Verify record exists
		if ( rs == null )
		{
			throw new WebException( RC.TargetDNE );
		}

		// Get the changes and update fields
		rs.PagName = WebConvert.Truncate( PagNameTextBox.Text.Trim(), 128 );
		rs.PagNavName = WebConvert.Truncate( PagNavNameTextBox.Text.Trim(), 50 );
		rs.PagHeading = WebConvert.Truncate( PagHeadingTextBox.Text.Trim(), 80 );
		rs.PagTitle = WebConvert.Truncate( PagTitleTextBox.Text.Trim(), 128 );
		rs.PagKeywords = PagKeywordsTextBox.Text.Trim();
		rs.PagDescription = PagDescriptionTextBox.Text.Trim();
		rs.PagBody = PagBodyTextBox.Text.Trim();

		// Update the sequence # if a top-level navigation page with no parent
		if ( rs.PagParentID == 0 )
		{
			rs.PagSequence = WebConvert.ToInt32( PagSequenceTextBox.Text.Trim(), 0 );
		}
		
		// Mark the timestamp
		rs.PagModifiedTS = DateTime.Now;
		
		// Sync to the database
		db.SubmitChanges();

		// Redirect back to the view
		Response.Redirect( "PagView.aspx?ID=" + id.ToString() );
	}
}