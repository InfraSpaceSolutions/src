/******************************************************************************
 * Filename: ForgotPasswordForm.cs
 * Project:  unitethiscity.com
 * 
 * Description:
 * Form processing class for the forgot password Form. 
 * Validates required fields
 * Stores submissions to the businesses table
 * Sends notification messages to configured recipient list
 * 
 * Revision History:
 * $Log: $
******************************************************************************/
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using Sancsoft.Web;
using System.Text;

/// <summary>
/// Customized form processing for the forgot password form
/// </summary>
public class ForgotPasswordForm : FormProcessor
{
    /// <summary>
    /// Create an instance of the form processor with the collection of
    /// form fields from the submitted form.
    /// </summary>
    /// <param name="flds">Request.Form from the submitted page</param>
    public ForgotPasswordForm(NameValueCollection flds)
        : base( flds )
    {
        NotifySubject = "Forgot Password Form Submission";
        FieldPrefix = "Acc";
    }

    /// <summary>
    /// Verify that the submitted fields are complete and valid
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public override bool Validate( )
    {
        if (!wv.RequiredFields("txtAccEmail"))
        {
            ResultCode = RC.DataIncomplete;
        }
        else
        {
            CheckAccount();
        }

        return (ResultCode == RC.Ok);
    }

    // Look up account and email
    // Get the target record
    public bool CheckAccount()
    {
        int accountCount;
        string email = WebConvert.Truncate(WebConvert.ToString(fields["txtAccEmail"], ""), 128);

        accountCount = db.TblAccounts.Count(target => target.AccEMail == email && target.AccEnabled == true);

        // Verify target record exists
        if (accountCount == 0)
        {
            ResultCode = RC.AccountDNE;
        }
        return (ResultCode == RC.Ok);
    }

    /// <summary>
    /// Generate the notification message to send to the account email address
    /// </summary>
    public override bool NotificationMessage( out string text, out string html )
    {
        StringBuilder msg = new StringBuilder();
        text = "";
        html = "";

        string email = WebConvert.Truncate(WebConvert.ToString(fields["txtAccEmail"], ""), 128);

        TblAccounts rs = db.TblAccounts.FirstOrDefault(row => row.AccEMail == email);
        if (rs == null)
        {
            ResultCode = RC.AccountDNE;
        }
        else
        {
            msg.AppendLine("Account Password Retrieval sent at " + DateTime.Now + ".");
            msg.AppendLine(new string('=', 60));
            msg.AppendLine("Email Address: " + rs.AccEMail);
            msg.AppendLine("Password: " + rs.AccPassword);
            msg.AppendLine("");
            msg.AppendLine("Please use the link below to login with your account:");
            msg.AppendLine(SiteSettings.GetValue("RootURL") + "/AccountLogin");
            msg.AppendLine(new string('=', 60));
            msg.AppendLine("");
            msg.AppendLine("");
        }
        text = msg.ToString();

        return false;
    }

    public override bool SendNotification( )
    {
        string text, html;
        bool ishtml;

        // generate the email message content
        ishtml = NotificationMessage(out text, out html);

        // get users email address to send account credentials email with login link.
        string email = WebConvert.Truncate(WebConvert.ToString(fields["txtAccEmail"], ""), 128).ToLower();

        // send it to the account email address
        EMail.SendStandard(NotifySubject, (ishtml) ? html : text, (ishtml) ? text : "", email, ReplyTo, ishtml);

        return true;
    }
}