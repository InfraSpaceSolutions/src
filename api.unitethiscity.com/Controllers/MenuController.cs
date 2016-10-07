/******************************************************************************
 * Filename: MenuController.cs
 * Project:  UTC WebAPI
 * 
 * Description:
 * Get the information for a business image gallery
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
    /// <summary>
    /// Get the menu items for a business
    /// </summary>
    public class MenuController : ApiController
    {
        /// <summary>
        /// Get the menu items for a business
        /// </summary>
        /// <param name="id">business</param>
        /// <returns>array of menu items in defined sequence</returns>
        public IEnumerable<MenuItem> Get(int id)
        {
            WebDBContext db = new WebDBContext();

            List<MenuItem> menu = new List<MenuItem>();

            IEnumerable<TblMenuItems> rs = db.TblMenuItems.Where(target => target.BusID == id).OrderBy(target => target.MenSeq);
            foreach (TblMenuItems row in rs)
            {
                menu.Add(Factory(row));
            }
            Logger.LogAction("Menu", 0, id);
            return menu;
        }

        [NonAction]
        protected MenuItem Factory(TblMenuItems rs)
        {
            MenuItem men = new MenuItem();

            // construct the location info elements
            men.Id = rs.MenID;
            men.BusId = rs.BusID;
            men.Sequence = rs.MenSeq;
            men.Name = rs.MenName;
            men.Price = rs.MenPrice;
            return men;
        }
    }
}
