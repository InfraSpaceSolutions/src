/******************************************************************************
 * Filename: GalleryItem.cs
 * Project:  UTC WebAPI
 * 
 * Description:
 * POCO for representing model of the general information for a gallery item
******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.unitethiscity.api.Models
{
    public class GalleryItem
    {
        public int Id { get; set; }
        public int BusId { get; set; }
        public int Sequence { get; set; }
        public Guid ImageId { get; set; }
    }
}