/******************************************************************************
 * Filename: FormProcessor.cs
 * Project:  unitethiscity.thinkm.co
 * 
 * Description:
 * Server side form processing base class.  Inherit from this class to 
 * create application specific form processing. Provides base functions for
 * validating form submissions on the server, storing the form submissions to
 * the database and email notification messages.
 * 
 * The object takes a name/value collection so it should work for both post
 * and get forms.
 * 
 * Building your own form processor class:
 * 1) Validation: override Validate() with the form specific tests; use the
 * webvalidator object to perform tests; if validation fails, no notifications
 * are sent and no records are stored
 * 
 * 2) Database Storage: override the StoreSubmission() method to write to
 * the appropriate location (i.e. database table), set the NewRecordID property
 * to the id of the new database record; default behavior is to not store
 * the form submission
 * 
 * 3) Notification: 
 *		a) override the NotificationMessage() generator to make a
 *		form specific message otherwise its just a bag of fields
 *		b) update the notification properties NotifySubject, ReplyTo and 
 *		RecipentList; defaults are provided
 *		c) the id of the new database record is available in the object (if
 *		applicable) so you can provide a link to the record online in the
 *		notification message
 *
 * Using the form processor class:
 * 1) Create a form processor
 *		a) FormProcessor formDo = new FormProcessor( Request.Form );
 * 2) Call ProcessForm in the postback of the page
 * 3) Check the object properities to handle the results
 *		a) ResultCode gives a familiar RC result; RC.Ok indicates no errors
 *		encountered
 *		b) ResultsTitle, ResultsMessage give strings from the error table
 *		by default
 *		c) ProcessedPost can be tested to see if this is a postback that
 *		has been processed - successful or not
 *		
 * Revision History:
 * $Log: /MonstersUnlimited/utc/unitethiscity.com/website/App_Code/FormProcessor.cs $
 * 
 * 2     1/17/13 12:46p Ncross
 * 
 * 1     1/16/13 6:02p Ncross
******************************************************************************/
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Configuration;
using Sancsoft.Web;

/// <summary>
/// Base class for simplified form processing on ASP Web Pages
/// </summary>
public class FormProcessor
{
    public const string DefaultSkipPrefix = "_";
    protected NameValueCollection fields;
    protected WebDBContext db;
    protected WebValidator wv;

    /// <summary>
    /// Definition of the standard field types; this is protected so derived classes can add
    /// to the array as needed for processing specific non-standard prefixes
    /// </summary>
    protected string[] fieldTypes = { "btn", "chk", "col", "dat", "eml", "fil", "hid", "img", "nbr", "pwd", "rad", "rng", "ser", "txt", "url" };

    /// <summary>
    /// Summary result code for form processing; anything other than RC.Ok
    /// indicates something has gone wrong
    /// </summary>
    public RC ResultCode
    {
        get;
        set;
    }

    /// <summary>
    /// Id of the new database record created when the form is processed or
    /// 0 if not processed or unable to create a record
    /// </summary>
    protected int newRecordID;
    public int NewRecordID
    {
        get
        {
            return newRecordID;
        }
    }

    /// <summary>
    /// True if a posted form has processed; check the result code
    /// for the results
    /// </summary>
    public bool ProcessedPost
    {
        get;
        set;
    }

    /// <summary>
    /// List of recipients for the notification messages.
    /// </summary>
    public string RecipientList
    {
        get;
        set;
    }

    /// <summary>
    /// Reply to address for the notifications
    /// </summary>
    public string ReplyTo
    {
        get;
        set;
    }

    /// <summary>
    /// Subject to use for notification messages
    /// </summary>
    public string NotifySubject
    {
        get;
        set;
    }

    /// <summary>
    /// Prefix of field names (keys) that should be stripped in
    /// the email message
    /// </summary>
    public string FieldPrefix
    {
        get;
        set;
    }

    /// <summary>
    /// Prefix of field names (keys) that should be skipped over
    /// in the email message.  The default value = "_"
    /// </summary>
    public string FieldSkipPrefix
    {
        get;
        set;
    }

    /// <summary>
    /// Create a form processor with the related fields for processing
    /// </summary>
    /// <param name="flds"></param>
    public FormProcessor( NameValueCollection flds )
    {
        // set the reference to the form fields
        fields = flds;
        // create a connection to the database
        db = new WebDBContext( );
        // create a web validator to use for checking fields
        wv = new WebValidator( fields );
        // default the direction of notifications to the configuration
        RecipientList = SiteSettings.GetValue( "EMailNotify" );
        // default the reply to address to the configuration
        ReplyTo = SiteSettings.GetValue( "EMailReplyTo" );
        // default the notification subject
        NotifySubject = "Online form submission from website";
        // start off with now field prefix; this should be set by the derived object
        FieldPrefix = "";
        // start off with the default skip prefix
        FieldSkipPrefix = DefaultSkipPrefix;
        // clear the result code
        ResultCode = RC.Ok;
        newRecordID = 0;
    }

    /// <summary>
    /// Attempt to process the submitted form
    /// </summary>
    /// <returns>true if form processing succeeded</returns>
    public virtual bool ProcessForm( )
    {
        // validate the submitted form
        if( Validate( ) )
        {
            // store the submission to the database (if applicable)
            if( StoreSubmission( ) )
            {
                // sent out the notification email message (if applicable)
                SendNotification( );
            }
        }
        // an attempt at processing the post has been completed
        ProcessedPost = true;
        return ( ResultCode == RC.Ok );
    }

    /// <summary>
    /// Generate a notification message from the request fields;  This should 
    /// be overridden by the specific form processor, but provides a dump
    /// of all submitted form fields
    /// Also includes the IP address and the id of the new record
    /// </summary>
    /// <param name="text">text only version of the message</param>
    /// <param name="html">html version of the message</param>
    /// <returns>true if there is an html version</returns>
    public virtual bool NotificationMessage( out string text, out string html )
    {
        text = "Online form submission at " + DateTime.Now.ToString( ) + "." + Environment.NewLine;
        text += new string( '=', 60 ) + Environment.NewLine;
        // add all of the fields to the message in whatever order we get them
        foreach( string key in fields )
        {
            // skip any keys that start with the skip prefix
            if( key.StartsWith( FieldSkipPrefix ) )
            {
                continue;
            }
            // add the field 
            text += StripFieldPrefix( key ) + ": '" + fields[key] + "'" + Environment.NewLine;
        }
        text += new string( '=', 60 ) + Environment.NewLine;
        // add the submitting ip address to the message
        text += "IP Address: " + HttpContext.Current.Request.ServerVariables["REMOTE_HOST"] + Environment.NewLine;
        text += "Database Record ID: " + NewRecordID.ToString( ) + Environment.NewLine;

        // we dont generate html by default
        html = "";
        return false;
    }

    /// <summary>
    /// Apply stripping of field prefixes from the field names to make notification
    /// messages less offensive
    /// </summary>
    /// <param name="key">key to strip</param>
    /// <returns>stripped key</returns>
    public virtual string StripFieldPrefix( string key )
    {
        foreach( string fldtype in fieldTypes )
        {
            if( key.StartsWith( fldtype + FieldPrefix ) )
            {
                key = key.Remove( 0, fldtype.Length + FieldPrefix.Length );
                break;
            }
        }
        return key;
    }

    /// <summary>
    /// Verify that the submitted fields are complete and valid
    /// </summary>
    /// <returns>true if submission valid</returns>
    public virtual bool Validate( )
    {
        return true;
    }

    /// <summary>
    /// Write the submitted form to the database if applicable; default behavior is to
    /// do nothing
    /// </summary>
    /// <returns>result of storing</returns>
    public virtual bool StoreSubmission( )
    {
        // we don't know how to store the submission, default behavior is to discard the
        // submission after notification
        newRecordID = 0;
        return true;
    }

    /// <summary>
    /// Send out the notification emails of the form submission; default behavior is to
    /// send to the configured notification list
    /// </summary>
    /// <returns>result of notification</returns>
    public virtual bool SendNotification( )
    {
        string text, html;
        bool ishtml;

        // generate the email message content
        ishtml = NotificationMessage( out text, out html );

        // send it to the configured recipient list
        EMail.SendStandard( NotifySubject, ( ishtml ) ? html : text, ( ishtml ) ? text : "", RecipientList, ReplyTo, ishtml );

        return true;
    }

    /// <summary>
    /// Get the error title for the result code
    /// </summary>
    /// <returns>error title</returns>
    public virtual string ResultTitle( )
    {
        TblErrors rsErr = db.TblErrors.SingleOrDefault( target => target.ErrID == (int)ResultCode );
        return ( rsErr == null ) ? "Unknown Error" : rsErr.ErrTitle;
    }

    /// <summary>
    /// Get the error message for the result code
    /// </summary>
    /// <returns>error title</returns>
    public virtual string ResultMessage( )
    {
        TblErrors rsErr = db.TblErrors.SingleOrDefault( target => target.ErrID == (int)ResultCode );
        return ( rsErr == null ) ? "An undefined error has occurred." : rsErr.ErrMessage;
    }
}