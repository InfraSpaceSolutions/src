using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for SummaryStats
/// </summary>
public class SummaryStats
{
    public string Name { get; set; }
    public string Link { get; set; }
    public int Today { get; set; }
    public int PastWeek { get; set; }
    public int ThisPeriod { get; set; }
    public int LastPeriod { get; set; }
    public int AllTime { get; set; }

    public SummaryStats(string name = "")
    {
        Name = name;
        Today = 0;
        PastWeek = 0;
        ThisPeriod = 0;
        LastPeriod = 0;
        AllTime = 0;
    }
}