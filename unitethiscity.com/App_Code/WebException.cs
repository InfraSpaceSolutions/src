/******************************************************************************
 * Filename: WebException.cs
 * Project:  UTC
 * 
 * Description:
 * Support generation of standard errors and web-site specific exceptions for
 * error processing
 * 
 * Relocated to namespace Sancsoft.Web for compatibility with Razor
 * 
 * Revision History:
 * $Log: /MonstersUnlimited/utc/unitethiscity.com/website/App_Code/WebException.cs $
 * 
 * 6     1/18/13 10:14a Namendola
 * Fixed spelling mistakes in the RC errors enum
 * 
 * 5     1/17/13 6:02p Namendola
 * Added exceptions that are focused around Auth Net Error Codes
 * 
 * 4     1/17/13 1:29p Ncross
 * 
 * 3     1/14/13 8:58a Mterry
 * integrated additional checks
 * 
 * 1     11/28/12 6:05p Ncross
******************************************************************************/
using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

namespace Sancsoft.Web
{
    /// <summary>
    /// Return codes for database driven web applicstions
    /// </summary>
	public enum RC
	{
		Ok,
		InternalError,
		TargetDNE,
		DataIncomplete,
		Duplicate,
		Dependencies,
		BadPassword,
		AccountDNE,
		AccountDisabled,
		DataInvalid,
		PasswordMismatch,
		UnderConstruction,
		AccessDenied,
		UploadError,
		EmailInvalid,
		DuplicateEmail,
        InvalidBirthdate,
		CCRBase = 100,
		CCRInvalidNumber,
		CCCRInvalidNumberForType,
		CCRInvalidLUHN,
		CCRInvalidExpiration,
		CCRInvalidExpired,
		CCRFault,
		ErrorDuringProwcessing = 201,
		TypeUnsupported,
		ParsingError,
		MethodNameInvalid,
		TransactionKeyInvalid,
		MerchantAuthNameInvalid,
		AuthenticationValuesInvalid,
		InactiveGateway,
		TestModeOn,
		UserAccessDenied,
		MethodCallDenied,
		DuplicateExists,
		InvalidFieldSubmitted,
		MissingRequiredField,
		InvalidFieldLength,
		InvalidFieldType,
		InvalidStartDate,
		CardExpirationDate,
		CustomerInformationRequired,
		eCheckNetSubscriptionsDisabled,
		CardSubscriptionsDisabled,
		IntervalOverflow,
		TrialOccurrenceRequired = 224,
		ARBDisabled,
		TrialInformationRequired,
		UnsuccessfulTest,
		TrialOccurrencesInvalid,
		PaymentInformationRequired,
		PaymentScheduleObjectRequired,
		AmountRequired,
		StartDateRequired,
		StartDateFixed,
		IntervalFixed,
		InvalidSubscriptionID,
		UnsupportedPaymentTypeSwitch,
		DisabledSubscriptionUpdating,
		DisabledSubscriptionCancellation,
		XMLNamespaceError = 245,
	}

    /// <summary>
    /// Web page exception for managing standard errors the occur on database-driven web applications
    /// </summary>
    public class WebException : Exception
    {
        protected RC returnCode;
        public RC ReturnCode
        {
            get
            {
                return this.returnCode;
            }
        }

        /// <summary>
        /// Default construction - throw an internal error
        /// </summary>
        public WebException( )
            : base( )
        {
            returnCode = RC.InternalError;
        }

        /// <summary>
        /// Make an exception based on one of our return codes
        /// </summary>
        /// <param name="rc">error code</param>
        public WebException( RC rc )
            : base( )
        {
            returnCode = rc;
        }

        /// <summary>
        /// Get an error message from the database.  Uses Unknown Error if
        /// the requested message is not in the database
        /// </summary>
        /// <param name="title">title for the error box</param>
        /// <param name="message">error message detail</param>
        public void GetError( out string title, out string message )
        {
            WebDBContext db = new WebDBContext( );

            TblErrors rs = db.TblErrors.SingleOrDefault( row => row.ErrID == (int)ReturnCode );
            if( rs == null )
            {
                title = "Error: Unknown Error";
                message = "An unknown error has occurred.";
                return;
            }

            title = "Error: " + rs.ErrTitle;
            message = rs.ErrMessage;
        }
    }
}