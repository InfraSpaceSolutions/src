/******************************************************************************
 * Filename: SiteSubscription.cs
 * Project:  unitethiscity.com
 * 
 * Description:
 * Class for managing an account subscription.  Interfaces local data storage
 * and PayPal connections through Payflow
******************************************************************************/
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;
using Sancsoft.Web;

public class SiteSubscription
{
    WebDBContext db = new WebDBContext();

    /// <summary>
    /// Define the launch date for the earliest billing date
    /// </summary>
    protected DateTime launchDate = new DateTime(2013, 5, 1);

    /// <summary>
    /// Properties
    /// </summary>
    #region Properties
    protected int subID;
    public int SubID
    {
        get
        {
            return subID;
        }
    }
    protected int accID;
    public int AccID
    {
        get
        {
            return accID;
        }
    }
    public int PrdID { get; set; }
    public string SubBillFName { get; set; }
    public string SubBillLName { get; set; }
    public string SubBillAddress { get; set; }
    public string SubBillCity { get; set; }
    public string SubBillState { get; set; }
    public string SubBillZip { get; set; }
    public int SubBillCtrID { get; set; }
    public int SubBillCarID { get; set; }
    public string SubBillCardNumber { get; set; }
    public int SubBillExpMonth { get; set; }
    public int SubBillExpYear { get; set; }
    public DateTime SubTSCreate { get; set; }
    public DateTime SubTSModify { get; set; }
    public string SubIPAddress { get; set; }
    public string SubPaymentMethodID { get; set; }
    public int PtyID { get; set; }
    public int ProID { get; set; }
    public DateTime SubBillDate { get; set; }

    public string PrdName { get; set; }
    public decimal PrdPrice { get; set; }
    public string PtyName { get; set; }
    public string AccEMail { get; set; }
    public string AccFName { get; set; }
    public string AccLName { get; set; }
    public string AccPhone { get; set; }
    public string CarName { get; set; }
    public string CtrName { get; set; }
    public string ProName { get; set; }

    // payment processing fields that are not persistent
    protected string cardNumber;
    public string CardNumber
    {
        get
        {
            return cardNumber;
        }
        set
        {
            cardNumber = value;
            SubBillCardNumber = CreditCardUtility.ExpungeCardNumber(CardNumber);
        }
    }
    public string CVV2 { get; set; }

    public RC ResultCode { get; set; }

    #endregion Properties


    /// <summary>
    /// Create a site account object using the current session variables; creates a null user if not authenticated
    /// </summary>
    public SiteSubscription()
    {
        int account = WebConvert.ToInt32(HttpContext.Current.Session["ACCOUNT_ID"], 0);
        if (account == 0)
        {
            throw new WebException(RC.InternalError);
        }

        /// get the subscription from the database or load defaults if no subscription found
        if (!LoadFromDatabase(account))
        {
            LoadDefaults();
        }
    }

    /// <summary>
    /// Create a site subscription for a specified account; creates a null subscription if the subscription could 
    /// not be found
    /// </summary>
    /// <param name="account"></param>
    public SiteSubscription(int account)
    {
        /// get the subscription from the database or load defaults if no subscription found
        if (!LoadFromDatabase(account))
        {
            LoadDefaults();
        }
    }

    /// <summary>
    /// Load the subscription information from the database for the specified account
    /// </summary>
    /// <param name="account">identify account</param>
    /// <returns>true if a subscription was loaded</returns>
    protected bool LoadFromDatabase(int account)
    {
        VwSubscriptions rs = db.VwSubscriptions.SingleOrDefault(target => target.AccID == account);
        if (rs ==null)
        {
            return false;
        }

        subID = rs.SubID;
        accID = rs.AccID;
        PrdID = rs.PrdID;
        SubBillFName = rs.SubBillFName;
        SubBillLName = rs.SubBillLName;
        SubBillAddress = rs.SubBillAddress;
        SubBillCity = rs.SubBillCity;
        SubBillState = rs.SubBillState;
        SubBillZip = rs.SubBillZip;
        SubBillCtrID = rs.SubBillCtrID;
        SubBillCarID = rs.SubBillCarID;
        SubBillCardNumber = rs.SubBillCardNumber;
        SubBillExpMonth = rs.SubBillExpMonth;
        SubBillExpYear = rs.SubBillExpYear;
        SubTSCreate = rs.SubTSCreate;
        SubTSModify = rs.SubTSCreate;
        SubIPAddress = rs.SubIPAddress;
        SubPaymentMethodID = rs.SubPaymentMethodID;
        PtyID = rs.PtyID;
        ProID = rs.ProID;
        SubBillDate = rs.SubBillDate;
        PrdName = rs.PrdName;
        PrdPrice = rs.PrdPrice;
        PtyName = rs.PtyName;
        AccEMail = rs.AccEMail;
        AccFName = rs.AccFName;
        AccLName = rs.AccLName;
        CarName = rs.CarName;
        CtrName = rs.CtrName;
        ProName = rs.ProName;
        AccPhone = rs.AccPhone;

        // load the properties from the database fields
        return true;
    }

    /// <summary>
    /// Load the object with default data
    /// </summary>
    protected void LoadDefaults()
    {
        subID = 0;
        accID = 0;
        PrdID = 0;
        SubBillFName = "";
        SubBillLName = "";
        SubBillAddress = "";
        SubBillCity = "";
        SubBillState = "";
        SubBillZip = "";
        SubBillCtrID = 0;
        SubBillCarID = 0;
        SubBillCardNumber = "";
        SubBillExpMonth = 0;
        SubBillExpYear = 0;
        SubTSCreate = DateTime.Now;
        SubTSModify = DateTime.Now;
        SubIPAddress = HttpContext.Current.Request.UserHostAddress;
        SubPaymentMethodID = "";
        PtyID = 0;
        ProID = 0;
        SubBillDate = DateTime.Today;
        PrdName = "";
        PrdPrice = 0;
        PtyName = "";
        AccEMail = "";
        AccFName = "";
        AccLName = "";
        CarName = "";
        CtrName = "";
        ProName = "";
        AccPhone = "";
    }

    /// <summary>
    /// Save changes to the account information
    /// </summary>
    /// <returns>true if changes saved</returns>
    public bool SaveChanges()
    {
        TblSubscriptions rs = db.TblSubscriptions.SingleOrDefault(target => target.AccID == AccID);
        if (rs == null)
        {
            rs = new TblSubscriptions();
            rs.AccID = AccID;
            db.TblSubscriptions.InsertOnSubmit(rs);
        }

        rs.SubID = subID;
        rs.PrdID = PrdID;
        rs.SubBillFName = SubBillFName;
        rs.SubBillLName = SubBillLName;
        rs.SubBillAddress = SubBillAddress;
        rs.SubBillCity = SubBillCity;
        rs.SubBillState = SubBillState;
        rs.SubBillZip = SubBillZip;
        rs.SubBillCtrID = SubBillCtrID;
        rs.SubBillCarID = SubBillCarID;
        rs.SubBillCardNumber = SubBillCardNumber;
        rs.SubBillExpMonth = SubBillExpMonth;
        rs.SubBillExpYear = SubBillExpYear;
        rs.SubTSCreate = SubTSCreate;
        rs.SubTSModify = SubTSModify;
        rs.SubIPAddress = SubIPAddress;
        rs.SubPaymentMethodID = SubPaymentMethodID;
        rs.PtyID = PtyID;
        rs.ProID = ProID;
        rs.SubBillDate = SubBillDate;

        db.SubmitChanges();

        return true;
    }

    /// <summary>
    /// Check to see if the subscription record is valid
    /// </summary>
    /// <returns>true if this is a valid subscription record</returns>
    public bool Valid()
    {
        return ((subID != 0) && (PtyID > 0));
    }

    /// <summary>
    /// Update the billing information
    /// </summary>
    /// <param name="flds">form fields</param>
    /// <returns>true if successful</returns>
    public bool UpdateBilling(NameValueCollection flds)
    {
        ResultCode = RC.Ok;

        // confirm that the required fields have been completed
        WebValidator wv = new WebValidator(flds);
        if (!wv.RequiredFields("txtSubBillCardNumber;selSubBillExpMonth;selSubBillExpYear;txtSubBillCVV;txtSubBillFName;txtSubBillLName;txtSubBillAddress;txtSubBillCity;selSubBillState;txtSubBillZip"))
        {
            ResultCode = RC.DataIncomplete;
            return false;
        }

        // capture and clean the submitted fields
        SubBillFName = WebConvert.Truncate(WebConvert.ToString(flds["txtSubBillFName"], ""), 50);
        SubBillLName = WebConvert.Truncate(WebConvert.ToString(flds["txtSubBillLName"], ""), 50);
        SubBillAddress = WebConvert.Truncate(WebConvert.ToString(flds["txtSubBillAddress"], ""), 128);
        SubBillCity = WebConvert.Truncate(WebConvert.ToString(flds["txtSubBillCity"], ""), 50);
        SubBillState = WebConvert.Truncate(WebConvert.ToString(flds["selSubBillState"], ""), 2);
        SubBillZip = WebConvert.Truncate(WebConvert.ToString(flds["txtSubBillZip"], ""), 20);
        SubBillCtrID = 186; // all orders are from the united states
        SubBillCarID = 0; // card type will be detected from the number
        CardNumber = WebConvert.Truncate(WebConvert.ToString(flds["txtSubBillCardNumber"], ""), 20);
        CVV2 = WebConvert.Truncate(WebConvert.ToString(flds["txtSubBillCVV"],""), 4); 
        SubBillExpMonth = WebConvert.ToInt32(flds["selSubBillExpMonth"], 0);
        SubBillExpYear = WebConvert.ToInt32(flds["selSubBillExpYear"], 0);


        // validate the credit card submission
        CreditCardUtility ccu = new CreditCardUtility(CardNumber, SubBillExpMonth, SubBillExpYear);
        int crint = (int)ccu.Check(true);
        ResultCode = (crint > 0) ? (RC)(crint + (int)RC.CCRBase) : RC.Ok;

        // get the detected credit card type
        SubBillCarID = (int)ccu.CardType;

        // stop here if we have an error
        if (ResultCode != RC.Ok)
        {
            return false;
        }

        // attempt to update the arb record
        if (!UpdateAuthorizeNet())
        {
            return false;
        }

        // save the changes to the subscription
        if (!SaveSubscription())
        {
            return false;
        }

        return (ResultCode == RC.Ok);
    }

    /// <summary>
    /// Cancel an existing subscription
    /// </summary>
    /// <returns>returns true if successful</returns>
    public bool CancelSubscription()
    {
        ResultCode = RC.Ok;
        switch (PtyID)
        {
            case (int)PaymentTypes.AuthNet:
                CancelAuthorizeNet();
                break;
            case (int)PaymentTypes.Paypal:
                CancelPaypal();
                break;
            default:
                throw new WebException(RC.DataInvalid);
        }

        if (ResultCode != RC.Ok)
        {
            return false;
        }

        // send a notification to the administrator that the subscription has been cancelled
        NotifyCancellation();

        // change the payment type to none and wipe the link identifier
        ProID = 0; // clear the assigned product
        PtyID = (int)PaymentTypes.None;
        SubPaymentMethodID = "";
        SubBillCardNumber = "";
        SubBillExpMonth = 0;
        SubBillExpYear = 0;
        
        SaveSubscription();
        return (ResultCode == RC.Ok);
    }

    /// <summary>
    /// Send a cancel request to Authorize.net
    /// </summary>
    /// <returns>true if successfully cancelled</returns>
    protected bool CancelAuthorizeNet()
    {
        AuthNetARB authnet = new AuthNetARB();

        authnet.CancelSubscription(SubPaymentMethodID);

        ResultCode = authnet.ResultCode;
        return (ResultCode == RC.Ok);
    }

    /// <summary>
    /// Paypal cancellations are done manually by the site administrators
    /// </summary>
    /// <returns>true if successfully cancelled</returns>
    protected bool CancelPaypal()
    {
        return (ResultCode == RC.Ok);
    }

    /// <summary>
    /// Update the associated authorize.net subscription
    /// </summary>
    /// <returns>true if successful, false RC contains error</returns>
    protected bool UpdateAuthorizeNet()
    {
        AuthNetARB authnet = new AuthNetARB();

        // load the general properties
        authnet.SubscriptionEMail = AccEMail;
        authnet.SubscriptionName = PrdName;
        authnet.SubscriptionPrice = PrdPrice;

        // load the billing information
        authnet.billAddr.First = SubBillFName;
        authnet.billAddr.Last = SubBillLName;
        authnet.billAddr.Phone = "";
        authnet.billAddr.Street = SubBillAddress;
        authnet.billAddr.City = SubBillCity;
        authnet.billAddr.State = SubBillState;
        authnet.billAddr.Zip = SubBillZip;
        authnet.billAddr.Country = "United States";

        // load the credit card fields
        authnet.CardNumber = CardNumber;
        authnet.CardCode = WebConvert.ToString(CVV2, "");
        authnet.CardExpirationMonth = SubBillExpMonth;
        authnet.CardExpirationYear = SubBillExpYear;

        // set the subscription saart date
        authnet.StartDate= SubBillDate;

        // pass along our subscription identifier
        authnet.SubscriptionID = SubPaymentMethodID;

        // attempt to update the subscription
        bool ret = authnet.UpdateSubscription(SubPaymentMethodID);

        // check the results and capture the result code
        if (ret == false)
        {
            ResultCode = authnet.ResultCode;
        }

        // send a notification for the changes to the billing
        NotifyUpdateBilling();

        return ret;
    }

    /// <summary>
    /// Save the subscription information to disk
    /// </summary>
    /// <returns></returns>
    public bool SaveSubscription()
    {
        WebDBContext db = new WebDBContext();
        TblSubscriptions rsSub;
        // either create a new record or update the existing subscription record
        if (SubID == 0)
        {
            rsSub = new TblSubscriptions();
            db.TblSubscriptions.InsertOnSubmit(rsSub);
            rsSub.SubTSCreate = DateTime.Now;
        }
        else
        {
            rsSub = db.TblSubscriptions.Single(target => target.SubID == SubID);
        }

        rsSub.AccID = AccID;
        rsSub.PrdID = PrdID;
        rsSub.SubBillFName = SubBillFName;
        rsSub.SubBillLName = SubBillLName;
        rsSub.SubBillAddress = SubBillAddress;
        rsSub.SubBillCity = SubBillCity;
        rsSub.SubBillState = SubBillState;
        rsSub.SubBillZip = SubBillZip;
        rsSub.SubBillCtrID = SubBillCtrID;
        rsSub.SubBillCarID = SubBillCarID;
        rsSub.SubBillCardNumber = SubBillCardNumber;
        rsSub.SubBillExpMonth = SubBillExpMonth;
        rsSub.SubBillExpYear = SubBillExpYear;
        rsSub.SubTSModify = DateTime.Now;
        rsSub.SubIPAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
        rsSub.SubPaymentMethodID = SubPaymentMethodID;
        rsSub.PtyID = PtyID;
        rsSub.ProID = ProID;
        rsSub.SubBillDate = SubBillDate;

        db.SubmitChanges();

        // get the identifier in case a new record was created
        subID = rsSub.SubID;

        return true;
    }

    /// <summary>
    /// Get the error title for the result code
    /// </summary>
    /// <returns>error title</returns>
    public virtual string ResultTitle()
    {
        TblErrors rsErr = db.TblErrors.SingleOrDefault(target => target.ErrID == (int)ResultCode);
        return (rsErr == null) ? "Unknown Error" : rsErr.ErrTitle;
    }

    /// <summary>
    /// Get the error message for the result code
    /// </summary>
    /// <returns>error title</returns>
    public virtual string ResultMessage()
    {
        TblErrors rsErr = db.TblErrors.SingleOrDefault(target => target.ErrID == (int)ResultCode);
        return (rsErr == null) ? "An undefined error has occurred." : rsErr.ErrMessage;
    }

    /// <summary>
    /// Send an email notification for a change to the billing information
    /// </summary>
    protected void NotifyUpdateBilling()
    {
        StringBuilder msg = new StringBuilder();

        msg.AppendLine("UTC Member Updated Billing Information at " + DateTime.Now + ".");
        msg.AppendLine(new string('=', 60));
        msg.AppendLine("Account ID: " + AccID.ToString());
        msg.AppendLine("Account Name: " + AccFName + " " + AccLName);
        msg.AppendLine("Account EMail: " + AccEMail);
        msg.AppendLine("Account Phone: " + AccPhone);
        msg.AppendLine(new string('=', 60));
        msg.AppendLine("Credit Card Info: " + CreditCardUtility.ExpungeCardNumber(SubBillCardNumber) + " (" + SubBillCarID + ") " + SubBillExpMonth.ToString() + "/" + SubBillExpYear.ToString());
        msg.AppendLine("Billing Address: " + SubBillFName + " " + SubBillLName + "; " + SubBillAddress + "; " + SubBillCity + ", " + SubBillState + " " + SubBillZip);
        msg.AppendLine(new string('=', 60));
        msg.AppendLine("Payment Method ID Code: " + SubPaymentMethodID);
        msg.AppendLine("Payment Type: " + PtyName);

        // send it to the administrator
        EMail.SendStandard("UTC Member Modified Billing Online", msg.ToString(), "", SiteSettings.GetValue("EMailNotify"), SiteSettings.GetValue("EMailReplyTo"), false);
    }

    /// <summary>
    /// Send an email notification for a cancellation of a subscription
    /// </summary>
    protected void NotifyCancellation()
    {
        StringBuilder msg = new StringBuilder();

        msg.AppendLine("UTC Member Cancelled Account at " + DateTime.Now + ".");
        msg.AppendLine(new string('=', 60));
        msg.AppendLine("Account ID: " + AccID.ToString());
        msg.AppendLine("Account Name: " + AccFName + " " + AccLName);
        msg.AppendLine("Account EMail: " + AccEMail);
        msg.AppendLine("Account Phone: " + AccPhone);
        msg.AppendLine(new string('=', 60));
        msg.AppendLine("Credit Card Info: " + CreditCardUtility.ExpungeCardNumber(SubBillCardNumber) + " (" + SubBillCarID + ") " + SubBillExpMonth.ToString() + "/" + SubBillExpYear.ToString());
        msg.AppendLine("Billing Address: " + SubBillFName + " " + SubBillLName + "; " + SubBillAddress + "; " + SubBillCity + ", " + SubBillState + " " + SubBillZip);
        msg.AppendLine(new string('=', 60));
        msg.AppendLine("Payment Method ID Code: " + SubPaymentMethodID);
        msg.AppendLine("Payment Type: " + PtyName);

        switch (PtyID)
        {
            case (int)PaymentTypes.AuthNet:
                msg.AppendLine("Authorize.net payment subscription information has been cancelled electronically.  No action is required.");
                break;
            case (int)PaymentTypes.Paypal:
                msg.AppendLine("This is a PayPal Payflow account.  Actions are required by UTC Staff.");
                msg.AppendLine("*** ACTION REQUIRED ***");
                msg.AppendLine("The subscription must be deactivated manually through the PayPal Payflow interface.  Use the 'Payment Method ID Code' to identify the subscription and cancel it through the PayPal merchant interface.");
                break;
            case (int)PaymentTypes.None:
                msg.AppendLine("The account does not have associated billing information.  No action is required.");
                break;
        }
        // send it to the administrator
        EMail.SendStandard("UTC Member Cancelled Online", msg.ToString(), "", SiteSettings.GetValue("EMailNotify"), SiteSettings.GetValue("EMailReplyTo"), false);
    }

}
