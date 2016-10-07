/******************************************************************************
 * Filename: SiteAccount.cs
 * Project:  unitethiscity.com
 * 
 * Description:
 * Class for managing the account of the authenticated user.  This class is 
 * to be instantiated based on the active account in the session. 
******************************************************************************/
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;
using Sancsoft.Web;

/// <summary>
/// Summary description for SiteAccount
/// </summary>
public class SiteAccount
{
    WebDBContext db;
    /// <summary>
    /// Properties
    /// </summary>
    #region Properties
    protected int accID;
    public int AccID
    {
        get 
        {
            return accID;
        }
    }
    protected Guid accGuid;
    public Guid AccGuid
    {
        get
        {
            return accGuid;
        }
    }
    protected int citID;
    public int CitID
    {
        get
        {
            return citID;
        }
    }
    protected int rfcID;
    public int RfcID
    {
        get
        {
            return rfcID;
        }
    }

    public string AccEMail
    {
        get;
        set;
    }

    public string AccFName
    {
        get;
        set;
    }

    public string AccLName
    {
        get;
        set;
    }

    public string AccPhone
    {
        get;
        set;
    }

    public int AtyID
    {
        get;
        set;
    }

    public DateTime AccBirthdate
    {
        get;
        set;
    }

    public string AccGender
    {
        get;
        set;
    }

    public string AccZip
    {
        get;
        set;
    }

    #endregion Properties   


    /// <summary>
    /// Support session based login by email address and password.  Attempts to login using the provided credentials. Throws
    /// exceptions on common errors
    /// </summary>
    /// <param name="emailAddress">Account email address</param>
    /// <param name="password">Password for the account</param>
    public static void Login(string emailAddress, string password)
    {
        WebDBContext db = new WebDBContext();

        // normalize the specification of the email address
        emailAddress = emailAddress.Trim();
        emailAddress = emailAddress.ToLower();
        // lookup the account by email address
        TblAccounts rs = db.TblAccounts.SingleOrDefault(target => target.AccEMail == emailAddress);

        // verify that we have a matching account
        if (rs == null)
        {
            throw new WebException(RC.AccountDNE);
        }

        // check the supplied password
        if ( rs.AccPassword != password )
        {
            throw new WebException(RC.BadPassword);
        }

        // make sure that the account is enabled
        if ( !rs.AccEnabled )
        {
            throw new WebException(RC.AccountDisabled);
        }

        // configure the session to contain the account information
        HttpContext.Current.Session["ACCOUNT_ID"] = rs.AccID.ToString();

        // capture the name and email into the session to support the existing administration interface
        HttpContext.Current.Session["ACCOUNT_EMAIL"] = rs.AccEMail;
        HttpContext.Current.Session["ACCOUNT_NAME"] = rs.AccFName + " " + rs.AccLName;
        HttpContext.Current.Session["ACCOUNT_AVATAR"] = "/images/AccountImage.png";
    }

    /// <summary>
    /// Logout any currently active account
    /// </summary>
    public static void Logout()
    {
        HttpContext.Current.Session.Remove("ACCOUNT_ID");
        HttpContext.Current.Session.Remove("ACCOUNT_EMAIL");
        HttpContext.Current.Session.Remove("ACCOUNT_NAME");
        HttpContext.Current.Session.Remove("BUSINESS_ID");
    }

    /// <summary>
    /// Check to see if there is an active site account
    /// </summary>
    /// <returns>true if session has account id</returns>
    public static bool IsLoggedIn()
    {
        return (WebConvert.ToInt32(HttpContext.Current.Session["ACCOUNT_ID"], 0) != 0);
    }

    /// <summary>
    /// Request a square avatar graphic for the active user
    /// </summary>
    /// <param name="size">dimensions of graphic</param>
    /// <returns>url for retrieving avatar graphic</returns>
    public static string ActiveAvatarURL(int size = 100)
    {
        return GravatarUrl(WebConvert.ToString(HttpContext.Current.Session["ACCOUNT_EMAIL"], ""), size);
    }

    /// <summary>
    /// Request a square avatar graphic for an email address
    /// </summary>
    /// <param name="email">account email</param>
    /// <param name="size">dimensions of graphic</param>
    /// <returns>url for retrieving avatar graphic</returns>
    public static string AccountAvatarURL(string email, int size = 100)
    {
        return GravatarUrl(email, size);
    }


    /// <summary>
    /// Create a gravatar url from the supplied email address
    /// </summary>
    /// <param name="emailAddress"></param>
    /// <param name="size"></param>
    /// <returns>full gravatar url for an image tag</returns>
    protected static string GravatarUrl(string emailAddress, int size)
    {
        bool secure = (HttpContext.Current.Request.ServerVariables["SERVER_PORT_SECURE"] == "1");
        Gravatar g = new Gravatar();
        return g.ImageUrl(emailAddress, size, secure);
    }

    /// <summary>
    /// Create a site account object using the current session variables; creates a null user if not authenticated
    /// </summary>
    public SiteAccount()
    {
        db = new WebDBContext();

        accID = WebConvert.ToInt32(HttpContext.Current.Session["ACCOUNT_ID"], 0);
        if (accID == 0)
        {
            throw new WebException(RC.InternalError);
        }

        TblAccounts rs = db.TblAccounts.Single(target => target.AccID == accID);
        
        // make sure that the account is enabled
        if (!rs.AccEnabled)
        {
            throw new WebException(RC.AccountDisabled);
        }

        // load the object properties
        accGuid = rs.AccGuid;
        citID = rs.CitID;
        rfcID = rs.RfcID;
        AccEMail = rs.AccEMail;
        AccFName = rs.AccFName;
        AccLName = rs.AccLName;
        AccPhone = rs.AccPhone;
        AccZip = rs.AccZip;
        AtyID = rs.AtyID;
        AccGender = rs.AccGender;
        AccBirthdate = rs.AccBirthdate;
    }

    /// <summary>
    /// Assign a new password to the account
    /// </summary>
    /// <param name="oldPassword">specify the current password</param>
    /// <param name="newPassword">the new password for the account</param>
    public void AssignNewPassword(string oldPassword, string newPassword)
    {
        // get the record to be updated
        TblAccounts rs = db.TblAccounts.SingleOrDefault(target => target.AccID == accID);
        if (rs == null)
        {
            throw new WebException(RC.TargetDNE);
        }

        // check the old password
        if (rs.AccPassword != oldPassword)
        {
            throw new WebException(RC.BadPassword);
        }

        // make sure that the new password is long enough
        if (newPassword.Length < WebConvert.ToInt32(SiteSettings.GetValue("PasswordMinLength"), 8))
        {
            throw new WebException(RC.DataInvalid);
        }

        // assign the new password
        rs.AccPassword = newPassword;
        db.SubmitChanges();
    }

    /// <summary>
    /// Save changes to the account information
    /// </summary>
    /// <returns>true if changes saved</returns>
    public bool SaveChanges()
    {
        // clean up the email address
        AccEMail = AccEMail.Trim();
        AccEMail = WebConvert.Truncate(AccEMail, 128);
        AccEMail = AccEMail.ToLower();

        // make sure that the email address will not create a duplicate account
        if (db.TblAccounts.Count(target => (target.AccEMail == AccEMail) && (target.AccID != accID)) > 0)
        {
            throw new WebException(RC.DuplicateEmail);
        }

        // save the changes to the account object
        TblAccounts rs = db.TblAccounts.Single(target => target.AccID == accID);
        rs.AccFName = WebConvert.Truncate(AccFName,50);
        rs.AccLName = WebConvert.Truncate(AccLName,50);
        rs.AccEMail = AccEMail;
        rs.AccPhone = WebConvert.Truncate(AccPhone,50);
        rs.AtyID = AtyID;
        rs.AccGender = WebConvert.Truncate(AccGender, 20);
        rs.AccBirthdate = AccBirthdate;
        rs.AccZip = WebConvert.Truncate(AccZip, 10);
        db.SubmitChanges();

        return true;
    }

    /// <summary>
    /// Check to see if the account has any access to the specified role
    /// </summary>
    /// <param name="rol">Role to check</param>
    /// <returns>true if user has this role assigned</returns>
    protected bool HasRole(Roles rol)
    {
        return (db.TblAccountRoles.Count(target => (target.RolID == (int)rol) && (target.AccID == AccID)) > 0);
    }

    /// <summary>
    /// Checks to see if the active account has the member role
    /// </summary>
    /// <returns>true if the user is a member</returns>
    public bool IsMember()
    {
        return HasRole(Roles.Member);
    }

    /// <summary>
    /// Checks to see if the active account has the administrator role
    /// </summary>
    /// <returns>true if the user is an administrator</returns>
    public bool IsAdministrator()
    {
        return HasRole(Roles.Administrator);
    }

    /// <summary>
    /// Checks to see if the active account has the business role for any business
    /// </summary>
    /// <returns>true if user has access to any business</returns>
    public bool IsBusiness()
    {
        return HasRole(Roles.Business);
    }

    /// <summary>
    /// Request a square avatar graphic
    /// </summary>
    /// <param name="size">dimensions of graphic</param>
    /// <returns>url for retrieving avatar graphic</returns>
    public string AvatarURL(int size = 100)
    {
        return GravatarUrl(AccEMail, size);
    }

    /// <summary>
    ///  Get a list of the businesses managed by this account
    /// </summary>
    /// <returns></returns>
    public List<SiteBusinessInfo> AccountBusinesses()
    {
        List<SiteBusinessInfo> busList = new List<SiteBusinessInfo>();
        var rsBus = from bus in db.VwAccountBusinesses
                    where bus.AccID == AccID
                    orderby bus.BusFormalName
                    select bus;
        foreach (VwAccountBusinesses bus in rsBus)
        {
            busList.Add(new SiteBusinessInfo(bus));
        }

        return busList;
    }

    /// <summary>
    /// Get the account type name for the current account
    /// </summary>
    /// <returns>name of account type  or invalid if cant figure it out</returns>
    public string AccountTypeName()
    {
        TblAccountTypes aty = db.TblAccountTypes.SingleOrDefault(target => target.AtyID == AtyID);
        return (aty != null) ? aty.AtyName : "Invalid";
    }
}
