/******************************************************************************
 * Filename: SSL.cs
 * Project:  UTC
 * 
 * Description:
 * Manage transition between SSL and non-SSL access to the site.
 * 
 * Revision History:
 * $Log: /MonstersUnlimited/utc/unitethiscity.com/website/App_Code/SSL.cs $
 * 
 * 2     1/18/13 6:46p Mterry
 * subscription payment and referral handling
 * 
 * 1     1/18/13 1:49p Mterry
******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for SSL
/// </summary>
public class SSL
{

    /// <summary>
    /// Force redirection to the SSL version of the site; checks the site configuration
    /// to see if redirection is required
    /// </summary>
    /// <returns>true - redirecting</returns>
    public static bool ForceSSL()
    {
        HttpContext context = HttpContext.Current;
        string url = context.Request.Url.ToString();
        int port = WebConvert.ToInt32(context.Request.ServerVariables["SERVER_PORT"], 0);

        // if ssl support is enabled, redirect to the ssl version of the page if we are on non-ssl version
        if ((WebConvert.ToBoolean(SiteSettings.GetValue("ForceSSL"), true)) && (port == 80))
        {
            // convert http:// to https:// and preserve the rest of the url
            url = url.Replace("http://", "https://");
            // redirect to the secure page
            context.Response.Redirect(url,false);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Force redirection to the non-SSL version of the site
    /// </summary>
    /// <returns></returns>
    public static bool UnforceSSL()
    {
        HttpContext context = HttpContext.Current;
        string url = context.Request.Url.ToString();
        int port = WebConvert.ToInt32(context.Request.ServerVariables["SERVER_PORT"], 0);

        // if ssl support is enabled, redirect to the normal version of the page 
        if ((port == 443) && url.StartsWith("https://"))
        {
            // convert https:// to http://and preserve the rest of the url
            url = url.Replace("https://", "http://");
            // redirect to the secure page
            context.Response.Redirect(url, false);
            return true;
        }
        return false;
    }
}