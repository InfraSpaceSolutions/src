/******************************************************************************
 * Filename: AccForcePassword.aspx.cs
 * Project:  unitethiscity.com Administration
 * 
 * Description:
 * Force an accounts's password.
 * 
 * Revision History:
 * $Log: $
******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sancsoft.Web;

public partial class admin_AccForcePassword : System.Web.UI.Page
{
	int id;
	WebDBContext db = new WebDBContext();

	protected void Page_Load( object sender, EventArgs e )
	{
		// Wire events
		SubmitButton.Click += new EventHandler( SubmitButton_Click );

		// Get the target id
		id = WebConvert.ToInt32( Request.QueryString["ID"], 0 );

		// Verify target id exists
		if ( id == 0 )
		{
			throw new WebException( RC.DataIncomplete );
		}

		if ( !Page.IsPostBack )
		{
			// Get the target record
			TblAccounts rs = db.TblAccounts.SingleOrDefault( target => target.AccID == id );

			// Verify target record exists
			if ( rs == null )
			{
				throw new WebException( RC.TargetDNE );
			}

			// Populate the page
			AccIDLiteral.Text = rs.AccID.ToString();
			AccEMailLiteral.Text = rs.AccEMail;
		}
	}

	void SubmitButton_Click( object sender, EventArgs e )
	{
		// Get the record
        TblAccounts rs = db.TblAccounts.Single( target => target.AccID == id );

		// Update the record
		// Create password if field is left blank
        if( !AccPasswordTextBox.Text.Equals( "" ) )
		{
			// Check for appropriate password length
            if( AccPasswordTextBox.Text.Length < 6 )
			{
				throw new WebException( RC.BadPassword );
			}

            rs.AccPassword = WebConvert.Truncate( AccPasswordTextBox.Text, 20 );
		}
		else
		{
            rs.AccPassword = Password.GenerateRandom( 6 );
		}

		// Sync to database
		db.SubmitChanges();

		// Redirect to target view page
        Response.Redirect( "AccView.aspx?ID=" + id.ToString( ) );
	}
}