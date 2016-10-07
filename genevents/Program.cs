/******************************************************************
 *  Filename : PROGRAM.CS
 *  Project  : GENEVENTS.EXE
 *  
 *  )|( Sanctuary Software Studio
 *  Copyright (c) 2013 - All rights reserved.
 *  
 *  Description :
 *  Commmand line program for generating events in the UTC database
 *  based on the currently active recurring events
 *  
 *  Usage:      GENEVENTS.EXE
 *  Example:    GENEVENTS.EXE
 *  
 ******************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace com.unitethiscity
{
    class Program
    {
        protected static LogConsole log;
        protected static WebDBContext db;
        protected static DateTime runDate;

        /// <summary>
        /// Program entry point
        /// </summary>
        /// <param name="args">command line arguments</param>
        static int Main(string[] args)
        {
            // create the output logfile
            log = new LogConsole();
            log.Open();

            // output the version and operational information
            // output program version information
            AssemblyName assemblyName = Assembly.GetEntryAssembly().GetName();
            log.WriteLine(assemblyName.Name + " Version " + assemblyName.Version.ToString());
            log.WriteLine("Copyright (c) 2013 - Sanctuary Software Studio, Inc.");

            // create a connection to the target database
            db = new WebDBContext();

            // work with current date
            runDate = DateTime.Now;

            // process the deal information for each business in the database
            log.WriteLine(String.Format("Processing {0} event jobs.", db.TblEventJobs.Count()));
            IEnumerable<VwEventJobs> rsEvj = db.VwEventJobs.OrderBy(target => target.EvjID);

            // analyze and process each recurring event
            foreach (VwEventJobs evj in rsEvj)
            {
                log.Write(evj.EvjID.ToString("0000 ") + evj.EvjName + ": ");
                ProcessEventJob(evj);
            }
            log.Close();
            return 0;
        }

        /// <summary>
        /// Create events based on the event job to support recurring events
        /// </summary>
        /// <param name="rsEvj">target event job</param>
        public static void ProcessEventJob(VwEventJobs rsEvj)
        {
            // confirm that the event job is enabled
            if (rsEvj.EvjEnabled == false)
            {
                log.WriteLine("DISABLED - event job is not enabled");
                return;
            }

            // discard event jobs that ended in the past
            if (rsEvj.EvjStopDate < runDate)
            {
                log.WriteLine("EXPIRED - event job ends in the past");
                return;
            }
            log.WriteLine("PROCESSING - event job is active");
            switch (rsEvj.EjtID)
            {
                case 1: // DAILY
                    ProcessEventJobDaily(rsEvj);
                    break;
                case 2: // WEEKLY
                    ProcessEventJobWeekly(rsEvj);
                    break;
                case 3: // MONTHLY
                    ProcessEventJobMonthly(rsEvj);
                    break;
                default:
                    log.WriteLine(".... Warning - job type " + rsEvj.EjtID + " is undefined or not supported.");
                    break;
            }
        }

        /// <summary>
        /// Generate events as needed for a daily job
        /// </summary>
        /// <param name="rsEvj"></param>
        public static void ProcessEventJobDaily(VwEventJobs rsEvj)
        {
            // start with the begin date
            DateTime dt = rsEvj.EvjBeginDate;

            if (rsEvj.EvjInterval < 1)
            {
                log.WriteLine(".... ERROR - Invalid daily interval =" + rsEvj.EvjInterval);
                return;
            }

            // iterate through days by the interval
            for (; dt < rsEvj.EvjStopDate; dt = dt.AddDays(rsEvj.EvjInterval))
            {
                // day needs to be on or before the stop date
                if (dt > rsEvj.EvjStopDate)
                {
                    log.WriteLine(".... Skip " + dt.ToShortDateString() + ": Event is beyond the stopping point");
                    continue;
                }

                if (MatchingEventExists(rsEvj.EvjID, dt) == 0)
                {
                    // no event exists yet, lets see if we should create one
                    TimeSpan daysTo = dt - runDate;
                    // don't generate events in the past
                    if (daysTo.Days < 0)
                    {
                        log.WriteLine(".... Skip " + dt.ToShortDateString() + ": Event is in the past =" + daysTo.Days.ToString());
                        continue;
                    }

                    // don't generate events out of the publication range
                    if (daysTo.Days > rsEvj.EvjDaysPublished)
                    {
                        log.WriteLine(".... Skip " + dt.ToShortDateString() + ": Event is too far in the future " + daysTo.Days.ToString());
                        continue;
                    }

                    // create the event
                    TblEvents rsEvt = new TblEvents();
                    rsEvt.EvjID = rsEvj.EvjID;
                    rsEvt.EttID = rsEvj.EttID;
                    rsEvt.BusID = rsEvj.BusID;
                    rsEvt.EvtStartDate = dt;
                    // set the end date based on the duration in days, limiting to 1 day
                    rsEvt.EvtEndDate = dt.AddDays(Math.Max(rsEvj.EvjDuration, 1) - 1);
                    rsEvt.EvtSummary = rsEvj.EvjSummary;
                    rsEvt.EvtBody = rsEvj.EvjBody;
                    db.TblEvents.InsertOnSubmit(rsEvt);
                    db.SubmitChanges();
                    log.WriteLine(".... Created Event #" + rsEvt.EvtID + " on " + dt.ToShortDateString());
                }
                else
                {
                    log.WriteLine(".... Matching event already exists for " + dt.ToShortDateString());
                    continue;
                }
            }
        }

        /// <summary>
        /// Generate events as needed for a weekly job
        /// </summary>
        /// <param name="rsEvj"></param>
        public static void ProcessEventJobWeekly(VwEventJobs rsEvj)
        {
            // confirm that the interval correlates to a day of the week
            if ((rsEvj.EvjInterval < 0) || (rsEvj.EvjInterval > 6))
            {
                log.WriteLine(".... ERROR - Invalid weekly interval =" + rsEvj.EvjInterval);
                return;
            }


            // iterate through time on a weekly basis
            DateTime dt = rsEvj.EvjBeginDate;
            // move to the first target day of the week on or after the begin date
            int dtdow = (int)dt.DayOfWeek;
            dt = dt.AddDays(rsEvj.EvjInterval - dtdow + (( dtdow <= rsEvj.EvjInterval ) ? 0 : 7));

            // iterate through weeks, creating events
            for (; dt < rsEvj.EvjStopDate; dt = dt.AddDays(7) )
            {
                if (MatchingEventExists(rsEvj.EvjID, dt) == 0)
                {
                    // no event exists yet, lets see if we should create one
                    TimeSpan daysTo = dt - runDate;
                    // don't generate events in the past
                    if (daysTo.Days < 0)
                    {
                        log.WriteLine(".... Skip " + dt.ToShortDateString() +  ": Event is in the past =" + daysTo.Days.ToString());
                        continue;
                    }

                    // don't generate events out of the publication range
                    if (daysTo.Days > rsEvj.EvjDaysPublished)
                    {
                        log.WriteLine(".... Skip " + dt.ToShortDateString() + ": Event is too far in the future " + daysTo.Days.ToString());
                        continue;
                    }

                    // create the event
                    TblEvents rsEvt = new TblEvents();
                    rsEvt.EvjID = rsEvj.EvjID;
                    rsEvt.EttID = rsEvj.EttID;
                    rsEvt.BusID = rsEvj.BusID;
                    rsEvt.EvtStartDate = dt;
                    // set the end date based on the duration in days, limiting to 1 day
                    rsEvt.EvtEndDate = dt.AddDays(Math.Max(rsEvj.EvjDuration, 1) - 1);
                    rsEvt.EvtSummary = rsEvj.EvjSummary;
                    rsEvt.EvtBody = rsEvj.EvjBody;
                    db.TblEvents.InsertOnSubmit(rsEvt);
                    db.SubmitChanges();
                    log.WriteLine(".... Created Event #" + rsEvt.EvtID + " on " + dt.ToShortDateString());
                }
                else
                {
                    log.WriteLine(".... Matching event already exists for " + dt.ToShortDateString());
                    continue;
                }
            }
        }

        /// <summary>
        /// Generate events as needed for a monthly job
        /// </summary>
        /// <param name="rsEvj"></param>
        public static void ProcessEventJobMonthly(VwEventJobs rsEvj)
        {
            // iterate on the first of the month
            DateTime fom = new DateTime(rsEvj.EvjBeginDate.Year, rsEvj.EvjBeginDate.Month, 1);
            // iterate through months, creating events
            for (; fom < rsEvj.EvjStopDate; fom = fom.AddMonths(1))
            {
                DateTime dt = fom.AddDays(rsEvj.EvjInterval - 1);
                // day needs to stay in the same month for processing
                if (dt.Month != fom.Month)
                {
                    log.WriteLine(".... Skip " + dt.ToShortDateString() + ": is not in month =" + fom.ToShortDateString());
                    continue;
                }
                // day needs to be on after the begin date
                if (dt < rsEvj.EvjBeginDate)
                {
                    log.WriteLine(".... Skip " + dt.ToShortDateString() + ": Event is before the starting point");
                    continue;
                }

                // day needs to be on or before the stop date
                if (dt > rsEvj.EvjStopDate)
                {
                    log.WriteLine(".... Skip " + dt.ToShortDateString() + ": Event is beyond the stopping point");
                    continue;
                }

                if (MatchingEventExists(rsEvj.EvjID, dt) == 0)
                {
                    // no event exists yet, lets see if we should create one
                    TimeSpan daysTo = dt - runDate;
                    // don't generate events in the past
                    if (daysTo.Days < 0)
                    {
                        log.WriteLine(".... Skip " + dt.ToShortDateString() + ": Event is in the past =" + daysTo.Days.ToString());
                        continue;
                    }

                    // don't generate events out of the publication range
                    if (daysTo.Days > rsEvj.EvjDaysPublished)
                    {
                        log.WriteLine(".... Skip " + dt.ToShortDateString() + ": Event is too far in the future " + daysTo.Days.ToString());
                        continue;
                    }

                    // create the event
                    TblEvents rsEvt = new TblEvents();
                    rsEvt.EvjID = rsEvj.EvjID;
                    rsEvt.EttID = rsEvj.EttID;
                    rsEvt.BusID = rsEvj.BusID;
                    rsEvt.EvtStartDate = dt;
                    // set the end date based on the duration in days, limiting to 1 day
                    rsEvt.EvtEndDate = dt.AddDays(Math.Max(rsEvj.EvjDuration, 1) - 1);
                    rsEvt.EvtSummary = rsEvj.EvjSummary;
                    rsEvt.EvtBody = rsEvj.EvjBody;
                    db.TblEvents.InsertOnSubmit(rsEvt);
                    db.SubmitChanges();
                    log.WriteLine(".... Created Event #" + rsEvt.EvtID + " on " + dt.ToShortDateString());
                }
                else
                {
                    log.WriteLine(".... Matching event already exists for " + dt.ToShortDateString());
                    continue;
                }
            }
        }

        /// <summary>
        /// Check to see if a matching event already exists for this job and start date
        /// </summary>
        /// <param name="evjid">identify job</param>
        /// <param name="evtStartDate">start date of event</param>
        /// <returns>identify event or 0 if doesn't exist</returns>
        public static int MatchingEventExists(int evjid, DateTime evtStartDate)
        {
            VwEvents rsEvt = db.VwEvents.SingleOrDefault(target => target.EvjID == evjid && target.EvtStartDate == evtStartDate);
            return (rsEvt != null) ? rsEvt.EvtID : 0;
        }
    }
}
