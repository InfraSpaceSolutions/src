/******************************************************************************
 * Filename: PayFlow.cs
 * Project:  unitethiscity.com
 * 
 * Description:
 * Object for managing communications with PayPal PayFlow e-commerce services
******************************************************************************/
using System;
using System.Collections.Specialized;
using System.Text;
using System.Web;
using Sancsoft.Web;
using PayPal.Payments.Common.Utility;
using PayPal.Payments.Communication;
using PayPal.Payments.DataObjects;

/// <summary>
/// Summary description for PayFlow
/// </summary>
public class PayFlow
{
    protected NameValueCollection ActionParm;
    protected NameValueCollection ActionResponse;
    protected string RawRequest;
    protected string RawResponse;

    /// <summary>
    /// Default constructor for PayFlow interface object
    /// </summary>
	public PayFlow()
	{
        ActionParm = new NameValueCollection();
	}

    /// <summary>
    /// Load the configured properties from the site settings
    /// </summary>
    protected void LoadActionFromSettings()
    {
        ActionParm.Add("PARTNER", SiteSettings.GetValue("PaypalPartner"));
        ActionParm.Add("VENDOR", SiteSettings.GetValue("PaypalVendor"));
        ActionParm.Add("USER", SiteSettings.GetValue("PaypalUser"));
        ActionParm.Add("PWD", SiteSettings.GetValue("PaypalPassword"));
    }

    /// <summary>
    /// Initialize the action with the supplied action code.  Creates a fresh
    /// action parameter collection initialized with the requested action and 
    /// the authentication from the settings
    /// </summary>
    /// <param name="actionCode">Request action: A,M,R,C,I,P</param>
    public void InitRecurringAction(string actionCode)
    {
        // create a fresh action parameters collection
        ActionParm = new NameValueCollection();

        // load the authentication parameters into the action
        LoadActionFromSettings();

        // must be "R" for recurring billing action types
        ActionParm.Add("TRXTYPE", "R");

        // add the requested action
        ActionParm.Add("ACTION", actionCode);
    }

    /// <summary>
    /// Add a parameter to the action parameter set
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void AddActionParameter(string key, string value)
    {
        ActionParm.Add(key.ToUpper(), value);
    }

    /// <summary>
    /// Get a value from the response parameter set
    /// </summary>
    /// <param name="key">field to get</param>
    /// <returns>value of field</returns>
    public string GetResponseParameter(string key)
    {
        return ActionResponse.Get(key);
    }

    /// <summary>
    /// Create a string request from a name value collection
    /// </summary>
    /// <param name="nvc">collection of parameters</param>
    /// <returns></returns>
    protected string RequestFromCollection(NameValueCollection nvc)
    {
        StringBuilder sb = new StringBuilder();

        // add each entry in the collection as key=value& 
        foreach (String key in nvc)
        {
            sb.Append(HttpUtility.UrlEncode(key));
            sb.Append("=");
            sb.Append(HttpUtility.UrlEncode(nvc[key]));
            sb.Append("&");
        }
        // remove the trailing & that we may have added
        if (sb.Length > 0)
        {
            sb.Remove(sb.Length - 1, 1);
        }
        return sb.ToString();
    }

    /// <summary>
    /// Convert the api response to a name value collection of parameters
    /// </summary>
    /// <param name="response">raw response string</param>
    /// <returns>name value collection of response values</returns>
    protected NameValueCollection CollectionFromResponse(string response)
    {
        NameValueCollection nvc = HttpUtility.ParseQueryString(response);
        return nvc;
    }

    /// <summary>
    /// Perform the configured action by submitting the action parameters to the api
    /// and capturing the response name value collection
    /// </summary>
    /// <returns>TRUE if result=0 - successful operation</returns>
    public bool PerformAction()
    {
        // create the payflow api
        PayflowNETAPI api = new PayflowNETAPI();
        // form the request from the name value collection
        RawRequest = RequestFromCollection(ActionParm);
        // submit the request and get the response
        RawResponse = api.SubmitTransaction(RawRequest, PayflowUtility.RequestId);
        // get the response into a name value collection
        ActionResponse = CollectionFromResponse(RawResponse);
        // check the result code to return a summary status
        int result = WebConvert.ToInt32(ActionResponse["RESULT"],-1);
        return (result == 0);
    }

    /// <summary>
    /// Convert a datetime to a date suitable for passing to the payflow api
    /// </summary>
    /// <param name="dt">datetime</param>
    /// <returns>string of date in "MMddyyyy" format</returns>
    public string DateToString(DateTime dt)
    {
        return String.Format("{0}{1}{2}", dt.Month, dt.Day, dt.Year);
    }
}