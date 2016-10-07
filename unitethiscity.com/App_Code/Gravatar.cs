/******************************************************************************
 * Filename: Gravatar.cs
 * Project:  unitethiscity.com
 * 
 * Description:
 * Utility class for getting Gravatar images for email addresses.
******************************************************************************/
using System;
using System.Security.Cryptography;
using System.Text;

/// <summary>
/// Utility class for retrieving Gravatar image links
/// </summary>
public class Gravatar
{
    /// <summary>
    /// Properties
    /// </summary>
    #region Properties
    public string DefaultImage
    {
        get;
        set;
    }

    public string ImageExtension
    {
        get;
        set;
    }
    #endregion


    // cryptography object for generating hashes
    protected MD5 md5Hash;

    /// <summary>
    /// Construcgtor
    /// </summary>
    public Gravatar()
    {
        md5Hash = MD5.Create();
        DefaultImage = "mm";
        ImageExtension = ".jpg";
    }

    /// <summary>
    /// Generate the hashed version of the email address for Gravatar use.  Cleans
    /// the email address for compatiblity with service
    /// </summary>
    /// <param name="emailAddress">raw email address</param>
    /// <returns>md5 hash of email address</returns>
    protected string GenerateHash(string emailAddress)
    {
        // clean up the email address
        emailAddress = emailAddress.ToLower();
        emailAddress = emailAddress.Trim();

        // create a hascii string version of the hash
        byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(emailAddress));
        StringBuilder sb = new StringBuilder();
        foreach (byte by in data)
        {
            sb.Append(by.ToString("x2"));
        }
        return sb.ToString();
    }

    /// <summary>
    /// Create a gravatar url from the supplied email address
    /// </summary>
    /// <param name="emailAddress"></param>
    /// <param name="size"></param>
    /// <returns>full gravatar url for an image tag</returns>
    public string ImageUrl(string emailAddress, int size, bool secure = false)
    {
        // use the secure url if requested, otherwise use the default
        string url = (secure) ? "https://secure.gravatar.com" : "http://www.gravatar.com";
        // generate a hashed version of the supplied email address
        string hash = GenerateHash(emailAddress);
        // if a default image is specified, add it to the query string
        string defaultQS = (DefaultImage != "") ? "&d=" + DefaultImage : "";
        // return the complete url appropriate for an image tag, use the configured image extension
        return url + "/avatar/" + hash + ImageExtension + "?s=" + size + defaultQS;
    }
}

