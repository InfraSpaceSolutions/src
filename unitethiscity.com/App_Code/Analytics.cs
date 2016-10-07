using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Analytics
/// </summary>
public class Analytics
{
    protected WebDBContext db;
    public List<SummaryAnalytics> today;
    public List<SummaryAnalytics> pastWeek;
    public List<SummaryAnalytics> thisMonth;
    public List<SummaryAnalytics> lastMonth;
    public List<SummaryAnalytics> allTime;

    public Analytics()
    {
        today = new List<SummaryAnalytics>();
        pastWeek = new List<SummaryAnalytics>();
        thisMonth = new List<SummaryAnalytics>();
        lastMonth = new List<SummaryAnalytics>();
        allTime = new List<SummaryAnalytics>();
    }

    public void CalculateAll()
    {
        today = CalculateSummary(DateTime.Today, DateTime.Today.AddDays(1));
        pastWeek = CalculateSummary(DateTime.Today.AddDays(-7), DateTime.Today.AddDays(1));
        thisMonth = CalculateSummary(new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1), new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(1));
        lastMonth = CalculateSummary(new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-1), new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1));
        allTime = CalculateSummary(new DateTime(2000, 1, 1), new DateTime(2100, 1, 1));
    }

    public void CalculateAll(int busid)
    {
        today = CalculateSummary(DateTime.Today, DateTime.Today.AddDays(1), busid);
        pastWeek = CalculateSummary(DateTime.Today.AddDays(-7), DateTime.Today.AddDays(1), busid);
        thisMonth = CalculateSummary(new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1), new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(1), busid);
        lastMonth = CalculateSummary(new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-1), new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1), busid);
        allTime = CalculateSummary(new DateTime(2000, 1, 1), new DateTime(2100, 1, 1), busid);
    }


    List<SummaryAnalytics> CalculateSummary(DateTime startTS, DateTime endTS)
    {
        db = new WebDBContext();

        double denominator = 1.0;

        List<SummaryAnalytics> anals = new List<SummaryAnalytics>();

        SummaryAnalytics calc;

        // number of user accounts in the system
        int allUsers = db.TblAccounts.Count();

        // new users
        calc = new SummaryAnalytics();
        denominator = (allUsers == 0) ? 1.0 : (double)(allUsers);
        calc.Name = "New Users";
        calc.Total = CountNewUsers(startTS, endTS);
        calc.Percent = calc.Total / denominator;
        anals.Add(calc);

        // active users
        calc = new SummaryAnalytics();
        denominator = (allUsers == 0) ? 1.0 : (double)(allUsers);
        calc.Name = "Active Users";
        calc.Total = CountActiveUsers(startTS, endTS);
        calc.Percent = calc.Total / denominator;
        anals.Add(calc);

        int allUserActions = CountUserActions(startTS, endTS);

        // gender male
        calc = new SummaryAnalytics();
        denominator = (allUserActions == 0) ? 1.0 : (double)(allUserActions);
        calc.Name = "Gender Male";
        calc.Total = CountGenderActions(startTS, endTS, "M");
        calc.Percent = calc.Total / denominator;
        anals.Add(calc);

        // gender female
        calc = new SummaryAnalytics();
        denominator = (allUserActions == 0) ? 1.0 : (double)(allUserActions);
        calc.Name = "Gender Female";
        calc.Total = CountGenderActions(startTS, endTS, "F");
        calc.Percent = calc.Total / denominator;
        anals.Add(calc);

        // gender unknown
        calc = new SummaryAnalytics();
        denominator = (allUserActions == 0) ? 1.0 : (double)(allUserActions);
        calc.Name = "Gender Unspecified";
        calc.Total = CountGenderActions(startTS, endTS, "?");
        calc.Percent = calc.Total / denominator;
        anals.Add(calc);

        int allActions = CountAllActions(startTS, endTS);

        // device IOS
        calc = new SummaryAnalytics();
        denominator = (allActions == 0) ? 1.0 : (double)(allActions);
        calc.Name = "Device IOS";
        calc.Total = CountDeviceActions(startTS, endTS, "IOS");
        calc.Percent = calc.Total / denominator;
        anals.Add(calc);

        // device Android
        calc = new SummaryAnalytics();
        denominator = (allActions == 0) ? 1.0 : (double)(allActions);
        calc.Name = "Device Android";
        calc.Total = CountDeviceActions(startTS, endTS, "ANDROID");
        calc.Percent = calc.Total / denominator;
        anals.Add(calc);

        // device Other
        calc = new SummaryAnalytics();
        denominator = (allActions == 0) ? 1.0 : (double)(allActions);
        calc.Name = "Device Other";
        calc.Total = CountDeviceActions(startTS, endTS, "?");
        calc.Percent = calc.Total / denominator;
        anals.Add(calc);

        // time of day 0-5
        calc = new SummaryAnalytics();
        denominator = (allActions == 0) ? 1.0 : (double)(allActions);
        calc.Name = "Time 00:00-05:59";
        calc.Total = CountTimeOfDayActions(startTS, endTS, 0, 5);
        calc.Percent = calc.Total / denominator;
        anals.Add(calc);

        // time of day 6-12
        calc = new SummaryAnalytics();
        denominator = (allActions == 0) ? 1.0 : (double)(allActions);
        calc.Name = "Time 06:00-11:59";
        calc.Total = CountTimeOfDayActions(startTS, endTS, 6, 11);
        calc.Percent = calc.Total / denominator;
        anals.Add(calc);

        // time of day 12-18
        calc = new SummaryAnalytics();
        denominator = (allActions == 0) ? 1.0 : (double)(allActions);
        calc.Name = "Time 12:00-17:59";
        calc.Total = CountTimeOfDayActions(startTS, endTS, 12, 17);
        calc.Percent = calc.Total / denominator;
        anals.Add(calc);

        // time of day 18-24
        calc = new SummaryAnalytics();
        denominator = (allActions == 0) ? 1.0 : (double)(allActions);
        calc.Name = "Time 18:00-23:59";
        calc.Total = CountTimeOfDayActions(startTS, endTS, 18, 23);
        calc.Percent = calc.Total / denominator;
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
        anals.Add(calc);

        // age group 18-24
        calc = new SummaryAnalytics();
        denominator = (allUserActions == 0) ? 1.0 : (double)(allUserActions);
        calc.Name = "Age 18 to 24";
        endBirthdate = DateTime.Today.AddYears(-18);
        startBirthdate = DateTime.Today.AddYears(-25).AddDays(1);
        calc.Total = CountAgeGroupActions(startTS, endTS, startBirthdate, endBirthdate);
        calc.Percent = calc.Total / denominator;
        anals.Add(calc);

        // age group 25-34
        calc = new SummaryAnalytics();
        denominator = (allUserActions == 0) ? 1.0 : (double)(allUserActions);
        calc.Name = "Age 25 to 34";
        endBirthdate = DateTime.Today.AddYears(-25);
        startBirthdate = DateTime.Today.AddYears(-35).AddDays(1);
        calc.Total = CountAgeGroupActions(startTS, endTS, startBirthdate, endBirthdate);
        calc.Percent = calc.Total / denominator;
        anals.Add(calc);

        // age group 35-49
        calc = new SummaryAnalytics();
        denominator = (allUserActions == 0) ? 1.0 : (double)(allUserActions);
        calc.Name = "Age 35 to 49";
        endBirthdate = DateTime.Today.AddYears(-35);
        startBirthdate = DateTime.Today.AddYears(-50).AddDays(1);
        calc.Total = CountAgeGroupActions(startTS, endTS, startBirthdate, endBirthdate);
        calc.Percent = calc.Total / denominator;
        anals.Add(calc);

        // age group 50-64
        calc = new SummaryAnalytics();
        denominator = (allUserActions == 0) ? 1.0 : (double)(allUserActions);
        calc.Name = "Age 50 to 64";
        endBirthdate = DateTime.Today.AddYears(-50);
        startBirthdate = DateTime.Today.AddYears(-65).AddDays(1);
        calc.Total = CountAgeGroupActions(startTS, endTS, startBirthdate, endBirthdate);
        calc.Percent = calc.Total / denominator;
        anals.Add(calc);

        // age group 65 and up
        calc = new SummaryAnalytics();
        denominator = (allUserActions == 0) ? 1.0 : (double)(allUserActions);
        calc.Name = "Age 65 and up";
        endBirthdate = DateTime.Today.AddYears(-65);
        startBirthdate = DateTime.Today.AddYears(-100).AddDays(1);
        calc.Total = CountAgeGroupActions(startTS, endTS, startBirthdate, endBirthdate);
        calc.Percent = calc.Total / denominator;
        anals.Add(calc);

        db.Dispose();

        return anals;
    }

    List<SummaryAnalytics> CalculateSummary(DateTime startTS, DateTime endTS, int busid)
    {
        db = new WebDBContext();

        double denominator = 1.0;

        List<SummaryAnalytics> anals = new List<SummaryAnalytics>();

        SummaryAnalytics calc;

        // number of user accounts in the system
        int allUsers = db.TblAccounts.Count();

        // active users
        calc = new SummaryAnalytics();
        denominator = (allUsers == 0) ? 1.0 : (double)(allUsers);
        calc.Name = "Active Users";
        calc.Total = CountActiveUsers(startTS, endTS, busid);
        calc.Percent = calc.Total / denominator;
        anals.Add(calc);

        int allUserActions = CountUserActions(startTS, endTS, busid);

        // gender male
        calc = new SummaryAnalytics();
        denominator = (allUserActions == 0) ? 1.0 : (double)(allUserActions);
        calc.Name = "Gender Male";
        calc.Total = CountGenderActions(startTS, endTS, "M", busid);
        calc.Percent = calc.Total / denominator;
        anals.Add(calc);

        // gender female
        calc = new SummaryAnalytics();
        denominator = (allUserActions == 0) ? 1.0 : (double)(allUserActions);
        calc.Name = "Gender Female";
        calc.Total = CountGenderActions(startTS, endTS, "F", busid);
        calc.Percent = calc.Total / denominator;
        anals.Add(calc);

        // gender unknown
        calc = new SummaryAnalytics();
        denominator = (allUserActions == 0) ? 1.0 : (double)(allUserActions);
        calc.Name = "Gender Unspecified";
        calc.Total = CountGenderActions(startTS, endTS, "?", busid);
        calc.Percent = calc.Total / denominator;
        anals.Add(calc);

        int allActions = CountAllActions(startTS, endTS, busid);

        // device IOS
        calc = new SummaryAnalytics();
        denominator = (allActions == 0) ? 1.0 : (double)(allActions);
        calc.Name = "Device IOS";
        calc.Total = CountDeviceActions(startTS, endTS, "IOS", busid);
        calc.Percent = calc.Total / denominator;
        anals.Add(calc);

        // device Android
        calc = new SummaryAnalytics();
        denominator = (allActions == 0) ? 1.0 : (double)(allActions);
        calc.Name = "Device Android";
        calc.Total = CountDeviceActions(startTS, endTS, "ANDROID", busid);
        calc.Percent = calc.Total / denominator;
        anals.Add(calc);

        // device Other
        calc = new SummaryAnalytics();
        denominator = (allActions == 0) ? 1.0 : (double)(allActions);
        calc.Name = "Device Other";
        calc.Total = CountDeviceActions(startTS, endTS, "?", busid);
        calc.Percent = calc.Total / denominator;
        anals.Add(calc);

        // time of day 0-5
        calc = new SummaryAnalytics();
        denominator = (allActions == 0) ? 1.0 : (double)(allActions);
        calc.Name = "Time 00:00-05:59";
        calc.Total = CountTimeOfDayActions(startTS, endTS, 0, 5, busid);
        calc.Percent = calc.Total / denominator;
        anals.Add(calc);

        // time of day 6-12
        calc = new SummaryAnalytics();
        denominator = (allActions == 0) ? 1.0 : (double)(allActions);
        calc.Name = "Time 06:00-11:59";
        calc.Total = CountTimeOfDayActions(startTS, endTS, 6, 11, busid);
        calc.Percent = calc.Total / denominator;
        anals.Add(calc);

        // time of day 12-18
        calc = new SummaryAnalytics();
        denominator = (allActions == 0) ? 1.0 : (double)(allActions);
        calc.Name = "Time 12:00-17:59";
        calc.Total = CountTimeOfDayActions(startTS, endTS, 12, 17, busid);
        calc.Percent = calc.Total / denominator;
        anals.Add(calc);

        // time of day 18-24
        calc = new SummaryAnalytics();
        denominator = (allActions == 0) ? 1.0 : (double)(allActions);
        calc.Name = "Time 18:00-23:59";
        calc.Total = CountTimeOfDayActions(startTS, endTS, 18, 23, busid);
        calc.Percent = calc.Total / denominator;
        anals.Add(calc);

        DateTime startBirthdate;
        DateTime endBirthdate;

        // age group unknown
        calc = new SummaryAnalytics();
        denominator = (allUserActions == 0) ? 1.0 : (double)(allUserActions);
        calc.Name = "Age < 18 or ?";
        endBirthdate = DateTime.Today;
        startBirthdate = DateTime.Today.AddYears(-18).AddDays(1);
        calc.Total = CountAgeGroupActions(startTS, endTS, startBirthdate, endBirthdate, busid);
        calc.Percent = calc.Total / denominator;
        anals.Add(calc);

        // age group 18-24
        calc = new SummaryAnalytics();
        denominator = (allUserActions == 0) ? 1.0 : (double)(allUserActions);
        calc.Name = "Age 18 to 24";
        endBirthdate = DateTime.Today.AddYears(-18);
        startBirthdate = DateTime.Today.AddYears(-25).AddDays(1);
        calc.Total = CountAgeGroupActions(startTS, endTS, startBirthdate, endBirthdate, busid);
        calc.Percent = calc.Total / denominator;
        anals.Add(calc);

        // age group 25-34
        calc = new SummaryAnalytics();
        denominator = (allUserActions == 0) ? 1.0 : (double)(allUserActions);
        calc.Name = "Age 25 to 34";
        endBirthdate = DateTime.Today.AddYears(-25);
        startBirthdate = DateTime.Today.AddYears(-35).AddDays(1);
        calc.Total = CountAgeGroupActions(startTS, endTS, startBirthdate, endBirthdate, busid);
        calc.Percent = calc.Total / denominator;
        anals.Add(calc);

        // age group 35-49
        calc = new SummaryAnalytics();
        denominator = (allUserActions == 0) ? 1.0 : (double)(allUserActions);
        calc.Name = "Age 35 to 49";
        endBirthdate = DateTime.Today.AddYears(-35);
        startBirthdate = DateTime.Today.AddYears(-50).AddDays(1);
        calc.Total = CountAgeGroupActions(startTS, endTS, startBirthdate, endBirthdate, busid);
        calc.Percent = calc.Total / denominator;
        anals.Add(calc);

        // age group 50-64
        calc = new SummaryAnalytics();
        denominator = (allUserActions == 0) ? 1.0 : (double)(allUserActions);
        calc.Name = "Age 50 to 64";
        endBirthdate = DateTime.Today.AddYears(-50);
        startBirthdate = DateTime.Today.AddYears(-65).AddDays(1);
        calc.Total = CountAgeGroupActions(startTS, endTS, startBirthdate, endBirthdate, busid);
        calc.Percent = calc.Total / denominator;
        anals.Add(calc);

        // age group 65 and up
        calc = new SummaryAnalytics();
        denominator = (allUserActions == 0) ? 1.0 : (double)(allUserActions);
        calc.Name = "Age 65 and up";
        endBirthdate = DateTime.Today.AddYears(-65);
        startBirthdate = DateTime.Today.AddYears(-100).AddDays(1);
        calc.Total = CountAgeGroupActions(startTS, endTS, startBirthdate, endBirthdate, busid);
        calc.Percent = calc.Total / denominator;
        anals.Add(calc);

        db.Dispose();

        return anals;
    }

    protected int CountNewUsers(DateTime startTS, DateTime endTS)
    {
        return db.VwLogs.Count(target => target.LogAction.Contains("SignUp-") && target.LogTS >= startTS && target.LogTS < endTS);
    }

    protected int CountActiveUsers(DateTime startTS, DateTime endTS)
    {
        return db.VwLogs.Where(target => target.LogTS >= startTS && target.LogTS < endTS && target.AccID != 0).Select(target => target.AccID).Distinct().Count();
    }

    protected int CountActiveUsers(DateTime startTS, DateTime endTS, int busid)
    {
        return db.VwLogs.Where(target => target.LogTS >= startTS && target.LogTS < endTS && target.AccID != 0 && target.BusID == busid).Select(target => target.AccID).Distinct().Count();
    }

    protected int CountUserActions(DateTime startTS, DateTime endTS)
    {
        return db.VwLogs.Count(target => target.LogTS >= startTS && target.LogTS < endTS && target.AccID != 0);
    }
    protected int CountUserActions(DateTime startTS, DateTime endTS, int busid)
    {
        return db.VwLogs.Count(target => target.LogTS >= startTS && target.LogTS < endTS && target.AccID != 0 && target.BusID == busid);
    }

    protected int CountAllActions(DateTime startTS, DateTime endTS)
    {
        return db.VwLogs.Count(target => target.LogTS >= startTS && target.LogTS < endTS);
    }

    protected int CountAllActions(DateTime startTS, DateTime endTS, int busid)
    {
        return db.VwLogs.Count(target => target.LogTS >= startTS && target.LogTS < endTS && target.BusID == busid);
    }

    protected int CountGenderActions(DateTime startTS, DateTime endTS, string gender)
    {
        return db.VwLogs.Count(target => target.LogTS >= startTS && target.LogTS < endTS && target.AccID != 0 && target.AccGender == gender);
    }

    protected int CountGenderActions(DateTime startTS, DateTime endTS, string gender, int busid)
    {
        return db.VwLogs.Count(target => target.LogTS >= startTS && target.LogTS < endTS && target.AccID != 0 && target.AccGender == gender && target.BusID == busid);
    }

    protected int CountAgeGroupActions(DateTime startTS, DateTime endTS, DateTime startBirthdate, DateTime endBirthdate)
    {
        return db.VwLogs.Count(target => target.LogTS >= startTS && target.LogTS < endTS && target.AccID != 0 && target.AccBirthdate >= startBirthdate && target.AccBirthdate <= endBirthdate);
    }

    protected int CountAgeGroupActions(DateTime startTS, DateTime endTS, DateTime startBirthdate, DateTime endBirthdate, int busid)
    {
        return db.VwLogs.Count(target => target.LogTS >= startTS && target.LogTS < endTS && target.AccID != 0 && target.AccBirthdate >= startBirthdate && target.AccBirthdate <= endBirthdate && target.BusID == busid);
    }

    protected int CountTimeOfDayActions(DateTime startTS, DateTime endTS, int startHour, int endHour)
    {
        return db.VwLogs.Count(target => target.LogTS >= startTS && target.LogTS < endTS && target.LogHour >= startHour && target.LogHour <= endHour);
    }

    protected int CountTimeOfDayActions(DateTime startTS, DateTime endTS, int startHour, int endHour, int busid)
    {
        return db.VwLogs.Count(target => target.LogTS >= startTS && target.LogTS < endTS && target.LogHour >= startHour && target.LogHour <= endHour && target.BusID == busid);
    }

    protected int CountDeviceActions(DateTime startTS, DateTime endTS, string deviceType)
    {
        return db.TblLogs.Count(target => target.LogTS >= startTS && target.LogTS < endTS && target.LogDevice == deviceType);
    }
    protected int CountDeviceActions(DateTime startTS, DateTime endTS, string deviceType, int busid)
    {
        return db.TblLogs.Count(target => target.LogTS >= startTS && target.LogTS < endTS && target.LogDevice == deviceType && target.BusID == busid);
    }
}