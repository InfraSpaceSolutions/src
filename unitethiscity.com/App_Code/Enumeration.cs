/******************************************************************************
 * Filename: Enumeration.cs
 * Project:  unitethiscity.com
 * 
 * Description:
 * Primary key values for various tables used throughout the website.
 * 
 * Revision History:
 * $Log: $
******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Account Roles
/// defined in TblRoles
/// </summary>
public enum Roles
{
    Administrator = 1,
    Member = 2,
    Business = 3,
    Charity = 4,
    SalesRep = 5,
    Associate = 6
}

public enum AccountRoles
{
    Administrator = 1,
    Member = 2,
    Business = 3,
    Charity = 4,
    SalesRep = 5,
    Associate = 6
}

public enum MessageJobStates
{ 
    Definition = 1,
    Queued = 2,
    Sending = 3,
    Sent = 4,
    Canceled = 5
}

public enum PaymentTypes 
{ 
    None = 0, 
    AuthNet, 
    Paypal 
};

public enum AccountTypes
{
    Perpetual = 1,
    Monthly = 2
}

