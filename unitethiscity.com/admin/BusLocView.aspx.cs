/******************************************************************************
 * Filename: BusLocView.aspx.cs
 * Project:  unitethiscity.com Administration
 * 
 * Description:
 * View a business location.
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
using Sancsoft.Web;

public partial class admin_BusLocView : System.Web.UI.Page
{
	int id;
    int locid;
	WebDBContext db = new WebDBContext();

	protected void Page_Load( object sender, EventArgs e )
	{
        // Wire the events
        EditButton.Click += new EventHandler(EditButton_Click);
        DeleteButton.Click += DeleteButton_Click;
        PropertiesRepeater.ItemCommand += new RepeaterCommandEventHandler(PropertiesRepeater_ItemCommand);
        ManagePropertyButton.Click += ManagePropertyButton_Click;
        FavoritesRepeater.ItemCommand += new RepeaterCommandEventHandler(FavoritesRepeater_ItemCommand);
        AddFavoriteButton.Click += AddFavoriteButton_Click;
        TipsRepeater.ItemCommand += new RepeaterCommandEventHandler(TipsRepeater_ItemCommand);
        AddTipButton.Click += AddTipButton_Click;
        RatingsRepeater.ItemCommand += new RepeaterCommandEventHandler(RatingsRepeater_ItemCommand);
        AddRatingButton.Click += AddRatingButton_Click;
        CheckInsRepeater.ItemCommand += new RepeaterCommandEventHandler(CheckInsRepeater_ItemCommand);
        AddCheckInButton.Click += AddCheckInButton_Click;
        LoyaltyDealButton.Click += EditLoyaltyDealButton_Click;

		// Get the target id
        id = WebConvert.ToInt32(Request.QueryString["ID"], 0);
        locid = WebConvert.ToInt32(Request.QueryString["LocID"], 0);

		// Verify id exists
		if ( id == 0 )
		{
			throw new WebException( RC.DataIncomplete );
		}

        // load the locations properties repeater
        RefreshProperties();
        RefreshFavorites();
        RefreshTips();
        RefreshRatings();
        RefreshCheckIns();

		if ( !Page.IsPostBack )
		{
			// Get the record
			VwBusinesses rs = db.VwBusinesses.SingleOrDefault( target => target.BusID == id );

			// Verify target record exists
			if ( rs == null )
			{
				throw new WebException( RC.TargetDNE );
			}

			// Populate the page
            BusIDLiteral.Text = rs.BusID.ToString();
            BusGuidLiteral.Text = rs.BusGuid.ToString();
            BusNameHyperLink.Text = rs.BusName;
            BusNameHyperLink.ToolTip = "View Business";
            BusNameHyperLink.NavigateUrl = "BusView.aspx?ID=" + rs.BusID.ToString();
            BusFormalNameLiteral.Text = rs.BusFormalName;

            // Get the target location record
            TblLocations rsLoc = db.TblLocations.SingleOrDefault(target => target.LocID == locid);
            if( rsLoc == null )
            {
				throw new WebException( RC.TargetDNE );
            }

            // Location info
            LocNameLiteral.Text = rsLoc.LocName;
            LocAddressLiteral.Text = rsLoc.LocAddress;
            LocCityLiteral.Text = rsLoc.LocCity;
            LocStateLiteral.Text = rsLoc.LocState;
            LocZipLiteral.Text = rsLoc.LocZip;
            LocPhoneLiteral.Text = Phone.Format(rsLoc.LocPhone);
            LocLatitudeLiteral.Text = WebConvert.ToDouble( rsLoc.LocLatitude, 0 ).ToString();
            LocLongitudeLiteral.Text = WebConvert.ToDouble( rsLoc.LocLongitude, 0 ).ToString();
            LocRatingLiteral.Text = WebConvert.ToString( rsLoc.LocRating, "0" );

            TblLoyaltyDeals rsLoyaltyDeal = db.TblLoyaltyDeals.SingleOrDefault(target => target.LocID == locid);
            if (rsLoyaltyDeal == null)
            {
                LoyaltyDealLiteral.Text = SiteSettings.GetValue("LoyaltyDefaultReward") + " (" + SiteSettings.GetValue("LoyaltyDefaultPoints") + ") - <b>UTC Default</b>";
            }
            else
            {
                LoyaltyDealLiteral.Text = rsLoyaltyDeal.LoySummary + " (" + rsLoyaltyDeal.LoyPoints.ToString() + ")";
            }

        }
    }

    void DeleteButton_Click(object sender, EventArgs e)
    {
        // check for dependencies ( does this location have any properties assigned to it )
        if (db.TblLocationProperties.Count(target => target.LocID == locid) != 0)
        {
            throw new WebException(RC.Dependencies);
        }

        // Get the record
        TblLocations rs = db.TblLocations.Single(target => target.LocID == locid);

        // Get account
        string name = rs.LocName;

        // Delete the record
        db.TblLocations.DeleteOnSubmit(rs);
        db.SubmitChanges();

        // Update the revision level of the data set
        DataRevision.Bump(Revisioned.LocationInfo);

        // Redirect to list page
        Response.Redirect("BusView.aspx?ID=" + id.ToString() + "&Name=" + name);
    }

    void EditButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("BusLocEdit.aspx?ID=" + id.ToString() + "&LocID=" + locid.ToString());
    }

    void RefreshProperties()
    {
        // Get this businesses locations to populate repeater
        PropertiesRepeater.DataSource = db.VwLocationProperties.Where(target => target.LocID == locid).OrderBy(target => target.PrpName);
        PropertiesRepeater.DataBind();

        // Show empty row if there are no deal definitions for this business
        if (db.TblLocationProperties.Count(target => target.LocID == locid) == 0)
        {
            NoPropertiesRow.Visible = true;
        }
    }

    void PropertiesRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        int lprID = WebConvert.ToInt32(e.CommandArgument, 0);

        db.TblLocationProperties.DeleteOnSubmit(db.TblLocationProperties.Single(target => target.LprID == lprID));
        db.SubmitChanges();

        Response.Redirect("BusLocView.aspx?ID=" + id.ToString() + "&LocID=" + locid.ToString());
    }

    void ManagePropertyButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("BusLocLprEdit.aspx?ID=" + id.ToString() + "&LocID=" + locid.ToString());
    }

    void FavoritesRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        int favID = WebConvert.ToInt32(e.CommandArgument, 0);

        db.TblFavorites.DeleteOnSubmit(db.TblFavorites.Single(target => target.FavID == favID));
        db.SubmitChanges();

        Response.Redirect("BusLocView.aspx?ID=" + id.ToString() + "&LocID=" + locid.ToString());
    }

    void RefreshFavorites()
    {
        // Get this locations favorites to populate repeater
        FavoritesRepeater.DataSource = db.VwFavorites.Where(target => target.LocID == locid).OrderBy(target => target.AccEMail);
        FavoritesRepeater.DataBind();

        // Show empty row if there are no favorites for this location
        if (db.TblFavorites.Count(target => target.LocID == locid) == 0)
        {
            NoFavoritesRow.Visible = true;
        }
    }

    void AddFavoriteButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("BusLocFavNew.aspx?ID=" + id.ToString() + "&LocID=" + locid.ToString());
    }

    void TipsRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        int tipID = WebConvert.ToInt32(e.CommandArgument, 0);

        db.TblTips.DeleteOnSubmit(db.TblTips.Single(target => target.TipID == tipID));
        db.SubmitChanges();

        Response.Redirect("BusLocView.aspx?ID=" + id.ToString() + "&LocID=" + locid.ToString());
    }

    void RefreshTips()
    {
        // Get this locations favorites to populate repeater
        TipsRepeater.DataSource = db.VwTips.Where(target => target.LocID == locid).OrderByDescending( target => target.TipTS );
        TipsRepeater.DataBind();

        // Show empty row if there are no favorites for this location
        if (db.TblTips.Count(target => target.LocID == locid) == 0)
        {
            NoTipsRow.Visible = true;
        }
    }

    void AddTipButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("BusLocTipNew.aspx?ID=" + id.ToString() + "&LocID=" + locid.ToString());
    }

    void RatingsRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        int ratID = WebConvert.ToInt32(e.CommandArgument, 0);

        db.TblRatings.DeleteOnSubmit(db.TblRatings.Single(target => target.RatID == ratID));
        db.SubmitChanges();

        //Update the businesses rating
        TblBusinesses rsBus = db.TblBusinesses.SingleOrDefault(target => target.BusID == id);
        // Verify target record exists
        if (rsBus != null)
        {
            rsBus.BusRating = CalculateBusinessesRating();
            // Submit to the db
            db.SubmitChanges();
        }

        //Update the locations rating
        TblLocations rsLoc = db.TblLocations.SingleOrDefault(target => target.LocID == locid);
        // Verify target record exists
        if (rsLoc != null)
        {
            rsLoc.LocRating = CalculateLocationsRating();
            // Submit to the db
            db.SubmitChanges();
        }

        Response.Redirect("BusLocView.aspx?ID=" + id.ToString() + "&LocID=" + locid.ToString());
    }

    void RefreshRatings()
    {
        // Get this locations favorites to populate repeater
        RatingsRepeater.DataSource = db.VwRatings.Where(target => target.LocID == locid).OrderByDescending(target => target.RatTS);
        RatingsRepeater.DataBind();

        // Show empty row if there are no favorites for this location
        if (db.TblRatings.Count(target => target.LocID == locid) == 0)
        {
            NoRatingsRow.Visible = true;
        }
    }

    void AddRatingButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("BusLocRatNew.aspx?ID=" + id.ToString() + "&LocID=" + locid.ToString());
    }

    void CheckInsRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        int chkID = WebConvert.ToInt32(e.CommandArgument, 0);

        db.TblCheckIns.DeleteOnSubmit(db.TblCheckIns.Single(target => target.ChkID == chkID));
        db.SubmitChanges();

        Response.Redirect("BusLocView.aspx?ID=" + id.ToString() + "&LocID=" + locid.ToString());
    }

    void RefreshCheckIns()
    {
        // Get this locations favorites to populate repeater
        CheckInsRepeater.DataSource = db.VwCheckIns.Where(target => target.LocID == locid).OrderByDescending(target => target.ChkTS);
        CheckInsRepeater.DataBind();

        // Show empty row if there are no favorites for this location
        if (db.TblCheckIns.Count(target => target.LocID == locid) == 0)
        {
            NoCheckInsRow.Visible = true;
        }
    }

    void AddCheckInButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("BusLocChkNew.aspx?ID=" + id.ToString() + "&LocID=" + locid.ToString());
    }

    void EditLoyaltyDealButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("BusLocLoyaltyDealEdit.aspx?ID=" + id.ToString() + "&LocID=" + locid.ToString());
    }


    double CalculateBusinessesRating()
    {
        int lngTotalAccountRatings = 0;
        double dblSumRatings = 0;
        double dblRating = 0;

        // get the total amount of ratings for this business( all business locations )
        lngTotalAccountRatings = db.VwRatings.Count(target => target.BusID == id);
        lngTotalAccountRatings = WebConvert.ToInt32(lngTotalAccountRatings, 0);

        // get the total ratings amount from all locations for this business
        dblSumRatings = db.VwRatings.Where(target => target.BusID == id).ToList().Sum(target => target.RatRating);
        dblSumRatings = WebConvert.ToDouble(dblSumRatings, 0);

        // get the average rating
        dblRating = WebConvert.ToDouble(dblSumRatings / lngTotalAccountRatings, 0);

        dblRating = Math.Round(dblRating, 2);

        if (double.IsNaN(dblRating))
        {
            dblRating = 0;
        }

        // return the average rating
        return dblRating;
    }

    double CalculateLocationsRating()
    {
        int lngTotalAccountRatings = 0;
        double dblSumRatings = 0;
        double dblRating = 0;

        // get the total amount of ratings for this location( number of ratings for this location )
        lngTotalAccountRatings = db.TblRatings.Count(target => target.LocID == locid);
        lngTotalAccountRatings = WebConvert.ToInt32(lngTotalAccountRatings, 0);

        // get the total ratings amount from all accounts( sum of ratings for this location )
        dblSumRatings = db.TblRatings.Where(target => target.LocID == locid).ToList().Sum(target => target.RatRating);
        dblSumRatings = WebConvert.ToDouble(dblSumRatings, 0);

        // get the average rating
        dblRating = WebConvert.ToDouble(dblSumRatings / lngTotalAccountRatings, 0);

        dblRating = Math.Round(dblRating, 2);

        if (double.IsNaN(dblRating))
        {
            dblRating = 0;
        }

        // return the average rating
        return dblRating;
    }
}