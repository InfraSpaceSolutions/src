/******************************************************************************
 * Filename: RfcEdit.aspx.cs
 * Project:  unitethiscity.com Administration
 * 
 * Description:
 * Edit an referral code.
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

public partial class admin_RfcEdit : System.Web.UI.Page
{
	int id;
	WebDBContext db = new WebDBContext();

	protected void Page_Load( object sender, EventArgs e )
	{

		// Wire events
        SubmitButton.Click += new EventHandler(OkButton_Click);
        RfcCodeDuplicate.ServerValidate += new ServerValidateEventHandler(RfcCodeDuplicate_ServerValidate);
        BusIDDropDownList.SelectedIndexChanged += new EventHandler(BusIDDropDownList_SelectedIndexChanged);

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
            VwReferralCodes rs = db.VwReferralCodes.SingleOrDefault( target => target.RfcID == id );

			// Verify target record exists
			if ( rs == null )
			{
				throw new WebException( RC.TargetDNE );
			}

			// Populate the page
            RfcIDLiteral.Text = rs.RfcID.ToString();
            RfcGuidLiteral.Text = rs.RfcGuid.ToString();
            RfcCodeTextBox.Text = rs.RfcCode.ToString();
            RfcAllowCheckinDropDownList.SelectedValue = rs.RfcAllowCheckin.ToString();
            RfcAllowRedeemDropDownList.SelectedValue = rs.RfcAllowRedeem.ToString();

            // Load the businesses dropdown list
            List<TblBusinesses> rsBusiness = (from rows in db.TblBusinesses orderby rows.BusName select rows).ToList();
            BusIDDropDownList.DataSource = rsBusiness;
            BusIDDropDownList.DataBind();

            // Load the accounts/owners dropdown list
            List<TblAccounts> rsAcc = (from rows in db.TblAccounts  orderby rows.AccEMail select rows ).ToList();
            RfcOwnerDropDownList.DataSource = rsAcc;
            RfcOwnerDropDownList.DataBind();

            BusIDDropDownList.SelectedValue = rs.BusID.ToString();
            RfcOwnerDropDownList.SelectedValue = rs.RfcOwner.ToString();

            // enable/disable the allow checkin/redeem options if a business is not selected
            if (BusIDDropDownList.SelectedValue != "0")
            {
                RfcAllowCheckinDropDownList.Enabled = true;
                RfcAllowRedeemDropDownList.Enabled = true;
            }
            else
            {
                RfcAllowCheckinDropDownList.Enabled = false;
                RfcAllowRedeemDropDownList.Enabled = false;
            }

            ProIDDropDownList.DataSource = db.TblPromotions;
            ProIDDropDownList.DataBind();
            ProIDDropDownList.SelectedValue = rs.ProID.ToString();
		}
	}

    void RfcCodeDuplicate_ServerValidate(object source, ServerValidateEventArgs args)
	{
		// Check for duplicate referral code
        args.IsValid = (db.TblReferralCodes.Count(target => target.RfcID != id && target.RfcCode == args.Value.Trim()) == 0);
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

	void OkButton_Click( object sender, EventArgs e )
	{
		// Validate the page
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


		// Get the record
        TblReferralCodes rs = db.TblReferralCodes.Single( target => target.RfcID == id );

		// Verify target record exists
		if ( rs == null )
		{
			throw new WebException( RC.TargetDNE );
		}

        // Update the record
        rs.RfcCode = WebConvert.Truncate(RfcCodeTextBox.Text.Trim(), 50).ToLower();
        rs.RfcOwner = rfcOwner;
        rs.BusID = busid;
        rs.ProID = WebConvert.ToInt32(ProIDDropDownList.SelectedValue, 0);

        if (WebConvert.ToInt32(BusIDDropDownList.SelectedValue, 0) != 0)
        {
            rs.RfcAllowCheckin = WebConvert.ToBoolean(RfcAllowCheckinDropDownList.SelectedValue, false);
            rs.RfcAllowRedeem = WebConvert.ToBoolean(RfcAllowRedeemDropDownList.SelectedValue, false);
        }
        else
        {
            rs.RfcAllowCheckin = false;
            rs.RfcAllowRedeem = false;
        }

		// Sync to database
		db.SubmitChanges();

		// Redirect to target view page
        Response.Redirect( "RfcView.aspx?ID=" + id.ToString( ) );
	}
}