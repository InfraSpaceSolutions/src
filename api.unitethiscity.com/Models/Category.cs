/******************************************************************************
 * Filename: Category.cs
 * Project:  UTC WebAPI
 * 
 * Description:
 * POCO for representing model of a category in the UTC locations hierarchy.
 * Supports 2-level hierarchies (group/class)
******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.unitethiscity.api.Models
{
    public class Category
    {
        public int GroupId { get; set; }
        public int CatId { get; set; }
        public string GroupName { get; set; }
        public string CatName { get; set; }
        public int Count { get; set; }
    }
}