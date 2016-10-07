/******************************************************************************
 * Filename: TestimonialsForm.cs
 * Project:  unitethiscity.thinkm.co
 * 
 * Description:
 * Form processing class for the testimonials Form. 
 * Validates required fields
 * Stores submissions to the testimonials table
 * Sends notification messages to configured recipient list
 * 
 * Revision History:
 * $Log: /MonstersUnlimited/utc/unitethiscity.com/website/App_Code/TestimonialsForm.cs $
 * 
 * 1     1/16/13 6:02p Ncross
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
public class TestimonialsForm : FormProcessor
{
    /// <summary>
    /// Create an instance of the form processor with the collection of
    /// form fields from the submitted form.
    /// </summary>
    /// <param name="flds">Request.Form from the submitted page</param>
    public TestimonialsForm( NameValueCollection flds )
        : base( flds )
    {
        NotifySubject = "Testimonial Form Submission";
        FieldPrefix = "Tst";
    }

    /// <summary>
    /// Verify that the submitted fields are complete and valid
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public override bool Validate( )
    {
        if( !wv.RequiredFields( "txtTstFName;txtTstLName;emlTstEMail;txtTstTestimonial" ) )
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
        int ismember = WebConvert.ToInt32( fields["radTstMember"], 0 );
        int ispartner = WebConvert.ToInt32( fields["radTstPartner"], 0 );

        // write the submission to the database
        TblTestimonials rsTst = new TblTestimonials( );
        rsTst.TstFName = WebConvert.Truncate( WebConvert.ToString( fields["txtTstFName"], "" ), 50 );
        rsTst.TstLName = WebConvert.Truncate( WebConvert.ToString( fields["txtTstLName"], "" ), 50 );
        rsTst.TstEMail = WebConvert.Truncate( WebConvert.ToString( fields["emlTstEMail"], "" ), 128 );
        rsTst.TstMember = ( ( ismember == 1 ) ? true : false );
        rsTst.TstPartner = ( ( ispartner == 1 ) ? true : false );
        rsTst.TstTestimonial = WebConvert.ToString( fields["txtTstTestimonial"], "" );
        rsTst.TstTimestamp = DateTime.Now;
        db.TblTestimonials.InsertOnSubmit( rsTst );
        db.SubmitChanges( );
        // keep id of the new record
        newRecordID = rsTst.TstID;

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

        int ismember = WebConvert.ToInt32( fields["radTstMember"], 0 );
        int ispartner = WebConvert.ToInt32( fields["radTstPartner"], 0 );

        msg.AppendLine( "Online form submission at " + DateTime.Now + "." );
        msg.AppendLine( new string( '=', 60 ) );
        msg.AppendLine( "First Name: " + WebConvert.Truncate( WebConvert.ToString( fields["txtTstFName"], "" ), 50 ) );
        msg.AppendLine( "Last Name: " + WebConvert.Truncate( WebConvert.ToString( fields["txtTstLName"], "" ), 50 ) );
        msg.AppendLine( "E-Mail Address: " + WebConvert.Truncate( WebConvert.ToString( fields["emlTstEMail"], "" ), 128 ) );
        msg.AppendLine( "UTC Member?: " + ( ( ismember == 1 ) ? "Yes" : "No" ) );
        msg.AppendLine( "UTC Partner?: " + ( ( ispartner == 1 ) ? "Yes" : "No" ) );
        msg.AppendLine( "Testimonial: " + WebConvert.ToString( fields["txtTstTestimonial"], "N/A" ) );
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