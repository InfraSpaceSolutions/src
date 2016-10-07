/******************************************************************************
 * Filename: AccNew.aspx.cs
 * Project:  acrtinc.com Administration
 * 
 * Description:
 * Create a new account.
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

public partial class admin_AccNew : System.Web.UI.Page
{
	WebDBContext db = new WebDBContext();

	protected void Page_Load( object sender, EventArgs e )
	{
		// Wire events
		SubmitButton.Click += new EventHandler( SubmitButton_Click );
        AccEMailDuplicate.ServerValidate += new ServerValidateEventHandler( AccEMailDuplicate_ServerValidate );

        if (!Page.IsPostBack)
        {
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

        }
	}

    void AccEMailDuplicate_ServerValidate( object source, ServerValidateEventArgs args )
	{
        args.IsValid = ( db.TblAccounts.Count( target => target.AccEMail == args.Value.Trim( ) ) == 0 );
	}

	void SubmitButton_Click( object sender, EventArgs e )
	{
		// Validate page
		if ( !Page.IsValid )
		{
			return;
		}

		// Create the record
		TblAccounts rs = new TblAccounts();

		// Populate fields
        rs.AccGuid = Guid.NewGuid();
        rs.AccFName = WebConvert.Truncate( AccFNameTextBox.Text.Trim( ), 50 );
        rs.AccLName = WebConvert.Truncate( AccLNameTextBox.Text.Trim( ), 50 );
        rs.AccEMail = WebConvert.Truncate( AccEMailTextBox.Text.Trim( ), 128 );
        rs.CitID = WebConvert.ToInt32(CitIDDropDownList.SelectedValue, 0);
        rs.RfcID = WebConvert.ToInt32(RfcIDDropDownList.SelectedValue, 0);
        rs.AccPhone = WebConvert.Truncate(AccPhoneTextBox.Text.Trim(), 50);
        rs.AccEnabled = true;
        rs.AtyID = WebConvert.ToInt32(AtyIDDropDownList.SelectedValue, 0);
        rs.AccGender = WebConvert.Truncate(AccGenderDropDownList.SelectedValue, 10);
        rs.AccBirthdate = WebConvert.ToDateTime(AccBirthdateTextBox, DateTime.Now);
        rs.AccZip = WebConvert.Truncate(AccZipTextBox.Text.Trim(), 10);

        rs.AccFacebookIdentifier = "";
        rs.AccTSCreated = DateTime.Now;

		// Create password if field is left blank
        if( !AccPasswordTextBox.Text.Equals( "" ) )
		{
			// Check for appropriate password length
			if ( AccPasswordTextBox.Text.Length < 6 )
			{
				throw new WebException( RC.BadPassword );
			}

            rs.AccPassword = WebConvert.Truncate( AccPasswordTextBox.Text, 50 );
		}
		else
		{
            rs.AccPassword = Password.GenerateRandom( 6 );
		}

		// Submit to the db
		db.TblAccounts.InsertOnSubmit( rs );
		db.SubmitChanges();

        // update the roles
        int accid = rs.AccID;
        UpdateRole(accid, (int)Roles.Member, 100, RolMemberCheckbox.Checked);
        UpdateRole(accid, (int)Roles.Administrator, 100, RolAdminCheckbox.Checked);
        UpdateRole(accid, (int)Roles.SalesRep, 100, RolSalesRepCheckbox.Checked);

		// Redirect to the view page
        Response.Redirect( "AccView.aspx?ID=" + rs.AccID.ToString( ) );
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