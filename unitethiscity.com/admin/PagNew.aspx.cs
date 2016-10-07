/****************************************************************************
 * Filename: PagNew.aspx.cs
 * Project:  unitethiscity.com Administration
 * 
 * Description:
 * Create a new page.
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

public partial class admin_PagNew : System.Web.UI.Page
{
	WebDBContext db = new WebDBContext();

    protected void Page_Load(object sender, EventArgs e)
    {
		// Wire events
		SubmitButton.Click += new EventHandler( SubmitButton_Click );
        PagParentLevelDropDownList.SelectedIndexChanged += new EventHandler( PagParentLevelDropDownList_SelectedIndexChanged );

		if ( !Page.IsPostBack )
		{
			// Load level 1 parent pages in dropdown list
            LoadEligibleParentPages( 1 );
		}
    }

    void PagParentLevelDropDownList_SelectedIndexChanged( object sender, EventArgs e )
    {
        int pagLevel = WebConvert.ToInt32( PagParentLevelDropDownList.SelectedValue, 0 );
        LoadEligibleParentPages( pagLevel );
    }

    void LoadEligibleParentPages( int pagLevel )
    {
        // Get the eligible parent pages
        List<TblPages> rsPag =
            ( from pag in db.TblPages
              where pag.PagID != 1 && pag.PagLevel == pagLevel
              orderby pag.PagName
              select pag ).ToList( );

        PagParentIDDropDownList.Items.Clear( );
        PagParentIDDropDownList.Items.Add( new ListItem( "None", "1" ) );
        foreach( var pag in rsPag )
        {
            ListItem li = new ListItem( pag.PagName, pag.PagID.ToString( ) );
            PagParentIDDropDownList.Items.Add( li );
        }
    }

	void SubmitButton_Click( object sender, EventArgs e )
	{
		// Get the form variables
        int pagLevel = WebConvert.ToInt32( PagParentLevelDropDownList.SelectedValue, 0 );
		int pagParentID = WebConvert.ToInt32( PagParentIDDropDownList.SelectedValue, 0 );

        if( pagLevel == 2 && pagParentID == 1)
        {
            pagParentID = 0;
        }

        pagLevel = SiteWebPage.CalculatePageLevel( pagParentID );

		string strPagName = PagNameTextBox.Text.Trim();
		string strPagFileName = ParseFileName( PagFilenameTextBox.Text.Trim() );

		// Check for duplicates
		// Filename must be unique
		if ( db.TblPages.Count( target => target.PagFilename == strPagFileName ) > 0 )
		{
			throw new WebException( RC.Duplicate );
		}

		// Get the max sequence
		int pagSequence = 1;
		string sql = "SELECT * FROM TblPages " +
			"WHERE PagSequence IN ( SELECT MAX( PagSequence ) " +
			"FROM TblPages WHERE PagParentID = " + pagParentID.ToString() + ")";
		TblPages rsMax = db.ExecuteQuery<TblPages>( sql, "" ).FirstOrDefault();

		if ( rsMax != null )
		{
			pagSequence = rsMax.PagSequence + 1;
		}

		// Create new page record
		TblPages rs = new TblPages();

		// Populate record fields
		rs.PagParentID = pagParentID;
		rs.PagLocked = false; // Default
		rs.PagPublished = false;
		rs.PagLevel = pagLevel;
		rs.PagSequence = pagSequence;
		rs.PagName = WebConvert.Truncate( strPagName, 128 );
		rs.PagFilename = WebConvert.Truncate( strPagFileName, 128 );
		rs.PagHeading = WebConvert.Truncate( PagHeadingTextBox.Text.Trim(), 80 );
		rs.PagNavName = WebConvert.Truncate( PagNavNameTextBox.Text.Trim(), 50 );
		rs.PagTitle = WebConvert.Truncate( PagTitleTextBox.Text.Trim(), 128 );
		rs.PagDescription = PagDescriptionTextBox.Text.Trim();
		rs.PagKeywords = PagKeywordsTextBox.Text.Trim();
		rs.PagBody = PagBodyTextBox.Text.Trim();
		rs.PagCreatedTS = DateTime.Now;
		rs.PagModifiedTS = null;
		
		// Insert the new page record
		db.TblPages.InsertOnSubmit( rs );

		// Sync to database
		db.SubmitChanges();

		// Create the physical page files
		try
		{
            SiteWebPage.CreatePageFiles( rs.PagID, strPagFileName );
		}
		catch ( Exception ex )
		{
			// File operation failed -- clean up our db record
			db.ExecuteCommand( "DELETE FROM TblPages WHERE PagID=" + rs.PagID );

			// Page creation error
			throw ex;
		}

		// Redirect to target view page
		Response.Redirect( "PagView.aspx?ID=" + rs.PagID.ToString() );
	}

	private string ParseFileName( string fileName )
	{
		string ret = fileName;

		// Make method more robust by including more illegal filename characters than defined in Path method
		var illegalChars = 
		Path.GetInvalidFileNameChars().Concat( new [] { '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '~', '\'', '+', '=', '{', '}', ':', '\\', ';', ',', '.', '?', '/' } );

		// Make sure there is no period, if so, remove anything to the right of it
		int periodIdx = ret.IndexOf( "." );
		if ( periodIdx > 0 )
		{
			ret = ret.Substring( 0, periodIdx );
		}

		// Remove any illegal characters,  as defined above
		foreach ( char c in illegalChars )
		{
			ret = ret.Replace( c.ToString(), "" );
		}

		// Remove all spaces
		foreach ( char ch in ret )
		{
			if ( ch == 32 )
			{
				ret = ret.Replace( ch.ToString(), "" );
			}
		}

		// Append the extension if we have a filename
		if ( ret.Length > 0 )
		{
			ret += ".cshtml";
		}

		return ret;
	}
}