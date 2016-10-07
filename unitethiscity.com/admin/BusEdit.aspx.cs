/******************************************************************************
 * Filename: BusEdit.aspx.cs
 * Project:  unitethiscity.com Administration
 * 
 * Description:
 * Edit an account.
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

public partial class admin_BusEdit : System.Web.UI.Page
{
	int id;
    bool fileError;
	WebDBContext db = new WebDBContext();

	protected void Page_Load( object sender, EventArgs e )
	{

		// Wire events
		SubmitButton.Click += new EventHandler( OkButton_Click );

		// Get the target id
		id = WebConvert.ToInt32( Request.QueryString["ID"], 0 );

		// Verify id exists
		if ( id == 0 )
		{
			throw new WebException( RC.DataIncomplete );
		}

        fileError = WebConvert.ToBoolean(Request.QueryString["fileError"], false);
        if (fileError)
        {
            ImageInvalidPanel.Visible = true;
        }

		if ( !Page.IsPostBack )
		{
			// Get the target record
            TblBusinesses rs = db.TblBusinesses.SingleOrDefault( target => target.BusID == id );

			// Verify target record exists
			if ( rs == null )
			{
				throw new WebException( RC.TargetDNE );
			}

            // Load the Category Dropdown List
            LoadCategoriesDropDownList();
            // Load the City Dropdown List
            LoadCitiesDropDownList();
            // Load the Accounts Dropdown List
            LoadAccountsDropDownList();

			// Populate the page
            BusIDLiteral.Text = rs.BusID.ToString( );
            BusGuidLiteral.Text = rs.BusGuid.ToString( );
            CatIDDropDownList.SelectedValue = rs.CatID.ToString();
            CitIDDropDownList.SelectedValue = rs.CitID.ToString();
            AccIDDropDownList.SelectedValue = rs.BusRepAccID.ToString();
            BusNameTextBox.Text = rs.BusName;
            BusFormalNameTextBox.Text = rs.BusFormalName;
            BusWebsiteTextBox.Text = rs.BusWebsite;
            BusFacebookLinkTextBox.Text = rs.BusFacebookLink;
            BusFacebookIDTextBox.Text = rs.BusFacebookID;
            BusSummaryTextBox.Text = rs.BusSummary;
            BusProximityRange.Text = rs.BusProximityRange.ToString();
            TblMenus rsMenu = db.TblMenus.SingleOrDefault(target => target.BusID == id);
            MenLinkTextBox.Text = (rsMenu != null) ? rsMenu.MenLink : "";

            EntertainerCheckbox.Checked = (db.TblEntertainers.Count(target => target.BusID == id) > 0);
		}
    }

    void LoadCategoriesDropDownList()
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

    void LoadCitiesDropDownList()
    {
        // Get the folders
        List<TblCities> rsCit =
            (from cit in db.TblCities
             orderby cit.CitName
             select cit).ToList();

        CitIDDropDownList.Items.Clear();
        ListItem liSelect = new ListItem("Select...", "");
        CitIDDropDownList.Items.Add(liSelect);
        foreach (var cit in rsCit)
        {
            ListItem li = new ListItem(cit.CitName, cit.CitID.ToString());
            CitIDDropDownList.Items.Add(li);
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

	void OkButton_Click( object sender, EventArgs e )
	{
		// Validate the page
		if ( !Page.IsValid )
		{
			return;
		}

		// Get the record
        TblBusinesses rs = db.TblBusinesses.SingleOrDefault( target => target.BusID == id );

		// Verify target record exists
		if ( rs == null )
		{
			throw new WebException( RC.TargetDNE );
		}

        // Update the record
        rs.CatID = WebConvert.ToInt32(CatIDDropDownList.SelectedValue, 0); 
        rs.CitID = WebConvert.ToInt32(CitIDDropDownList.SelectedValue, 0);
        rs.BusRepAccID = WebConvert.ToInt32(AccIDDropDownList.SelectedValue, 0);
        rs.BusName = WebConvert.Truncate(BusNameTextBox.Text.Trim(), 50);
        rs.BusFormalName = WebConvert.Truncate(BusFormalNameTextBox.Text.Trim(), 128);
        rs.BusWebsite = WebConvert.Truncate(BusWebsiteTextBox.Text.Trim(), 255);
        rs.BusFacebookLink = WebConvert.Truncate(BusFacebookLinkTextBox.Text.Trim(), 128);
        rs.BusFacebookID = WebConvert.Truncate(BusFacebookIDTextBox.Text.Trim(), 50);
        rs.BusSummary = WebConvert.Truncate(BusSummaryTextBox.Text.Trim(), 140);
        rs.BusProximityRange = WebConvert.ToInt32(BusProximityRange.Text, 1000000);
        
		// Sync to database
        db.SubmitChanges();

        // upsert or delete the menu item
        string menuLink = MenLinkTextBox.Text.Trim();
        TblMenus rsMenu = db.TblMenus.SingleOrDefault(target => target.BusID == id);
        if (menuLink.Length > 0)
        {
            if (rsMenu == null)
            {
                rsMenu = new TblMenus();
                rsMenu.BusID = id;
                db.TblMenus.InsertOnSubmit(rsMenu);
            }
            rsMenu.MenLink = menuLink;
            db.SubmitChanges();
        }
        else
        {
            if (rsMenu != null)
            {
                db.TblMenus.DeleteOnSubmit(rsMenu);
                db.SubmitChanges();
            }
        }

        // Update the revision level of the data set
        DataRevision.Bump(Revisioned.LocationInfo);

        // Create/Update the image file
        bool fileError;
        ImageManager.CreateUpdateBusinessImage(LogoFileUpload, id, out fileError);

        // update the entertainer flag for this business
        TblEntertainers rsEntertainers = db.TblEntertainers.SingleOrDefault(target => target.BusID == id);
        if (EntertainerCheckbox.Checked)
        {
            // if not already marked as an entertainer - add the record to the set
            if (rsEntertainers == null)
            {
                rsEntertainers = new TblEntertainers();
                rsEntertainers.BusID = id;
                db.TblEntertainers.InsertOnSubmit(rsEntertainers);
                db.SubmitChanges();
            }
        }
        else
        {
            // if marked as an entertainer, delete the record
            if (rsEntertainers != null)
            {
                db.TblEntertainers.DeleteOnSubmit(rsEntertainers);
                db.SubmitChanges();
            }
        }

		// Redirect to target view page
        if (fileError)
        {
            Response.Redirect("BusEdit.aspx?ID=" + id.ToString() + "&fileError=" + fileError.ToString());
        }
        else
        {
            Response.Redirect("BusView.aspx?ID=" + id.ToString() + "&fileError=" + fileError.ToString());
        }
	}
}