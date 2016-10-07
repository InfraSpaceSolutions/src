/******************************************************************************
 * Filename: FreeSignUpController.cs
 * Project:  UTC WebAPI
 * 
 * Description:
 * Controller for support of free account creation and management
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
    public class FreeSignUpController : ApiController
    {
        WebDBContext db;
        protected const int DefaultCityID = 1;
        protected const int ProductID = 2;
        protected const int AccountTypeID = 1;

        /// <summary>
        /// Attempt to create a new account with a free subscription
        /// </summary>
        /// <param name="jn">sign up form contents</param>
        /// <returns>login credentials for the new accountr</returns>
        public LoginCredentials Post(FreeSignUp jn)
        {
            db = new WebDBContext();

            // check all of the values provided, throwing exceptions when issues are found
            jn.AccEMail = CleanRequiredField("Email Address", jn.AccEMail, 128);
            jn.AccFName = CleanRequiredField("First Name", jn.AccFName, 50);
            jn.AccLName = CleanRequiredField("Last Name", jn.AccLName, 50);
            jn.AccPhone = WebConvert.Truncate(jn.AccPhone, 50);

            jn.AccBirthdate = CleanRequiredField("Birth Date", jn.AccBirthdate, 50);
            // force a valid/supported gender string
            jn.AccGender = CleanRequiredField("Gender", jn.AccGender, 20);
            jn.AccGender = jn.AccGender.ToUpper();
            jn.AccGender = (jn.AccGender.Length >= 1) ? jn.AccGender.Substring(0, 1) : "?";

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

            DateTime birthDate = WebConvert.ToDateTime(jn.AccBirthdate, DateTime.Today);
            // check that the birthdate is reasonable
            int age = DateTime.Today.Year - birthDate.Year;
            if (birthDate > DateTime.Today.AddYears(-age))
            {
                age--;
            }
            // users must be 18 or over and we only expect less than 100
            if ((age < 18) || (age > 100))
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Members must be 18 or older. Please enter a valid date of birth."));
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

            // create the new account
            TblAccounts rsAcc = new TblAccounts();
            rsAcc.AccEMail = jn.AccEMail;
            rsAcc.AccFName = jn.AccFName;
            rsAcc.AccLName = jn.AccLName;
            rsAcc.AccPhone = jn.AccPhone;
            rsAcc.AccPassword = Password.GenerateRandom(WebConvert.ToInt32(SiteSettings.GetValue("PasswordMinLength"), 8));
            rsAcc.AccGuid = Guid.NewGuid();
            rsAcc.AccEnabled = true;
            rsAcc.AtyID = (int)AccountTypes.Monthly;
            rsAcc.RfcID = IdentifyReferral(jn.PromoCode);
            rsAcc.CitID = DefaultCityID;
            rsAcc.AccGender = jn.AccGender;
            rsAcc.AccBirthdate = birthDate;
            rsAcc.AccZip = "";

            db.TblAccounts.InsertOnSubmit(rsAcc);
            db.SubmitChanges();

            // create the member role for the account
            TblAccountRoles rsAcr = new TblAccountRoles();
            rsAcr.AccID = rsAcc.AccID;
            rsAcr.RolID = (int)Roles.Member;
            rsAcr.BusID = 0;
            db.TblAccountRoles.InsertOnSubmit(rsAcr);
            db.SubmitChanges();

            // send the email notifications
            NotifyMember(rsAcc);
            NotifyAdministrator(jn);

            // create and return the credentials for the new account
            LoginCredentials lc = new LoginCredentials();
            lc.Account = rsAcc.AccEMail;
            lc.Password = rsAcc.AccPassword;

            Logger.LogAction("SignUp", rsAcc.AccID, 0);

            return lc;
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
            return (ret.Length > maxlen) ? ret.Substring(0, maxlen) : ret;
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
        protected void NotifyAdministrator(FreeSignUp jn)
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
                msg.AppendLine("Account Gender: " + jn.AccGender);
                msg.AppendLine("Account Date of Birth: " + jn.AccBirthdate);
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
    }
}
