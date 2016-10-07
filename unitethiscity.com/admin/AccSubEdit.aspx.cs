/******************************************************************************
 * Filename: AccSubEdit.aspx.cs
 * Project:  unitethiscity.com Administration
 * 
 * Description:
 * Edit a subscription for an account.
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

public partial class admin_AccSubEdit : System.Web.UI.Page
{
    int id;
    int subid;
	WebDBContext db = new WebDBContext();

	protected void Page_Load( object sender, EventArgs e )
	{
        // Wire events
        SubmitButton.Click += new EventHandler(SubmitButton_Click);
        PrdIDDropDownList.SelectedIndexChanged += new EventHandler(PrdIDDropDownList_SelectedIndexChanged);

        // Get the target id
        id = WebConvert.ToInt32(Request.QueryString["ID"], 0);
        subid = WebConvert.ToInt32(Request.QueryString["SubID"], 0);

        if (!Page.IsPostBack)
        {
            // Get the record
            VwAccounts rs = db.VwAccounts.SingleOrDefault(target => target.AccID == id);

            // Verify target record exists
            if (rs == null)
            {
                throw new WebException(RC.TargetDNE);
            }

            // Populate the page
            AccIDLiteral.Text = rs.AccID.ToString();
            AccGuidHyperLink.Text = rs.AccGuid.ToString();
            AccGuidHyperLink.ToolTip = "View Account";
            AccGuidHyperLink.NavigateUrl = "AccView.aspx?ID=" + id.ToString();
            AccFNameLiteral.Text = rs.AccFName;
            AccLNameLiteral.Text = rs.AccLName;
            AccEMailHyperLink.NavigateUrl = "mailto:" + rs.AccEMail;
            AccEMailHyperLink.Text = rs.AccEMail;
            AccEMailHyperLink.ToolTip = "Send E-mail";

            // get the subscription record
            VwSubscriptions rsSub = db.VwSubscriptions.SingleOrDefault(target => target.SubID == subid);
            if (rsSub == null)
            {
                throw new WebException(RC.TargetDNE);
            }

            // set the form values using the subscription record
            SubIDLiteral.Text = rsSub.SubID.ToString();
            PrdPriceLiteral.Text = String.Format("{0:C}", rsSub.PrdPrice);
            SubBillFNameTextBox.Text = rsSub.SubBillFName;
            SubBillLNameTextBox.Text = rsSub.SubBillLName;
            SubBillAddressTextBox.Text = rsSub.SubBillAddress;
            SubBillCityTextBox.Text = rsSub.SubBillCity;
            SubBillZipTextBox.Text = rsSub.SubBillZip;
            CarNameLiteral.Text = rsSub.CarName;
            SubBillCardNumberLiteral.Text = rsSub.SubBillCardNumber;
            SubBillExpMonthLiteral.Text = rsSub.SubBillExpMonth.ToString();
            SubBillExpYearLiteral.Text = rsSub.SubBillExpYear.ToString();
            SubTSCreateLiteral.Text = rsSub.SubTSCreate.ToString();
            SubTSModifyLiteral.Text = rsSub.SubTSModify.ToString();
            SubIPAddressLiteral.Text = rsSub.SubIPAddress.ToString();
            SubPaymentMethodIDLiteral.Text = rsSub.SubPaymentMethodID.ToString();

            // Load the states dropdown list
            List<TblProducts> rsPrd = (from rows in db.TblProducts select rows).ToList();
            PrdIDDropDownList.DataSource = rsPrd;
            PrdIDDropDownList.DataBind();

            PrdIDDropDownList.SelectedValue = rsSub.PrdID.ToString();

            // Load the states dropdown list
            List<TblStates> rsStates = (from rows in db.TblStates where rows.CtrID == 186 select rows).ToList();
            StaIDDropDownList.DataSource = rsStates;
            StaIDDropDownList.DataBind();

            StaIDDropDownList.SelectedValue = rsSub.SubBillState.ToString();

            // Load the country dropdown list
            List<TblCountries> rsCountries = (from rows in db.TblCountries where rows.CtrID == 186 select rows).ToList();
            CtrIDDropDownList.DataSource = rsCountries;
            CtrIDDropDownList.DataBind();

            CtrIDDropDownList.SelectedValue = rsSub.SubBillCtrID.ToString();
        }
	}

    void PrdIDDropDownList_SelectedIndexChanged(object sender, EventArgs e)
    {
        int prdID = WebConvert.ToInt32(PrdIDDropDownList.SelectedValue, 0);

        // check business for an assigned deal definition
        TblProducts rsPrd = db.TblProducts.SingleOrDefault(target => target.PrdID == prdID);
        if (rsPrd != null)
        {
            PrdPriceLiteral.Text = String.Format("{0:C}", rsPrd.PrdPrice);
        }
    }

    void SubmitButton_Click(object sender, EventArgs e)
    {
        // Validate page
        if (!Page.IsValid)
        {
            return;
        }

        // Get the record
        TblSubscriptions rs = db.TblSubscriptions.Single(target => target.AccID == id);

        // Verify target record exists
        if (rs == null)
        {
            throw new WebException(RC.TargetDNE);
        }

        // Update the record
        rs.PrdID = WebConvert.ToInt32(PrdIDDropDownList.SelectedValue, 0);
        rs.SubBillFName = WebConvert.Truncate(SubBillFNameTextBox.Text.Trim(), 50);
        rs.SubBillLName = WebConvert.Truncate(SubBillLNameTextBox.Text.Trim(), 50);
        rs.SubBillAddress = WebConvert.Truncate(SubBillAddressTextBox.Text.Trim(), 128);
        rs.SubBillCity = WebConvert.Truncate(SubBillCityTextBox.Text.Trim(), 50);
        rs.SubBillState = WebConvert.ToString( StaIDDropDownList.SelectedValue, "" );
        rs.SubBillZip = WebConvert.Truncate(SubBillZipTextBox.Text.Trim(), 20);
        rs.SubBillCtrID = WebConvert.ToInt32( CtrIDDropDownList.SelectedValue, 0 );
        rs.SubTSModify = DateTime.Now;

        // Sync to database
        db.SubmitChanges();

        // Redirect to the location view page
        Response.Redirect("AccSubView.aspx?ID=" + id.ToString() + "&SubID=" + subid.ToString());
    }
}