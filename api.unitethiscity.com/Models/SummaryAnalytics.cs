/******************************************************************************
 * Filename: SummaryAnalytics.cs
 * Project:  UTC WebAPI
 * 
 * Description:
 * POCO for representing model of summary analytics
******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.unitethiscity.api.Models
{
    public class SummaryAnalytics
    {
        public string Name { get; set; }
        public int Total { get; set; }
        public double Percent { get; set; }
        public int Group { get; set; }
    }
}