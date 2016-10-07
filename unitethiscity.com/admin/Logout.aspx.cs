/******************************************************************************
 * Filename: Logout.aspx.cs
 * Project:  unitethiscity.com Administration
 * 
 * Description:
 * Log a user out by removing all session variables and redirecting
 * back to the login page.
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

public partial class admin_Logout : System.Web.UI.Page
{
	protected void Page_Load( object sender, EventArgs e )
	{
        CookieManager.AccountLogout( );

		// Go to the login page
		Response.Redirect( "Login.aspx" );
	}
}