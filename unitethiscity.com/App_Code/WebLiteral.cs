/******************************************************************************
* Filename: WebLiteral.cs
* Project:  acrtinc.com Administration
* 
* Description:
* Quick and easy way to draw a web literal tag with-in a .cs code behind file.
* 
* Revision History:
* $Log: /MonstersUnlimited/utc/unitethiscity.com/website/App_Code/WebLiteral.cs $
 * 
 * 1     1/13/13 3:42p Ncross
 * 
 * 1     11/28/12 6:05p Ncross
******************************************************************************/
using System;
using System.Web.UI.WebControls;

/// <summary>
/// Standard web literal control instantiation
/// </summary>
public class WebLiteral : Literal
{
	public WebLiteral( string text )
		: base()
	{
		this.Text = text;
	}
}