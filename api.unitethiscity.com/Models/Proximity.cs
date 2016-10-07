/******************************************************************************
 * Filename: Proximity.cs
 * Project:  UTC WebAPI
 * 
 * Description:
 * POCO for representing model of a push token location
******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.unitethiscity.api.Models
{
    public class Proximity
    {
        public string PutToken { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

    }
}