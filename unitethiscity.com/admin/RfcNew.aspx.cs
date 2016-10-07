/******************************************************************************
 * Filename: RfcNew.aspx.cs
 * Project:  unitethiscity.com Administration
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

public partial class admin_RfcNew : System.Web.UI.Page
{
	WebDBContext db = new WebDBContext();

	protected void Page_Load( object sender, EventArgs e )
	{
		// Wire events
		SubmitButton.Click += new EventHandler( SubmitButton_Click );
        RfcCodeDuplicate.ServerValidate += new ServerValidateEventHandler(RfcCodeDuplicate_ServerValidate);
        BusIDDropDownList.SelectedIndexChanged += new EventHandler(BusIDDropDownList_SelectedIndexChanged);

        if (!Page.IsPostBack)
        {
            // Load the businesses dropdown list
            List<TblBusinesses> rsBusiness = (from rows in db.TblBusinesses orderby rows.BusName select rows).ToList();
            BusIDDropDownList.DataSource = rsBusiness;
            BusIDDropDownList.DataBind();

            // Load the referrals dropdown list
            List<TblAccounts> rsAcc = (from rows in db.TblAccounts orderby rows.AccEMail select rows).ToList();
            RfcOwnerDropDownList.DataSource = rsAcc;
            RfcOwnerDropDownList.DataBind();

            // disable the allow checkin/redeem dropdowns
            RfcAllowCheckinDropDownList.Enabled = false;
            RfcAllowRedeemDropDownList.Enabled = false;

            ProIDDropDownList.DataSource = db.TblPromotions;
            ProIDDropDownList.DataBind();
        }
	}

    void RfcCodeDuplicate_ServerValidate(object source, ServerValidateEventArgs args)
	{
        args.IsValid = ( db.TblReferralCodes.Count( target => target.RfcCode == args.Value.Trim( ) ) == 0 );
	}

    void BusIDDropDownList_SelectedIndexChanged(object sender, EventArgs e)
    {
        int busid = WebConvert.ToInt32(BusIDDropDownList.SelectedValue, 0);
        if (busid != 0)
        {
            RfcAllowCheckinDropDownList.Enabled = true;
            RfcAllowRedeemDropDownList.Enabled = true;
        }
        else
        {
            RfcAllowCheckinDropDownList.Enabled = false;
            RfcAllowRedeemDropDownList.Enabled = false;
        }
    }

	void SubmitButton_Click( object sender, EventArgs e )
	{
		// Validate page
		if ( !Page.IsValid )
		{
			return;
		}

        int busid = WebConvert.ToInt32(BusIDDropDownList.SelectedValue, 0);
        int rfcOwner = WebConvert.ToInt32(RfcOwnerDropDownList.SelectedValue, 0);

        // if we select to assign per business - attempt to do so
        if (rfcOwner == -1)
        {
            TblAccountRoles rsAcr = db.TblAccountRoles.FirstOrDefault(target => target.BusID == busid && target.RolID == (int)Roles.Business);
            rfcOwner = (rsAcr == null) ? 0 : rsAcr.AccID;
        }

		// Create the record
		TblReferralCodes rs = new TblReferralCodes();

		// Populate fields
        rs.RfcGuid = Guid.NewGuid();
        rs.RfcCode = WebConvert.Truncate(RfcCodeTextBox.Text.Trim(), 50).ToLower();
        rs.RfcOwner = rfcOwner;
        rs.BusID = busid;
        rs.RfcAllowCheckin = WebConvert.ToBoolean(RfcAllowCheckinDropDownList.SelectedValue, false);
        rs.RfcAllowRedeem = WebConvert.ToBoolean(RfcAllowRedeemDropDownList.SelectedValue, false);
        rs.ProID = WebConvert.ToInt32(ProIDDropDownList.SelectedValue, 0);

		// Submit to the db
		db.TblReferralCodes.InsertOnSubmit( rs );
		db.SubmitChanges();

		// Redirect to the view page
        Response.Redirect( "RfcView.aspx?ID=" + rs.RfcID.ToString( ) );
	}
}