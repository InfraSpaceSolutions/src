/******************************************************************************
 * Filename: CategoryController.cs
 * Project:  UTC WebAPI
 * 
 * Description:
 * Controller for retrieving categories
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
    public class CategoryController : ApiController
    {
        /// <summary>
        /// Get the categories as a 2-level tree in a list, sorted by
        /// group first (parent category) and category.  Only retrieves categories
        /// that contain one or more locations
        /// </summary>
        /// <returns>enumerable collection of categories</returns>
        public IEnumerable<Category> GetAllCategories()
        {
            WebDBContext db = new WebDBContext();
            List<Category> categories = new List<Category>();

            // get all of the groups
            IQueryable<VwCategories> rsGrp = db.VwCategories.Where(target => target.CatParentID == 0).OrderBy( target => target.CatName);
            foreach (VwCategories group in rsGrp)
            {
                // get the categories in the group
                IQueryable<VwCategories> rsCat = db.VwCategories.Where(target => target.CatParentID == group.CatID).OrderBy(target=>target.CatName);
                foreach (VwCategories rs in rsCat)
                {
                    Category cat = new Category() { CatId = rs.CatID, CatName = rs.CatName, GroupId = rs.CatParentID, GroupName = rs.CatParentName, Count = 0 };
                    // get a count of the enabled businesses in the specified category
                    cat.Count = db.TblBusinesses.Count(target => target.BusEnabled && target.CatID == cat.CatId);
                    // add them all, we can filter on the receiver
                    categories.Add(cat);
                }
            }
            Logger.LogAction("Category");
            return categories;
        }
    }
}
