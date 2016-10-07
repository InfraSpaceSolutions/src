/************************************************************************************
 * Filename: CookieManager.cs
 * Project:  unitethiscity.com Administration
 * 
 * Description:
 * Class for managing the various named cookies and session variables of the website.
 * 
 * Revision History:
 * $Log: $
*************************************************************************************/
using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using Sancsoft.Web;

/// <summary>
/// Class for managing the various named cookies and session variables of the website
/// </summary>
public class CookieManager
{
	//************************************************************************************************
	// ACCOUNT( ROLE ADMINISTRATOR ) SESSION

	/// <summary>
	/// Return the account ID in the session
	/// </summary>
	/// <returns>Account ID</returns>
	public static string SesAccID
	{
		get { return WebConvert.ToString( HttpContext.Current.Session["ACCOUNT_ID"], "" ); }
	}

	/// <summary>
	/// Return the account email address in the session
	/// </summary>
	/// <returns>Account email address</returns>
	public static string SesAccEMail
	{
		get { return WebConvert.ToString( HttpContext.Current.Session["ACCOUNT_EMAIL"], "" ); }
	}

	/// <summary>
	/// Return the account name in the session
	/// </summary>
	/// <returns>Account name</returns>
	public static string SesAccName
	{
		get { return WebConvert.ToString( HttpContext.Current.Session["ACCOUNT_NAME"], "" ); }
	}

	/// <summary>
	/// Create an account session
	/// </summary>
	/// <param name="rsAdm">Administrator database view object</param>
	public static void AccountLogin( TblAccounts rsAcc )
	{
        HttpContext.Current.Session["ACCOUNT_ID"] = rsAcc.AccID.ToString( );
        HttpContext.Current.Session["ACCOUNT_EMAIL"] = rsAcc.AccEMail;
        HttpContext.Current.Session["ACCOUNT_NAME"] = rsAcc.AccFName + " " + rsAcc.AccLName;
	}

	/// <summary>
	/// Remove an account session
	/// </summary>
	public static void AccountLogout( )
	{
        HttpContext.Current.Session.Remove( "ACCOUNT_ID" );
        HttpContext.Current.Session.Remove( "ACCOUNT_EMAIL" );
        HttpContext.Current.Session.Remove( "ACCOUNT_NAME" );
	}
}