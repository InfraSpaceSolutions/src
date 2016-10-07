/******************************************************************************
 * Filename: SummaryStats.cs
 * Project:  UTC WebAPI
 * 
 * Description:
 * POCO for representing model of summary statisticss
******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.unitethiscity.api.Models
{
    public class SummaryStats
    {
        public string Name { get; set; }
        public string Link { get; set; }
        public int Today { get; set; }
        public int PastWeek { get; set; }
        public int ThisPeriod { get; set; }
        public int LastPeriod { get; set; }
        public int AllTime { get; set; }
    }
}