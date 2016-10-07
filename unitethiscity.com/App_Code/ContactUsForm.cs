/******************************************************************************
 * Filename: ContactUsForm.cs
 * Project:  unitethiscity.thinkm.co
 * 
 * Description:
 * Form processing class for the contact us Form. 
 * Validates required fields
 * Stores submissions to the contactus table
 * Sends notification messages to configured recipient list
 * 
 * Revision History:
 * $Log: /MonstersUnlimited/utc/unitethiscity.com/website/App_Code/ContactUsForm.cs $
 * 
 * 2     1/18/13 12:08p Ncross
 * 
 * 1     1/18/13 12:07p Ncross 
******************************************************************************/
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using Sancsoft.Web;
using System.Text;

/// <summary>
/// Customized form processing for the page comments form
/// </summary>
public class ContactUsForm : FormProcessor
{
    /// <summary>
    /// Create an instance of the form processor with the collection of
    /// form fields from the submitted form.
    /// </summary>
    /// <param name="flds">Request.Form from the submitted page</param>
    public ContactUsForm( NameValueCollection flds )
        : base( flds )
    {
        NotifySubject = "Contact Us Form Submission";
        FieldPrefix = "Con";
    }

    /// <summary>
    /// Verify that the submitted fields are complete and valid
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public override bool Validate( )
    {
        if( !wv.RequiredFields( "txtConFName;txtConLName;emlConEMail;txtConComments" ) )
        {
            ResultCode = RC.DataIncomplete;
        }
        return ( ResultCode == RC.Ok );
    }

    /// <summary>
    /// Write the submitted form to the database
    /// </summary>
    /// <returns>id of new database record, 0 if not written</returns>
    public override bool StoreSubmission( )
    {
        int ismember = WebConvert.ToInt32( fields["radConMember"], 0 );
        int ispartner = WebConvert.ToInt32( fields["radConPartner"], 0 );

        // write the submission to the database
        TblContactUs rsCon = new TblContactUs( );
        rsCon.ConFName = WebConvert.Truncate( WebConvert.ToString( fields["txtConFName"], "" ), 50 );
        rsCon.ConLName = WebConvert.Truncate( WebConvert.ToString( fields["txtConLName"], "" ), 50 );
        rsCon.ConEMail = WebConvert.Truncate( WebConvert.ToString( fields["emlConEMail"], "" ), 128 );
        rsCon.ConMember = ( ( ismember == 1 ) ? true : false );
        rsCon.ConPartner = ( ( ispartner == 1 ) ? true : false );
        rsCon.ConComments = WebConvert.ToString( fields["txtConComments"], "" );
        rsCon.ConTimestamp = DateTime.Now;
        db.TblContactUs.InsertOnSubmit( rsCon );
        db.SubmitChanges( );
        // keep id of the new record
        newRecordID = rsCon.ConID;

        return true;
    }

    /// <summary>
    /// Generate the notification message for the comment form
    /// </summary>
    public override bool NotificationMessage( out string text, out string html )
    {
        StringBuilder msg = new StringBuilder( );
        text = "";
        html = "";

        int ismember = WebConvert.ToInt32( fields["radConMember"], 0 );
        int ispartner = WebConvert.ToInt32( fields["radConPartner"], 0 );

        msg.AppendLine( "Online form submission at " + DateTime.Now + "." );
        msg.AppendLine( new string( '=', 60 ) );
        msg.AppendLine( "First Name: " + WebConvert.Truncate( WebConvert.ToString( fields["txtConFName"], "" ), 50 ) );
        msg.AppendLine( "Last Name: " + WebConvert.Truncate( WebConvert.ToString( fields["txtConLName"], "" ), 50 ) );
        msg.AppendLine( "E-Mail Address: " + WebConvert.Truncate( WebConvert.ToString( fields["emlConEMail"], "" ), 128 ) );
        msg.AppendLine( "UTC Member?: " + ( ( ismember == 1 ) ? "Yes" : "No" ) );
        msg.AppendLine( "UTC Partner?: " + ( ( ispartner == 1 ) ? "Yes" : "No" ) );
        msg.AppendLine( "Message: " + WebConvert.ToString( fields["txtConComments"], "N/A" ) );
        msg.AppendLine( new string( '=', 60 ) );

        text = msg.ToString( );

        return false;
    }

    public override bool SendNotification( )
    {
        string text, html;
        bool ishtml;

        // generate the email message content
        ishtml = NotificationMessage( out text, out html );

        // send it to the configured recipient list
        EMail.SendStandard( NotifySubject, ( ishtml ) ? html : text, ( ishtml ) ? text : "", RecipientList, ReplyTo, ishtml );

        return true;
    }
}