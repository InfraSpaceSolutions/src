/******************************************************************************
 * Filename: DataRevisionController.cs
 * Project:  UTC WebAPI
 * 
 * Description:
 * Controller for enumerating and modifying member favorite locations
******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using com.unitethiscity.api.Models;

namespace com.unitethiscity.api.Controllers
{
    public class DataRevisionController : ApiController
    {
        /// <summary>
        /// Get the revision level of a data set
        /// </summary>
        /// <param name="id">identify data revision</param>
        /// <returns>Data revision</returns>
        public DataRevision Get(int id)
        {
            WebDBContext db = new WebDBContext();
            DataRevision drv = new DataRevision();
            drv.DrvId = id;
            TblDataRevisions rsDrv = db.TblDataRevisions.SingleOrDefault(target => target.DrvID == id);
            if (rsDrv == null)
            {
                drv.Name = "";
                drv.Revision = 0;
                drv.Timestamp = DateTime.Now;
            }
            else
            {
                drv.Name = rsDrv.DrvName;
                drv.Revision = rsDrv.DrvRevision;
                drv.Timestamp = rsDrv.DrvTS;
            }
            return drv;
        }

        /// <summary>
        /// Get the revision level of a data set
        /// </summary>
        /// <param name="name">name of the data revision</param>
        /// <returns>Data revision</returns>
        public DataRevision Get(string name)
        {
            WebDBContext db = new WebDBContext();
            DataRevision drv = new DataRevision();
            drv.Name = name;
            TblDataRevisions rsDrv = db.TblDataRevisions.SingleOrDefault(target => target.DrvName == name);
            if (rsDrv == null)
            {
                drv.DrvId = 0;
                drv.Revision = 0;
                drv.Timestamp = DateTime.Now;
            }
            else
            {
                drv.DrvId = rsDrv.DrvID;
                drv.Revision = rsDrv.DrvRevision;
                drv.Timestamp = rsDrv.DrvTS;
            }
            return drv;
        }
    }
}
