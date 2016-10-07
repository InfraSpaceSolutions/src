/******************************************************************************
 * Filename: AccView.aspx.cs
 * Project:  unitethiscity.com Administration
 * 
 * Description:
 * View an account.
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

public partial class admin_AccView : System.Web.UI.Page
{
	int id;
	WebDBContext db = new WebDBContext();

	protected void Page_Load( object sender, EventArgs e )
	{
		// Wire events
        AccEnabledLinkButton.Click += new EventHandler( AccEnabledLinkButton_Click );
		EditButton.Click += new EventHandler( EditButton_Click );
		DeleteButton.Click += new EventHandler( DeleteButton_Click );
        ForcePasswordButton.Click += new EventHandler(ForcePasswordButton_Click);
        SendCredentialButton.Click += new EventHandler(SendCredentialButton_Click);
        RolesRepeater.ItemCommand += new RepeaterCommandEventHandler(RolesRepeater_ItemCommand);
        AddRoleButton.Click += AddRoleButton_Click;
        FavoritesRepeater.ItemCommand += new RepeaterCommandEventHandler(FavoritesRepeater_ItemCommand);
        AddFavoriteButton.Click += AddFavoriteButton_Click;
        TipsRepeater.ItemCommand += new RepeaterCommandEventHandler(TipsRepeater_ItemCommand);
        AddTipButton.Click += AddTipButton_Click;
        RatingsRepeater.ItemCommand += new RepeaterCommandEventHandler(RatingsRepeater_ItemCommand);
        AddRatingButton.Click += AddRatingButton_Click;
        CheckInsRepeater.ItemCommand += new RepeaterCommandEventHandler(CheckInsRepeater_ItemCommand);
        AddCheckInButton.Click += AddCheckInButton_Click;
        RedemptionsRepeater.ItemCommand += new RepeaterCommandEventHandler(RedemptionsRepeater_ItemCommand);
        SubscriptionRepeater.ItemCommand += new RepeaterCommandEventHandler(SubscriptionRepeater_ItemCommand);
        AddSubscriptionButton.Click += AddSubscriptionButton_Click;
        OptOutRepeater.ItemCommand += new RepeaterCommandEventHandler(OptOutRepeater_ItemCommand);

        // Get the target id
        id = WebConvert.ToInt32( Request.QueryString["ID"], 0 );

		// Verify id exists
		if ( id == 0 )
		{
			throw new WebException( RC.DataIncomplete );
		}

		// Determine if user has successfully sent credentials to the account
		bool welcomed = WebConvert.ToBoolean( Request.QueryString["welcomed"], false );

		// Show welcome panel
		if ( welcomed == true )
		{
			WelcomePanel.Visible = true;
			WelcomeMessageLabel.Text = "The welcome message has been successfully sent to the account indicated below.";
		}

        // Display the account roles in the repeater
        RefreshRoles();
        RefreshFavorites();
        RefreshTips();
        RefreshRatings();
        RefreshCheckIns();
        RefreshRedemptions();
        RefreshSubscription();
        RefreshOptOuts();

        // prepare the role listings
        bool isMember = (db.TblAccountRoles.Count(target => target.AccID == id && target.RolID == (int)Roles.Member) > 0);
        bool isAdmin = (db.TblAccountRoles.Count(target => target.AccID == id && target.RolID == (int)Roles.Administrator) > 0);
        bool isSalesRep = (db.TblAccountRoles.Count(target => target.AccID == id && target.RolID == (int)Roles.SalesRep) > 0);
        RolMemberLiteral.Visible = isMember;
        RolAdminLiteral.Visible = isAdmin;
        RolSalesRepLiteral.Visible = isSalesRep;

		if ( !Page.IsPostBack )
		{
			// Get the record
			VwAccounts rs = db.VwAccounts.SingleOrDefault( target => target.AccID == id );

			// Verify target record exists
			if ( rs == null )
			{
				throw new WebException( RC.TargetDNE );
			}

			// Set text for enabled link button
            AccEnabledLinkButton.Text = ( rs.AccEnabled ) ? "Disable" : "Enable";

			// Populate the page
            AccIDLiteral.Text = rs.AccID.ToString( );
            AccGuidLiteral.Text = rs.AccGuid.ToString();
            AccEnabledLiteral.Text = rs.AccEnabled ? "Yes" : "No";
            AccFNameLiteral.Text = rs.AccFName;
            AccLNameLiteral.Text = rs.AccLName;
            AccEMailHyperLink.NavigateUrl = "mailto:" + rs.AccEMail;
            AccEMailHyperLink.Text = rs.AccEMail;
            AccEMailHyperLink.ToolTip = "Send E-mail";
            CitNameLiteral.Text = rs.CitName;
            AccPhoneLiteral.Text = rs.AccPhone;
            RfcCodeLiteral.Text = ( rs.RfcCode != null ) ? rs.RfcCode : "N/A" ;
            AtyNameLiteral.Text = ( rs.AtyName != null ) ? rs.AtyName : "Invalid";

            AccGenderLiteral.Text = rs.AccGender;
            AccBirthDateLiteral.Text = rs.AccBirthdate.ToShortDateString();
            AccZipLiteral.Text = rs.AccZip;

            AccFacebookIdentifierLiteral.Text = rs.AccFacebookIdentifier;
            AccTSCreatedLiteral.Text = rs.AccTSCreated.ToShortDateString() + " " + rs.AccTSCreated.ToShortTimeString();

            // display the qr code for this business
            AccQRImage.ImageUrl = QRGenerator.ImageURL(Encryption.MemberIdentifierQURI(rs.AccID), 200);

            TblAccountAnalytics rsAccAnal = db.TblAccountAnalytics.SingleOrDefault(target => target.AccID == rs.AccID);
            if (rsAccAnal != null)
            {
                GlobalStatsLiteral.Visible = rsAccAnal.AcaGlobalStats;
                GlobalAnalyticsLiteral.Visible = rsAccAnal.AcaGlobalAnalytics;
                BusinessStatsLiteral.Visible = rsAccAnal.AcaBusinessStats;
                BusinessAnalyticsLiteral.Visible = rsAccAnal.AcaBusinessAnalytics;
            }
        }
	}

    void AddSubscriptionButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("AccSubNew.aspx?ID=" + id.ToString());
    }

    void AccEnabledLinkButton_Click( object sender, EventArgs e )
	{
		// Get the target account record
        TblAccounts rsAcc = db.TblAccounts.SingleOrDefault( target => target.AccID == id );

		// Verify target account record exists
        if( rsAcc == null )
		{
			throw new WebException( RC.TargetDNE );
		}

		// Toggle enabled bit
        rsAcc.AccEnabled = ( rsAcc.AccEnabled ) ? false : true;

		// Sync to database
		db.SubmitChanges();

		// Reload the page
        Response.Redirect( "AccView.aspx?ID=" + id.ToString( ) );
	}

	void DeleteButton_Click( object sender, EventArgs e )
	{
		// Get the record
        TblAccounts rs = db.TblAccounts.Single( Target => Target.AccID == id );

        // delete the dependent items
        // account roles
        db.ExecuteCommand("DELETE from tblAccountRoles where accid=" + id.ToString());
        // favorites
        db.ExecuteCommand("DELETE from tblFavorites where accid=" + id.ToString());
        // tips
        db.ExecuteCommand("DELETE from tblTips where accid=" + id.ToString());
        // ratings
        db.ExecuteCommand("DELETE from tblRatings where accid=" + id.ToString());
        // checkins
        db.ExecuteCommand("DELETE from tblCheckins where accid=" + id.ToString());
        // redemptions
        db.ExecuteCommand("DELETE from tblRedemptions where accid=" + id.ToString());
        // subscriptions
        db.ExecuteCommand("DELETE from tblSubscriptions where accid=" + id.ToString());

		// Get account
        string account = rs.AccEMail;

		// Delete the record
		db.TblAccounts.DeleteOnSubmit( rs );
		db.SubmitChanges();

		// Redirect to list page
        Response.Redirect( "AccList.aspx?Account=" + account );
	}

    void SendCredentialButton_Click(object sender, EventArgs e)
    {
        // Get the target record from the database
        TblAccounts rs = db.TblAccounts.SingleOrDefault(target => target.AccID == id);

        // Verify the record exists
        if (rs == null)
        {
            throw new WebException(RC.TargetDNE);
        }

        // Set up the e-mail message
        string subject = "UniteThisCity.com Account Credentials";
        string body = "Dear "
            + rs.AccFName + " " + rs.AccLName + "," + Environment.NewLine + Environment.NewLine;

        body += "Your UniteThisCity.com Account has been created for you online. Please use the login credentials below to access the UniteThisCity.com." + Environment.NewLine + Environment.NewLine;
        body += "Below is your login information" + Environment.NewLine;
        body += "________________________________" + Environment.NewLine + Environment.NewLine;
        body += "E-mail Address: " + rs.AccEMail + Environment.NewLine;
        body += "Password: " + rs.AccPassword + Environment.NewLine;
        body += "Log in at URL: " + SiteSettings.GetValue("RootUrl") + "/Account" + Environment.NewLine + Environment.NewLine;
        body += "Thank You," + Environment.NewLine;
        body += "UniteThisCity.com Management";

        // Send the email
        EMail.SendStandard(subject, body, "", rs.AccEMail, SiteSettings.GetValue("EMailReplyTo"), false);

        // Sync to the database
        db.SubmitChanges();

        // Show status message: Email has been sent
        Response.Redirect("AccView.aspx?ID=" + rs.AccID + "&welcomed=true");
    }

    void RolesRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        int acrID = WebConvert.ToInt32(e.CommandArgument, 0);

        db.TblAccountRoles.DeleteOnSubmit(db.TblAccountRoles.Single(target => target.AcrID == acrID));
        db.SubmitChanges();

        Response.Redirect("AccView.aspx?ID=" + id.ToString());
    }

    void RefreshRoles()
    {
        // Get this accounts roles to populate repeater
        RolesRepeater.DataSource = db.VwAccountRoles.Where(target => target.AccID == id && target.BusID != 0).OrderBy(target => target.RolName).ThenBy(target=>target.BusName);
        RolesRepeater.DataBind();

        // Show empty row if there are no deal definitions for this business
        if (db.TblAccountRoles.Count(target => target.AccID == id) == 0)
        {
            NoRolesRow.Visible = true;
        }
    }

    void AddRoleButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("AccAcrNew.aspx?ID=" + id.ToString());
    }

    void FavoritesRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        int favID = WebConvert.ToInt32(e.CommandArgument, 0);

        db.TblFavorites.DeleteOnSubmit(db.TblFavorites.Single(target => target.FavID == favID));
        db.SubmitChanges();

        Response.Redirect("AccView.aspx?ID=" + id.ToString());
    }

    void RefreshFavorites()
    {
        // Get this accounts roles to populate repeater
        FavoritesRepeater.DataSource = db.VwFavorites.Where(target => target.AccID == id).OrderBy(target => target.LocName);
        FavoritesRepeater.DataBind();

        // Show empty row if there are no deal definitions for this business
        if (db.TblFavorites.Count(target => target.AccID == id) == 0)
        {
            NoFavoritesRow.Visible = true;
        }
    }

    void AddFavoriteButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("AccFavNew.aspx?ID=" + id.ToString());
    }

    void TipsRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        int tipID = WebConvert.ToInt32(e.CommandArgument, 0);

        db.TblTips.DeleteOnSubmit(db.TblTips.Single(target => target.TipID == tipID));
        db.SubmitChanges();

        Response.Redirect("AccView.aspx?ID=" + id.ToString());
    }

    void RefreshTips()
    {
        // Get this accounts roles to populate repeater
        TipsRepeater.DataSource = db.VwTips.Where(target => target.AccID == id).OrderByDescending(target => target.TipTS);
        TipsRepeater.DataBind();

        // Show empty row if there are no deal definitions for this business
        if (db.TblTips.Count(target => target.AccID == id) == 0)
        {
            NoTipsRow.Visible = true;
        }
    }

    void AddTipButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("AccTipNew.aspx?ID=" + id.ToString());
    }

    void RatingsRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        int ratID = WebConvert.ToInt32(e.CommandArgument, 0);

        db.TblRatings.DeleteOnSubmit(db.TblRatings.Single(target => target.RatID == ratID));
        db.SubmitChanges();

        Response.Redirect("AccView.aspx?ID=" + id.ToString());
    }

    void RefreshRatings()
    {
        // Get this accounts roles to populate repeater
        RatingsRepeater.DataSource = db.VwRatings.Where(target => target.AccID == id).OrderByDescending(target => target.RatTS);
        RatingsRepeater.DataBind();

        // Show empty row if there are no deal definitions for this business
        if (db.TblRatings.Count(target => target.AccID == id) == 0)
        {
            NoRatingRow.Visible = true;
        }
    }

    void AddRatingButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("AccRatNew.aspx?ID=" + id.ToString());
    }

    void CheckInsRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        int chkID = WebConvert.ToInt32(e.CommandArgument, 0);

        db.TblCheckIns.DeleteOnSubmit(db.TblCheckIns.Single(target => target.ChkID == chkID));
        db.SubmitChanges();

        Response.Redirect("AccView.aspx?ID=" + id.ToString());
    }

    void RedemptionsRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        int redID = WebConvert.ToInt32(e.CommandArgument, 0);

        db.TblRedemptions.DeleteOnSubmit(db.TblRedemptions.Single(target => target.RedID == redID));
        db.SubmitChanges();

        Response.Redirect("AccView.aspx?ID=" + id.ToString());
    }

    void RefreshCheckIns()
    {
        // Get this accounts roles to populate repeater
        CheckInsRepeater.DataSource = db.VwCheckIns.Where(target => target.AccID == id).OrderByDescending(target => target.ChkTS);
        CheckInsRepeater.DataBind();

        // Show empty row if there are no deal definitions for this business
        if (db.TblCheckIns.Count(target => target.AccID == id) == 0)
        {
            NoCheckInsRow.Visible = true;
        }
    }

    void AddCheckInButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("AccChkNew.aspx?ID=" + id.ToString());
    }

    void RefreshRedemptions()
    {
        // Get this accounts roles to populate repeater
        RedemptionsRepeater.DataSource = db.VwRedemptions.Where(target => target.AccID == id).OrderByDescending(target => target.RedTS);
        RedemptionsRepeater.DataBind();

        // Show empty row if there are no deal definitions for this business
        if (db.VwRedemptions.Count(target => target.AccID == id) == 0)
        {
            NoRedemptionsRow.Visible = true;
        }
    }

    void AddRedemptionsButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("AccRedNew.aspx?ID=" + id.ToString());
    }

    void SubscriptionRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        int subID = WebConvert.ToInt32(e.CommandArgument, 0);

        db.TblSubscriptions.DeleteOnSubmit(db.TblSubscriptions.Single(target => target.SubID == subID));
        db.SubmitChanges();

        Response.Redirect("AccView.aspx?ID=" + id.ToString());
    }

    void OptOutRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        int optID = WebConvert.ToInt32(e.CommandArgument, 0);

        db.TblOptOuts.DeleteOnSubmit(db.TblOptOuts.Single(target => target.OptID == optID && target.AccID == id));
        db.SubmitChanges();

        Response.Redirect("AccView.aspx?ID=" + id.ToString());
    }

    void RefreshSubscription()
    {
        // Get this accounts roles to populate repeater
        SubscriptionRepeater.DataSource = db.VwSubscriptions.Where(target => target.AccID == id).OrderByDescending(target => target.SubTSCreate);
        SubscriptionRepeater.DataBind();

        // Show empty row if there are no subscriptions for this account
        if (db.TblSubscriptions.Count(target => target.AccID == id) == 0)
        {
            NoSubscriptionRow.Visible = true;
        }
    }

    void RefreshOptOuts()
    {
        // Get this businesses events
        OptOutRepeater.DataSource = db.VwOptOuts.Where(target => target.AccID == id).OrderBy(target => target.OptID);
        OptOutRepeater.DataBind();

        // Show empty row if there are no events for this business
        if (db.VwOptOuts.Count(target => target.AccID == id) == 0)
        {
            NoOptOutsRow.Visible = true;
        }
    }

    void EditButton_Click( object sender, EventArgs e )
	{
        Response.Redirect( "AccEdit.aspx?ID=" + id.ToString( ) );
	}

	void ForcePasswordButton_Click( object sender, EventArgs e )
	{
        Response.Redirect( "AccForcePassword.aspx?ID=" + id.ToString( ) );
	}
}