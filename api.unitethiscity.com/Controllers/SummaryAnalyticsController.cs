/******************************************************************************
 * Filename: SummaryStatsController.cs
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
    public class SummaryAnalyticsController : ApiController
    {
        protected WebDBContext db;

        /// <summary>
        /// Get the summary statistics for a business
        /// </summary>
        /// <param name="token">user token</param>
        /// <param name="id">1=today, 2=past week, 3=this period, 4=last period, 5=all-time; defaults to all-time</param>
        /// <returns></returns>
        public IEnumerable<SummaryAnalytics> Get(Guid token, int id)
        {
            db = new WebDBContext();

            int accID = APIToken.IdentifyAccount(db, token);
            if (accID == 0)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Access invalid.  Please login again."));
            }


            DateTime startTS = DateTime.Now;
            DateTime endTS = DateTime.Now;

            switch (id)
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

            IEnumerable<SummaryAnalytics> anals = CalculateSummaryAnalytics(startTS, endTS);
            db.Dispose();
            db = null;

            Logger.LogAction("Analytics-Summary", accID);

            return anals;
        }

        /// <summary>
        /// Get the summary statistics for a business
        /// </summary>
        /// <param name="token">user token</param>
        /// <param name="id">1=today, 2=past week, 3=this period, 4=last period, 5=all-time; defaults to all-time</param>
        /// <param name="busID">target business identifier</param>
        /// <returns></returns>
        public IEnumerable<SummaryAnalytics> Get(Guid token, int id, int busID)
        {
            db = new WebDBContext();

            int accID = APIToken.IdentifyAccount(db, token);
            if (accID == 0)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Access invalid.  Please login again."));
            }

            DateTime startTS = DateTime.Now;
            DateTime endTS = DateTime.Now;

            switch (id)
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

            IEnumerable<SummaryAnalytics> anals = CalculateSummaryAnalytics(startTS, endTS, busID);
            db.Dispose();
            db = null;

            Logger.LogAction("Analytics-Business", accID, busID);

            return anals;
        }

        protected int CountNewUsers(DateTime startTS, DateTime endTS)
        {
            return db.VwLogs.Count(target => target.LogAction.Contains("SignUp-") && target.LogTS >= startTS && target.LogTS < endTS);
        }

        protected int CountActiveUsers(DateTime startTS, DateTime endTS)
        {
            return db.VwLogs.Where(target => target.LogTS >= startTS && target.LogTS < endTS && target.AccID != 0).Select( target => target.AccID).Distinct().Count();
        }
        protected int CountActiveUsers(DateTime startTS, DateTime endTS, int busID)
        {
            return db.VwLogs.Where(target => target.LogTS >= startTS && target.LogTS < endTS && target.AccID != 0 && target.BusID == busID).Select(target => target.AccID).Distinct().Count();
        }

        protected int CountUserActions(DateTime startTS, DateTime endTS)
        {
            return db.VwLogs.Count(target => target.LogTS >= startTS && target.LogTS < endTS && target.AccID != 0);
        }

        protected int CountUserActions(DateTime startTS, DateTime endTS, int busID)
        {
            return db.VwLogs.Count(target => target.LogTS >= startTS && target.LogTS < endTS && target.AccID != 0 && target.BusID == busID);
        }

        protected int CountAllActions(DateTime startTS, DateTime endTS)
        {
            return db.VwLogs.Count(target => target.LogTS >= startTS && target.LogTS < endTS);
        }

        protected int CountAllActions(DateTime startTS, DateTime endTS, int busID)
        {
            return db.VwLogs.Count(target => target.LogTS >= startTS && target.LogTS < endTS && target.BusID == busID);
        }

        protected int CountGenderActions(DateTime startTS, DateTime endTS, string gender)
        {
            return db.VwLogs.Count(target => target.LogTS >= startTS && target.LogTS < endTS && target.AccID != 0 && target.AccGender == gender);
        }

        protected int CountGenderActions(DateTime startTS, DateTime endTS, string gender, int busID)
        {
            return db.VwLogs.Count(target => target.LogTS >= startTS && target.LogTS < endTS && target.AccID != 0 && target.AccGender == gender && target.BusID == busID);
        }

        protected int CountAgeGroupActions(DateTime startTS, DateTime endTS, DateTime startBirthdate, DateTime endBirthdate)
        {
            return db.VwLogs.Count(target => target.LogTS >= startTS && target.LogTS < endTS && target.AccID != 0 && target.AccBirthdate >= startBirthdate && target.AccBirthdate <= endBirthdate);
        }

        protected int CountAgeGroupActions(DateTime startTS, DateTime endTS, DateTime startBirthdate, DateTime endBirthdate, int busID)
        {
            return db.VwLogs.Count(target => target.LogTS >= startTS && target.LogTS < endTS && target.AccID != 0 && target.AccBirthdate >= startBirthdate && target.AccBirthdate <= endBirthdate && target.BusID == busID);
        }

        protected int CountTimeOfDayActions(DateTime startTS, DateTime endTS, int startHour, int endHour)
        {
            return db.VwLogs.Count(target => target.LogTS >= startTS && target.LogTS < endTS && target.LogHour >= startHour && target.LogHour <= endHour);
        }

        protected int CountTimeOfDayActions(DateTime startTS, DateTime endTS, int startHour, int endHour, int busID)
        {
            return db.VwLogs.Count(target => target.LogTS >= startTS && target.LogTS < endTS && target.LogHour >= startHour && target.LogHour <= endHour && target.BusID == busID);
        }

        protected int CountDeviceActions(DateTime startTS, DateTime endTS, string deviceType)
        {
            return db.TblLogs.Count(target => target.LogTS >= startTS && target.LogTS < endTS && target.LogDevice== deviceType);
        }

        protected int CountDeviceActions(DateTime startTS, DateTime endTS, string deviceType, int busID)
        {
            return db.TblLogs.Count(target => target.LogTS >= startTS && target.LogTS < endTS && target.LogDevice == deviceType && target.BusID == busID);
        }

        protected IEnumerable<SummaryAnalytics> CalculateSummaryAnalytics(DateTime startTS, DateTime endTS)
        {
            double denominator = 1.0;

            List<SummaryAnalytics> anals = new List<SummaryAnalytics>();

            SummaryAnalytics calc;

            // total redemptions
            int allRedemptions = db.VwRedemptions.Count(target => target.RedTS >= startTS && target.RedTS < endTS);
            calc = new SummaryAnalytics();
            denominator = db.VwRedemptions.Count();
            calc.Name = "Social Redemptions";
            calc.Total = db.VwRedemptions.Count(target => target.RedTS >= startTS && target.RedTS < endTS);
            calc.Percent = calc.Total / denominator;
            calc.Group = 0;
            anals.Add(calc);

            // number of user accounts in the system
            int allUsers = db.TblAccounts.Count();

            // new users
            calc = new SummaryAnalytics();
            denominator = (allUsers == 0) ? 1.0 : (double)(allUsers);
            calc.Name = "New Users";
            calc.Total = CountNewUsers(startTS, endTS);
            calc.Percent = calc.Total / denominator;
            calc.Group = 1;
            anals.Add(calc);

            // active users
            calc = new SummaryAnalytics();
            denominator = (allUsers == 0) ? 1.0 : (double)(allUsers);
            calc.Name = "Active Users";
            calc.Total = CountActiveUsers(startTS, endTS);
            calc.Percent = calc.Total / denominator;
            calc.Group = 1;
            anals.Add(calc);

            int allUserActions = CountUserActions(startTS, endTS);


            // gender male
            calc = new SummaryAnalytics();
            denominator = (allUserActions == 0) ? 1.0 : (double)(allUserActions);
            calc.Name = "Gender Male";
            calc.Total = CountGenderActions(startTS, endTS, "M");
            calc.Percent = calc.Total / denominator;
            calc.Group = 2;
            anals.Add(calc);

            // gender female
            calc = new SummaryAnalytics();
            denominator = (allUserActions == 0) ? 1.0 : (double)(allUserActions);
            calc.Name = "Gender Female";
            calc.Total = CountGenderActions(startTS, endTS, "F");
            calc.Percent = calc.Total / denominator;
            calc.Group = 2;
            anals.Add(calc);

            // gender unknown
            calc = new SummaryAnalytics();
            denominator = (allUserActions == 0) ? 1.0 : (double)(allUserActions);
            calc.Name = "Gender Unspecified";
            calc.Total = CountGenderActions(startTS, endTS, "?");
            calc.Percent = calc.Total / denominator;
            calc.Group = 2;
            anals.Add(calc);

            int allActions = CountAllActions(startTS, endTS);

            // device IOS
            calc = new SummaryAnalytics();
            denominator = (allActions == 0) ? 1.0 : (double)(allActions);
            calc.Name = "Device IOS";
            calc.Total = CountDeviceActions(startTS, endTS, "IOS");
            calc.Percent = calc.Total / denominator;
            calc.Group = 3;
            anals.Add(calc);

            // device Android
            calc = new SummaryAnalytics();
            denominator = (allActions == 0) ? 1.0 : (double)(allActions);
            calc.Name = "Device Android";
            calc.Total = CountDeviceActions(startTS, endTS, "ANDROID");
            calc.Percent = calc.Total / denominator;
            calc.Group = 3;
            anals.Add(calc);

            // device Other
            calc = new SummaryAnalytics();
            denominator = (allActions == 0) ? 1.0 : (double)(allActions);
            calc.Name = "Device Other";
            calc.Total = CountDeviceActions(startTS, endTS, "?");
            calc.Percent = calc.Total / denominator;
            calc.Group = 3;
            anals.Add(calc);

            // time of day 0-5
            calc = new SummaryAnalytics();
            denominator = (allActions == 0) ? 1.0 : (double)(allActions);
            calc.Name = "Time 00:00-05:59";
            calc.Total = CountTimeOfDayActions(startTS, endTS, 0, 5);
            calc.Percent = calc.Total / denominator;
            calc.Group = 4;
            anals.Add(calc);

            // time of day 6-12
            calc = new SummaryAnalytics();
            denominator = (allActions == 0) ? 1.0 : (double)(allActions);
            calc.Name = "Time 06:00-11:59";
            calc.Total = CountTimeOfDayActions(startTS, endTS, 6, 11);
            calc.Percent = calc.Total / denominator;
            calc.Group = 4;
            anals.Add(calc);

            // time of day 12-18
            calc = new SummaryAnalytics();
            denominator = (allActions == 0) ? 1.0 : (double)(allActions);
            calc.Name = "Time 12:00-17:59";
            calc.Total = CountTimeOfDayActions(startTS, endTS, 12, 17);
            calc.Percent = calc.Total / denominator;
            calc.Group = 4;
            anals.Add(calc);

            // time of day 18-24
            calc = new SummaryAnalytics();
            denominator = (allActions == 0) ? 1.0 : (double)(allActions);
            calc.Name = "Time 18:00-23:59";
            calc.Total = CountTimeOfDayActions(startTS, endTS, 18, 23);
            calc.Percent = calc.Total / denominator;
            calc.Group = 4;
            anals.Add(calc);

            DateTime startBirthdate;
            DateTime endBirthdate;

            // age group unknown
            calc = new SummaryAnalytics();
            denominator = (allUserActions == 0) ? 1.0 : (double)(allUserActions);
            calc.Name = "Age < 18 or ?";
            endBirthdate = DateTime.Today;
            startBirthdate = DateTime.Today.AddYears(-18).AddDays(1);
            calc.Total = CountAgeGroupActions(startTS, endTS, startBirthdate, endBirthdate);
            calc.Percent = calc.Total / denominator;
            calc.Group = 5;
            anals.Add(calc);

            // age group 18-24
            calc = new SummaryAnalytics();
            denominator = (allUserActions == 0) ? 1.0 : (double)(allUserActions);
            calc.Name = "Age 18 to 24";
            endBirthdate = DateTime.Today.AddYears(-18);
            startBirthdate = DateTime.Today.AddYears(-25).AddDays(1);
            calc.Total = CountAgeGroupActions(startTS, endTS, startBirthdate, endBirthdate);
            calc.Percent = calc.Total / denominator;
            calc.Group = 5;
            anals.Add(calc);

            // age group 25-34
            calc = new SummaryAnalytics();
            denominator = (allUserActions == 0) ? 1.0 : (double)(allUserActions);
            calc.Name = "Age 25 to 34";
            endBirthdate = DateTime.Today.AddYears(-25);
            startBirthdate = DateTime.Today.AddYears(-35).AddDays(1);
            calc.Total = CountAgeGroupActions(startTS, endTS, startBirthdate, endBirthdate);
            calc.Percent = calc.Total / denominator;
            calc.Group = 5;
            anals.Add(calc);

            // age group 35-49
            calc = new SummaryAnalytics();
            denominator = (allUserActions == 0) ? 1.0 : (double)(allUserActions);
            calc.Name = "Age 35 to 49";
            endBirthdate = DateTime.Today.AddYears(-35);
            startBirthdate = DateTime.Today.AddYears(-50).AddDays(1);
            calc.Total = CountAgeGroupActions(startTS, endTS, startBirthdate, endBirthdate);
            calc.Percent = calc.Total / denominator;
            calc.Group = 5;
            anals.Add(calc);

            // age group 50-64
            calc = new SummaryAnalytics();
            denominator = (allUserActions == 0) ? 1.0 : (double)(allUserActions);
            calc.Name = "Age 50 to 64";
            endBirthdate = DateTime.Today.AddYears(-50);
            startBirthdate = DateTime.Today.AddYears(-65).AddDays(1);
            calc.Total = CountAgeGroupActions(startTS, endTS, startBirthdate, endBirthdate);
            calc.Percent = calc.Total / denominator;
            calc.Group = 5;
            anals.Add(calc);

            // age group 65 and up
            calc = new SummaryAnalytics();
            denominator = (allUserActions == 0) ? 1.0 : (double)(allUserActions);
            calc.Name = "Age 65 and up";
            endBirthdate = DateTime.Today.AddYears(-65);
            startBirthdate = DateTime.Today.AddYears(-100).AddDays(1);
            calc.Total = CountAgeGroupActions(startTS, endTS, startBirthdate, endBirthdate);
            calc.Percent = calc.Total / denominator;
            calc.Group = 5;
            anals.Add(calc);

            return anals;
        }
        protected IEnumerable<SummaryAnalytics> CalculateSummaryAnalytics(DateTime startTS, DateTime endTS, int busID)
        {
            double denominator = 1.0;

            List<SummaryAnalytics> anals = new List<SummaryAnalytics>();

            SummaryAnalytics calc;

            // number of user accounts in the system
            int allUsers = db.TblAccounts.Count();

            // active users
            calc = new SummaryAnalytics();
            denominator = (allUsers == 0) ? 1.0 : (double)(allUsers);
            calc.Name = "Active Users";
            calc.Total = CountActiveUsers(startTS, endTS, busID);
            calc.Percent = calc.Total / denominator;
            calc.Group = 1;
            anals.Add(calc);

            int allUserActions = CountUserActions(startTS, endTS, busID);


            // gender male
            calc = new SummaryAnalytics();
            denominator = (allUserActions == 0) ? 1.0 : (double)(allUserActions);
            calc.Name = "Gender Male";
            calc.Total = CountGenderActions(startTS, endTS, "M", busID);
            calc.Percent = calc.Total / denominator;
            calc.Group = 2;
            anals.Add(calc);

            // gender female
            calc = new SummaryAnalytics();
            denominator = (allUserActions == 0) ? 1.0 : (double)(allUserActions);
            calc.Name = "Gender Female";
            calc.Total = CountGenderActions(startTS, endTS, "F", busID);
            calc.Percent = calc.Total / denominator;
            calc.Group = 2;
            anals.Add(calc);

            // gender unknown
            calc = new SummaryAnalytics();
            denominator = (allUserActions == 0) ? 1.0 : (double)(allUserActions);
            calc.Name = "Gender Unspecified";
            calc.Total = CountGenderActions(startTS, endTS, "?", busID);
            calc.Percent = calc.Total / denominator;
            calc.Group = 2;
            anals.Add(calc);

            int allActions = CountAllActions(startTS, endTS, busID);

            // device IOS
            calc = new SummaryAnalytics();
            denominator = (allActions == 0) ? 1.0 : (double)(allActions);
            calc.Name = "Device IOS";
            calc.Total = CountDeviceActions(startTS, endTS, "IOS", busID);
            calc.Percent = calc.Total / denominator;
            calc.Group = 3;
            anals.Add(calc);

            // device Android
            calc = new SummaryAnalytics();
            denominator = (allActions == 0) ? 1.0 : (double)(allActions);
            calc.Name = "Device Android";
            calc.Total = CountDeviceActions(startTS, endTS, "ANDROID", busID);
            calc.Percent = calc.Total / denominator;
            calc.Group = 3;
            anals.Add(calc);

            // device Other
            calc = new SummaryAnalytics();
            denominator = (allActions == 0) ? 1.0 : (double)(allActions);
            calc.Name = "Device Other";
            calc.Total = CountDeviceActions(startTS, endTS, "?", busID);
            calc.Percent = calc.Total / denominator;
            calc.Group = 3;
            anals.Add(calc);

            // time of day 0-5
            calc = new SummaryAnalytics();
            denominator = (allActions == 0) ? 1.0 : (double)(allActions);
            calc.Name = "Time 00:00-05:59";
            calc.Total = CountTimeOfDayActions(startTS, endTS, 0, 5, busID);
            calc.Percent = calc.Total / denominator;
            calc.Group = 4;
            anals.Add(calc);

            // time of day 6-12
            calc = new SummaryAnalytics();
            denominator = (allActions == 0) ? 1.0 : (double)(allActions);
            calc.Name = "Time 06:00-11:59";
            calc.Total = CountTimeOfDayActions(startTS, endTS, 6, 11, busID);
            calc.Percent = calc.Total / denominator;
            calc.Group = 4;
            anals.Add(calc);

            // time of day 12-18
            calc = new SummaryAnalytics();
            denominator = (allActions == 0) ? 1.0 : (double)(allActions);
            calc.Name = "Time 12:00-17:59";
            calc.Total = CountTimeOfDayActions(startTS, endTS, 12, 17, busID);
            calc.Percent = calc.Total / denominator;
            calc.Group = 4;
            anals.Add(calc);

            // time of day 18-24
            calc = new SummaryAnalytics();
            denominator = (allActions == 0) ? 1.0 : (double)(allActions);
            calc.Name = "Time 18:00-23:59";
            calc.Total = CountTimeOfDayActions(startTS, endTS, 18, 23, busID);
            calc.Percent = calc.Total / denominator;
            calc.Group = 4;
            anals.Add(calc);

            DateTime startBirthdate;
            DateTime endBirthdate;

            // age group unknown
            calc = new SummaryAnalytics();
            denominator = (allUserActions == 0) ? 1.0 : (double)(allUserActions);
            calc.Name = "Age < 18 or ?";
            endBirthdate = DateTime.Today;
            startBirthdate = DateTime.Today.AddYears(-18).AddDays(1);
            calc.Total = CountAgeGroupActions(startTS, endTS, startBirthdate, endBirthdate, busID);
            calc.Percent = calc.Total / denominator;
            calc.Group = 5;
            anals.Add(calc);

            // age group 18-24
            calc = new SummaryAnalytics();
            denominator = (allUserActions == 0) ? 1.0 : (double)(allUserActions);
            calc.Name = "Age 18 to 24";
            endBirthdate = DateTime.Today.AddYears(-18);
            startBirthdate = DateTime.Today.AddYears(-25).AddDays(1);
            calc.Total = CountAgeGroupActions(startTS, endTS, startBirthdate, endBirthdate, busID);
            calc.Percent = calc.Total / denominator;
            calc.Group = 5;
            anals.Add(calc);

            // age group 25-34
            calc = new SummaryAnalytics();
            denominator = (allUserActions == 0) ? 1.0 : (double)(allUserActions);
            calc.Name = "Age 25 to 34";
            endBirthdate = DateTime.Today.AddYears(-25);
            startBirthdate = DateTime.Today.AddYears(-35).AddDays(1);
            calc.Total = CountAgeGroupActions(startTS, endTS, startBirthdate, endBirthdate, busID);
            calc.Percent = calc.Total / denominator;
            calc.Group = 5;
            anals.Add(calc);

            // age group 35-49
            calc = new SummaryAnalytics();
            denominator = (allUserActions == 0) ? 1.0 : (double)(allUserActions);
            calc.Name = "Age 35 to 49";
            endBirthdate = DateTime.Today.AddYears(-35);
            startBirthdate = DateTime.Today.AddYears(-50).AddDays(1);
            calc.Total = CountAgeGroupActions(startTS, endTS, startBirthdate, endBirthdate, busID);
            calc.Percent = calc.Total / denominator;
            calc.Group = 5;
            anals.Add(calc);

            // age group 50-64
            calc = new SummaryAnalytics();
            denominator = (allUserActions == 0) ? 1.0 : (double)(allUserActions);
            calc.Name = "Age 50 to 64";
            endBirthdate = DateTime.Today.AddYears(-50);
            startBirthdate = DateTime.Today.AddYears(-65).AddDays(1);
            calc.Total = CountAgeGroupActions(startTS, endTS, startBirthdate, endBirthdate, busID);
            calc.Percent = calc.Total / denominator;
            calc.Group = 5;
            anals.Add(calc);

            // age group 65 and up
            calc = new SummaryAnalytics();
            denominator = (allUserActions == 0) ? 1.0 : (double)(allUserActions);
            calc.Name = "Age 65 and up";
            endBirthdate = DateTime.Today.AddYears(-65);
            startBirthdate = DateTime.Today.AddYears(-100).AddDays(1);
            calc.Total = CountAgeGroupActions(startTS, endTS, startBirthdate, endBirthdate, busID);
            calc.Percent = calc.Total / denominator;
            calc.Group = 5;
            anals.Add(calc);

            return anals;
        }
    }
}
