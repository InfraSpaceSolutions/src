/******************************************************************************
 * Filename: SiteAdmin.master.cs
 * Project:  unitethiscity.com Administration
 * 
 * Description:
 * The master page layout of the administration website.  This master page
 * should define markup only.
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

public partial class admin_SiteAdmin : System.Web.UI.MasterPage
{
	protected void Page_Load( object sender, EventArgs e )
	{
        PageHeaderLoginPanel.Visible = ( CookieManager.SesAccEMail != "" );
        AccEMailLiteral.Text = CookieManager.SesAccEMail;
	}
}