/******************************************************************
 *  Filename : PROGRAM.CS
 *  Project  : GENMESSAGES.EXE
 *  
 *  )|( Sanctuary Software Studio
 *  Copyright (c) 2013 - All rights reserved.
 *  
 *  Description :
 *  Commmand line program for generating messages based on the 
 *  currently defined message jobs
 *  
 *  Usage:      GENMESSAGES.EXE
 *  Example:    GENMESSAGES.EXE
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
        protected static int daysToExpire;
        protected static DateTime expDate;
        protected static int daysForActive;

        public enum MessageJobStates
        {
            Definition = 1,
            Queued = 2,
            Sending = 3,
            Sent = 4,
            Canceled = 5
        }

        public enum AccountRoles
        {
            Administrator = 1,
            Member = 2,
            Business = 3,
            Charity = 4,
            SalesRep = 5,
            Associate = 6
        }

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

            // display the run time
            log.WriteLine( "Message processing time is " + runDate.ToShortDateString() + " " + runDate.ToShortTimeString());

            // get the expiration time
            daysToExpire = WebConvert.ToInt32(SiteSettings.GetValue("DaysToExpireMessages"), 31);
            expDate = runDate.AddDays(daysToExpire);
            log.LogWriteLine("New message expiration time is " + expDate.ToShortDateString() + " " + expDate.ToShortTimeString());

            // get the activity time
            daysForActive = WebConvert.ToInt32(SiteSettings.GetValue("DaysForActiveMessages"), 60); 

            // output some overall information for debugging
            log.WriteLine(String.Format("Found {0} message jobs in the system.", db.TblMessageJobs.Count()));
            log.WriteLine(String.Format("Processing {0} message jobs queued for processing.", db.TblMessageJobs.Where(target=>target.MjsID == (int)MessageJobStates.Queued).Count()));

            IEnumerable<VwMessageJobs> rsMsj = db.VwMessageJobs.Where(target=>target.MjsID == (int)MessageJobStates.Queued).OrderBy(target => target.MsjID);

            // analyze and process each recurring event
            foreach (VwMessageJobs msj in rsMsj)
            {
                log.Write(msj.MsjID.ToString("0000 ") + msj.MsgSummary + ": ");
                ProcessMessageJob(msj);
            }

            // clean up and exit
            log.Close();
            return 0;
        }

        /// <summary>
        /// Perform processing on a single message job
        /// </summary>
        /// <param name="rsMsj">target message job</param>
        public static void ProcessMessageJob(VwMessageJobs rsMsj)
        {
            // confirm that the event job is queued for processing
            if (rsMsj.MjsID != (int)MessageJobStates.Queued)
            {
                log.WriteLine("Skipped - message job is not queued for processing");
                return;
            }

            // check to see if it is time to send the message yet
            if (rsMsj.MsjSendTS > runDate)
            {
                log.WriteLine("Pending - message configured to send at " + rsMsj.MsjSendTS.ToShortDateString() + " " + rsMsj.MsjSendTS.ToShortTimeString());
                return;
            }

            // ready to send
            log.Write("SENDING - ");

            // first thing - mark the state as sending so we don't re-enter and the customer cant change the message job
            TblMessageJobs msj = db.TblMessageJobs.Single(target => target.MsjID == rsMsj.MsjID);
            msj.MjsID = (int)MessageJobStates.Sending;
            // next - lets set the message timestamp based on the current time - don't use run time to spread out messages timestamps naturally
            TblMessages msg = db.TblMessages.Single(target => target.MsgID == rsMsj.MsgID);
            msg.MsgTS = DateTime.Now;
            // configure the message to expire
            msg.MsgExpires = expDate;
            // apply the job and message changes
            db.SubmitChanges();

            // build an enumerabler collection of the accounts that are to receive this message
            IEnumerable<TblAccounts> rsTo;

            // Select recipients based on the associated recipient role
            switch (rsMsj.RolID)
            {
                case (int)AccountRoles.Administrator:
                    log.Write("to all administrators - ");
                    var rsAdm = from row in db.VwAccountRoles where row.RolID == (int)AccountRoles.Administrator select row.AccID;
                    rsTo = from acc in db.TblAccounts where rsAdm.Contains(acc.AccID) select acc;
                    break;
                case (int)AccountRoles.Member:
                    if ( rsMsj.BusID != 0 )
                    {
                        // it is to a business, send it to fans and active users who haven't opted out
                        log.Write("to members per business - ");
                        // get the fans of the business
                        List<int> rsFans = db.VwFavorites.Where(target => target.BusID == rsMsj.BusID).Select(target => target.AccID).ToList();
                        log.WriteLine("Fans");
                        LogAccountList(rsFans);
                        // get a distinct list of active users for the target business
                        List<int> rsActive = db.TblLogs
                            .Where(target => target.LogTS >= DateTime.Today.AddDays(-daysForActive) && target.BusID == rsMsj.BusID && ((target.LogAction == "Social-Redeem") || (target.LogAction == "Loyalty")))
                            .Select(target => target.AccID).Distinct().ToList();
                        log.WriteLine("Actives");
                        LogAccountList(rsActive);
                        List<int> rsActiveSlop = db.TblLogs
                            .Where(target => target.LogTS >= DateTime.Today.AddDays(-daysForActive) && target.BusID == rsMsj.BusID && ((target.LogAction == "Social-Redeem") || (target.LogAction == "Loyalty")))
                            .Select(target => target.AccID).ToList();
                        List<int> rsOptOut = db.TblOptOuts.Where(target => target.BusID == rsMsj.BusID).Select(target => target.AccID).ToList();
                        log.WriteLine("OptOuts");
                        LogAccountList(rsOptOut);

                        // build the unified list for distribution
                        List<int> listTo = new List<int>();
                        // add the fans
                        listTo.AddRange(rsFans);
                        // add the actives
                        listTo = listTo.Union(rsActive).ToList();
                        // remove opt outs
                        listTo = listTo.Except(rsOptOut).ToList();
                        // create the distribution query results using the completed distribution list
                        rsTo = db.TblAccounts.Where(target => listTo.Contains(target.AccID));
                    }
                    else
                    {
                        log.Write("to all members - ");
                        var rsMbr = from row in db.VwAccountRoles where row.RolID == (int)AccountRoles.Member select row.AccID;
                        rsTo = from acc in db.TblAccounts where rsMbr.Contains(acc.AccID) select acc;
                    }
                    break;
                case (int)AccountRoles.Business:
                    if ( rsMsj.BusID != 0 )
                    {
                        // it is to a business, send it to business accounts
                        log.Write("to business per business - ");
                        var rsBus = from row in db.VwAccountRoles where (row.BusID == rsMsj.BusID && row.RolID == (int)AccountRoles.Business) select row.AccID;
                        rsTo = from acc in db.TblAccounts where rsBus.Contains(acc.AccID) select acc;
                    }
                    else
                    {
                        log.Write("to all businesses - ");
                        var rsAllBus = from row in db.VwAccountRoles where (row.RolID == (int)AccountRoles.Business) select row.AccID;
                        rsTo = from acc in db.TblAccounts where rsAllBus.Contains(acc.AccID) select acc;
                    }
                    break;
                case (int)AccountRoles.Charity:
                    log.Write("MESSAGING CHARITIES NOT CURRENTLY SUPPORTED ");
                    rsTo = null;
                    break;
                case (int)AccountRoles.SalesRep:
                    log.Write("to all sales reps - ");
                    var rsSal = from row in db.VwAccountRoles where row.RolID == (int)AccountRoles.SalesRep select row.AccID;
                    rsTo = from acc in db.TblAccounts where rsSal.Contains(acc.AccID) select acc;
                    break;
                case (int)AccountRoles.Associate:
                    if ( rsMsj.BusID != 0 )
                    {
                        // it is to a business, send it to business accounts
                        log.Write("to associates per business - ");
                        var rsAss = from row in db.VwAccountRoles where (row.BusID == rsMsj.BusID && row.RolID == (int)AccountRoles.Associate) select row.AccID;
                        rsTo = from acc in db.TblAccounts where rsAss.Contains(acc.AccID) select acc;
                    }
                    else
                    {
                        log.Write("to all associates - ");
                        var rsAllAss = from row in db.VwAccountRoles where (row.RolID == (int)AccountRoles.Associate) select row.AccID;
                        rsTo = from acc in db.TblAccounts where rsAllAss.Contains(acc.AccID) select acc;
                    }
                    break;
                default:
                    log.Write("UNKNOWN OR UNSUPPORTED MESSAGE ROLE " + rsMsj.RolID);
                    rsTo = null;
                    break;
            }

            if (rsTo == null)
            {
                log.WriteLine("No recipients found, done");
                return;
            }

            // give us a count for reference
            log.WriteLine(rsTo.Count().ToString() + " recipients");

            foreach (TblAccounts acc in rsTo)
            {
                log.Write(".... " + acc.AccEMail + " ");
                int inbid = SendInboxMessage(rsMsj.MsgID, acc.AccID);
                log.Write("(Inbox " + inbid + ") ");
                log.WriteLine("");
                EnqueuePushNotifiations(inbid);
            }

            // last thing - mark the state as sent so we don't re-enter and the customer cant change the message job
            // and we don't try this again
            msj.MjsID = (int)MessageJobStates.Sent;
            db.SubmitChanges();

        }

        /// <summary>
        /// Utility method to dump the contents of an account list for debugging
        /// </summary>
        /// <param name="accList"></param>
        public static void LogAccountList(List<int> accList)
        {
            log.WriteLine("List contains " + accList.Count() + " items");
            foreach (int accid in accList)
            {
                TblAccounts acc = db.TblAccounts.SingleOrDefault(target => target.AccID == accid);
                if (acc != null)
                {
                    log.WriteLine("Account #" + accid + "= " + acc.AccEMail);
                }
                else
                {
                    log.WriteLine("Account #" + accid + " <<NOTFOUND>>");
                }
            }
        }

        /// <summary>
        /// Send a message to an account inbox
        /// </summary>
        /// <param name="msgid">message identifier</param>
        /// <param name="accid">recipient</param>
        /// <returns>id of new message</returns>
        public static int SendInboxMessage(int msgid, int accid)
        {
            TblInboxMessages inb = new TblInboxMessages();
            inb.MsgID = msgid;
            inb.InbToAccID = accid;
            inb.InbRead = false;
            db.TblInboxMessages.InsertOnSubmit(inb);
            db.SubmitChanges();
            return inb.InbID;
        }

        /// <summary>
        /// Create any push notification requests based on the inbox message
        /// </summary>
        /// <param name="inbid">identify inbox message</param>
        public static void EnqueuePushNotifiations(int inbid)
        {
            VwInboxMessages inb = db.VwInboxMessages.SingleOrDefault(target=>target.InbID == inbid);
            if ( inb == null )
            {
                log.WriteLine("ERROR: missing inbox message for creation of notifications inbid=" + inbid);
                return;
            }
            // get a collection of all of the push tokens for this account
            IEnumerable<VwPushTokens> rsPut = db.VwPushTokens.Where(target => target.AccID == inb.InbToAccID);
            foreach (VwPushTokens put in rsPut)
            {
                log.Write("........ Push (token id " + put.PutID + ") ");
                if (!put.AccEnabled)
                {
                    log.WriteLine("Skipped - account disabled");
                    continue;
                }
                if (!put.PutEnabled)
                {
                    log.WriteLine("Skipped - token disabled");
                    continue;
                }
                TblPushNotifications pun = new TblPushNotifications();
                pun.PnsID = (int)PushNotificationStates.Queued;
                pun.PutID = put.PutID;
                pun.MsgID = inb.MsgID;
                pun.PunAlert = inb.MsgSummary;
                pun.PunBadgeID = 1;
                pun.PunCreateTS = runDate;
                pun.PunSendTS = null;
                db.TblPushNotifications.InsertOnSubmit(pun);
                db.SubmitChanges();
                log.WriteLine("Enqueued (#" + pun.PunID + ")");
            }
        }
    }
}
