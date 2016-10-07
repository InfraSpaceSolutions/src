/*********************************************************************************
 * Filename: Login.aspx.cs
 * Project:  unitethiscity.com Administration
 * 
 * Description:
 * The login page that allows account administrators to access the administration website.
 * 
 * Revision History:
 * $Log: $
**********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.IO;
using Sancsoft.Web;

public partial class admin_Login : System.Web.UI.Page
{
	WebDBContext db = new WebDBContext();

	protected void Page_Load( object sender, EventArgs e )
	{
		// Wire the events
		OkButton.Click += new EventHandler( OkButton_Click );
	}

	void OkButton_Click( object sender, EventArgs e )
	{
        string accEMail = AccEMailTextBox.Text.Trim( );
        string accPassword = AccPasswordTextBox.Text;

        TblAccounts rs = db.TblAccounts.SingleOrDefault( target => target.AccEMail == accEMail );

		// Check the account
		if ( rs == null )
		{
            throw new WebException( RC.AccountDNE );
		}

        TblAccountRoles rsAcr = db.TblAccountRoles.SingleOrDefault( target => target.AccID == rs.AccID && target.RolID == (int)Roles.Administrator );
        // Verify the account has administrator access
        if( rsAcr == null )
        {
            throw new WebException( RC.AccessDenied );
        }

		// Check the password
		if ( rs.AccPassword != accPassword )
		{
			throw new WebException( RC.BadPassword );
		}

		// Verify the account is enabled
		if ( !rs.AccEnabled )
		{
            throw new WebException( RC.AccountDisabled );
		}

        // Log in the administrator
        CookieManager.AccountLogin( rs );

		// Set up the redirection
		string redirection = WebConvert.ToString( Session["ACCOUNT_LOGIN_REDIRECTION"], "Default.aspx" );

		// Go to the home page
		Response.Redirect( redirection );
	}
}