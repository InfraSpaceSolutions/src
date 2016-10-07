/******************************************************************************
 * Filename: AuthNetARB.cs
 * Project:  unitethiscity.com
 * 
 * Description:
 * Authorize.net interface class.  Uses the Authorize.NET ARB API to provide
 * Create, Update, Cancel and GetStatus functions for subscriptions paid
 * through authorize.net
******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sancsoft.Web;
using System.Collections.Specialized;

/// <summary>
/// Summary description for AuthNetARB
/// </summary>
public class AuthNetARB
{
    /// <summary>
    /// Connection to Authorize.net; lazy creation - it is null until needed
    /// </summary>
    protected AuthorizeNet.SubscriptionGateway subscriptionGateway;

    /// <summary>
    /// Billing address variable used by create and update calls
    /// </summary>
    public AuthorizeNet.Address billAddr;

    /// <summary>
    /// Authorize.net account login
    /// </summary>
    public string login;

    /// <summary>
    /// Authorize.net transcation api key
    /// </summary>
    public string transactionKey;

    /// <summary>
    /// Mode of operation - test or live
    /// </summary>
    public AuthorizeNet.ServiceMode mode;

    /// <summary>
    /// Raw error message from authorize.net for evaluation
    /// </summary>
    public string ErrorMessage { get; set; }

    /// <summary>
    /// Summary result code for operations for web message integration
    /// </summary>
    public RC ResultCode { get; set; }

    /// <summary>
    /// Identifies a subscription to Authorize.NET; set to effect a target
    /// subscription, or get when a new subscription is created
    /// </summary>
    public string SubscriptionID { get; set; }

    /// <summary>
    /// Email address associated with the subscription
    /// </summary>
    public string SubscriptionEMail { get; set; }

    /// <summary>
    /// The name of the subscription
    /// </summary>
    public string SubscriptionName { get; set; }

    /// <summary>
    /// The cost of the subscription
    /// </summary>
    public decimal SubscriptionPrice { get; set; }

    /// <summary>
    /// Credit card number
    /// </summary>
    public string CardNumber { get; set; }

    /// <summary>
    /// Credit card CVV2 code
    /// </summary>
    public string CardCode { get; set; }

    /// <summary>
    /// Credit card expiration month
    /// </summary>
    public int CardExpirationMonth { get; set; }

    /// <summary>
    /// Credit card expiration year
    /// </summary>
    public int CardExpirationYear { get; set; }

    /// <summary>
    /// Start date for the subscription
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Create an authorize.net recurring billing gateway for use
    /// </summary>
    /// <param name="testMode">true - operate in test mode, which is useless</param>
    public AuthNetARB(bool testMode = false)
	{
        // load the gateway configuration from the database settings
        login = SiteSettings.GetValue("AuthNetLogin");
        transactionKey = SiteSettings.GetValue("AuthNetTransactionKey");
        // support selection of test mode, even though it doesn't really do us any good
        mode = (testMode) ? AuthorizeNet.ServiceMode.Test : AuthorizeNet.ServiceMode.Live;

        // wait to create the gateway until we need it
        subscriptionGateway = null;

        // create an empty address to hold properties set by caller
        billAddr = new AuthorizeNet.Address();

        // clear out the error messaging
        ClearError();

        // default other properties and variables
        SubscriptionID = "";
	}

    /// <summary>
    /// Cancel the specified subscription
    /// </summary>
    /// <param name="subscriptionID">identify subscription</param>
    /// <returns>true - cancel successful</returns>
    public bool CancelSubscription(string subscriptionID)
    {
        ClearError();
        AcquireSubscriptionGateway();

        SubscriptionID = subscriptionID;
        try
        {
            if (!subscriptionGateway.CancelSubscription(subscriptionID))
            {
                ErrorMessage = "Cancel failed, subscription identifier not found.";
                ResultCode = RC.TargetDNE;
            }
        }
        catch (Exception e)
        {
            ErrorMessage = e.Message;
            ResultCode = GetAuthExceptionCode(e.Message);
        }
        return (ResultCode == RC.Ok);
    }

    /// <summary>
    /// Create a new subscription on Authorize.net.  The billing address and card properites
    /// on the object must be updated with the information to be sent with the request.
    /// </summary>
    /// <returns>true - update successful</returns>
    public bool CreateSubscription()
    {
        ClearError();
        AcquireSubscriptionGateway();
        AuthorizeNet.SubscriptionRequest subReq;

        // create and configure the subscription request object

        // all of our subscriptions are currently monthly
        subReq = AuthorizeNet.SubscriptionRequest.CreateMonthly(SubscriptionEMail,
            SubscriptionName, SubscriptionPrice);
        // assign the billing address
        subReq.BillingAddress = billAddr;
        // assign the credit card information
        subReq.CardNumber = CardNumber;
        subReq.CardCode = CardCode;
        subReq.CardExpirationMonth = CardExpirationMonth;
        subReq.CardExpirationYear = CardExpirationYear;
        // assign the starting date for billing
        subReq.StartsOn = StartDate;
        // this is a create, subscription id is blank until we get one
        SubscriptionID = "";

        try
        {
            AuthorizeNet.ISubscriptionRequest response = subscriptionGateway.CreateSubscription(subReq);
            // confirm that we received a valid subscription identifier
            if (WebConvert.ToLong(response.SubscriptionID, 0) == 0)
            {
                ResultCode = RC.InternalError;
                ErrorMessage = "Create failed, subscription identifier not returned.";
            }
            else
            {
                // get the id for our new subscription
                SubscriptionID = response.SubscriptionID;
            }
        }
        catch (Exception e)
        {
            ErrorMessage = e.Message;
            ResultCode = GetAuthExceptionCode(e.Message);
        }
        return (ResultCode == RC.Ok);
    }

    /// <summary>
    /// Gets the string status of a subscription on Authorize.net ARB.
    /// </summary>
    /// <param name="subscriptionID">identify subscription</param>
    /// <returns>string status code from Auth.net or "Invalid" if cant retrieve</returns>
    public string GetSubscriptionStatus(string subscriptionID)
    {
        string status = "Invalid";
        AuthorizeNet.APICore.ARBSubscriptionStatusEnum statusCode;

        ClearError();
        AcquireSubscriptionGateway();

        SubscriptionID = subscriptionID;

        try
        {
            statusCode = subscriptionGateway.GetSubscriptionStatus(subscriptionID);
            status = statusCode.ToString();
        }
        catch (Exception e)
        {
            ErrorMessage = e.Message;
            ResultCode = GetAuthExceptionCode(e.Message);
        }
        return status;
    }

    /// <summary>
    /// Update the billing information associated with a subscription.  The billing address
    /// and card properties on the object must be updated with the information to be sent with the update.
    /// </summary>
    /// <returns>true - update successful</returns>
    public bool UpdateSubscription(string subscriptionID)
    {
        ClearError();
        AcquireSubscriptionGateway();
        AuthorizeNet.SubscriptionRequest subReq;

        // create and configure the subscription request object

        // all of our subscriptions are currently monthly
        subReq = AuthorizeNet.SubscriptionRequest.CreateMonthly(SubscriptionEMail, 
            SubscriptionName, SubscriptionPrice);
        // assign the billing address
        subReq.BillingAddress = billAddr;
        // assign the credit card information
        subReq.CardNumber = CardNumber;
        subReq.CardCode = CardCode;
        subReq.CardExpirationMonth = CardExpirationMonth;
        subReq.CardExpirationYear = CardExpirationYear;
        // assign the starting date for billing
        subReq.StartsOn = StartDate;
        // use the subscription id passed in
        SubscriptionID = subscriptionID;
        subReq.SubscriptionID = subscriptionID;

        try
        {
            if (!subscriptionGateway.UpdateSubscription(subReq))
            {
                ErrorMessage = "Update failed, subscription identifier not found.";
                ResultCode = RC.TargetDNE;
            }
        }
        catch (Exception e)
        {
            ErrorMessage = e.Message;
            ResultCode = GetAuthExceptionCode(e.Message);
        }
        return (ResultCode == RC.Ok);
    }

    /// <summary>
    /// Lazy creation of the subscription gateway object
    /// </summary>
    /// <returns>new or existing subscription gateway</returns>
    protected void AcquireSubscriptionGateway()
    {
        if (subscriptionGateway == null)
        {
            subscriptionGateway = new AuthorizeNet.SubscriptionGateway(login, transactionKey, mode);
        }
    }

    /// <summary>
    /// Parse an Authorize.net error code into a result code
    /// </summary>
    /// <param name="errorMsg">Authoriz.net error message string</param>
    /// <returns>sancsoft return code</returns>
    private RC GetAuthExceptionCode(string errorMsg)
    {
        RC rc = RC.InternalError;

        // All Auth Net Error Codes start with E000 and start at E00001 and go to E00045. 
        // Skips E00023 and E00039 - E00044
        int index = errorMsg.IndexOf("E000");
        if (index >= 0)
        {
            // Get the Error Code substring from the message
            string errorCodeString = errorMsg.Substring(index, 6).Trim();
            //Convert the errorcode string to an integer string and then convert to an integer
            errorCodeString = errorCodeString.Replace("E0", "");
            int errorCode = WebConvert.ToInt32(errorCodeString, (int)RC.InternalError);
            // All Auth Net error Codes in UTC start at 201 and go to 245 skipping 223 and 239 - 242
            errorCode += 200;
            rc = (RC)errorCode;
        }
        return rc;
    }

    /// <summary>
    /// Clear any current error message on the object
    /// </summary>
    protected void ClearError()
    {
        ResultCode = RC.Ok;
        ErrorMessage = "";
    }

    /// <summary>
    /// Assign the billing address information from a name value collection collected through
    /// an online form
    /// </summary>
    /// <param name="flds">collection of form fields</param>
    public void AssignFromForm(NameValueCollection flds)
    {
        // assign the fields from the standard form fields
        billAddr.First = WebConvert.Truncate(WebConvert.ToString(flds["txtSubBillFName"],""),50);
        billAddr.Last = WebConvert.Truncate(WebConvert.ToString(flds["txtSubBillLName"], ""), 50);
        billAddr.Street = WebConvert.Truncate(WebConvert.ToString(flds["txtSubBillAddress"], ""), 128);
        billAddr.City = WebConvert.Truncate(WebConvert.ToString(flds["txtSubBillCity"], ""), 50);
        billAddr.State = WebConvert.Truncate(WebConvert.ToString(flds["txtSubBillState"], ""), 2);
        billAddr.Zip = WebConvert.Truncate(WebConvert.ToString(flds["txtSubBillZip"], ""), 20);
        billAddr.Country = "United States";
        billAddr.Phone = "";

        // assign the credit card fields
        CardNumber = WebConvert.Truncate(WebConvert.ToString(flds["txtSubBillCardNumber"], ""), 20);
        CardCode = WebConvert.Truncate(WebConvert.ToString(flds["txtSubBillCVV"], ""), 4);
        CardExpirationMonth = WebConvert.ToInt32(flds["selSubBillExpMonth"], 0);
        CardExpirationYear = WebConvert.ToInt32(flds["selSubBillExpYear"], 0);
    }

    /// <summary>
    /// Assign the billing address information from a subscription database record
    /// </summary>
    /// <param name="rsSub">subscription databse record</param>
    public void AssignFromDatabase(VwSubscriptions rsSub)
    {
        // assign the billing address information
        billAddr.First = rsSub.SubBillFName;
        billAddr.Last = rsSub.SubBillLName;
        billAddr.Street = rsSub.SubBillAddress;
        billAddr.City = rsSub.SubBillCity;
        billAddr.State = rsSub.SubBillState;
        billAddr.Zip = rsSub.SubBillZip;
        // we always operate in the united states
        billAddr.Country = "United States";
        // we don't handle the phone number
        billAddr.Phone = "";

        // assign the expiration date from the record
        CardExpirationMonth = rsSub.SubBillExpMonth;
        CardExpirationYear = rsSub.SubBillExpYear;
    }
}