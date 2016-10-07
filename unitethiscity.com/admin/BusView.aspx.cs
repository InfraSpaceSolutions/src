/******************************************************************************
 * Filename: BusView.aspx.cs
 * Project:  unitethiscity.com Administration
 * 
 * Description:
 * View a business.
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
using System.IO;
using Sancsoft.Web;

public partial class admin_BusView : System.Web.UI.Page
{
	public int id;
    int currentBusAssignedDldID;
	WebDBContext db = new WebDBContext();

	protected void Page_Load( object sender, EventArgs e )
	{
		// Wire events
        BusEnabledLinkButton.Click += new EventHandler(BusEnabledLinkButton_Click);
        BusRequirePinLinkButton.Click += new EventHandler(BusRequirePinLinkButton_Click);
		EditButton.Click += new EventHandler( EditButton_Click );
        DeleteButton.Click += new EventHandler(DeleteButton_Click);
        DealDefinitionsRepeater.ItemCommand += new RepeaterCommandEventHandler(DealDefinitionsRepeater_ItemCommand);
        AddDealDefinitionButton.Click += new EventHandler(AddDealDefinitionButton_Click);
        UpdateBusAssignedDldIDButton.Click += UpdateBusAssignedDldIDButton_Click;
        PinsRepeater.ItemCommand += new RepeaterCommandEventHandler(PinsRepeater_ItemCommand);
        AddPinButton.Click += AddPinButton_Click;
        LocationsRepeater.ItemCommand += new RepeaterCommandEventHandler(LocationsRepeater_ItemCommand);
        AddLocationButton.Click += AddLocationButton_Click;
        PropertiesRepeater.ItemCommand += new RepeaterCommandEventHandler(PropertiesRepeater_ItemCommand);
        ManagePropertyButton.Click += ManagePropertyButton_Click;
        ImageDeleteButton.Click += ImageDeleteButton_Click;
        RedemptionsRepeater.ItemCommand += new RepeaterCommandEventHandler(RedemptionsRepeater_ItemCommand);
        AddRedemptionButton.Click += AddRedemptionButton_Click;
        EventsRepeater.ItemCommand += new RepeaterCommandEventHandler(EventsRepeater_ItemCommand);
        AddEventButton.Click += AddEventButton_Click;
        RecurringEventsRepeater.ItemCommand += new RepeaterCommandEventHandler(RecurringEventsRepeater_ItemCommand);
        AddRecurringEventButton.Click += AddRecurringEventButton_Click;
        OptOutRepeater.ItemCommand += new RepeaterCommandEventHandler(OptOutRepeater_ItemCommand);
        GalleryItemsButton.Click += GalleryItemsButton_Click;
        MenuItemsButton.Click += MenuItemsButton_Click;

		// Get the target id
		id = WebConvert.ToInt32( Request.QueryString["ID"], 0 );

		// Verify id exists
		if ( id == 0 )
		{
			throw new WebException( RC.DataIncomplete );
		}

        // If navigated to from delete function
        // Get the name that has been deleted
        string strName = WebConvert.ToString(Request.QueryString["Name"], "");

        // Verify there is an administrator account name being passed on query string
        if (!strName.Equals(""))
        {
            // Show deleted message
            MessagePanel.Visible = true;
            DeleteMessageLabel.Text = "'" + strName + "' has been deleted.";
        }

        // Assigned Deal Definition updated status label
        bool busAssignedDldIDUpdate = WebConvert.ToBoolean(Request.QueryString["DldUpdated"], false);

        // Display the assigned deal definition updated status message
        if (busAssignedDldIDUpdate)
        {
            // Show deleted message
            UpdateMessageLabel.Visible = true;
            UpdateMessageLabel.Style.Add("color", "green");
            UpdateMessageLabel.Style.Add("font-weight", "bold");
            UpdateMessageLabel.Text = "Assigned Deal Definition has been updated";
        }
        // If no deal is assigned that show add deal message
        if (db.TblDealDefinitions.Count(target => target.BusID == id) == 0)
        {
            // show add deal definition message
            UpdateBusAssignedDldIDButton.Enabled = false;
            UpdateMessageLabel.Visible = true;
            UpdateMessageLabel.Style.Add("color", "red");
            UpdateMessageLabel.Style.Add("font-weight", "bold");
            UpdateMessageLabel.Text = "Please add a deal definition for this business";
        }

        // Bind data to repeaters
        RefreshDealDefinitions();
        RefreshLocations();
        RefreshProperties();
        RefreshPins();
        RefreshRedemptions();
        RefreshRoles();
        RefreshEvents();
        RefreshRecurringEvents();
        RefreshOptOuts();
        
		if ( !Page.IsPostBack )
		{
			// Get the record
			VwBusinesses rs = db.VwBusinesses.SingleOrDefault( target => target.BusID == id );

			// Verify target record exists
			if ( rs == null )
			{
				throw new WebException( RC.TargetDNE );
			}

            // Load the Accounts Dropdown List
            LoadBusAssignedDldIDDropDownList();

			// Set text for enabled link button
            BusEnabledLinkButton.Text = (rs.BusEnabled) ? "Disable" : "Enable";
            BusRequirePinLinkButton.Text = (rs.BusRequirePin) ? "Disable" : "Enable";

			// Populate the page
            BusIDLiteral.Text = rs.BusID.ToString();
            BusGuidLiteral.Text = rs.BusGuid.ToString();
            BusEnabledLiteral.Text = rs.BusEnabled ? "Yes" : "No";
            BusRequirePinLiteral.Text = rs.BusRequirePin ? "Yes" : "No";
            CitNameLiteral.Text = rs.CitName;
            AccEMailHyperLink.Text = rs.BusRepEMail + " (" + rs.BusRepAccID + ")";
            AccEMailHyperLink.NavigateUrl = "AccView.aspx?ID=" + rs.BusRepAccID;
            AccEMailHyperLink.ToolTip = "View Account";
            BusNameLiteral.Text = rs.BusName;
            BusFormalNameLiteral.Text = rs.BusFormalName;
            BusSummaryLiteral.Text = rs.BusSummary;
            CatNameHyperLink.Text = rs.CatName;
            CatNameHyperLink.ToolTip = "View Category";
            CatNameHyperLink.NavigateUrl = "CatView.aspx?ID=" + rs.CatID.ToString();
            BusRatingLiteral.Text = WebConvert.ToString( rs.BusRating, "0" );
            BusWebsiteHyperLink.NavigateUrl = rs.BusWebsite;
            BusWebsiteHyperLink.Text = rs.BusWebsite;
            BusWebsiteHyperLink.ToolTip = "View Website";
            BusWebsiteHyperLink.Target = "_blank";
            BusFacebookLinkHyperLink.NavigateUrl = rs.BusFacebookLink;
            BusFacebookLinkHyperLink.Text = rs.BusFacebookLink;
            BusFacebookLinkHyperLink.ToolTip = "View Facebook Page";
            BusFacebookIDHyperLink.Target = "_blank";
            BusFacebookIDHyperLink.NavigateUrl = "http://www.facebook.com/" + rs.BusFacebookID;
            BusFacebookIDHyperLink.Text = "http://www.facebook.com/" + rs.BusFacebookID;
            BusFacebookIDHyperLink.ToolTip = "View Facebook Profile";
            BusFacebookIDHyperLink.Target = "_blank";

            BusProximityRangeLiteral.Text = (rs.BusProximityRange < 1000000 ) ? rs.BusProximityRange.ToString(): "Disabled";

            BusAssignedDldIDDropDownList.SelectedValue = (rs.BusAssignedDldID != 0) ? rs.BusAssignedDldID.ToString() : "";

            currentBusAssignedDldID = rs.BusAssignedDldID;

            EntertainerLiteral.Text = (db.TblEntertainers.Count(target => target.BusID == id) > 0) ? "Yes" : "No";

            // Check if an image exists for this business
            if (System.IO.File.Exists(SiteSettings.GetValue("BusinessImagesPath") + rs.BusGuid + ".png" ))
            {
                // display the 100 x 100 image
                System.Web.UI.WebControls.Image img = new System.Web.UI.WebControls.Image();
                img.ImageUrl = "/BusinessImages/" + rs.BusGuid.ToString() + ".png";
                BusImagePanel.Controls.Add(img);
                BusImagePanel.Style.Add("margin-bottom", "13px");
                BusImagePanel.Visible = true;
            }

            // display the qr code for this business
            BusQRImage.ImageUrl = QRGenerator.ImageURL(Encryption.BusinessIdentifierQURI(rs.BusID), 200);

            TblMenus rsMenu = db.TblMenus.SingleOrDefault(target => target.BusID == id);
            MenLinkHyperLink.Text = (rsMenu != null) ? rsMenu.MenLink : "";
            MenLinkHyperLink.NavigateUrl = (rsMenu != null) ? rsMenu.MenLink : "";
        }
    }

    void ImageDeleteButton_Click(object sender, EventArgs e)
    {
        bool fileError;
        ImageManager.CreateDefaultBusinessImage(id, out fileError);

        Response.Redirect("BusView.aspx?ID=" + id.ToString());
    }

    void LoadBusAssignedDldIDDropDownList()
    {
        // Get the folders
        List<TblDealDefinitions> rsDld =
            (from dld in db.TblDealDefinitions
             where dld.BusID == id
             orderby dld.DldName
             select dld).ToList();

        BusAssignedDldIDDropDownList.Items.Clear();
        ListItem liSelect = new ListItem("None", "");
        BusAssignedDldIDDropDownList.Items.Add(liSelect);
        foreach (var dld in rsDld)
        {
            ListItem li = new ListItem(dld.DldName, dld.DldID.ToString());
            BusAssignedDldIDDropDownList.Items.Add(li);
        }
    }

    void DealDefinitionsRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        int dldID = WebConvert.ToInt32(e.CommandArgument, 0);

        // Is this deal definition currently being used by the business
        if (db.TblBusinesses.Count(target => target.BusAssignedDldID == dldID) != 0)
        {
            throw new WebException(RC.Dependencies);
        }

        db.TblDealDefinitions.DeleteOnSubmit(db.TblDealDefinitions.Single(target => target.DldID == dldID));
        db.SubmitChanges();

        Response.Redirect("BusView.aspx?ID=" + id.ToString());
    }

    void PinsRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        int pinID = WebConvert.ToInt32(e.CommandArgument, 0);

        db.TblPins.DeleteOnSubmit(db.TblPins.Single(target => target.PinID == pinID));
        db.SubmitChanges();

        Response.Redirect("BusView.aspx?ID=" + id.ToString());
    }

    void RedemptionsRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        int redID = WebConvert.ToInt32(e.CommandArgument, 0);

        db.TblRedemptions.DeleteOnSubmit(db.TblRedemptions.Single(target => target.RedID == redID));
        db.SubmitChanges();

        Response.Redirect("BusView.aspx?ID=" + id.ToString());
    }

    void LocationsRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        int locID = WebConvert.ToInt32(e.CommandArgument, 0);

        // check for dependencies ( does this location have any properties assigned to it )
        if (db.TblLocationProperties.Count(target => target.LocID == locID) != 0)
        {
            throw new WebException(RC.Dependencies);
        }

        db.TblLocations.DeleteOnSubmit(db.TblLocations.Single(target => target.LocID == locID));
        db.SubmitChanges();

        Response.Redirect("BusView.aspx?ID=" + id.ToString());
    }

    void PropertiesRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        int bprID = WebConvert.ToInt32(e.CommandArgument, 0);

        db.TblBusinessProperties.DeleteOnSubmit(db.TblBusinessProperties.Single(target => target.BprID == bprID));
        db.SubmitChanges();

        Response.Redirect("BusView.aspx?ID=" + id.ToString());
    }

    void EventsRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        int evtID = WebConvert.ToInt32(e.CommandArgument, 0);

        if (db.TblEventLinks.SingleOrDefault(target => target.EvtID == evtID) != null)
        {
            db.TblEventLinks.DeleteOnSubmit(db.TblEventLinks.SingleOrDefault(target => target.EvtID == evtID));
        }
        db.TblEvents.DeleteOnSubmit(db.TblEvents.Single(target => target.EvtID == evtID && target.BusID == id ));
        db.SubmitChanges();

        Response.Redirect("BusView.aspx?ID=" + id.ToString());
    }

    void RecurringEventsRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        int evjID = WebConvert.ToInt32(e.CommandArgument, 0);

        db.TblEventJobs.DeleteOnSubmit(db.TblEventJobs.Single(target => target.EvjID == evjID && target.BusID == id));
        db.SubmitChanges();

        Response.Redirect("BusView.aspx?ID=" + id.ToString());
    }

    void OptOutRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        int optID = WebConvert.ToInt32(e.CommandArgument, 0);

        db.TblOptOuts.DeleteOnSubmit(db.TblOptOuts.Single(target => target.OptID == optID && target.BusID == id));
        db.SubmitChanges();

        Response.Redirect("BusView.aspx?ID=" + id.ToString());
    }


    void RefreshDealDefinitions()
    {
        // Get this businesses deal definitions to populate repeater
        DealDefinitionsRepeater.DataSource = db.TblDealDefinitions.Where(target => target.BusID == id).OrderBy(target => target.DldName);
        DealDefinitionsRepeater.DataBind();

        // Show empty row if there are no deal definitions for this business
        if (db.TblDealDefinitions.Count(target => target.BusID == id) == 0)
        {
            NoDealDefinitionsRow.Visible = true;
        }
    }

    void RefreshLocations()
    {
        // Get this businesses locations to populate repeater
        LocationsRepeater.DataSource = db.TblLocations.Where(target => target.BusID == id).OrderBy(target => target.LocName);
        LocationsRepeater.DataBind();

        // Show empty row if there are no deal definitions for this business
        if (db.TblLocations.Count(target => target.BusID == id) == 0)
        {
            NoLocationsRow.Visible = true;
        }
    }

    void RefreshRoles()
    {
        // Get this businesses locations to populate repeater
        RolesRepeater.DataSource = db.VwAccountRoles.Where(target => target.BusID == id).OrderBy(target => target.AccLName).ThenBy( target => target.AccFName);
        RolesRepeater.DataBind();

        // Show empty row if there are no deal definitions for this business
        if (db.VwAccountRoles.Count(target => target.BusID == id) == 0)
        {
            NoRolesRow.Visible = true;
        }
    }

    void RefreshProperties()
    {
        // Get this businesses locations to populate repeater
        PropertiesRepeater.DataSource = db.VwBusinessProperties.Where(target => target.BusID == id).OrderBy(target => target.PrpName);
        PropertiesRepeater.DataBind();

        // Show empty row if there are no deal definitions for this business
        if (db.TblBusinessProperties.Count(target => target.BusID == id) == 0)
        {
            NoPropertiesRow.Visible = true;
        }
    }

    void RefreshPins()
    {
        // Get this businesses deal definitions to populate repeater
        PinsRepeater.DataSource = db.TblPins.Where(target => target.BusID == id).OrderBy(target => target.PinName);
        PinsRepeater.DataBind();

        // Show empty row if there are no deal definitions for this business
        if (db.TblPins.Count(target => target.BusID == id) == 0)
        {
            NoPinsRow.Visible = true;
        }
    }

    void RefreshRedemptions()
    {
        // Get this businesses redemptions for the current deal
        RedemptionsRepeater.DataSource = db.VwRedemptions.Where(target => target.BusID == id).OrderByDescending(target => target.RedTS);
        RedemptionsRepeater.DataBind();

        // Show empty row if there are no deal definitions for this business
        if (db.VwRedemptions.Count(target => target.BusID == id) == 0)
        {
            NoRedemptionsRow.Visible = true;
        }
    }

    void RefreshEvents()
    {
        // Get this businesses events
        EventsRepeater.DataSource = db.VwEvents.Where(target => target.BusID == id).OrderBy(target => target.EvtStartDate);
        EventsRepeater.DataBind();

        // Show empty row if there are no events for this business
        if (db.VwEvents.Count(target => target.BusID == id) == 0)
        {
            NoEventsRow.Visible = true;
        }
    }

    void RefreshRecurringEvents()
    {
        // Get this businesses events
        RecurringEventsRepeater.DataSource = db.VwEventJobs.Where(target => target.BusID == id).OrderBy(target => target.EvjID);
        RecurringEventsRepeater.DataBind();

        // Show empty row if there are no events for this business
        if (db.VwEventJobs.Count(target => target.BusID == id) == 0)
        {
            NoRecurringEventsRow.Visible = true;
        }
    }

    void RefreshOptOuts()
    {
        // Get this businesses events
        OptOutRepeater.DataSource = db.VwOptOuts.Where(target => target.BusID == id).OrderBy(target => target.OptID);
        OptOutRepeater.DataBind();

        // Show empty row if there are no events for this business
        if (db.VwOptOuts.Count(target => target.BusID == id) == 0)
        {
            NoOptOutsRow.Visible = true;
        }
    }

    void BusEnabledLinkButton_Click( object sender, EventArgs e )
	{
		// Get the target record
        TblBusinesses rsBus = db.TblBusinesses.SingleOrDefault( target => target.BusID == id );

		// Verify target record exists
        if( rsBus == null )
		{
			throw new WebException( RC.TargetDNE );
		}

		// Toggle enabled bit
        rsBus.BusEnabled = (rsBus.BusEnabled) ? false : true;

		// Sync to database
		db.SubmitChanges();

		// Reload the page
        Response.Redirect("BusView.aspx?ID=" + id.ToString());
	}

    void BusRequirePinLinkButton_Click(object sender, EventArgs e)
    {
        // Get the target record
        TblBusinesses rsBus = db.TblBusinesses.SingleOrDefault(target => target.BusID == id);

        // Verify target record exists
        if (rsBus == null)
        {
            throw new WebException(RC.TargetDNE);
        }

        // Toggle enabled bit
        rsBus.BusRequirePin = (rsBus.BusRequirePin) ? false : true;

        // Sync to database
        db.SubmitChanges();

        // Reload the page
        Response.Redirect("BusView.aspx?ID=" + id.ToString());
    }

    void UpdateBusAssignedDldIDButton_Click(object sender, EventArgs e)
    {
        // Get the target record
        TblBusinesses rsBus = db.TblBusinesses.SingleOrDefault(target => target.BusID == id);

        // Verify target record exists
        if (rsBus == null)
        {
            throw new WebException(RC.TargetDNE);
        }

        // Toggle enabled bit
        rsBus.BusAssignedDldID = WebConvert.ToInt32(BusAssignedDldIDDropDownList.SelectedValue, 0); 

        // Sync to database
        db.SubmitChanges();

        // Reload the page
        Response.Redirect("BusView.aspx?ID=" + id.ToString() + "&DldUpdated=True");
    }

	void DeleteButton_Click( object sender, EventArgs e )
	{
		// Get the record
        TblBusinesses rs = db.TblBusinesses.SingleOrDefault(target => target.BusID == id);

        // Verify target record exists
        if (rs == null)
        {
            throw new WebException(RC.TargetDNE);
        }

		// Get the business name thats deleted to display on the list page
        string name = rs.BusName;

		// Delete the record
		db.TblBusinesses.DeleteOnSubmit( rs );
		db.SubmitChanges();

        // delete the location dependencies
        IEnumerable<TblLocations> rsLoc = db.TblLocations.Where(target => target.BusID == id);
        foreach (TblLocations loc in rsLoc)
        {
            db.ExecuteCommand("DELETE from tblLocationProperties where locid=" + loc.LocID.ToString());
            db.ExecuteCommand("DELETE from tblCheckins where locid=" + loc.LocID.ToString());
            db.ExecuteCommand("DELETE from tblFavorites where locid=" + loc.LocID.ToString());
            db.ExecuteCommand("DELETE from tblRatings where locid=" + loc.LocID.ToString());
            db.ExecuteCommand("DELETE from tblTips where locid=" + loc.LocID.ToString());
        }

        // delete the dependent items
        // categories
        db.ExecuteCommand("DELETE from tblBusinessCategories where busid=" + id.ToString());
        // properties
        db.ExecuteCommand("DELETE from tblBusinessProperties where busid=" + id.ToString());
        // deal definitions
        db.ExecuteCommand("DELETE from tblDealDefinitions where busid=" + id.ToString());
        // deals
        db.ExecuteCommand("DELETE from tblDeals where busid=" + id.ToString());
        // account roles
        db.ExecuteCommand("DELETE from tblAccountRoles where busid=" + id.ToString());
        // pins
        db.ExecuteCommand("DELETE from tblPins where busid=" + id.ToString());
        // referral codes
        db.ExecuteCommand("DELETE from tblReferralCodes where busid=" + id.ToString());

        // delete the locations for the business
        db.ExecuteCommand("DELETE from tblLocations where busid=" + id.ToString());

        // delete any associated entertainer flag
        db.ExecuteCommand("DELETE from tblEntertainers where busid=" + id.ToString());

        // Update the revision level of the data set
        DataRevision.Bump(Revisioned.LocationInfo);

        // Redirect to list page
        Response.Redirect("BusList.aspx?Name=" + name);
	}

	void EditButton_Click( object sender, EventArgs e )
	{
        Response.Redirect( "BusEdit.aspx?ID=" + id.ToString( ) );
	}

    void AddDealDefinitionButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("BusDldNew.aspx?ID=" + id.ToString());
    }

    void AddPinButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("BusPinNew.aspx?ID=" + id.ToString());
    }

    void AddRedemptionButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("BusRedNew.aspx?ID=" + id.ToString());
    }

    void AddLocationButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("BusLocNew.aspx?ID=" + id.ToString());
    }

    void AddEventButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("BusEvtNew.aspx?ID=" + id.ToString());
    }

    void AddRecurringEventButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("BusEvjNew.aspx?ID=" + id.ToString());
    }

    void ManagePropertyButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("BusBrpEdit.aspx?ID=" + id.ToString());
    }

    void GalleryItemsButton_Click( object sender, EventArgs e )
    {
        Response.Redirect("BusGalleryItems.aspx?ID=" + id.ToString());
    }

    void MenuItemsButton_Click( object sender, EventArgs e )
    {
        Response.Redirect("BusMenuItems.aspx?ID=" + id.ToString());
    }
}