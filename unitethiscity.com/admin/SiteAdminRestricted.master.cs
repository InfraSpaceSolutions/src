/******************************************************************************
 * Filename: SiteAdminRestricted.master.cs
 * Project:  unitethiscity.com Administration
 * 
 * Description:
 * The master page website logic for the administration website.  Enforce
 * user login, manage session variables, etc.  This master page should define
 * code and navigation only.
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

public partial class admin_SiteAdminRestricted : System.Web.UI.MasterPage
{
	protected void Page_Init( object sender, EventArgs e )
	{
        // Clear any captured login redirection page
		Session.Remove( "ACCOUNT_LOGIN_REDIRECTION" );

		// Verify the administrator is logged in
		if ( CookieManager.SesAccID == "" )
		{
			// Capture the current page and querystring to the session
			string url = Request.ServerVariables["URL"];
			if ( Request.ServerVariables["QUERY_STRING"] != "" )
			{
				url += "?" + Request.ServerVariables["QUERY_STRING"];
			}
            Session["ACCOUNT_LOGIN_REDIRECTION"] = url;

			// Redirect to the login screen
			Response.Redirect( "Login.aspx" );
		}
	}
}