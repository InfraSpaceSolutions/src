/******************************************************************************
 * Filename: StatUtility.cs
 * Project:  UTC WebAPI
 * 
 * Description:
 * A set of shared utility functions for statistics formatting
******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.unitethiscity.api.Models
{
    public class StatUtility
    {
        /// <summary>
        /// Format a member name as first name last initial
        /// </summary>
        /// <param name="fname">nullable first name</param>
        /// <param name="lname">nullable last name</param>
        /// <returns>Firstname L.</returns>
        public static string FormatMemberName(string fname, string lname)
        {
            string firstname = fname ?? "Guest";
            string lastname = lname ?? "User";

            return firstname + " " + lastname.Substring(0, 1) + ".";
        }

        public static string FormatSortableDate(DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd");
        }
        public static string FormatSortableTimestamp(DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd-hh:mm:ss");
        }
        public static string FormatDisplayTimestamp(DateTime dt)
        {
            return dt.ToString("MM-dd-yyyy hh:mm");
        }

        public static void ScopeTimestamps(int scope, out DateTime startTS, out DateTime endTS)
        {
            switch (scope)
            {
                case 1: // today
                    startTS = DateTime.Today;
                    endTS = startTS.AddDays(1);
                    break;
                case 2: // past week
                    endTS = DateTime.Today.AddDays(1);
                    startTS = endTS.AddDays(-7);
                    break;
                case 3: // this month
                    startTS = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                    endTS = startTS.AddMonths(1);
                    break;
                case 4: // past month
                    startTS = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-1);
                    endTS = startTS.AddMonths(1);
                    break;
                default: // all time
                    startTS = new DateTime(2000, 1, 1);
                    endTS = new DateTime(2100, 1, 1);
                    break;

            }
        }
    }
}