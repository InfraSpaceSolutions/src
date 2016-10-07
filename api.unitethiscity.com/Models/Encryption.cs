/******************************************************************************
 * Filename: Encryption.cs
 * Project:  UTC WebAPI
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

namespace com.unitethiscity.api.Models
{
    /// <summary>
    /// Static class that provides encryption utility operations
    /// </summary>
    public static class Encryption
    {
        public static Guid passwordHashKey = new Guid("063a64a2-92ed-4953-bb27-dde61238ca5e");
        public static Guid memberHashKey = new Guid("ef0cd82a-8be1-4208-b6ae-0eaf97e7e14b");
        public static Guid businessHashKey = new Guid("4dc01879-29b6-4212-9654-fe4e6069bc2d");

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
        /// Calculate teh hash for the identification of a business
        /// </summary>
        /// <param name="locid">dentify location</param>
        /// <returns>encrypted version of business identifier</returns>
        public static string CalculateBusinessHash(int locid)
        {
            WebDBContext db = new WebDBContext();
            VwLocations rs = db.VwLocations.Single(target => target.LocID == locid);
            string raw = String.Format("{0}-{1}", rs.BusGuid.ToString().ToLower(), businessHashKey.ToString().ToLower());
            return GenerateHash(raw);
        }

        /// <summary>
        /// Calculate teh hash for the identification of a business
        /// </summary>
        /// <param name="busid">identify business</param>
        /// <returns>encrypted version of business identifier</returns>
        public static string CalculateBusinessHashByBusID(int busid)
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
}