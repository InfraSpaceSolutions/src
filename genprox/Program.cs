/******************************************************************
 *  Filename : PROGRAM.CS
 *  Project  : GENPROX.EXE
 *  
 *  )|( Sanctuary Software Studio
 *  Copyright (c) 2013 - All rights reserved.
 *  
 *  Description :
 *  Commmand line program for performing a simple test of push 
 *  notifications using the PushSharp library
 *  
 *  Usage:      GENPROX.EXE
 *  Example:    GENPROX.EXE
 *  
 ******************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Threading;
using System.Device.Location;

namespace com.unitethiscity
{
    class Program
    {
        protected static LogConsole log;
        protected static WebDBContext db;
        protected static int proxWindowMin;
        protected static int proxWindowMax;
        protected static int proxDwellMin;
        protected static double proxRangeKm;
        protected static int proxActiveStartHr;
        protected static int proxActiveStopHr;
        protected static bool proxDebug;
        protected static string proxMessage;
        protected static DateTime proxStart;
        protected static DateTime proxEnd;
        protected static DateTime proxTime;
        protected static DateTime proxDwellCutoff;
        protected static List<VwLocationsList> locList;
        protected static int activePeriod;

        public enum PushNotificationStates
        {
            Undefined = 0,
            Queued = 1,
            Sent = 2,
            Error_Acccount = 3,
            Error_Device = 4,
            Error_Push = 5,
            Error_Other = 6
        }

        public enum PushDeviceTypes
        {
            Undefined = 0,
            IOS = 1,
            GCM = 2
        }

        static void Main(string[] args)
        {
            // create the output logfile
            log = new LogConsole();
            log.Open();

            // output the version and operational information
            // output program version information
            AssemblyName assemblyName = Assembly.GetEntryAssembly().GetName();
            log.WriteLine(assemblyName.Name + " Version " + assemblyName.Version.ToString());
            log.WriteLine("Copyright (c) 2014 - Sanctuary Software Studio, Inc.");

            // create a connection to the target database
            db = new WebDBContext();

            // load our settings from the database
            proxWindowMin = WebConvert.ToInt32(SiteSettings.GetValue("ProxWindowMin"), 10);
            proxWindowMax = WebConvert.ToInt32(SiteSettings.GetValue("ProxWindowMax"), 20);
            proxDwellMin = WebConvert.ToInt32(SiteSettings.GetValue("ProxDwellMin"), 1440);
            proxRangeKm = WebConvert.ToDouble(SiteSettings.GetValue("ProxRangeKm"), 5);
            proxActiveStartHr = WebConvert.ToInt32(SiteSettings.GetValue("ProxActiveStartHr"), 8);
            proxActiveStopHr = WebConvert.ToInt32(SiteSettings.GetValue("ProxActiveStopHr"), 17);
            proxDebug = WebConvert.ToBoolean(SiteSettings.GetValue("ProxDebug"), true);
            proxMessage = WebConvert.Truncate(SiteSettings.GetValue("ProxMessage"), 120);

            // calculate the start of the proximity window by going back a number of minutes from the current time
            proxTime = DateTime.Now;
            proxStart = proxTime.AddMinutes(-proxWindowMax);
            proxEnd = proxTime.AddMinutes(-proxWindowMin);

            proxDwellCutoff = proxTime.AddMinutes(-proxDwellMin);

            // determine the active period
            activePeriod = Period.IdentifyPeriod(proxTime);

            // echo the settings
            log.WriteLine("Connection String = '" + ConfigurationManager.AppSettings["WEBDB_CONNECTIONSTRING"] + "'");
            log.WriteLine("Window = '" + proxWindowMax.ToString() +  " to " +proxWindowMin.ToString() + "' minutes, > " + proxStart.ToShortTimeString() + " and < " + proxEnd.ToShortTimeString());
            log.WriteLine("Dwell = '" + proxDwellMin.ToString() + "' minutes, < " + proxDwellCutoff.ToShortDateString() + " " + proxDwellCutoff.ToShortTimeString());
            log.WriteLine("Range = '" + proxRangeKm.ToString() + "' km");
            log.WriteLine("Active Time = '" + proxActiveStartHr.ToString() + " to  " + proxActiveStopHr.ToString() + "'");
            log.WriteLine("Debug Mode = '" + proxDebug.ToString() + "'");

            // make sure that we are in the active window for sending proximity notifications
            if ((proxTime < DateTime.Today.AddHours(proxActiveStartHr)) || (proxTime > DateTime.Today.AddHours(proxActiveStopHr)))
            {
                log.WriteLine("Outside of active time window - suppress generation of proximity notifications");
                return;
            }


            // get the list of locations for processing
            locList = db.VwLocationsList.Where(target=>target.BusEnabled == true).ToList();

            // get the collection of proximities that have occurred within the window
            List<TblProximities> proxList = db.TblProximities.Where(target => target.PrxModifyTS >= proxStart && target.PrxModifyTS <= proxEnd).ToList();
            log.WriteLine("Found '" + proxList.Count() + "' proximities in the time window to process.");
            foreach (TblProximities prox in proxList)
            {
                log.WriteLine("Processing prox #" + prox.PrxID + " for device " + prox.PutToken);
                // skip if we have sent them a notification within the dwell time
                if (db.VwPushNotifications.Count(target => target.PutToken == prox.PutToken && target.PunCreateTS >= proxDwellCutoff) > 0)
                {
                    log.WriteLine("Dwelling - token already used within dwell period");
                    continue;
                }

                // generate a notification for the device if we find unredeemed offers in range
                ProximityNotification(prox);
            }
            log.WriteLine("Processed all proximity entries");
        }

        /// <summary>
        /// Returns true if a proximity record is in range to the supplied location
        /// </summary>
        /// <param name="prox"></param>
        /// <param name="loc"></param>
        /// <returns></returns>
        static bool ProximityInRange(TblProximities prox, VwLocationsList loc)
        {
            GeoCoordinate proxCoord = new GeoCoordinate(prox.PrxLatitude, prox.PrxLongitude);
            GeoCoordinate locCoord = new GeoCoordinate(loc.LocLatitude, loc.LocLongitude);
            double dist = proxCoord.GetDistanceTo(locCoord) / 1000.0;
            return (dist < proxRangeKm);
        }


        static void ProximityNotification(TblProximities prox)
        {
            // get the account for this user
            TblPushTokens put = db.TblPushTokens.SingleOrDefault(target=>target.PutToken== prox.PutToken);
            if (put == null)
            {
                log.WriteLine("Processing prox #" + prox.PrxID.ToString() + " - no registered token found!" );
                return;
            }

            int accountId = put.AccID;

            log.WriteLine("Processing prox #" + prox.PrxID.ToString() + " for account #" + accountId.ToString());

            TblAccounts acc = db.TblAccounts.SingleOrDefault(target => target.AccID == accountId);
            if (acc == null)
            {
                log.WriteLine("Account #" + accountId.ToString() + " - not found!");
                return;
            }
            if (!acc.AccEnabled)
            {
                log.WriteLine("Account #" + accountId.ToString() + " - disabled!");
                return;
            }

            // find a location within range
            foreach (VwLocationsList loc in locList)
            {
                // if we aren't in range, keep looking
                if (!ProximityInRange(prox, loc))
                {
                    continue;
                }

                // found a location within range - lets see if it has an unredeemed deal

                // confirm location has a deal or continue
                TblDeals rsDeal = db.TblDeals.SingleOrDefault(target => target.BusID == loc.BusID && target.PerID == activePeriod);
                if (rsDeal == null)
                {
                    continue;
                }

                // check if the user has redeemed this deal
                if (db.TblRedemptions.Count(target => target.DelID == rsDeal.DelID && target.AccID == accountId) > 0)
                {
                    continue;
                }

                // we have a winner, queue a notification
                log.Write("Prox notification: ");
                log.Write("Token = '" + put.PutToken + "', ");
                log.Write("Location = #" + loc.LocID + " = '" + loc.BusName + "', ");
                log.Write("Account = #" + accountId + ", ");
                log.WriteLine("");

                if (!proxDebug)
                {
                    TblPushNotifications pushNot = new TblPushNotifications();
                    pushNot.MsgID = 0;
                    pushNot.PnsID = (int)PushNotificationStates.Queued;
                    pushNot.PutID = put.PutID;
                    pushNot.PunAlert = proxMessage;
                    pushNot.PunBadgeID = 1;
                    pushNot.PunCreateTS = DateTime.Now;
                    db.TblPushNotifications.InsertOnSubmit(pushNot);
                    db.SubmitChanges();
                    log.WriteLine("Queued notification #" + pushNot.PutID.ToString() + " for delivery");
                }
                else
                {
                    log.WriteLine("Notification not queued - operating in debug mode");
                }
                return;
            }
            log.WriteLine("No unredeemed locations found in range");
        }
    }
}
