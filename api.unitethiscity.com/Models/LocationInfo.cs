/******************************************************************************
 * Filename: LocationSummary.cs
 * Project:  UTC WebAPI
 * 
 * Description:
 * POCO for representing model of the general information for a location to
 * support list views.
******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.unitethiscity.api.Models
{
    public class LocationInfo
    {
        public int Id { get; set; }
        public int BusId { get; set; }
        public Guid BusGuid { get; set; }
        public int CitId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public double Rating { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int CatId { get; set; }
        public string CatName { get; set; }
        public List<string> Properties;
        public bool Entertainer { get; set; }
    
    }
}