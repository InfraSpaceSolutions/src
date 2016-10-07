/******************************************************************************
 * Filename: SiteSettings.cs
 * Project:  GENPROX.EXE
 * 
 * Description:
 * Manage access to the settings in the database.
 * 
******************************************************************************/

using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Xml.Linq;
using System.Text;

namespace com.unitethiscity
{
    /// <summary>
    /// Provides access for accessing and assigning the system settings stored
    /// in the database.  Settings have been moved from the application configuration
    /// to the database to support access by supporting applications;
    /// Note these all throw an exception if the requested setting does not
    /// exist.
    /// </summary>
    public class SiteSettings
    {
        // FieldTypes tells us how to edit a setting
        public enum FieldTypes
        {
            SingleLine = 1,
            MultiLine,
            HTML
        }

        /// <summary>
        /// Get a setting value from the database by id
        /// </summary>
        /// <param name="setID"></param>
        /// <returns></returns>
        public static string GetValue(int setID)
        {
            WebDBContext db = new WebDBContext();
            TblSettings rs = db.TblSettings.Single(target => target.SetID == setID);
            return rs.SetValue;
        }

        /// <summary>
        /// Get a setting value by name
        /// </summary>
        /// <param name="setName"></param>
        /// <returns></returns>
        public static string GetValue(string setName)
        {
            WebDBContext db = new WebDBContext();
            TblSettings rs = db.TblSettings.Single(target => target.SetName == setName);
            return rs.SetValue;
        }

        /// <summary>
        /// Assign a setting by its id
        /// </summary>
        /// <param name="setID"></param>
        /// <param name="setValue"></param>
        public static void SetValue(int setID, string setValue)
        {
            WebDBContext db = new WebDBContext();
            TblSettings rs = db.TblSettings.Single(target => target.SetID == setID);
            rs.SetValue = setValue;
            db.SubmitChanges();
        }

        /// <summary>
        /// Assign a setting by its name
        /// </summary>
        /// <param name="setName"></param>
        /// <param name="setValue"></param>
        public static void SetValue(string setName, string setValue)
        {
            WebDBContext db = new WebDBContext();
            TblSettings rs = db.TblSettings.Single(target => target.SetName == setName);
            rs.SetValue = setValue;
            db.SubmitChanges();
        }

        /// <summary>
        /// Get the field type for a setting.  Throws if setting doesn't exist
        /// </summary>
        /// <param name="setID">identify setting</param>
        /// <returns>FieldTypes enumerable</returns>
        public FieldTypes GetFieldType(int setID)
        {
            WebDBContext db = new WebDBContext();
            TblSettings rs = db.TblSettings.Single(target => target.SetID == setID);
            return (FieldTypes)rs.SetType;
        }

        /// <summary>
        /// Get the field type for a setting.  Throws if setting doesn't exist
        /// </summary>
        /// <param name="setName">identify setting</param>
        /// <returns>FieldTypes enumerable</returns>
        public FieldTypes GetFieldType(string setName)
        {
            WebDBContext db = new WebDBContext();
            TblSettings rs = db.TblSettings.Single(target => target.SetName == setName);
            return (FieldTypes)rs.SetType;
        }
    }
}