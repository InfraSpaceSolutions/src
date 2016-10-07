/******************************************************************************
 * Filename: AccEdit.aspx.cs
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

public partial class admin_AccEdit : System.Web.UI.Page
{
	int id;
	WebDBContext db = new WebDBContext();

	protected void Page_Load( object sender, EventArgs e )
	{

		// Wire events
		SubmitButton.Click += new EventHandler( OkButton_Click );
        AccEMailDuplicate.ServerValidate += new ServerValidateEventHandler( AccEMailDuplicate_ServerValidate );

		// Get the target id
		id = WebConvert.ToInt32( Request.QueryString["ID"], 0 );

		// Verify id exists
		if ( id == 0 )
		{
			throw new WebException( RC.DataIncomplete );
		}

		if ( !Page.IsPostBack )
		{
			// Get the target record
            TblAccounts rs = db.TblAccounts.SingleOrDefault( target => target.AccID == id );

			// Verify target record exists
			if ( rs == null )
			{
				throw new WebException( RC.TargetDNE );
			}

			// Populate the page
            AccIDLiteral.Text = rs.AccID.ToString( );
            AccGuidLiteral.Text = rs.AccGuid.ToString();
            AccFNameTextBox.Text = rs.AccFName;
            AccLNameTextBox.Text = rs.AccLName;
            AccEMailTextBox.Text = rs.AccEMail;
            AccPhoneTextBox.Text = rs.AccPhone;

            AccFacebookIdentifierTextBox.Text = rs.AccFacebookIdentifier.ToString();

            AccTSCreatedTextBox.Text = rs.AccTSCreated.ToShortDateString() + " " + rs.AccTSCreated.ToShortTimeString();

            // Load the cities dropdown list
            List<TblCities> rsCities = (from rows in db.TblCities select rows).ToList();
            CitIDDropDownList.DataSource = rsCities;
            CitIDDropDownList.DataBind();

            // Load the referrals dropdown list
            List<TblReferralCodes> rsRfc = (from rows in db.TblReferralCodes select rows).ToList();
            RfcIDDropDownList.DataSource = rsRfc;
            RfcIDDropDownList.DataBind();

            // load the account type dropdown
            IEnumerable<TblAccountTypes> rsAty = (from rows in db.TblAccountTypes select rows);
            AtyIDDropDownList.DataSource = rsAty;
            AtyIDDropDownList.DataBind();

            CitIDDropDownList.SelectedValue = rs.CitID.ToString();
            RfcIDDropDownList.SelectedValue = rs.RfcID.ToString();
            AtyIDDropDownList.SelectedValue = rs.AtyID.ToString();

            // prepare the role checkboxes
            bool isMember = (db.TblAccountRoles.Count(target => target.AccID == id && target.RolID == (int)Roles.Member) > 0);
            bool isAdmin = (db.TblAccountRoles.Count(target => target.AccID == id && target.RolID == (int)Roles.Administrator) > 0);
            bool isSalesRep = (db.TblAccountRoles.Count(target => target.AccID == id && target.RolID == (int)Roles.SalesRep) > 0);
            RolMemberCheckbox.Checked = isMember;
            RolAdminCheckbox.Checked = isAdmin;
            RolSalesRepCheckbox.Checked = isSalesRep;

            AccGenderDropDownList.SelectedValue = rs.AccGender;
            AccBirthdateTextBox.Text = rs.AccBirthdate.ToShortDateString();
            AccZipTextBox.Text = rs.AccZip;

            TblAccountAnalytics rsAccAnal = db.TblAccountAnalytics.SingleOrDefault(target => target.AccID == rs.AccID);
            if (rsAccAnal != null)
            {
                GlobalStatsCheckbox.Checked = rsAccAnal.AcaGlobalStats;
                GlobalAnalyticsCheckbox.Checked = rsAccAnal.AcaGlobalAnalytics;
                BusinessStatsCheckbox.Checked = rsAccAnal.AcaBusinessStats;
                BusinessAnalyticsCheckbox.Checked = rsAccAnal.AcaBusinessAnalytics;
            }
            else
            {
                GlobalStatsCheckbox.Checked = false;
                GlobalAnalyticsCheckbox.Checked = false;
                BusinessStatsCheckbox.Checked = false;
                BusinessAnalyticsCheckbox.Checked = false;
            }
        }
	}

	void AccEMailDuplicate_ServerValidate( object source, ServerValidateEventArgs args )
	{
		// Check for duplicate account name
        args.IsValid = ( db.TblAccounts.Count( target => target.AccID != id && target.AccEMail == args.Value.Trim( ) ) == 0 );
	}

	void OkButton_Click( object sender, EventArgs e )
	{
		// Validate the page
		if ( !Page.IsValid )
		{
			return;
		}

		// Get the record
        TblAccounts rs = db.TblAccounts.Single( target => target.AccID == id );

		// Verify target record exists
		if ( rs == null )
		{
			throw new WebException( RC.TargetDNE );
		}

		// Update the record
        rs.AccFName = WebConvert.Truncate( AccFNameTextBox.Text.Trim( ), 50 );
        rs.AccLName = WebConvert.Truncate( AccLNameTextBox.Text.Trim( ), 50 );
        rs.AccEMail = WebConvert.Truncate(AccEMailTextBox.Text.Trim(), 128);
        rs.CitID = WebConvert.ToInt32(CitIDDropDownList.SelectedValue, 0);
        rs.RfcID = WebConvert.ToInt32(RfcIDDropDownList.SelectedValue, 0);
        rs.AccPhone = WebConvert.Truncate(AccPhoneTextBox.Text.Trim(), 50);
        rs.AccFacebookIdentifier = WebConvert.Truncate(AccFacebookIdentifierTextBox.Text.Trim(), 50);
        DateTime newTSCreated = DateTime.Now;
        if (DateTime.TryParse(AccTSCreatedTextBox.Text, out newTSCreated))
        {
            rs.AccTSCreated = newTSCreated;
        }
                
        rs.AccBirthdate = WebConvert.ToDateTime(AccBirthdateTextBox.Text, DateTime.Today);
        rs.AccGender = WebConvert.ToString(AccGenderDropDownList.SelectedValue, "?");
        rs.AccZip = WebConvert.Truncate(AccZipTextBox.Text.Trim(), 10);

		// Sync to database
		db.SubmitChanges();

        // upsert the statistics and analytics permissions
        TblAccountAnalytics rsAccAnal = db.TblAccountAnalytics.SingleOrDefault(target => target.AccID == rs.AccID);
        if (rsAccAnal == null)
        {
            rsAccAnal = new TblAccountAnalytics();
            rsAccAnal.AccID = rs.AccID;
            db.TblAccountAnalytics.InsertOnSubmit(rsAccAnal);
        }
        rsAccAnal.AcaGlobalStats = GlobalStatsCheckbox.Checked;
        rsAccAnal.AcaGlobalAnalytics = GlobalAnalyticsCheckbox.Checked;
        rsAccAnal.AcaBusinessStats = BusinessStatsCheckbox.Checked;
        rsAccAnal.AcaBusinessAnalytics = BusinessAnalyticsCheckbox.Checked;
        db.SubmitChanges();

        // update the roles
        UpdateRole(id, (int)Roles.Member, 100, RolMemberCheckbox.Checked);
        UpdateRole(id, (int)Roles.Administrator, 100, RolAdminCheckbox.Checked);
        UpdateRole(id, (int)Roles.SalesRep, 100, RolSalesRepCheckbox.Checked);

		// Redirect to target view page
        Response.Redirect( "AccView.aspx?ID=" + id.ToString( ) );
	}

    /// <summary>
    /// Update the role for the account - either setting the access level, creating a new record or deleting
    /// the record
    /// </summary>
    /// <param name="accid">account identifier</param>
    /// <param name="rolid">role identifier</param>
    /// <param name="acl">access level</param>
    /// <param name="giveRole">true - assign role</param>
    void UpdateRole(int accid, int rolid, int acl, bool giveRole)
    {
        TblAccountRoles rs = db.TblAccountRoles.SingleOrDefault(target => target.AccID == accid && target.RolID == rolid);
        if (giveRole)
        {
            // add the role if it doesnt exist
            if (rs == null)
            {
                rs = new TblAccountRoles();
                db.TblAccountRoles.InsertOnSubmit(rs);
                rs.AccID = accid;
                rs.RolID = rolid;
                rs.BusID = 0;
            }
            // set the access level to 100 by default
            rs.AclID = acl;
        }
        else
        {
            if (rs != null)
            {
                db.TblAccountRoles.DeleteOnSubmit(rs);
            }
        }
        db.SubmitChanges();
    }

}