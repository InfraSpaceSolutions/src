/******************************************************************************
 * Filename: CharityRegistrationForm.cs
 * Project:  unitethiscity.thinkm.co
 * 
 * Description:
 * Form processing class for the business registration Form. 
 * Validates required fields
 * Stores submissions to the businesses table
 * Sends notification messages to configured recipient list
 * 
 * Revision History:
 * $Log: /MonstersUnlimited/utc/unitethiscity.com/website/App_Code/CharityRegistrationForm.cs $
 * 
 * 1     3/11/13 4:51p Ncross
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
public class CharityRegistrationForm : FormProcessor
{
    /// <summary>
    /// Create an instance of the form processor with the collection of
    /// form fields from the submitted form.
    /// </summary>
    /// <param name="flds">Request.Form from the submitted page</param>
    public CharityRegistrationForm( NameValueCollection flds )
        : base( flds )
    {
        NotifySubject = "Charity Registration Form Submission";
        FieldPrefix = "Cha";
    }

    /// <summary>
    /// Verify that the submitted fields are complete and valid
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public override bool Validate( )
    {
        if (!wv.RequiredFields("txtChaFName;txtChaLName;txtChaCharityName;emlChaEMail;txtChaPhone"))
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
        // write the submission to the database
        TblCharityRegistrations rsCha = new TblCharityRegistrations( );
        rsCha.ChaFName = WebConvert.Truncate(WebConvert.ToString(fields["txtChaFName"], ""), 50);
        rsCha.ChaLName = WebConvert.Truncate(WebConvert.ToString(fields["txtChaLName"], ""), 50);
        rsCha.ChaCharityName = WebConvert.Truncate(WebConvert.ToString(fields["txtChaCharityName"], ""), 128);
        rsCha.ChaEMail = WebConvert.Truncate(WebConvert.ToString(fields["emlChaEMail"], ""), 128);
        rsCha.ChaPhone = WebConvert.Truncate(WebConvert.ToString(fields["txtChaPhone"], ""), 50);
        rsCha.ChaAdditionalInfo = WebConvert.ToString(fields["txtChaAdditionalInfo"], "");
        rsCha.ChaTimestamp = DateTime.Now;
        db.TblCharityRegistrations.InsertOnSubmit( rsCha );
        db.SubmitChanges( );
        // keep id of the new record
        newRecordID = rsCha.ChaID;

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

        msg.AppendLine( "Online form submission at " + DateTime.Now + "." );
        msg.AppendLine( new string( '=', 60 ) );
        msg.AppendLine( "First Name: " + WebConvert.Truncate( WebConvert.ToString( fields["txtChaFName"], "" ), 50 ) );
        msg.AppendLine("Last Name: " + WebConvert.Truncate(WebConvert.ToString(fields["txtChaLName"], ""), 50));
        msg.AppendLine("Charity Name: " + WebConvert.Truncate(WebConvert.ToString(fields["txtChaCharityName"], ""), 128));
        msg.AppendLine( "E-Mail Address: " + WebConvert.Truncate( WebConvert.ToString( fields["emlChaEMail"], "" ), 128 ) );
        msg.AppendLine( "Phone: " + WebConvert.Truncate( WebConvert.ToString( fields["txtChaPhone"], "" ), 50 ) );
        msg.AppendLine( "AdditionalInfo: " + WebConvert.ToString( fields["txtChaAdditionalInfo"], "N/A" ) );
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