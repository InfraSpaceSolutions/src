/******************************************************************************
 * Filename: Referrals.cs
 * Project:  UTC
 * 
 * Description:
 * Identify a referral code and manage the associated cookie to credit sign ups
 * 
 * Revision History:
 * $Log: /MonstersUnlimited/utc/unitethiscity.com/website/App_Code/Referrals.cs $
 * 
 * 1     1/18/13 6:45p Mterry
******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Class for managing inbound referrals, creating cookings and 
/// </summary>
public class Referrals
{
    public const int COOKIE_HOURS = 72;

    /// <summary>
    /// Look up a referral code and create the associated cookie on the user's
    /// browser to credit the referral
    /// </summary>
    /// <param name="referralCode">referral code</param>
    /// <returns>true if referral cookie assigned</returns>
    public static bool AssignReferral(string referralCode)
    {
        // remove any existing referral cookie
        HttpContext.Current.Session.Remove("REFERRAL_ID");

        // force all referral codes to upper case for comparison
        referralCode = referralCode.ToLower();

        WebDBContext db = new WebDBContext();
        TblReferralCodes rsRfc;

        // get the referral code
        rsRfc = db.TblReferralCodes.SingleOrDefault(target => target.RfcCode == referralCode);
        if (rsRfc == null)
        {
            return false;
        }

        // we found or made a referral code - assign the cookie to the user
        HttpCookie referralCookie = new HttpCookie( "REFERRAL_ID", rsRfc.RfcGuid.ToString());
        referralCookie.Expires = DateTime.Now.AddHours(COOKIE_HOURS);
        HttpContext.Current.Response.Cookies.Add( referralCookie );
        return true;
    }

    /// <summary>
    /// Remove any assigned referral code
    /// </summary>
    /// <returns>Guid that has been removed</returns>
    public static Guid RemoveReferral()
    {
        Guid ret = GetReferralIdentifier();
        HttpContext.Current.Response.Cookies["REFERRAL_ID"].Expires = DateTime.Now.AddHours(-COOKIE_HOURS);
        HttpContext.Current.Session.Remove("REFERRAL_ID");
        return ret;
    }

    /// <summary>
    /// Get an associated referral code for the user
    /// </summary>
    /// <returns>Guid of the referrer or null guid if no referrer</returns>
    public static Guid GetReferralIdentifier()
    {
        Guid ret = Guid.Empty;
        string referralId = WebConvert.CookieValue("REFERRAL_ID");
        if (!Guid.TryParse(referralId, out ret))
        {
            ret = Guid.Empty;
        }

        return ret;
    }

    /// <summary>
    /// Get the id that corresponds to the active referral code
    /// </summary>
    /// <returns></returns>
    public static int GetReferralID()
    {
        Guid rfcGuid = GetReferralIdentifier();
        WebDBContext db = new WebDBContext();
        // get the referral code if it exists
        TblReferralCodes rsRfc = db.TblReferralCodes.SingleOrDefault(target => target.RfcGuid == rfcGuid);
        return (rsRfc == null) ? 0 : rsRfc.RfcID;
    }

    /// <summary>
    /// Get the assigned referral code
    /// </summary>
    /// <returns>string of referral code or blank if not assigned</returns>
    public static string GetReferralCode()
    {
        Guid rfcGuid = GetReferralIdentifier();
        WebDBContext db = new WebDBContext();
        // get the referral code if it exists
        TblReferralCodes rsRfc = db.TblReferralCodes.SingleOrDefault(target => target.RfcGuid == rfcGuid);
        return (rsRfc == null) ? "" : rsRfc.RfcCode;
    }

    /// <summary>
    /// Get the promotion id with the current referral code
    /// </summary>
    /// <returns>identify promotion</returns>
    public static int GetPromotionID()
    {
        Guid rfcGuid = GetReferralIdentifier();
        WebDBContext db = new WebDBContext();
        // get the referral code if it exists
        TblReferralCodes rsRfc = db.TblReferralCodes.SingleOrDefault(target => target.RfcGuid == rfcGuid);
        return (rsRfc == null) ? 0 : rsRfc.ProID;
    }
}