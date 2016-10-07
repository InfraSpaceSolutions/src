/******************************************************************************
 * Filename: BusinessRegistrationForm.cs
 * Project:  unitethiscity.thinkm.co
 * 
 * Description:
 * Form processing class for the business registration Form. 
 * Validates required fields
 * Stores submissions to the businesses table
 * Sends notification messages to configured recipient list
 * 
 * Revision History:
 * $Log: /MonstersUnlimited/utc/unitethiscity.com/website/App_Code/BusinessRegistrationForm.cs $
 * 
 * 1     1/17/13 12:46p Ncross
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
public class BusinessRegistrationForm : FormProcessor
{
    /// <summary>
    /// Create an instance of the form processor with the collection of
    /// form fields from the submitted form.
    /// </summary>
    /// <param name="flds">Request.Form from the submitted page</param>
    public BusinessRegistrationForm( NameValueCollection flds )
        : base( flds )
    {
        NotifySubject = "Business Registration Form Submission";
        FieldPrefix = "Bus";
    }

    /// <summary>
    /// Verify that the submitted fields are complete and valid
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public override bool Validate( )
    {
        if( !wv.RequiredFields( "txtBurFName;txtBurLName;txtBurBusinessName;selBurCategory;emlBurEMail;txtBurPhone" ) )
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
        TblBusinessRegistrations rsBur = new TblBusinessRegistrations( );
        rsBur.BurFName = WebConvert.Truncate( WebConvert.ToString( fields["txtBurFName"], "" ), 50 );
        rsBur.BurLName = WebConvert.Truncate( WebConvert.ToString( fields["txtBurLName"], "" ), 50 );
        rsBur.BurBusinessName = WebConvert.Truncate( WebConvert.ToString( fields["txtBurBusinessName"], "" ), 128 );
        rsBur.BurCategory = WebConvert.Truncate( WebConvert.ToString( fields["selBurCategory"], "" ), 80 );
        rsBur.BurEMail = WebConvert.Truncate( WebConvert.ToString( fields["emlBurEMail"], "" ), 128 );
        rsBur.BurPhone = WebConvert.Truncate( WebConvert.ToString( fields["txtBurPhone"], "" ), 50 );
        rsBur.BurAdditionalInfo = WebConvert.ToString( fields["txtBurAdditionalInfo"], "" );
        rsBur.BurTimestamp = DateTime.Now;
        db.TblBusinessRegistrations.InsertOnSubmit( rsBur );
        db.SubmitChanges( );
        // keep id of the new record
        newRecordID = rsBur.BurID;

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
        msg.AppendLine( "First Name: " + WebConvert.Truncate( WebConvert.ToString( fields["txtBurFName"], "" ), 50 ) );
        msg.AppendLine( "Last Name: " + WebConvert.Truncate( WebConvert.ToString( fields["txtBurLName"], "" ), 50 ) );
        msg.AppendLine( "Business Name: " + WebConvert.Truncate( WebConvert.ToString( fields["txtBurBusinessName"], "" ), 128 ) );
        msg.AppendLine( "Business Category: " + WebConvert.Truncate( WebConvert.ToString( fields["selBurCategory"], "" ), 80 ) );
        msg.AppendLine( "E-Mail Address: " + WebConvert.Truncate( WebConvert.ToString( fields["emlBurEMail"], "" ), 128 ) );
        msg.AppendLine( "Phone: " + WebConvert.Truncate( WebConvert.ToString( fields["txtBurPhone"], "" ), 50 ) );
        msg.AppendLine( "AdditionalInfo: " + WebConvert.ToString( fields["txtBurAdditionalInfo"], "N/A" ) );
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