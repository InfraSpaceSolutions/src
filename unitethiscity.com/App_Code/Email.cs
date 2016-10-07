/******************************************************************************
 * Filename: Email.cs
 * Project:  unitethiscity.thinkm.co
 * 
 * Description:
 * Class to send electronic mail through the website.
 * 
 * Revision History:
 * $Log: /MonstersUnlimited/utc/unitethiscity.com/website/App_Code/Email.cs $
 * 
 * 2     1/17/13 12:46p Ncross
 * 
 * 1     1/16/13 6:02p Ncross
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
using System.Net;
using System.Net.Mail;
using System.Net.Mime;

/// <summary>
/// Class to send electronic mail through the website
/// </summary>
public static class EMail
{
    // Custom debugging setting
    static bool isDebug = WebConvert.ToBoolean( ConfigurationManager.AppSettings["IS_DEBUG"], false );
    // Set up the email server credential variables from the configuration file
    static string EMailServer = SiteSettings.GetValue( "EMailServer" );
    static string EMailAccount = SiteSettings.GetValue( "EMailAccount" );
    static string EMailPassword = SiteSettings.GetValue( "EMailPassword" );
    static string EMailSender = SiteSettings.GetValue( "EMailSender" );
    static string EMailFrom = SiteSettings.GetValue( "EMailFrom" );
    static string EMailFromDisplayName = SiteSettings.GetValue( "EMailFromDisplayName" );
    static string EMailReplyTo = SiteSettings.GetValue( "EMailReplyTo" );
    static string EMailNotify = SiteSettings.GetValue( "EMailNotify" );
    static int EMailServerPort = WebConvert.ToInt32( SiteSettings.GetValue( "EMailServerPort" ), 25 );

    /// <summary>
    /// Send an standard e-mail message from the website
    /// </summary>
    /// <param name="emlSubject">Subject text for the message</param>
    /// <param name="emlBody">Body text for the message</param>
    /// <param name="emlAltBody">Alternate body text for the message</param>
    /// <param name="emlRecipients">Recipient list for the message separated by a semi-colon (;)</param>
    /// <param name="emlReplyTo">List of custom reply-to recipients</param>
    /// <param name="emlIsHtml">Sets the mode of email message to html or plain text</param>
    public static void SendStandard( string emlSubject, string emlBody, string emlAltBody, string emlRecipients, string emlReplyTo, bool emlIsHtml )
    {
        // Create the email message
        MailMessage objMail = new MailMessage( );

        // Define the email message
        objMail.Sender = new MailAddress( EMailSender );
        objMail.From = new MailAddress( EMailFrom, EMailFromDisplayName );

        // Check the custom reply to value
        if( emlReplyTo != "" )
        {
            objMail.ReplyToList.Add( new MailAddress( emlReplyTo ) );
        }
        else
        {
            objMail.ReplyToList.Add( new MailAddress( EMailReplyTo, EMailFromDisplayName ) );
        }
        objMail.IsBodyHtml = emlIsHtml;

        // Check the the debug setting
        if( isDebug )
        {
            // Check the html status of the email and append the intended recipients to the body of the email message
            if( emlIsHtml )
            {
                emlBody += "<br><br>Intended Recipient(s): " + emlRecipients;
            }
            else
            {
                emlBody += Environment.NewLine + Environment.NewLine + "Intended Recipient(s): " + emlRecipients;
            }

            // Modify the subject indicating that we are in debug mode
            emlSubject = "[DEBUG MODE] - " + emlSubject;

            // Overide the recipient list with email notify
            emlRecipients = EMailNotify;
        }

        // Set the subject
        objMail.Subject = emlSubject;

        // Setup the mail message in either plain text or html
        if( !emlIsHtml )
        {
            // Plain text message
            objMail.Body = emlBody;
        }
        else
        {
            // HTML message -- set the alternate views for HTML and plain text
            ContentType cttp = new ContentType( "text/plain" );
            AlternateView altBody = AlternateView.CreateAlternateViewFromString( emlAltBody, cttp );
            AlternateView htmBody = AlternateView.CreateAlternateViewFromString( emlBody, System.Text.Encoding.UTF8, MediaTypeNames.Text.Html );

            // Plain text alternate view MUST be added before the HTML alternate view in order
            // for the message to appear correctly in all client e-mail applications
            objMail.AlternateViews.Add( altBody );
            objMail.AlternateViews.Add( htmBody );
        }

        // Send the e-mail message to the recipient list
        SMTP_Send( objMail, emlRecipients );
    }

    /// <summary>
    /// Set up the SMTP client and send out an email message
    /// </summary>
    /// <param name="message">Formatted message to be sent</param>
    /// <param name="emlRecipients">Recipient list for the message separated by a semi-colon (;)</param>
    static void SMTP_Send( MailMessage message, string recipients )
    {
        // Connect to the smtp client
        SmtpClient objEmlClient = new SmtpClient( EMailServer, EMailServerPort );
        objEmlClient.UseDefaultCredentials = false;

        // Set up the credentials
        NetworkCredential objSmtpCredentials = new NetworkCredential( EMailAccount, EMailPassword );
        objEmlClient.Credentials = objSmtpCredentials;

        // Send the email to each recipient
        char[] addrDelimeter = { ';' };
        foreach( var emlAddr in recipients.Split( addrDelimeter ) )
        {
            // Add the recipient and catch and remove any invalid email addresses
            try
            {
                message.To.Add( new MailAddress( emlAddr ) );
            }
            catch( FormatException )
            {
                continue;
            }

            // Send the email message
            objEmlClient.Send( message );

            // Clear the recipient
            message.To.Clear( );
        }
    }
}