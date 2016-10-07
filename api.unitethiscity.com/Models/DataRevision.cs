/******************************************************************************
 * Filename: DataRevision
 * Project:  UTC WebAPI
 * 
 * Description:
 * Utility class for calculating periods from dates and timestamps
******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.unitethiscity.api.Models
{
    /// <summary>
    /// Enumeration of revisionable items
    /// </summary>
    public enum Revisioned
    {
        LocationInfo = 1,
        Categories = 2,
        Deals = 3,
        EventInfo = 4
    }

    /// <summary>
    /// Utility class for handling data revisions
    /// </summary>
    public class DataRevision
    {
        public int DrvId { get; set; }
        public string Name { get; set; }
        public int Revision { get; set; }
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Get the revision of a data set by identifier
        /// </summary>
        /// <param name="id">identify dataset</param>
        /// <returns>revision level, 0 if unknown</returns>
        static public int GetRevision(Revisioned id)
        {
            WebDBContext db = new WebDBContext();
            TblDataRevisions drv = db.TblDataRevisions.SingleOrDefault(target => target.DrvID == (int)id);
            return (drv != null) ? drv.DrvRevision : 0;
        }

        /// <summary>
        /// Get the revision of a data set by name
        /// </summary>
        /// <param name="name">identify dataset</param>
        /// <returns>revision level, 0 if unknown</returns>
        static public int GetRevision(string name)
        {
            WebDBContext db = new WebDBContext();
            TblDataRevisions drv = db.TblDataRevisions.SingleOrDefault(target => target.DrvName == name);
            return (drv != null) ? drv.DrvRevision : 0;
        }

        /// <summary>
        /// Set the revision of a data set by id
        /// </summary>
        /// <param name="id">identify dataset</param>
        /// <param name="rev">rev number to assign</param>
        static public void SetRevision(Revisioned id, int rev)
        {
            WebDBContext db = new WebDBContext();
            TblDataRevisions drv = db.TblDataRevisions.SingleOrDefault(target => target.DrvID == (int)id);
            drv.DrvRevision = rev;
            drv.DrvTS = DateTime.Now;
            db.SubmitChanges();
        }

        /// <summary>
        /// Set the revision of a data set by name
        /// </summary>
        /// <param name="name">identify dataset</param>
        /// <param name="rev">rev number to assign</param>
        static public void SetRevision(string name, int rev)
        {
            WebDBContext db = new WebDBContext();
            TblDataRevisions drv = db.TblDataRevisions.SingleOrDefault(target => target.DrvName == name);
            drv.DrvRevision = rev;
            drv.DrvTS = DateTime.Now;
            db.SubmitChanges();
        }

        /// <summary>
        /// Increase the revision of a data set by id
        /// </summary>
        /// <param name="id">identify dataset</param>
        static public void Bump(Revisioned id)
        {
            WebDBContext db = new WebDBContext();
            TblDataRevisions drv = db.TblDataRevisions.SingleOrDefault(target => target.DrvID == (int)id);
            drv.DrvRevision = drv.DrvRevision + 1;
            drv.DrvTS = DateTime.Now;
            db.SubmitChanges();
        }

        /// <summary>
        /// Increase the revision of a data set by name
        /// </summary>
        /// <param name="name">identify dataset</param>
        static public void Bump(string name)
        {
            WebDBContext db = new WebDBContext();
            TblDataRevisions drv = db.TblDataRevisions.SingleOrDefault(target => target.DrvName == name);
            drv.DrvRevision = drv.DrvRevision + 1;
            drv.DrvTS = DateTime.Now;
            db.SubmitChanges();
        }
    }
}