/******************************************************************************
 * Filename: Phone.cs
 * Project:  unitethiscity.com
 * 
 * Description:
 * Utility class for formatting of telelphone numbers
******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

/// <summary>
/// Summary description for Phone
/// </summary>
public static class Phone
{
    /// <summary>
    /// Clean a phone number to just the digits
    /// </summary>
    /// <param name="rawPhone">string containing phone number</param>
    /// <returns>digits only string</returns>
    public static string Clean(string rawPhone)
    {
        return Regex.Replace(rawPhone, @"[^\d]", "");
    }

    /// <summary>
    /// Format a phone number in the desired default format
    /// +1-123-456-7890
    /// </summary>
    /// <param name="rawPhone">string of a phone number</param>
    /// <returns>formatting string of phone number</returns>
    public static string Format(string rawPhone)
    {
        // generate a clean version of the phone number
        string clean = Phone.Clean(rawPhone);
        // force the number to 10 digits
        clean = clean.PadRight(10,'0');
        return string.Format("+1-{0}-{1}-{2}", clean.Substring(0, 3), clean.Substring(3, 3), clean.Substring(6, 4));
    }
}