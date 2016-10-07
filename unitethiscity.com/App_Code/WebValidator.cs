/******************************************************************************
 * Filename: WebValidator.cs
 * Project:  UTC
 * 
 * Description:
 * Server side validator tool to support field validation in ASP.NET
 * 
 * Revision History:
 * $Log: /MonstersUnlimited/utc/unitethiscity.com/website/App_Code/WebValidator.cs $
 * 
 * 2     1/13/13 9:16p Mterry
 * merge in object dev
******************************************************************************/
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for WebValidator
/// </summary>
public class WebValidator
{
	private NameValueCollection fields;

	/// <summary>
	/// Construct a web validator by getting a reference to
	/// the field value collection in question
	/// </summary>
	/// <param name="fields"></param>
	public WebValidator( NameValueCollection flds = null)
	{
		fields = flds;
	}

	/// <summary>
	/// Checks a single key for a non-empty value
	/// </summary>
	/// <param name="key"></param>
	/// <returns></returns>
	public bool RequiredField(string key)
	{
		string value = fields[key].Trim();
		return (value.Length > 0);
	}

	/// <summary>
	/// Check all of the keys in the array for non-empty values
	/// </summary>
	/// <param name="keyArray">array of keys</param>
	/// <returns>true if all are non-empty</returns>
	public bool RequiredFields(string[] keyArray)
	{
		foreach (string key in keyArray)
		{
			if (!RequiredField(key))
			{
				return false;
			}
		}
		return true;
	}

	/// <summary>
	/// Check a list of required fields delimited by semi-colon
	/// </summary>
	/// <param name="keyArray">array of keys</param>
	/// <returns>true if all are non-empty</returns>
	public bool RequiredFields(string fields)
	{
		string[] keyArray = fields.Split(';');
		foreach (string key in keyArray)
		{
			if (!RequiredField(key))
			{
				return false;
			}
		}
		return true;
	}
}