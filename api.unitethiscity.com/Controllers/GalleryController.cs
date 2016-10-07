/******************************************************************************
 * Filename: GalleryController.cs
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
    /// Get the summary information for one or more locations
    /// </summary>
    public class GalleryController : ApiController
    {
        /// <summary>
        /// Get the ordered gallery of images for a business
        /// </summary>
        /// <param name="id">business identifier</param>
        /// <returns>gallery items order by sequence</returns>
        public IEnumerable<GalleryItem> Get(int id)
        {
            WebDBContext db = new WebDBContext();

            // working list of location summaries to return 
            List<GalleryItem> gallery = new List<GalleryItem>();

            IEnumerable<TblGalleryItems> rs = db.TblGalleryItems.Where(target => target.BusID == id).OrderBy(target => target.GalSeq);
            foreach (TblGalleryItems row in rs)
            {
                gallery.Add(Factory(row));
            }
            Logger.LogAction("Gallery", 0, id);
            return gallery;
        }

        /// <summary>
        /// Create a gallery item from a record
        /// </summary>
        /// <param name="rs">database record</param>
        /// <returns>model</returns>
        [NonAction]
        protected GalleryItem Factory(TblGalleryItems rs)
        {
            GalleryItem gal = new GalleryItem();

            // construct the location info elements
            gal.Id = rs.GalID;
            gal.BusId = rs.BusID;
            gal.Sequence = rs.GalSeq;
            gal.ImageId = rs.GalGuid;
            return gal;
        }
    }
}
