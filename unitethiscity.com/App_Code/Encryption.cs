/******************************************************************************
 * Filename: Encryption.cs
 * Project:  unitethiscity.com
 * 
 * Description:
 * Support for generating encrypted tokens and keys.  Uses combinations of
 * static keys and dynamic data to generate MD5 hashes.
******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Text;

/// <summary>
/// Static class that provides encryption utility operations
/// </summary>
public static class Encryption
{
    public static string memberIdentifierQURL = "/qr/m/";
    public static string businessIdentifierQURL = "/qr/b/";
    public static string referralQURL = "/ref/";
    public static Guid passwordHashKey = new Guid("063a64a2-92ed-4953-bb27-dde61238ca5e");
    public static Guid memberHashKey = new Guid("ef0cd82a-8be1-4208-b6ae-0eaf97e7e14b");
    public static Guid businessHashKey = new Guid("4dc01879-29b6-4212-9654-fe4e6069bc2d");

    /// <summary>
    /// Generate the query string that is used to identify a member
    /// </summary>
    /// <param name="accid">account identifier</param>
    /// <returns>query string</returns>
    public static string MemberIdentifierQueryString(int accid)
    {
        string h = CalculateMemberHash(accid);
        string qr = string.Format("a={0}&h={1}", accid, h).ToLower();
        return qr;
    }

    /// <summary>
    /// Generate the QR URI that identifies a member
    /// </summary>
    /// <param name="accid">account identifier</param>
    /// <returns>complete QR uri for member</returns>
    public static string MemberIdentifierQURI(int accid)
    {
        return string.Format("{0}{1}?{2}", SiteSettings.GetValue("RootURL"), memberIdentifierQURL, MemberIdentifierQueryString(accid));
    }

    /// <summary>
    /// Generate the query string that is used to identify a business
    /// </summary>
    /// <param name="busid">identify business</param>
    /// <returns>query string</returns>
    public static string BusinessIdentifierQueryString(int busid)
    {
        string h = CalculateBusinessHash(busid);
        string qr = string.Format("b={0}&h={1}", busid, h).ToLower();
        return qr;
    }

    /// <summary>
    /// Generate the QR URI that identifies a business
    /// </summary>
    /// <param name="busid">identify business</param>
    /// <returns>complete QR uri for business</returns>
    public static string BusinessIdentifierQURI(int busid)
    {
        return string.Format("{0}{1}?{2}", SiteSettings.GetValue("RootURL"), businessIdentifierQURL, BusinessIdentifierQueryString(busid));
    }

    /// <summary>
    /// Generate the query string from the code string
    /// </summary>
    /// <param name="code">referral code</param>
    /// <returns>query string</returns>
    public static string ReferralQueryString(string code)
    {
        return string.Format("code={0}", HttpUtility.UrlEncode(code.ToLower()));
    }

    /// <summary>
    /// Generate the query string for a referral code
    /// </summary>
    /// <param name="rfcid">identify referral code</param>
    /// <returns>query string</returns>
    public static string ReferralQueryString(int rfcid)
    {
        WebDBContext db = new WebDBContext();
        TblReferralCodes rs = db.TblReferralCodes.Single(target => target.RfcID == rfcid);
        return ReferralQueryString(rs.RfcCode);
    }

    /// <summary>
    /// Generate the URI for a referral code 
    /// </summary>
    /// <param name="code">referral code string</param>
    /// <returns>URL string</returns>
    public static string ReferralQURI(string code)
    {
        return string.Format("{0}{1}?{2}", "http://www.unitethiscity.com", referralQURL, ReferralQueryString(code));
    }

    /// <summary>
    /// Generate the URI for a referral record
    /// </summary>
    /// <param name="rfcid">referral code identifier</param>
    /// <returns>URL string</returns>
    public static string ReferralQURI(int rfcid)
    {
        return string.Format("{0}{1}?{2}", "http://www.unitethiscity.com", referralQURL, ReferralQueryString(rfcid));
    }

    /// <summary>
    /// Calculate the hashed version of the password
    /// </summary>
    /// <param name="password">clear text password</param>
    /// <returns>Hashed password</returns>
    public static string CalculatePasswordHash(string password)
    {
        string raw = String.Format("{0}-{1}", password, passwordHashKey.ToString().ToLower());
        return GenerateHash(raw);
    }

    /// <summary>
    /// Calculate the hash for identification of a member
    /// </summary>
    /// <param name="accid">identify account</param>
    /// <returns>encrypted version of account identifier</returns>
    public static string CalculateMemberHash(int accid)
    {
        WebDBContext db = new WebDBContext();
        TblAccounts rs = db.TblAccounts.Single(target => target.AccID == accid);
        string raw = String.Format("{0}-{1}", rs.AccGuid.ToString().ToLower(), memberHashKey.ToString().ToLower());
        return GenerateHash(raw);
    }

    /// <summary>
    /// Calculate the hash for the identification of a business
    /// </summary>
    /// <param name="busid">Identify business</param>
    /// <returns>encrypted version of business identifier</returns>
    public static string CalculateBusinessHash(int busid)
    {
        WebDBContext db = new WebDBContext();
        TblBusinesses rs = db.TblBusinesses.Single(target => target.BusID == busid);
        string raw = String.Format("{0}-{1}", rs.BusGuid.ToString().ToLower(), businessHashKey.ToString().ToLower());
        return GenerateHash(raw);
    }

    /// <summary>
    /// Calclulate an MD5 encryption of a string
    /// </summary>
    /// <param name="raw">unencrypted string</param>
    /// <returns>encrypted version of raw</returns>
    public static string GenerateHash(string raw)
    {
        MD5 md5Hash = MD5.Create();
        // create a hascii string version of the hash
        byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(raw));
        StringBuilder sb = new StringBuilder();
        foreach (byte by in data)
        {
            sb.Append(by.ToString("x2"));
        }
        return sb.ToString();
    }
}
