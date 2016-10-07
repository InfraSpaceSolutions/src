/******************************************************************************
 * Filename: Enumeration.cs
 * Project:  UTC WebAPI
 * 
 * Description:
 * A collection of enumerated types that match the database entries
******************************************************************************/

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

public enum AccountTypes
{
    Perpetual = 1,
    Monthly = 2
}

public enum PaymentTypes
{
    None = 0,
    AuthNet,
    Paypal
};

