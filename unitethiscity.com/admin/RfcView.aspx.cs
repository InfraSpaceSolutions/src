/******************************************************************************
 * Filename: RfcView.aspx.cs
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

public partial class admin_RfcView : System.Web.UI.Page
{
	int id;
	WebDBContext db = new WebDBContext();

	protected void Page_Load( object sender, EventArgs e )
	{
		// Wire events
		EditButton.Click += new EventHandler( EditButton_Click );
        DeleteButton.Click += new EventHandler(DeleteButton_Click);

        // Get the target id
		id = WebConvert.ToInt32( Request.QueryString["ID"], 0 );

		// Verify id exists
		if ( id == 0 )
		{
			throw new WebException( RC.DataIncomplete );
		}

        // Display the account roles in the repeater
        RefreshAccounts();

		if ( !Page.IsPostBack )
		{
			// Get the record
			VwReferralCodes rs = db.VwReferralCodes.SingleOrDefault( target => target.RfcID == id );

			// Verify target record exists
			if ( rs == null )
			{
				throw new WebException( RC.TargetDNE );
			}

			// Populate the page
            RfcIDLiteral.Text = rs.RfcID.ToString();
            RfcGuidLiteral.Text = rs.RfcGuid.ToString();
            RfcAllowCheckinLiteral.Text = rs.RfcAllowCheckin ? "Yes" : "No";
            RfcAllowRedeemLiteral.Text = rs.RfcAllowRedeem ? "Yes" : "No";
            RfcCodeLiteral.Text = rs.RfcCode;
            RfcCodeHyperlink.Text = Encryption.ReferralQURI(rs.RfcCode);
            RfcCodeHyperlink.NavigateUrl = RfcCodeHyperlink.Text;
            AccEMailLiteral.Text = (rs.RfcOwner != 0) ? "<a href=\"AccView.aspx?ID=" + rs.RfcOwner + "\">" + rs.AccEMail + "</a>" : "N/A";
            BusFormalNameLiteral.Text = (rs.BusID != 0) ? rs.BusFormalName : "N/A";
            RfcQRImage.ImageUrl = QRGenerator.ImageURL(RfcCodeHyperlink.NavigateUrl, 200);
            AccCountLiteral.Text = "(" + WebConvert.ToString(db.TblAccounts.Count(target => target.RfcID == id), "") + ")";
            ProNameLiteral.Text = (rs.ProID != 0) ? rs.ProName : "N/A";
		}
	}

    void RefreshAccounts()
    {
        // Get this accounts roles to populate repeater
        AccountsRepeater.DataSource = db.VwAccounts.Where(target => target.RfcID == id).OrderBy(target => target.AccEMail);
        AccountsRepeater.DataBind();

        // Show empty row if there are no deal definitions for this business
        if (db.TblAccounts.Count(target => target.RfcID == id) == 0)
        {
            NoAccountsRow.Visible = true;
        }
    }

    void DeleteButton_Click(object sender, EventArgs e)
    {
        // Get the record
        TblReferralCodes rs = db.TblReferralCodes.Single(Target => Target.RfcID == id);

        // remove the deleted referral code from all accounts that use it
        foreach (TblAccounts acc in db.TblAccounts.Where(target => target.RfcID == id))
        {
            acc.RfcID = 0;
        }

        // Get account
        string code = rs.RfcCode;

        // Delete the record
        db.TblReferralCodes.DeleteOnSubmit(rs);

        // sync to database
        db.SubmitChanges();

        // Redirect to list page
        Response.Redirect("RfcList.aspx?Code=" + code);
    }

	void EditButton_Click( object sender, EventArgs e )
	{
        Response.Redirect( "RfcEdit.aspx?ID=" + id.ToString( ) );
	}
}