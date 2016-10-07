/******************************************************************************
 * Filename: SiteWebPage.cs
 * Project:  unitethiscity.com
 * 
 * Description:
 * Page content object
 * 
 * Revision History:
 * $Log: $
******************************************************************************/
using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.IO;
using System.Collections.Generic;
using Sancsoft.Web;

/// <summary>
/// Correlate physical web pages with the database representation
/// </summary>
public class SiteWebPage
{
    protected WebDBContext db;

    /// <summary>
    /// ID of the page associated with this object
    /// </summary>
    public int PageID
    {
        get;
        set;
    }

    /// <summary>
    /// ID of the site homepage
    /// </summary>
    public int HomePageID
    {
        get;
        set;
    }

    /// <summary>
    /// ID of the section for this page
    /// </summary>
    public int SectionPageID
    {
        get;
        set;
    }

	/// <summary>
    /// ID of the group for this page
    /// </summary>
    public int GroupPageID
    {
        get;
        set;
    }

    /// <summary>
    /// Locked bit of the active page
    /// </summary>
    public bool Locked
    {
        get;
        set;
    }

    /// <summary>
    /// Locked bit of the active page
    /// </summary>
    public bool Published
    {
        get;
        set;
    }

    /// <summary>
    /// Filename of the active page
    /// </summary>
    public string Filename
    {
        get;
        set;
    }

    /// <summary>
    /// Name of the active page
    /// </summary>
    public string Name
    {
        get;
        set;
    }

    /// <summary>
    /// NavName of the active page
    /// </summary>
    public string NavName
    {
        get;
        set;
    }

    /// <summary>
    /// pagHeading of the active page
    /// </summary>
    public string Heading
    {
        get;
        set;
    }

    /// <summary>
    /// Title of the active page
    /// </summary>
    public string Title
    {
        get;
        set;
    }

    /// <summary>
    /// Description for the active page
    /// </summary>
    public string Description
    {
        get;
        set;
    }

    /// <summary>
    /// Keywords forr the active page
    /// </summary>
    public string Keywords
    {
        get;
        set;
    }

    /// <summary>
    /// Body Content for the active page
    /// </summary>
    public string Body
    {
        get;
        set;
    }

    /// <summary>
    /// Body Content for the active page
    /// </summary>
    public string PageParentHeading
    {
        get;
        set;
    }

    /// <summary>
    /// Recommended image for facebook
    /// </summary>
    public string OpenGraphImageURI
    {
        get;
        set;
    }

    /// <summary>
    /// Recommended secure image for facebook
    /// </summary>
    public string SecureOpenGraphImageURI
    {
        get;
        set;
    }

    /// <summary>
	/// Construct an object from the name of the page file
	/// </summary>
	/// <param name="appRelativeVirtualPath">the filename of the target page</param>
	public SiteWebPage(string appRelativeVirtualPath)
	{
		// get a connection to the database
		db = new WebDBContext();

		// keep track of the executing filename
		Filename = appRelativeVirtualPath;
		// identify the page by the executing filename
		PageID = IdentifyPageByFilename(appRelativeVirtualPath);

		// get the page info from the database and configure the properties
		LoadPageData();
	}

	/// <summary>
    /// Construct an object from the id of the page
    /// </summary>
    /// <param name="appRelativeVirtualPath">the filename of the target page</param>
    public SiteWebPage( int pagID = 0)
    {
        // get a connection to the database
        db = new WebDBContext( );

        // identify the page by the executing filename
        PageID = pagID;

		// keep track of the executing filename
		Filename = IdentifyPageByID( PageID );

		// get the page info from the database and configure the properties
		LoadPageData();
    }

	/// <summary>
	/// Load the page information from the database once it has been identified
	/// </summary>
	void LoadPageData()
	{
		// identify the home page from site settings
		HomePageID = WebConvert.ToInt32(SiteSettings.GetValue("HomePageID"), 0);

		// get the site navigation list
		List<VwNavigation> navList = db.VwNavigation.ToList();

		// build a list of the parents of this page
		// list is top parent (home page) down to the page itself
		// size of list tells you depth of the navigation
		List<int> pageStack = new List<int>();
		for (int pid = PageID; pid != 0; )
		{
			pageStack.Insert(0, pid);
			VwNavigation pidPage = navList.Single(target => target.PagID == pid);
			pid = pidPage.PagParentID;
		}

		// if this page has a section use it; otherwise revert to self
        SectionPageID = ( pageStack.Count > 1 ) ? pageStack[1] : PageID;
        // if this page has a group use it; otherwise revert to section
        GroupPageID = ( pageStack.Count > 2 ) ? pageStack[2] : SectionPageID;

		// get the active page record to set the content variables
		TblPages pag = db.TblPages.SingleOrDefault(target => target.PagID == PageID);
        if (pag != null)
        {
            // get the configured values from the page record
            Name = pag.PagName;
            NavName = pag.PagNavName;
            Heading = pag.PagHeading;
            Locked = pag.PagLocked;
            Published = pag.PagPublished;
            Body = pag.PagBody;
            Title = pag.PagTitle;
            Description = pag.PagDescription;
            Keywords = pag.PagKeywords;
        }
        else
        {
            // page is unmanaged
            Name = "_undefined";
            NavName = "_undefined";
            Heading = "_undefined";
            Locked = false;
            Published = true;
            Body = "";
            Title = SiteSettings.GetValue("DefaultTitle");
            Description = SiteSettings.GetValue( "DefaultDescription");
            Keywords = SiteSettings.GetValue("DefaultKeywords");
        }

        // assign the facebook graphics from the settings to all pages - customize manually on the page as needed
        OpenGraphImageURI = SiteSettings.GetValue("OpenGraphImageURI");
        SecureOpenGraphImageURI = SiteSettings.GetValue("SecureOpenGraphImageURI");

        // get the active page record to set the content variables
        TblPages pagParent = db.TblPages.SingleOrDefault(target => target.PagID == SectionPageID);
        if (pagParent != null)
        {
            if(pag.PagID == pagParent.PagID)
            {
                PageParentHeading = pagParent.PagNavName;
            }
            else
            {
                PageParentHeading = pagParent.PagHeading;
            }
        }
        else
        {
            PageParentHeading = (pag != null) ? pag.PagHeading : "";
        }
	}

    /// <summary>
    /// Apply the supplied name to the page - all associated properties
    /// </summary>
    /// <param name="name">string to apply as the name</param>
    public void ApplyName(string name)
    {
        Name = name;
        NavName = name;
        Heading = name;
    }

    /// <summary>
    /// Identify a web page id by its filename
    /// </summary>
    /// <param name="appRelativeVirtualPath"></param>
    /// <returns></returns>
    public static int IdentifyPageByFilename( string appRelativeVirtualPath )
    {
        int pagid = 0;				// assume that the page can't be identified
        // open the database so that we can find the page
        WebDBContext db = new WebDBContext( );
        // if the arvp contains
        if( appRelativeVirtualPath.StartsWith( "~/" ) == true )
        {
            string arvp = appRelativeVirtualPath.Remove( 0, 2 );
            TblPages pag = db.TblPages.SingleOrDefault( target => target.PagFilename == arvp );
            if( pag != null )
            {
                pagid = pag.PagID;
            }
        }
        if (appRelativeVirtualPath.StartsWith("~//") == true)
        {
            string arvp = appRelativeVirtualPath.Remove(0, 3);
            TblPages pag = db.TblPages.SingleOrDefault(target => target.PagFilename == arvp);
            if (pag != null)
            {
                pagid = pag.PagID;
            }
        }
        return pagid;
    }

	/// <summary>
	/// Identify a web page filename by id
	/// </summary>
	/// <param name="appRelativeVirtualPath"></param>
	/// <returns></returns>
	public static string IdentifyPageByID(int pagid)
	{
		string filename = "";
		// open the database so that we can find the page
		WebDBContext db = new WebDBContext();
		TblPages pag = db.TblPages.SingleOrDefault(target => target.PagID == pagid);
		if (pag != null)
		{
			filename = pag.PagFilename;
		}
		return filename;
	}

    /// <summary>
    /// Is this page the home page for the site
    /// </summary>
    /// <returns>true if its the home page</returns>
    public bool IsHomePage( )
    {
        return ( HomePageID == PageID );
    }

    /// <summary>
    /// Calculate the level of a page with the specified parent id.
    /// </summary>
    /// <param name="parentID">The PagID of the target page's parent.</param>
    /// <returns>The level of the target page.</returns>
    public static int CalculatePageLevel( int parentID )
    {
        int level = 0;
        TblPages rsPag = null;
        WebDBContext db = new WebDBContext( );

        do
        {
            // Get the parent page
            rsPag = db.TblPages.SingleOrDefault( target => target.PagID == parentID );
            // Move up a level
            if( rsPag != null )
            {
                level++;
                parentID = rsPag.PagParentID;
            }

        } while( rsPag != null );

        return level;
    }

    /// <summary>
    /// Delete a page from the database and its physical page files.
    /// </summary>
    /// <param name="pagID">The pagID of the page to delete.</param>
    public static void DeletePage( int pagID )
    {
        WebDBContext db = new WebDBContext( );

        // Get the page record from the database
        TblPages rs = db.TblPages.SingleOrDefault( target => target.PagID == pagID );
        // Verify the page record exists
        if( rs == null )
        {
            throw new WebException( RC.TargetDNE );
        }

        // Get the paths to the physical page files
        string filePath = Path.Combine( SiteSettings.GetValue( "WebsitePath" ), rs.PagFilename );

        // Make sure the files exists
        if( !File.Exists( filePath ) )
        {
            throw new WebException( RC.TargetDNE );
        }

        // Delete the physical page files -- an exception is not thrown if the files do not exist
        File.Delete( filePath );

        // Delete the page
        db.TblPages.DeleteOnSubmit( rs );
        db.SubmitChanges( );
    }

    /// <summary>
    /// Create the physical page file from the specified template.
    /// </summary>
    /// <param name="pagID">The pagID of the page that needs files.</param>
    /// <param name="fileName">The name of the new files.</param>
    public static void CreatePageFiles( int pagID, string fileName )
    {
        WebDBContext db = new WebDBContext( );

        // Get the template information
        TblPages rs = db.TblPages.Single( target => target.PagID == pagID );

        // Verify target record exists
        if( rs == null )
        {
            throw new WebException( RC.TargetDNE );
        }

        // Determine template file based on page type
        string templateFilePath = "";

        templateFilePath = Path.Combine( SiteSettings.GetValue( "TemplatePath" ), "_Template.cshtml" );

        // Make sure the template files exists
        if( !System.IO.File.Exists( templateFilePath ) )
        {
            throw new WebException( RC.InternalError );
        }

        // Setup the filepaths
        string newFilePath = Path.Combine( SiteSettings.GetValue("WebsitePath"), fileName );

        // Create the new files from the templates
        CreateFileFromTemplate( rs, templateFilePath, newFilePath );
    }

    /// <summary>
    /// Create a new page file at the specified location from the specified template.
    /// </summary>
    /// <param name="rs">The database record containing the new page data.</param>
    /// <param name="templateFilePath">The file path of the template file to use.</param>
    /// <param name="newFilePath">The target file path for the new page file.</param>
    private static void CreateFileFromTemplate( TblPages rs, string templateFilePath, string newFilePath )
    {
        // Make sure the new file target does not already exist
        if( !File.Exists( newFilePath ) )
        {
            // Open the template file for reading
            using( TextReader reader = new StreamReader( templateFilePath ) )
            {
                // Open the new file for writing
                using( TextWriter writer = File.CreateText( newFilePath ) )
                {
                    string line;

                    // Process the template file a line at a time
                    while( ( line = reader.ReadLine( ) ) != null )
                    {
                        // Merge the new page data with the template fields
                        MergePageTemplateFields( ref line, rs );

                        // Write the merged line
                        writer.WriteLine( line );
                    }

                    // Close the textwriter
                    writer.Close( );
                }

                // Close the textreader
                reader.Close( );
            }
        }
    }

    /// <summary>
    /// Merge any fields in the supplied template text with the supplied page data.
    /// </summary>
    /// <param name="text">The text from the template that is to be replaced.</param>
    /// <param name="rs">The database record containing the new page data.</param>
    private static void MergePageTemplateFields( ref string text, TblPages rs )
    {
        text = text.Replace( "{PagID}", rs.PagID.ToString( ) );
    }
}