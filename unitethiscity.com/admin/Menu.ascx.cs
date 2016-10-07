/******************************************************************************
 * Filename: Menu.ascx.cs
 * Project:  unitethiscity.com Administration
 * 
 * Description:
 * A generic menu control used for administration website pages.
 * Controls included:
 * Browse, Create, Download Groups
 * 
 * Revision History:
 * $Log: $
******************************************************************************/
using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

public partial class admin_Menu : System.Web.UI.UserControl
{
	private bool showAdd = true;
	private bool showBrowse = true;
    private bool showMenu = true;

	public string Name = "";
	public string Prefix = "";
	public string Type = "";

	public bool ShowAdd
	{
		get { return showAdd; }
		set { showAdd = value; }
	}

	public bool ShowBrowse
	{
		get { return showBrowse; }
		set { showBrowse = value; }
	}

	public bool ShowMenu
	{
		get { return showMenu; }
		set { showMenu = value; }
    }
}