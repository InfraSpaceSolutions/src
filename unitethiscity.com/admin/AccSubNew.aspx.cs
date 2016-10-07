/******************************************************************************
 * Filename: AccSubNew.aspx.cs
 * Project:  unitethiscity.com Administration
 * 
 * Description:
 * Create a new subscription
******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sancsoft.Web;

public partial class admin_AccSubNew : System.Web.UI.Page
{
    int id;
    WebDBContext db = new WebDBContext();

    protected void Page_Load(object sender, EventArgs e)
    {
        // Wire events
        SubmitButton.Click += new EventHandler(SubmitButton_Click);

        // Get the target id
        id = WebConvert.ToInt32(Request.QueryString["ID"], 0);

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

            // Load the products dropdown list
            List<TblProducts> rsPrd = (from rows in db.TblProducts select rows).ToList();
            PrdIDDropDownList.DataSource = rsPrd;
            PrdIDDropDownList.DataBind();

            PrdIDDropDownList.SelectedValue = "1";

            // Load the states dropdown list
            List<TblStates> rsStates = (from rows in db.TblStates where rows.CtrID == 186 select rows).ToList();
            StaIDDropDownList.DataSource = rsStates;
            StaIDDropDownList.DataBind();

            StaIDDropDownList.SelectedValue = "OH";

            // Load the country dropdown list
            List<TblCountries> rsCountries = (from rows in db.TblCountries where rows.CtrID == 186 select rows).ToList();
            CtrIDDropDownList.DataSource = rsCountries;
            CtrIDDropDownList.DataBind();

            CtrIDDropDownList.SelectedValue = "186";

            // load the promotions dropdown list
            ProIDDropDownList.DataSource = db.TblPromotions;
            ProIDDropDownList.DataBind();
            
            // load the payment types
            PtyIDDropDownList.DataSource = db.TblPaymentTypes;
            PtyIDDropDownList.DataBind();

            SubBillExpMonth.Text = DateTime.Today.Month.ToString();
            SubBillExpYear.Text = DateTime.Today.Year.ToString();
        }
    }

    void SubmitButton_Click(object sender, EventArgs e)
    {
        // Validate page
        if (!Page.IsValid)
        {
            return;
        }

        // get the account
        TblAccounts rsAcc = db.TblAccounts.Single(target=>target.AccID == id);

        // get the product
        int prdID = WebConvert.ToInt32(PrdIDDropDownList.SelectedValue, 0);
        TblProducts rsPrd = db.TblProducts.Single(target=>target.PrdID == prdID);

        // creating a new subscription
        TblSubscriptions rs = new TblSubscriptions();

        // create the record
        rs.AccID = id;
        rs.PrdID = prdID;
        rs.SubBillFName = WebConvert.Truncate(SubBillFNameTextBox.Text.Trim(), 50);
        rs.SubBillLName = WebConvert.Truncate(SubBillLNameTextBox.Text.Trim(), 50);
        rs.SubBillAddress = WebConvert.Truncate(SubBillAddressTextBox.Text.Trim(), 128);
        rs.SubBillCity = WebConvert.Truncate(SubBillCityTextBox.Text.Trim(), 50);
        rs.SubBillState = WebConvert.ToString(StaIDDropDownList.SelectedValue, "");
        rs.SubBillZip = WebConvert.Truncate(SubBillZipTextBox.Text.Trim(), 20);
        rs.SubBillCtrID = WebConvert.ToInt32(CtrIDDropDownList.SelectedValue, 0);
        rs.SubBillCarID = 0;
        rs.SubBillCardNumber = WebConvert.ToString(SubBillCardNumber.Text, "");
        rs.SubBillExpMonth = WebConvert.ToInt32(SubBillExpMonth.Text, 0);
        rs.SubBillExpYear = WebConvert.ToInt32(SubBillExpYear.Text, 0);
        rs.SubIPAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
        rs.SubPaymentMethodID = "";
        rs.PtyID = WebConvert.ToInt32(PtyIDDropDownList.SelectedValue, 0);
        rs.ProID = WebConvert.ToInt32(ProIDDropDownList.SelectedValue, 0);
        // set the billing date based on the promotion
        switch (rs.ProID)
        {
            case 1:
                rs.SubBillDate = DateTime.Today.AddMonths(1);
                break;
            default:
                rs.SubBillDate = DateTime.Today.AddDays(1);
                break;
        }
        // override with an explicit billing date
        if (SubBillDateTextBox.Text.Trim().Length > 0)
        {
            rs.SubBillDate = WebConvert.ToDateTime(SubBillDateTextBox.Text, DateTime.Today.AddDays(1));
        }

        // set the timestamps to now
        rs.SubTSCreate = DateTime.Now;
        rs.SubTSModify = DateTime.Now;

        // pre-process the credit card information
        CreditCardUtility ccu = new CreditCardUtility(rs.SubBillCardNumber, rs.SubBillExpMonth, rs.SubBillExpYear);
        CreditCardUtility.CardResults cardResult = ccu.Check(detectCardType:true);
        rs.SubBillCarID = (int)ccu.CardType;

        // if this is auth.net, create the subscription record
        if ( rs.PtyID == (int)PaymentTypes.AuthNet)
        {
            // we need to check the credit card
            if ( cardResult != CreditCardUtility.CardResults.OK )
            {
                int errorNum = (int)Sancsoft.Web.RC.CCRBase + (int)cardResult;
                throw new WebException((Sancsoft.Web.RC)errorNum);
            }
            AuthNetARB arb = new AuthNetARB();
            arb.CardNumber = ccu.CardNumber;
            rs.SubBillCardNumber = ccu.ExpungedCardNumber();
            arb.CardCode = WebConvert.ToString(CardCodeTextBox.Text, "");
            arb.CardExpirationMonth = rs.SubBillExpMonth;
            arb.CardExpirationYear = rs.SubBillExpYear;

            arb.billAddr.First = rs.SubBillFName;
            arb.billAddr.Last = rs.SubBillLName;
            arb.billAddr.Street = rs.SubBillAddress;
            arb.billAddr.City = rs.SubBillCity;
            arb.billAddr.State = rs.SubBillState;
            arb.billAddr.Zip = rs.SubBillZip;
            arb.billAddr.Country = "United States";
            
            arb.SubscriptionEMail = rsAcc.AccEMail;
            arb.SubscriptionName = rsPrd.PrdName;
            arb.SubscriptionPrice = rsPrd.PrdPrice;
            arb.StartDate = rs.SubBillDate;

            if (!arb.CreateSubscription())
            {
                throw new WebException(arb.ResultCode);
            }
            rs.SubPaymentMethodID = arb.SubscriptionID;
        }
        db.TblSubscriptions.InsertOnSubmit(rs);

        // Sync to database
        db.SubmitChanges();

        // Redirect to the location view page
        Response.Redirect("AccSubView.aspx?ID=" + id.ToString() + "&SubID=" + rs.SubID.ToString());
    }
}