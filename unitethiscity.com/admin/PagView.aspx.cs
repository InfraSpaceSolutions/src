/****************************************************************************
 * Filename: PagView.aspx.cs
 * Project:  unitethiscity.com Administration
 * 
 * Description:
 * View page details and list of corresponding children pages.
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
using System.IO;
using Sancsoft.Web;

public partial class admin_PagView : System.Web.UI.Page
{
	int id;
	WebDBContext db = new WebDBContext();

	protected void Page_Load( object sender, EventArgs e )
	{
		// Wire events
		EditButton.Click += new EventHandler( EditButton_Click );
		EditHTMLButton.Click += new EventHandler( EditHTMLButton_Click );
		DeleteButton.Click += new EventHandler( DeleteButton_Click );
		PagPublishedLinkButton.Click += new EventHandler( PagPublishedLinkButton_Click );
		GeneratePageFileLinkButton.Click += new EventHandler( GeneratePageFileLinkButton_Click );
		ManageChildrenSequenceButton.Click += new EventHandler( ManageChildrenSequenceButton_Click );

		// Get the target id
		id = WebConvert.ToInt32( Request.QueryString["ID"], 0 );

		// Verify target id exists
		if ( id == 0 )
		{
			throw new WebException( RC.DataIncomplete );
		}

		// Bind data to children pages repeater
		RefreshChildrenPages();

		// Get the record
		VwPages rs = db.VwPages.SingleOrDefault( target => target.PagID == id );

		// Verify target page record exists
		if ( rs == null )
		{
			throw new WebException( RC.TargetDNE );
		}

        // Check if the page file has been created
        GeneratePageFileLinkButton.Visible = false;
		if ( !System.IO.File.Exists( SiteSettings.GetValue("WebsitePath") + rs.PagFilename ) )
		{
            GeneratePageFileLinkButton.Visible = true;
		}

		if ( !Page.IsPostBack )
		{
			// Set text for publish link button
			PagPublishedLinkButton.Text = ( rs.PagPublished ) ? "Unpublish" : "Publish"; 

			// Populate the page
			PagIDLiteral.Text = rs.PagID.ToString();
			PagSequenceLiteral.Text = rs.PagSequence.ToString();
			PagLevelLiteral.Text = rs.PagLevel.ToString();
			PagPublishedLiteral.Text = rs.PagPublished ? "Yes" : "No";
			PagNameLiteral.Text = rs.PagName;
            PagFilenameHyperlink.ToolTip = SiteSettings.GetValue("RootUrl") + "/" + rs.PagFilename;
            PagFilenameHyperlink.NavigateUrl = SiteSettings.GetValue("RootUrl") + "/" + rs.PagFilename;
			PagFilenameHyperlink.Text = rs.PagFilename;
			PagNavNameLiteral.Text = rs.PagNavName;
			PagCreatedTSLiteral.Text = rs.PagCreatedTS.ToString();
			PagModifiedTSLiteral.Text = WebConvert.ToString( rs.PagModifiedTS, "N/A" );
			PagHeadingLiteral.Text = rs.PagHeading;
			PagTitleLiteral.Text = rs.PagTitle;
			PagKeywordsLiteral.Text = WebConvert.ToString( rs.PagKeywords, "N/A" );
			PagDescriptionLiteral.Text = WebConvert.ToString( rs.PagDescription, "N/A" );
			PagBodyLiteral.Text = rs.PagBody + "<br style=\"clear:both;\" />"; // Force a clear float to have image(s) fit inside div

			// Display the page heading on the public display if the page is not in main navigation
			if ( rs.PagLevel > 0 )
			{
				DisplayPageHeadingLiteral.Text = rs.PagHeading;
			}

			// If page is locked, disable the delete button and show that appropriate literal text
			if ( rs.PagLocked == true )
			{
				DeleteButton.Enabled = false;
				NoDeleteLabel.Visible = true;
				PagLockedLiteral.Text = "Yes";
			}
			else
			{
				PagLockedLiteral.Text = "No";
			}

			// Create link to parent page view, if any
			string parentName = WebConvert.ToString( rs.PagParentName, "None" );
			if ( parentName != "None" )
			{
				PagParentNameLiteral.Text = "<a href=\"PagView.aspx?ID=" + rs.PagParentID.ToString() + "\" title=\"View Parent\">" + parentName + "</a>";
			}
			else if ( parentName == "None" )
			{
				PagParentNameLiteral.Text = parentName;
			}
		}
	}

    void GeneratePageFileLinkButton_Click( object sender, EventArgs e )
    {
		// Get page record corresponding to the page file that needs generated
        TblPages rs = db.TblPages.SingleOrDefault( target => target.PagID == id );

		// Verify target page record exists
		if ( rs == null )
		{
			throw new WebException( RC.TargetDNE );
		}

		// Create the physical page file
        SiteWebPage.CreatePageFiles( id, rs.PagFilename );

		// Redirect to view page
        Response.Redirect( "PagView.aspx?ID=" + id.ToString( ) );
    }

	void RefreshChildrenPages( )
	{
		// Get children pages to populate repeater
		ChildrenPagesRepeater.DataSource = db.TblPages.Where( target => target.PagParentID == id ).OrderBy( target => target.PagSequence );
		ChildrenPagesRepeater.DataBind();

		// Show empty row if there are no children pages
		if ( db.TblPages.Count( target => target.PagParentID == id ) == 0 )
		{
			NoChildrenRow.Visible = true;
		}
	}

	void PagPublishedLinkButton_Click( object sender, EventArgs e )
	{
		// Get the target page record
		TblPages rsPag = db.TblPages.SingleOrDefault( target => target.PagID == id );

		// Verify target page record exists
		if ( rsPag == null )
		{
			throw new WebException( RC.TargetDNE );
		}

		// Toggle published bit
		rsPag.PagPublished = ( rsPag.PagPublished ) ? false : true;

		// Update published timestamp bit
		rsPag.PagModifiedTS = DateTime.Now;
		
		// Sync to database
		db.SubmitChanges();

		// Reload the page
		Response.Redirect( "PagView.aspx?ID=" + id.ToString() );
	}

	void EditButton_Click( object sender, EventArgs e )
	{
		Response.Redirect( "PagEdit.aspx?ID=" + id );
	}

	void EditHTMLButton_Click( object sender, EventArgs e )
	{
		Response.Redirect( "PagEditHTML.aspx?ID=" + id );
	}

	void ManageChildrenSequenceButton_Click( object sender, EventArgs e )
	{
		Response.Redirect( "PagChildSequence.aspx?ID=" + id.ToString() );
	}

	void DeleteButton_Click( object sender, EventArgs e )
	{
        // for now do not allow them to delete pages
        //throw new WebException( RC.Dependencies );

		// Make sure the page has no children
		// Throw dependency exception if page has children
		if ( db.TblPages.Count( target => target.PagParentID == id ) != 0 )
		{
			throw new WebException( RC.Dependencies );
		}

		// Otherwise, get target page record
		TblPages rs = db.TblPages.SingleOrDefault( target => target.PagID == id );

		// Verify page record exists
		if ( rs == null )
		{
			throw new WebException( RC.TargetDNE );
		}

		// Get page title for delete message
		string name = rs.PagName;

		// Update the sequence number for pages with the same parent as the target
		List<TblPages> pageList =
			( from rows in db.TblPages
			  where rows.PagParentID == rs.PagParentID && rows.PagSequence > rs.PagSequence
			  orderby rows.PagSequence
			  select rows ).ToList();

		// Decrement sequence number
		foreach ( var page in pageList )
		{
			page.PagSequence--;
		}

		// Sync to database
		db.SubmitChanges();

		// Delete page row from database and delete physical page files from website path
        SiteWebPage.DeletePage( id );

		// Redirect to target view page
		Response.Redirect( "PagList.aspx?Name=" + name );
	}
}