/******************************************************************************
 * Filename: SubscriptionController.cs
 * Project:  UTC WebAPI
 * 
 * Description:
 * Controller for support of subscription creation and management
******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using com.unitethiscity.api.Models;

namespace com.unitethiscity.api.Controllers
{
    public class SubscriptionController : ApiController
    {
        WebDBContext db;
        protected const int DefaultCityID = 1;

        /// <summary>
        /// Attempt to create a new account and subscription through Authorize.net
        /// </summary>
        /// <param name="jn">sign up form contents</param>
        /// <returns>login credentials for the new accountr</returns>
        public LoginCredentials Post(JoinNow jn)
        {
            db = new WebDBContext();

            // check all of the values provided, throwing exceptions when issues are found
            jn.AccEMail = CleanRequiredField("Email Address", jn.AccEMail, 128);
            jn.AccFName = CleanRequiredField("First Name", jn.AccFName, 50);
            jn.AccLName = CleanRequiredField("Last Name", jn.AccLName, 50);
            jn.AccPhone = CleanRequiredField("Phone Number", jn.AccPhone, 50);

            jn.BillFName = CleanRequiredField("Billing First Name", jn.BillFName, 50);
            jn.BillLName = CleanRequiredField("Billing Last Name", jn.BillLName, 50);
            jn.BillAddress = CleanRequiredField("Billing Street", jn.BillAddress, 128);
            jn.BillCity = CleanRequiredField("Billling City", jn.BillCity, 50);
            jn.BillState = CleanRequiredField("Billing State", jn.BillState, 2);
            jn.BillZip = CleanRequiredField("Billing Zip", jn.BillZip, 20);

            jn.CardNumber = CleanRequiredField("Card Number", jn.CardNumber, 20);

            // promo codes are all evaluated in lowercase only
            jn.PromoCode = jn.PromoCode.ToLower();

            // clean the email address to standard format
            jn.AccEMail = jn.AccEMail.Trim().ToLower();
            // make sure that the email address is valid
            Regex emailRegex = new Regex(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
            if (!emailRegex.IsMatch(jn.AccEMail))
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Email address invalid."));
            }

            // make sure the email address isn't already in use
            if (db.TblAccounts.Count(target => target.AccEMail == jn.AccEMail) > 0)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Email address already in use."));
            }

            // check the credit card information using our utility class
    		CreditCardUtility ccu = new CreditCardUtility( jn.CardNumber, jn.CardExpMonth, jn.CardExpYear);
	        int crint = (int)ccu.Check( true );
            if (crint > 0)
            {
                TblErrors err = db.TblErrors.SingleOrDefault(target => target.ErrID == (crint + 100));
                if (err != null)
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, err.ErrMessage));
                }
                else
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Credit card invalid (" + crint.ToString() + ")"));
                }
            }

            // identify the associated referral credit from the supplied promo code
            int rfcid = IdentifyReferral(jn.PromoCode);
            // identify the associated promotion from the supplied promo code
            int proid = IdentifyPromotion(jn.PromoCode);

            DateTime firstBillingDate = DateTime.Today.AddDays(1);
            // apply the promotion to the request
            switch (proid)
            {
                case 1:
                    firstBillingDate = DateTime.Today.AddMonths(1);
                    break;
            }

            // get the target product - or blow up trying
            TblProducts rsPrd =  db.TblProducts.Single(target=>target.PrdID == jn.PrdID);

            // create the connection to authorize.net and make a new subscription
            AuthNetARB authNet = new AuthNetARB();
            authNet.SubscriptionEMail = jn.AccEMail;
            authNet.SubscriptionName = rsPrd.PrdName;
            authNet.SubscriptionPrice = rsPrd.PrdPrice;
            authNet.billAddr.First = jn.BillFName;
            authNet.billAddr.Last = jn.BillLName;
            authNet.billAddr.Street = jn.BillAddress;
            authNet.billAddr.City = jn.BillCity;
            authNet.billAddr.State = jn.BillState;
            authNet.billAddr.Zip = jn.BillZip;
            authNet.billAddr.Country = "United States";
            authNet.CardNumber = jn.CardNumber;
            authNet.CardCode = WebConvert.ToString(jn.CardCode, "");
            authNet.CardExpirationMonth = jn.CardExpMonth;
            authNet.CardExpirationYear = jn.CardExpYear;
            authNet.StartDate = firstBillingDate;
            if (!authNet.CreateSubscription())
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, authNet.ErrorMessage));
            }


            // create the new account
            TblAccounts rsAcc = new TblAccounts();
            rsAcc.AccEMail = jn.AccEMail;
            rsAcc.AccFName = jn.AccFName;
            rsAcc.AccLName = jn.AccLName;
            rsAcc.AccPhone = jn.AccPhone;
            rsAcc.AccPassword = Password.GenerateRandom(WebConvert.ToInt32(SiteSettings.GetValue("PasswordMinLength"),8));
            rsAcc.AccGuid = Guid.NewGuid();
            rsAcc.AccEnabled = true;
            rsAcc.AtyID = (int)AccountTypes.Monthly;
            rsAcc.RfcID = IdentifyReferral(jn.PromoCode);
            rsAcc.CitID = DefaultCityID;

            db.TblAccounts.InsertOnSubmit(rsAcc);
            db.SubmitChanges();
            
            // create the member role for the account
            TblAccountRoles rsAcr = new TblAccountRoles();
            rsAcr.AccID = rsAcc.AccID;
            rsAcr.RolID = (int)Roles.Member;
            rsAcr.BusID = 0;
            db.TblAccountRoles.InsertOnSubmit(rsAcr);
            db.SubmitChanges();

            // create the subscription record
            TblSubscriptions rsSub = new TblSubscriptions();
            rsSub.AccID = rsAcc.AccID;
            rsSub.PrdID = jn.PrdID;
            rsSub.SubBillFName = jn.BillFName;
            rsSub.SubBillLName = jn.BillLName;
            rsSub.SubBillAddress = jn.BillAddress;
            rsSub.SubBillCity = jn.BillCity;
            rsSub.SubBillState = jn.BillState;
            rsSub.SubBillZip = jn.BillZip;
            rsSub.SubBillCtrID = 186;       // only supports the United States
            rsSub.SubBillCardNumber = ccu.ExpungedCardNumber();
            rsSub.SubBillExpMonth = jn.CardExpMonth;
            rsSub.SubBillExpYear = jn.CardExpYear;
            rsSub.SubTSCreate = DateTime.Now;
            rsSub.SubTSModify = DateTime.Now;
            rsSub.SubIPAddress = "";
            rsSub.PtyID = (int)PaymentTypes.AuthNet;
            rsSub.ProID = IdentifyPromotion(jn.PromoCode);
            rsSub.SubPaymentMethodID = authNet.SubscriptionID;
            rsSub.SubBillDate = firstBillingDate;
            db.TblSubscriptions.InsertOnSubmit(rsSub);
            db.SubmitChanges();

            // send the email notifications
            NotifyMember(rsAcc);
            NotifyAdministrator(jn);

            // create and return the credentials for the new account
            LoginCredentials lc = new LoginCredentials();
            lc.Account = rsAcc.AccEMail;
            lc.Password = rsAcc.AccPassword;
            return lc;
        }

        /// <summary>
        /// Cancel a subscription by sending a delete with matching login credentials
        /// </summary>
        /// <param name="token">identify account</param>
        /// <param name="lc">credentials to confirm delete</param>
        public void Delete(Guid token, LoginCredentials lc)
        {
            db = new WebDBContext();

            int accID = APIToken.IdentifyAccount(db, token);
            if (accID == 0)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Access invalid.  Please login again."));
            }

            // look up the account we are trying to login as by email address
            TblAccounts rs = db.TblAccounts.SingleOrDefault(target => target.AccEMail == lc.Account);
            if (rs == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Account not found"));
            }

            // confirm that the account is enabled
            if (!rs.AccEnabled)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Account disabled"));
            }

            // check the supplied password - check both encrypted and unencrypted varieties
            // this is to support testing as well as encrypted passwords in the database
            String hashword = Encryption.CalculatePasswordHash(rs.AccPassword);
            if ((hashword != lc.Password) && (rs.AccPassword != lc.Password))
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Bad password"));
            }
            TblSubscriptions rsSub = db.TblSubscriptions.SingleOrDefault(target => target.AccID == accID);
            if (rsSub == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Subscription not found."));
            }
            // we can only cancel Authorize.NET subscriptions through the service currently
            if (rsSub.PtyID != (int)PaymentTypes.AuthNet)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Online cancel not avaialble. Please contact UTC."));
            }

            // cancel the subscription via Authorize.net interface
            AuthNetARB authnet = new AuthNetARB();
            if (!authnet.CancelSubscription(rsSub.SubPaymentMethodID))
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, authnet.ErrorMessage));
            }

            NotifyCancellation(lc, rsSub);

            // update the subscription record
            rsSub.ProID = 0;
            rsSub.PtyID = 0;
            rsSub.SubPaymentMethodID = "";
            rsSub.SubBillCardNumber = "";
            rsSub.SubBillExpMonth = 0;
            rsSub.SubBillExpYear = 0;
            db.SubmitChanges();
        }

        /// <summary>
        /// Request the password be delivered to the email address for an account
        /// </summary>
        /// <param name="emailAddress">email address of account for request password</param>
        public void GetPassword(string emailAddress)
        {
            db = new WebDBContext();

            emailAddress = emailAddress.ToLower().Trim();
            TblAccounts rsAcc = db.TblAccounts.SingleOrDefault(target => target.AccEMail == emailAddress);
            if (rsAcc == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Account not found"));
            }

            StringBuilder msg = new StringBuilder();

            msg.AppendLine("Account Password Retrieval sent at " + DateTime.Now + ".");
            msg.AppendLine(new string('=', 60));
            msg.AppendLine("Email Address: " + rsAcc.AccEMail);
            msg.AppendLine("Password: " + rsAcc.AccPassword);
            msg.AppendLine("");
            msg.AppendLine("Please use the link below to login with your account:");
            msg.AppendLine(SiteSettings.GetValue("RootURL") + "/AccountLogin");
            msg.AppendLine(new string('=', 60));
            msg.AppendLine("");
            msg.AppendLine("");

            EMailer emailer = new EMailer();
            // send it to the account email address
            emailer.SendStandard("Forgot Password Rquest", msg.ToString(), "", emailAddress, SiteSettings.GetValue("EMailReplyTo"), false);
        }

        /// <summary>
        /// Cleans a field (trims it, truncates it).  Verifies that the field is not blank or throws exception.
        /// </summary>
        /// <param name="name">display name of the field</param>
        /// <param name="value">value to test</param>
        /// <param name="maxlen">truncate to this length</param>
        protected string CleanRequiredField(string name, string value, int maxlen)
        {
            string ret = value.Trim();

            if (ret.Length == 0)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Required field '" + name + "' is blank"));
            }
            return (ret.Length > maxlen) ? ret.Substring( 0, maxlen) : ret;
        }

        /// <summary>
        /// Identify the promotion that goes with the supplied promo code
        /// </summary>
        /// <param name="code">code for evaluation</param>
        /// <returns>Promotion # if found and enabled, 0 if not</returns>
        protected int IdentifyPromotion(string code)
        {
            int proid = 0;
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

        /// <summary>
        /// Identify the referral code that goes with the provided promotition code
        /// </summary>
        /// <param name="code">code for evaluation</param>
        /// <returns>Referral id or 0 for none</returns>
        protected int IdentifyReferral(string code)
        {
            // find a referral code that matches
            TblReferralCodes rfc = db.TblReferralCodes.SingleOrDefault(target => target.RfcCode == code);
            return (rfc != null) ? rfc.RfcID : 0;
        }

        /// <summary>
        /// Send a notification to the new member with the password information
        /// </summary>
        /// <param name="rs">Account record</param>
        protected void NotifyMember(TblAccounts rs)
        {
            try
            {
                // get the message from the database
                string msgBody = SiteSettings.GetValue("EMail_JoinUs_Body");

                // substitute our properties into the message from the JoinNow object
                msgBody = msgBody.Replace("{AccFName}", rs.AccFName);
                msgBody = msgBody.Replace("{AccEMail}", rs.AccEMail);
                msgBody = msgBody.Replace("{AccPassword}", rs.AccPassword);

                // substitute site settings into the message
                IEnumerable<TblSettings> rsSet = db.TblSettings.Where(target => target.SetID > 0);
                foreach (var row in rsSet)
                {
                    msgBody = msgBody.Replace("{" + row.SetName + "}", row.SetValue);
                }
                // send it to the new member
                EMailer email = new EMailer();
                email.SendStandard(SiteSettings.GetValue("EMail_JoinUs_Subject"), msgBody, "", rs.AccEMail, SiteSettings.GetValue("EMailReplyTo"), false);
            }
            catch (Exception)
            {
                // dont allow this to fail, its just a notification via email
            }
        }


        /// <summary>
        /// Send a notification to the administrator that a new account has been created
        /// </summary>
        /// <param name="jn">JoinNow properties</param>
        protected void NotifyAdministrator(JoinNow jn)
        {
            StringBuilder msg = new StringBuilder();

            try
            {
                msg.AppendLine("New UTC Member Registration at " + DateTime.Now + ".");
                msg.AppendLine(new string('=', 60));
                msg.AppendLine("Account Name: " + jn.AccFName + " " + jn.AccLName);
                msg.AppendLine("Account EMail: " + jn.AccEMail);
                msg.AppendLine("Account Phone: " + jn.AccPhone);
                msg.AppendLine(new string('=', 60));
                msg.AppendLine("Credit Card Info: " + CreditCardUtility.ExpungeCardNumber(jn.CardNumber) + "  " + jn.CardExpMonth.ToString() + "/" + jn.CardExpYear.ToString());
                msg.AppendLine("Billing Address: " + jn.BillFName + " " + jn.BillLName + "; " + jn.BillAddress + "; " + jn.BillCity + ", " + jn.BillState + " " + jn.BillZip);
                msg.AppendLine(new string('=', 60));
                msg.AppendLine("Promo Code: " + jn.PromoCode);
                msg.AppendLine("Source: Mobile App");
                EMailer email = new EMailer();
                email.SendStandard("UTC New Member Registration", msg.ToString(), "", SiteSettings.GetValue("EMailNotify"), SiteSettings.GetValue("EMailReplyTo"), false);
            }
            catch (Exception)
            {
                // dont allow this to fail, its just a notification via email
            }
        }

        /// <summary>
        /// Send an email notification for a cancellation of a subscription
        /// </summary>
        protected void NotifyCancellation(LoginCredentials lc, TblSubscriptions rsSub)
        {
            StringBuilder msg = new StringBuilder();

            msg.AppendLine("UTC Member Cancelled Account at " + DateTime.Now + ".");
            msg.AppendLine(new string('=', 60));
            msg.AppendLine("Account ID: " + rsSub.AccID.ToString());
            msg.AppendLine("Account EMail: " + lc.Account);
            msg.AppendLine(new string('=', 60));
            msg.AppendLine("Credit Card Info: " + rsSub.SubBillCardNumber + " (" + rsSub.SubBillCarID + ") " + rsSub.SubBillExpMonth.ToString() + "/" + rsSub.SubBillExpYear.ToString());
            msg.AppendLine("Billing Address: " + rsSub.SubBillFName + " " + rsSub.SubBillLName + "; " + rsSub.SubBillAddress + "; " + rsSub.SubBillCity + ", " + rsSub.SubBillState + " " + rsSub.SubBillZip);
            msg.AppendLine(new string('=', 60));
            msg.AppendLine("Payment Method ID Code: " + rsSub.SubPaymentMethodID);

            switch (rsSub.PtyID)
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
            EMailer email = new EMailer();
            email.SendStandard("UTC Member Cancelled From App", msg.ToString(), "", SiteSettings.GetValue("EMailNotify"), SiteSettings.GetValue("EMailReplyTo"), false);
        }

    }
}
