/******************************************************************************
 * Filename: RegisterMember.cs
 * Project:  UTC
 * 
 * Description:
 * Service side class for managing the registration process for a new member
 * through the web interface.
 * 
 * The object takes a name/value collection so it should work for both post
 * and get forms.
******************************************************************************/
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Configuration;
using Sancsoft.Web;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Text;
using System.Reflection;
using PayPal.Payments.Common.Utility;
using PayPal.Payments.Communication;
using PayPal.Payments.DataObjects;

/// <summary>
/// Summary description for RegisterMember
/// </summary>
public class RegisterMember
{
    WebDBContext db;

	/// <summary>
	/// Properties
	/// </summary>
	public RC ResultCode { get; set; }

	public int AccID;
	public int SubID;
    
    /// <summary>
	/// Account Properties
	/// </summary>
	public int CitID { get; set; }
	public int RfcID { get; set; }
	public string AccEMail { get; set; }
	public string AccFName { get; set; }
	public string AccLName { get; set; }
	public string AccPhone { get; set; }
	public string AccPassword { get; set; }
	public bool AccAgreeToTerms { get; set; }
    public Guid AccGuid { get; set; }
    public string AccFacebookIdentifer { get; set; }
    
    /// <summary>
    /// Demographics for free accounts
    /// </summary>
    public string AccGender { get; set; }
    public DateTime AccBirthdate { get; set; }
    public string AccZip { get; set; }

	/// <summary>
	/// Billing Properties
	/// </summary>
	public int PrdID { get; set; }
	public string PromoCode { get; set; }
	public string SubBillCardNumber { get; set; }
	public int SubBillCarID { get; set; }
	public int SubBillExpMonth { get; set; }
	public int SubBillExpYear { get; set; }
	public int SubBillCVV { get; set; }
	public string SubBillFName { get; set; }
	public string SubBillLName { get; set; }
	public string SubBillAddress { get; set; }
	public string SubBillCity { get; set; }
	public string SubBillState { get; set; }
	public string SubBillZip { get; set; }
	public int SubBillCtrID { get; set; }

	/// <summary>
	/// Subscription Properties
	/// </summary>
	public string SubPaymentMethodID { get; set; }
	public PaymentTypes PaymentType { get; set; }
    public int ProID { get; set; }
    public DateTime FirstBillingDate { get; set; }

	/// <summary>
	/// Default constructor; initializes fields
	/// </summary>
	public RegisterMember()
	{
        db = new WebDBContext();

		ResultCode = RC.Ok;
		RfcID = Referrals.GetReferralID();

        // load the promocode with the referral code if it is assigned
        PromoCode = Referrals.GetReferralCode();

        // start billing tommorrow by default
        FirstBillingDate = DateTime.Today.AddDays(1);

        // clear the payment identifier
        SubPaymentMethodID = "";
	}

    /// <summary>
    /// Format the display of the date of birth.  If it is not a reasonable date,
    /// it gets blanked out
    /// </summary>
    /// <returns>date string for form field</returns>
    public string DateOfBirthDisplay()
    {
        return ((AccBirthdate >= DateTime.Today) || (AccBirthdate < DateTime.Today.AddYears(-100)) ? "" : AccBirthdate.ToShortDateString());
    }

	/// <summary>
	/// Process the form submission for Join Us/Create Account page
	/// </summary>
	/// <param name="flds">Collection of form fields from web page</param>
	/// <returns>true=success; false=error - get error code for details</returns>
	public bool ProcessCreateAccount( NameValueCollection flds )
	{
		ResultCode = RC.Ok;

		// confirm that the required fields have been completed
		WebValidator wv = new WebValidator( flds );
		if ( !wv.RequiredFields( "emlAccEMail;txtAccFName;txtAccLName;txtAccZip" ) )
		{
			ResultCode = RC.DataIncomplete;
			return false;
		}

		// all submissions are currently for Cleveland
		CitID = 1;

		// capture and clean the submitted fields
		AccEMail = WebConvert.Truncate( WebConvert.ToString( flds["emlAccEMail"], "" ).ToLower(), 128 );
		AccFName = WebConvert.Truncate( WebConvert.ToString( flds["txtAccFName"], "" ), 50 );
		AccLName = WebConvert.Truncate( WebConvert.ToString( flds["txtAccLName"], "" ), 50 );
		AccPhone = WebConvert.Truncate( WebConvert.ToString( flds["txtAccPhone"], "" ), 50 );

        // no facebook identifier provided
        AccFacebookIdentifer = "";

        // capture the demographic information
        AccGender = WebConvert.Truncate(WebConvert.ToString(flds["selAccGender"], "?"), 10);
        AccBirthdate = WebConvert.ToDateTime(flds["txtAccBirthDate"], DateTime.Today);
        AccZip = WebConvert.Truncate(WebConvert.ToString(flds["txtAccZip"], ""), 10);

        // assign a new guid to the accdount
        AccGuid = Guid.NewGuid();

		// create a random password for the new account
		AccPassword = Password.GenerateRandom( WebConvert.ToInt32( SiteSettings.GetValue( "PasswordMinLength" ), 8 ) );

		// confirm that the email is formatted like an email address
		Regex emailRegex = new Regex( @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" );
		if ( !emailRegex.IsMatch( AccEMail ) )
		{
			ResultCode = RC.EmailInvalid;
			return false;
		}

		// confirm that the email is not already associated with an account
		WebDBContext db = new WebDBContext();
		if ( db.TblAccounts.Where( target => target.AccEMail == AccEMail ).Count() > 0 )
		{
			ResultCode = RC.DuplicateEmail;
			return false;
		}

        // check that the birthdate is reasonable
        int age = DateTime.Today.Year - AccBirthdate.Year;
        if (AccBirthdate > DateTime.Today.AddYears(-age))
        {
            age--;
        }
        // users must be 18 or over and we only expect less than 100
        if ((age < 18) || (age > 100))
        {
            ResultCode = RC.InvalidBirthdate;
            return false;
        }

		return true;
	}

    /// <summary>
    /// Process the billing information for a free account - basically, fill it out with placeholders
    /// </summary>
    /// <param name="flds">form fields</param>
    /// <returns>always succeeds</returns>
    public bool ProcessFreeBillingInfo(NameValueCollection flds)
    {
        ResultCode = RC.Ok;

        // capture and clean the submitted fields
        PrdID = 2; // force it to be a free account
        SubBillFName = "n/a";
        SubBillLName = "n/a";
        SubBillAddress = "n/a";
        SubBillCity = "n/a";
        SubBillState = "na";
        SubBillZip = "n/a";
        SubBillCtrID = 186; // all orders are from the united states
        SubBillCardNumber = "n/a";
        SubBillCarID = 0; // card type will be detected from the number
        SubBillCVV = 0;
        SubBillExpMonth = 0;
        SubBillExpYear = 0;

        // capture the promocode and update the referral identifier
        PromoCode = WebConvert.ToString(flds["txtPromoCode"], "");
        // remove the default promo code value from the web form to avoid confusion on notifications
        PromoCode = (PromoCode == "PROMO CODE?") ? "" : PromoCode;
        // convert the promocode to lowercase for comparisons
        PromoCode = PromoCode.ToLower();
        // assign the promotion from the promo code
        ProID = IdentifyPromotion(PromoCode);
        // update the referral based on the promo code
        RfcID = IdentifyReferral(PromoCode);

        // apply the promotion to the first billing date
        switch (ProID)
        {
            case 1:
                // credit the member one month for free
                FirstBillingDate = DateTime.Today.AddMonths(1);
                break;
            default:
                // no promotion - billing date starts tommorrow
                FirstBillingDate = DateTime.Today.AddDays(1);
                break;
        }

        // get the detected credit card type
        SubBillCarID = 0;

        return true;
    }


	/// <summary>
	/// Process the form submission for the Join Us/Billing Info page
	/// </summary>
	/// <param name="flds">Collection of form fields from web page</param>
	/// <returns>true=success; false=error - get error code for details</returns>
	public bool ProcessBillingInfo( NameValueCollection flds )
	{
		ResultCode = RC.Ok;

		// confirm that the required fields have been completed
		WebValidator wv = new WebValidator( flds );
		if ( !wv.RequiredFields( "radPrdID;txtSubBillCardNumber;selSubBillExpMonth;selSubBillExpYear;txtSubBillCVV;txtSubBillFName;txtSubBillLName;txtSubBillAddress;txtSubBillCity;selSubBillState;txtSubBillZip" ) )
		{
			ResultCode = RC.DataIncomplete;
			return false;
		}

		// capture and clean the submitted fields
		PrdID = WebConvert.ToInt32( flds["radPrdID"], 0 );
		SubBillFName = WebConvert.Truncate( WebConvert.ToString( flds["txtSubBillFName"], "" ), 50 );
		SubBillLName = WebConvert.Truncate( WebConvert.ToString( flds["txtSubBillLName"], "" ), 50 );
		SubBillAddress = WebConvert.Truncate( WebConvert.ToString( flds["txtSubBillAddress"], "" ), 128 );
		SubBillCity = WebConvert.Truncate( WebConvert.ToString( flds["txtSubBillCity"], "" ), 50 );
		SubBillState = WebConvert.Truncate( WebConvert.ToString( flds["selSubBillState"], "" ), 2 );
		SubBillZip = WebConvert.Truncate( WebConvert.ToString( flds["txtSubBillZip"], "" ), 20 );
		SubBillCtrID = 186; // all orders are from the united states
		SubBillCardNumber = WebConvert.Truncate( WebConvert.ToString( flds["txtSubBillCardNumber"], "" ), 20 );
		SubBillCarID = 0; // card type will be detected from the number
		SubBillCVV = WebConvert.ToInt32( flds["txtSubBillCVV"], 0 );
		SubBillExpMonth = WebConvert.ToInt32( flds["selSubBillExpMonth"], 0 );
		SubBillExpYear = WebConvert.ToInt32( flds["selSubBillExpYear"], 0 );

        // capture the promocode and update the referral identifier
        PromoCode = WebConvert.ToString(flds["txtPromoCode"], "");
        // remove the default promo code value from the web form to avoid confusion on notifications
        PromoCode = (PromoCode == "PROMO CODE?") ? "" : PromoCode;
        // convert the promocode to lowercase for comparisons
        PromoCode = PromoCode.ToLower();
        // assign the promotion from the promo code
        ProID = IdentifyPromotion(PromoCode);
        // update the referral based on the promo code
        RfcID = IdentifyReferral(PromoCode);

        // apply the promotion to the first billing date
        switch (ProID)
        {
            case 1:
                // credit the member one month for free
                FirstBillingDate = DateTime.Today.AddMonths(1);
                break;
            default:
                // no promotion - billing date starts tommorrow
                FirstBillingDate = DateTime.Today.AddDays(1);
                break;
        }

		// validate the credit card submission
		CreditCardUtility ccu = new CreditCardUtility( SubBillCardNumber, SubBillExpMonth, SubBillExpYear );
		int crint = (int)ccu.Check( true );
		ResultCode = ( crint > 0 ) ? (RC)( crint + (int)RC.CCRBase ) : RC.Ok;

		// get the detected credit card type
		SubBillCarID = (int)ccu.CardType;

		return ( ResultCode == RC.Ok );
	}

    /// <summary>
    /// Process the form submission for a free account - perform the 
    /// confirmation step
    /// </summary>
    /// <param name="flds"></param>
    /// <returns>true - doesn't normally fail</returns>
    public bool ProcessFreeConfirmation(NameValueCollection flds)
    {
        ResultCode = RC.Ok;

        if (ResultCode != RC.Ok)
        {
            throw new Sancsoft.Web.WebException(ResultCode);
        }

        // create the account record
        CreateAccount();

        // free accounts do not require a subscription

        // send out the new member notification to the member
        NotifyNewMember();

        // send out the new member notification to the administrators
        NotifyAdministrator();

        return true;
    }


	/// <summary>
	/// Process the form submission for the Join Us/Confirmation page
	/// </summary>
	/// <param name="flds">Collection of form fields from web page</param>
	/// <returns>true=success; false=error - get error code for details</returns>
	public bool ProcessConfirmation( NameValueCollection flds )
	{
		ResultCode = RC.Ok;

        // create the payment link on authorize.net
        CreateAuthNetSubscription();

        if (ResultCode != RC.Ok)
        {
            throw new Sancsoft.Web.WebException(ResultCode);
        }

		// create the account record
		CreateAccount();

		// create the subscription
		CreateSubscription();

		// send out the new member notification to the member
		NotifyNewMember();

		// send out the new member notification to the administrators
		NotifyAdministrator();

		return true;
	}

	/// <summary>
	/// Create an account record from the fields in the object
	/// </summary>
	protected void CreateAccount()
	{
		// create a new account record
		TblAccounts rsAcc = new TblAccounts();
        rsAcc.AccGuid = AccGuid;
		rsAcc.CitID = CitID;
		rsAcc.RfcID = RfcID;
		rsAcc.AccEMail = AccEMail;
		rsAcc.AccFName = AccFName;
		rsAcc.AccLName = AccLName;
		rsAcc.AccPhone = AccPhone;
		rsAcc.AccPassword = AccPassword;
		rsAcc.AccEnabled = true;

        rsAcc.AccFacebookIdentifier = AccFacebookIdentifer;
        rsAcc.AccTSCreated = DateTime.Now;

        rsAcc.AccBirthdate = AccBirthdate;
        rsAcc.AccGender = AccGender;
        rsAcc.AccZip = AccZip;

        // all accounts are created as perpetual now that the service is free!
        rsAcc.AtyID = (int)AccountTypes.Perpetual;

		db.TblAccounts.InsertOnSubmit( rsAcc );
		db.SubmitChanges();

		// capture the id of the new account to link up roles
		AccID = rsAcc.AccID;

		// create a member role for the account
		TblAccountRoles rsAcr = new TblAccountRoles();
		rsAcr.AccID = AccID;
		rsAcr.RolID = (int)Roles.Member;
		rsAcr.BusID = 0; // no business associated with a member role
		db.TblAccountRoles.InsertOnSubmit( rsAcr );
		db.SubmitChanges();
	}

	/// <summary>
	/// Create a subscription record from the fields in the object
	/// </summary>
	protected void CreateSubscription()
	{
        PaymentType = PaymentTypes.AuthNet;

        // make sure that we got back a id to identify the account
		Debug.Assert( AccID != 0 );

		// create a new subscription record
		TblSubscriptions rsSub = new TblSubscriptions();
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
		rsSub.SubBillCardNumber = CreditCardUtility.ExpungeCardNumber( SubBillCardNumber );
		rsSub.SubBillExpMonth = SubBillExpMonth;
		rsSub.SubBillExpYear = SubBillExpYear;
		rsSub.SubPaymentMethodID = "";
		rsSub.SubTSCreate = DateTime.Now;
		rsSub.SubTSModify = DateTime.Now;
		rsSub.SubIPAddress = "";
		rsSub.PtyID = (int)PaymentType;
        rsSub.ProID = ProID;
        rsSub.SubPaymentMethodID = SubPaymentMethodID;
        rsSub.SubBillDate = FirstBillingDate;

		db.TblSubscriptions.InsertOnSubmit( rsSub );
		db.SubmitChanges();

		// capture the id of the new subscription
		SubID = rsSub.SubID;
	}

	/// <summary>
	/// Get the error title for the result code
	/// </summary>
	/// <returns>error title</returns>
	public virtual string ResultTitle()
	{
		TblErrors rsErr = db.TblErrors.SingleOrDefault( target => target.ErrID == (int)ResultCode );
		return ( rsErr == null ) ? "Unknown Error" : rsErr.ErrTitle;
	}

	/// <summary>
	/// Get the error message for the result code
	/// </summary>
	/// <returns>error title</returns>
	public virtual string ResultMessage()
	{
		TblErrors rsErr = db.TblErrors.SingleOrDefault( target => target.ErrID == (int)ResultCode );
		return ( rsErr == null ) ? "An undefined error has occurred." : rsErr.ErrMessage;
	}

	/// <summary>
	/// Get the name of the assigned product for purchase
	/// </summary>
	/// <returns>Product name as string or "n/a" if no product assigned</returns>
	public string ProductName()
	{
		string productName = "n/a";
		TblProducts rsPrd = db.TblProducts.SingleOrDefault( target => target.PrdID == PrdID );
		if ( rsPrd != null )
		{
			productName = rsPrd.PrdName;
		}
		return productName;
	}

	/// <summary>
	/// Get the price of the assigned product 
	/// </summary>
	/// <returns>product price as double or 0 if no product assigned</returns>
	public decimal ProductPrice()
	{
		decimal productPrice = 0;
		TblProducts rsPrd = db.TblProducts.SingleOrDefault( target => target.PrdID == PrdID );
		if ( rsPrd != null )
		{
			productPrice = rsPrd.PrdPrice;
		}

		return productPrice;
	}

	/// <summary>
	/// Creates an Reccuring Payment Subscription using AuthNet
	/// </summary>
	protected bool CreateAuthNetSubscription()
	{
		PaymentType = PaymentTypes.AuthNet;
		// Converts the product price to a decimal number.
		decimal prodPrice = WebConvert.ToDecimal( ProductPrice(), 0 );

        AuthNetARB authnet = new AuthNetARB();

        // load the general properties
        authnet.SubscriptionEMail = AccEMail;
        authnet.SubscriptionName = ProductName();
        authnet.SubscriptionPrice = ProductPrice();

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
        authnet.CardNumber = SubBillCardNumber;
        authnet.CardCode = WebConvert.ToString(SubBillCVV, "");
        authnet.CardExpirationMonth = SubBillExpMonth;
        authnet.CardExpirationYear = SubBillExpYear;

        // set the subscription saart date
        authnet.StartDate = FirstBillingDate;

        // attempt to update the subscription
        if ( authnet.CreateSubscription() )
        {
            SubPaymentMethodID = authnet.SubscriptionID;
        }
        else
        {
            ResultCode = authnet.ResultCode;
        }

        return (ResultCode == RC.Ok);
	}

	/// <summary>
	/// Send a notification to the new member
	/// </summary>
	protected void NotifyNewMember()
	{
 		WebDBContext db = new WebDBContext();

        // get the message from the database
        string msgBody = SiteSettings.GetValue("EMail_JoinUs_Body");

        // substitute our properties into the message
        var type = this.GetType();
        var props = type.GetProperties();
        foreach (var p in props)
        {
            msgBody = msgBody.Replace("{" + p.Name + "}", p.GetValue(this,null).ToString());
        }

        // substitute site settings into the message
        IEnumerable<TblSettings> rsSet = db.TblSettings.Where(target=>target.SetID > 0);
        foreach (var row in rsSet)
        {
            msgBody = msgBody.Replace("{" + row.SetName + "}", row.SetValue);
        }

		// send it to the new member
        EMail.SendStandard(SiteSettings.GetValue("EMail_JoinUs_Subject"), msgBody, "", AccEMail, SiteSettings.GetValue("EMailReplyTo"), false);
    }

	/// <summary>
	/// Send a notification of a new member to the administrator
	/// </summary>
	protected void NotifyAdministrator()
	{
		StringBuilder msg = new StringBuilder();

		msg.AppendLine( "New UTC Member Registration at " + DateTime.Now + "." );
		msg.AppendLine( new string( '=', 60 ) );
		msg.AppendLine( "Account ID: " + AccID.ToString() );
		msg.AppendLine( "Account Name: " + AccFName + " " + AccLName );
		msg.AppendLine( "Account EMail: " + AccEMail );
		msg.AppendLine( "Account Phone: " + AccPhone );
		msg.AppendLine( new string( '=', 60 ) );
        msg.AppendLine("Account Gender: " + AccGender);
        msg.AppendLine("Account Date of Birth: " + AccBirthdate.ToShortDateString());
        msg.AppendLine("Account Zip: " + AccZip);
		msg.AppendLine( new string( '=', 60 ) );
		msg.AppendLine( "Promo Code: " + PromoCode );
		msg.AppendLine( "Referral ID: " + RfcID.ToString() );

		// send it to the administrator
		EMail.SendStandard( "UTC New Member Registration", msg.ToString(), "", SiteSettings.GetValue( "EMailNotify" ), SiteSettings.GetValue( "EMailReplyTo" ), false );
	}

    /// <summary>
    /// Find the applicable referral for a promo code
    /// </summary>
    /// <param name="code">check against referral codes to get identifier</param>
    /// <returns>referral identifer or 0 for none</returns>
    public int IdentifyReferral(string code)
    {
        WebDBContext db = new WebDBContext();
        // find a referral code that matches
        TblReferralCodes rfc = db.TblReferralCodes.SingleOrDefault(target => target.RfcCode == code);
        return (rfc != null) ? rfc.RfcID : 0;
    }

    /// <summary>
    /// Find the applicable promotion for a promo or referral code
    /// </summary>
    /// <param name="code">check against referral codes to get promo identifier</param>
    /// <returns>promotion identifer or 0 for no promotion</returns>
    public int IdentifyPromotion(string code)
    {
        int proid = 0;
        WebDBContext db = new WebDBContext();
        // find a referral code that matches
        TblReferralCodes rfc = db.TblReferralCodes.SingleOrDefault(target => target.RfcCode == code);
        if (rfc != null)
        {
            proid = rfc.ProID;
        }
        // get the record for the promotion
        TblPromotions pro = db.TblPromotions.SingleOrDefault(target => target.ProID == proid);
        if (pro != null)
        {
            // make sure that the promotion is still enabled
            if (pro.ProEnabled)
            {
                // return the applicable promotion 
                return proid;
            }
        }
        // fell through - no applicable promotion
        return 0;
    }

}