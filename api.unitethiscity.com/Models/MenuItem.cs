/******************************************************************************
 * Filename: MenuItem.cs
 * Project:  UTC WebAPI
 * 
 * Description:
 * POCO for representing model of the general information for a menu item
******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.unitethiscity.api.Models
{
    public class MenuItem
    {
        public int Id { get; set; }
        public int BusId { get; set; }
        public int Sequence { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }

    }
}