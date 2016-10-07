/******************************************************************************
 * Filename: AccSubView.aspx.cs
 * Project:  unitethiscity.com Administration
 * 
 * Description:
 * View a subscription for an account.
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

public partial class admin_AccSubView : System.Web.UI.Page
{
    int id;
    int subid;
	WebDBContext db = new WebDBContext();

	protected void Page_Load( object sender, EventArgs e )
	{
        // Wire events
        EditButton.Click += EditButton_Click;
        CancelButton.Click += CancelButton_Click;
        DeleteButton.Click += DeleteButton_Click;

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

            SubIDLiteral.Text = rsSub.SubID.ToString();
            PrdNameLiteral.Text = rsSub.PrdName;
            PrdPriceLiteral.Text = rsSub.PrdPrice.ToString("C");
            SubBillFNameLiteral.Text = rsSub.SubBillFName;
            SubBillLNameLiteral.Text = rsSub.SubBillLName;
            SubBillAddressLiteral.Text = rsSub.SubBillAddress;
            SubBillCityLiteral.Text = rsSub.SubBillCity;
            SubBillStateLiteral.Text = rsSub.SubBillState;
            SubBillZipLiteral.Text = rsSub.SubBillZip;
            SubBillCtrNameLiteral.Text = rsSub.CtrName;
            PtyNameLiteral.Text = rsSub.PtyName;
            CarNameLiteral.Text = rsSub.CarName;
            SubBillCardNumberLiteral.Text = rsSub.SubBillCardNumber;
            SubBillExpMonthLiteral.Text = rsSub.SubBillExpMonth.ToString();
            SubBillExpYearLiteral.Text = rsSub.SubBillExpYear.ToString();
            SubTSCreateLiteral.Text = rsSub.SubTSCreate.ToString();
            SubTSModifyLiteral.Text = rsSub.SubTSModify.ToString();
            SubIPAddressLiteral.Text = rsSub.SubIPAddress.ToString();
            SubPaymentMethodIDLiteral.Text = rsSub.SubPaymentMethodID.ToString();
            ProIDLiteral.Text = rsSub.ProID.ToString();
            ProNameLiteral.Text = WebConvert.ToString(rsSub.ProName, "N/A");
            SubBillDateLiteral.Text = rsSub.SubBillDate.ToShortDateString();
        }
	}

    void DeleteButton_Click(object sender, EventArgs e)
    {
        TblSubscriptions rsSub = db.TblSubscriptions.Single(target => target.SubID == subid);
        if (rsSub.PtyID != (int)PaymentTypes.None)
        {
            throw new WebException(RC.Dependencies);
        }

        db.TblSubscriptions.DeleteOnSubmit(rsSub);
        db.SubmitChanges();
        Response.Redirect("AccView.aspx?ID=" + id.ToString());
    }

    void CancelButton_Click(object sender, EventArgs e)
    {
        TblSubscriptions rsSub = db.TblSubscriptions.Single(target => target.SubID == subid);
        if (rsSub.PtyID == (int)PaymentTypes.AuthNet)
        {
            AuthNetARB arb = new AuthNetARB();
            if (!arb.CancelSubscription(rsSub.SubPaymentMethodID))
            {
                throw new Sancsoft.Web.WebException(arb.ResultCode);
            }
        }
        rsSub.ProID = 0; 
        rsSub.PtyID = (int)PaymentTypes.None;
        rsSub.SubPaymentMethodID = "";
        rsSub.SubBillCardNumber = "";
        rsSub.SubBillExpMonth = 0;
        rsSub.SubBillExpYear = 0;
        db.SubmitChanges();
        Response.Redirect("AccSubView.aspx?ID=" + id.ToString() + "&SubID=" + subid.ToString());
    }

    void EditButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("AccSubEdit.aspx?ID=" + id.ToString() + "&SubID=" + subid.ToString());
    }

}