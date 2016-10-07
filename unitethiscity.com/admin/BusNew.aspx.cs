/******************************************************************************
 * Filename: BusNew.aspx.cs
 * Project:  acrtinc.com Administration
 * 
 * Description:
 * Create a new business.
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

public partial class admin_BusNew : System.Web.UI.Page
{
    bool fileError;
	WebDBContext db = new WebDBContext();

	protected void Page_Load( object sender, EventArgs e )
	{
		// Wire events
        SubmitButton.Click += new EventHandler(SubmitButton_Click);

        fileError = WebConvert.ToBoolean(Request.QueryString["fileError"], false);
        if (fileError)
        {
            ImageInvalidPanel.Visible = true;
        }

        if (!Page.IsPostBack)
        {
            // Load the Category Dropdown List
            LoadCategoryDropDownList();
            // Load the City Dropdown List
            LoadCitiesDropDownList();
            // Load the Accounts Dropdown List
            LoadAccountsDropDownList();
        }
	}

    void LoadCategoryDropDownList()
    {
        // Get the folders
        List<VwCategories> rsCat =
            (from cat in db.VwCategories
             where cat.CatParentID != 0
             orderby cat.CatParentName, cat.CatName
             select cat).ToList();

        CatIDDropDownList.Items.Clear();
        ListItem liSelect = new ListItem("Select...", "");
        CatIDDropDownList.Items.Add(liSelect);
        foreach (var cat in rsCat)
        {
            if (cat.CatParentID != 0)
            {
                ListItem li = new ListItem(cat.CatParentName + ": " + cat.CatName, cat.CatID.ToString());
                CatIDDropDownList.Items.Add(li);
            }
            else
            {
                ListItem li = new ListItem(cat.CatName, cat.CatID.ToString());
                CatIDDropDownList.Items.Add(li);
            }
        }
    }

    void LoadCitiesDropDownList( )
    {
        // Get the folders
        List<TblCities> rsCit =
            ( from cit in db.TblCities
              orderby cit.CitName
              select cit ).ToList( );

        CitIDDropDownList.Items.Clear();
        ListItem liSelect = new ListItem("Select...", "");
        CitIDDropDownList.Items.Add(liSelect);
        foreach( var cit in rsCit )
        {
            ListItem li = new ListItem( cit.CitName, cit.CitID.ToString( ) );
            CitIDDropDownList.Items.Add( li );
        }
    }

    void LoadAccountsDropDownList()
    {
        // Get the folders
        List<VwAccountRoles> rsAcc =
            (from acc in db.VwAccountRoles
             where acc.RolID == (int)Roles.SalesRep 
             orderby acc.AccEMail
             select acc).ToList();

        AccIDDropDownList.Items.Clear();
        ListItem liSelect = new ListItem("Select...", "");
        AccIDDropDownList.Items.Add(liSelect);
        foreach (var acc in rsAcc)
        {
            ListItem li = new ListItem(acc.AccEMail, acc.AccID.ToString());
            AccIDDropDownList.Items.Add(li);
        }
    }

	void SubmitButton_Click( object sender, EventArgs e )
    {
        bool fileError;

		// Validate page
		if ( !Page.IsValid )
		{
			return;
		}

        if (LogoFileUpload.HasFile)
        {
            // Validate for image/* file types only
            if (!LogoFileUpload.PostedFile.ContentType.Contains("image/"))
            {
                // Log the error
                fileError = true;
                ImageInvalidPanel.Visible = true;
                return;
            }
        }
		// Create the record
		TblBusinesses rs = new TblBusinesses();

		// Populate fields
        rs.BusGuid = Guid.NewGuid();
        rs.CatID = WebConvert.ToInt32(CatIDDropDownList.SelectedValue, 0); 
        rs.CitID = WebConvert.ToInt32( CitIDDropDownList.SelectedValue, 0 );
        rs.BusRepAccID = WebConvert.ToInt32(AccIDDropDownList.SelectedValue, 0);
        rs.BusName = WebConvert.Truncate( BusNameTextBox.Text.Trim( ), 50 );
        rs.BusFormalName = WebConvert.Truncate( BusFormalNameTextBox.Text.Trim( ), 128 );
        rs.BusSummary = WebConvert.Truncate(BusSummaryTextBox.Text.Trim(), 140);
        rs.BusDescription = WebConvert.Truncate(BusSummaryTextBox.Text.Trim(), 140);
        rs.BusEnabled = false;
        rs.BusRating = 0;
        rs.BusWebsite = WebConvert.Truncate(BusWebsiteTextBox.Text.Trim(), 255);
        rs.BusFacebookLink = WebConvert.Truncate(BusFacebookLinkTextBox.Text.Trim(), 128);
        rs.BusFacebookID = WebConvert.Truncate(BusFacebookIDTextBox.Text.Trim(), 50);
        rs.BusAssignedDldID = 0;

		// Submit to the db
		db.TblBusinesses.InsertOnSubmit( rs );
		db.SubmitChanges();

        // Create the image file
        if (LogoFileUpload.HasFile)
        {
            ImageManager.CreateUpdateBusinessImage(LogoFileUpload, rs.BusID, out fileError);
        }
        else
        {
            //default logo with businesses guid
            ImageManager.CreateDefaultBusinessImage(rs.BusID, out fileError);

        }

        // set the entertainer flag for this business if applicable
        if (EntertainerCheckbox.Checked)
        {
            TblEntertainers rsEntertainers = new TblEntertainers();
            rsEntertainers.BusID = rs.BusID;
            db.TblEntertainers.InsertOnSubmit(rsEntertainers);
            db.SubmitChanges();
        }

        // Update the revision level of the data set
        DataRevision.Bump(Revisioned.LocationInfo);

        // Redirect to the view page
        Response.Redirect("BusView.aspx?ID=" + rs.BusID.ToString() + "&fileError=" + fileError.ToString());
	}
}