/******************************************************************
 *  Filename : PROGRAM.CS
 *  Project  : GENPUSH.EXE
 *  
 *  )|( Sanctuary Software Studio
 *  Copyright (c) 2013 - All rights reserved.
 *  
 *  Description :
 *  Commmand line program for performing a simple test of push 
 *  notifications using the PushSharp library
 *  
 *  Usage:      GENPUSH.EXE
 *  Example:    GENPUSH.EXE
 *  
 ******************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Configuration;
using System.Text.RegularExpressions;
using PushSharp;
using PushSharp.Core;
using PushSharp.Apple;
using PushSharp.Android;

namespace com.unitethiscity
{
    class Program
    {
        protected static LogConsole log;
        protected static WebDBContext db;
        protected static PushBroker pushBroker;
        protected static bool tracing;

        protected static string push_IOS_CertPassword;
        protected static string push_IOS_CertFilename;
        protected static string push_GCM_APIKey;
        protected static bool push_IOS_UseProductionGateway;

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
            log.WriteLine("Copyright (c) 2013-2016 - Sanctuary Software Studio, Inc.");

            // create a connection to the target database
            db = new WebDBContext();

            // load our settings from the database
            push_IOS_CertPassword = SiteSettings.GetValue("Push_IOS_CertPassword");
            push_IOS_CertFilename = SiteSettings.GetValue("Push_IOS_CertFilename");
            push_IOS_UseProductionGateway = WebConvert.ToBoolean(SiteSettings.GetValue("Push_IOS_UseProductionGateway"), false);
            push_GCM_APIKey = SiteSettings.GetValue("Push_GCM_APIKey");
            tracing = WebConvert.ToBoolean(SiteSettings.GetValue("Push_Tracing"), true);

            // echo the settings
            log.WriteLine("Connection String = '" + ConfigurationManager.AppSettings["WEBDB_CONNECTIONSTRING"] + "'");
            log.WriteLine("Push_IOS_CertFilename = '" + push_IOS_CertFilename + "'");
            log.WriteLine("Push_IOS_CertPassword = '" + push_IOS_CertPassword + "'");
            log.WriteLine("Push_IOS_UseProductionGateway = '" + push_IOS_UseProductionGateway.ToString() + "'");
            log.WriteLine("Push_GCM_APIKey = '" + push_GCM_APIKey + "'");
            log.WriteLine("Push_Tracing = '" + tracing.ToString() + "'");

            //Create our push services broker
            pushBroker = new PushBroker();

            //Wire up the events for all the services that the broker registers
            pushBroker.OnNotificationSent += NotificationSent;
            pushBroker.OnChannelException += ChannelException;
            pushBroker.OnServiceException += ServiceException;
            pushBroker.OnNotificationFailed += NotificationFailed;
            pushBroker.OnDeviceSubscriptionExpired += DeviceSubscriptionExpired;
            pushBroker.OnDeviceSubscriptionChanged += DeviceSubscriptionChanged;
            pushBroker.OnChannelCreated += ChannelCreated;
            pushBroker.OnChannelDestroyed += ChannelDestroyed;

            // configure for apple push notifications
            var appleCert = File.ReadAllBytes(push_IOS_CertFilename);
            // Need to disable the certificate check while we are running 2.x of PushSharp - Apple reworked the certificates
            var appleSettings = new ApplePushChannelSettings(push_IOS_UseProductionGateway, appleCert,
                push_IOS_CertPassword, disableCertificateCheck:true);
            pushBroker.RegisterAppleService(appleSettings);

            // configure for google push notifications
            var googleSettings = new GcmPushChannelSettings(push_GCM_APIKey);
            pushBroker.RegisterGcmService(googleSettings);

            // get a list of all notifications to process
            IEnumerable<int> rsPunID = db.TblPushNotifications.Where(target => target.PnsID ==
                (int)PushNotificationStates.Queued).Select( target=>target.PunID);
            log.WriteLine("Found " + rsPunID.Count() + " notifications queued for processing");

            // process the push notifications that have been queued
            foreach (int punid in rsPunID)
            {
                SendNotification(punid);
            }
            //log.WriteLine("Waiting for feedback to run...");
            //Thread.Sleep(60000);

            //Stop and wait for the queues to drains
            log.WriteLine("Waiting for message queues to finish...");
            pushBroker.StopAllServices();
            log.WriteLine("Message queues finished.");
        }

        /// <summary>
        /// Send out the notification based on the id
        /// </summary>
        /// <param name="punid">identify push notification</param>
        static void SendNotification(int punid)
        {
            VwPushNotifications pun = db.VwPushNotifications.SingleOrDefault(target => target.PunID == punid);
            log.Write("PUN #" + punid + " ");
            if (pun == null)
            {
                log.WriteLine("ERROR: not found in view");
                CloseNotification(punid, PushNotificationStates.Error_Other);
                return;
            }

            // check for disabled accounts
            if (!pun.AccEnabled)
            {
                log.WriteLine("Account disabled, closing notification");
                CloseNotification(punid, PushNotificationStates.Error_Acccount);
                return;
            }

            // check for disabled devices
            if (!pun.PutEnabled)
            {
                log.WriteLine("Push token disabled, closing notification");
                CloseNotification(punid, PushNotificationStates.Error_Device);
                return;
            }

            // dispatch based on the device type
            switch (pun.PdtID)
            {
                case (int)PushDeviceTypes.IOS:
                    pushBroker.QueueNotification(new AppleNotification()
                                        .ForDeviceToken(pun.PutToken)
                                        .WithAlert(pun.PunAlert)
                                        .WithBadge(0));
                    CloseNotification(punid, PushNotificationStates.Sent);
                    log.WriteLine("Queued for IOS delivery");
                    break;
                case (int)PushDeviceTypes.GCM:
                    pushBroker.QueueNotification(new GcmNotification()
                                        .ForDeviceRegistrationId(pun.PutToken)
                                        .WithJson("{\"alert\":\"" + pun.PunAlert + "\",\"badge\":0}"));
                    CloseNotification(punid, PushNotificationStates.Sent);
                    log.WriteLine("Queued for GCM delivery");
                   break;
                default:
                    log.WriteLine("Unsupported type " + pun.PdtID + ", closing notification");
                    CloseNotification(punid, PushNotificationStates.Error_Push);
                    break;
            }
        }

        /// <summary>
        /// Close a push notification with the supplied state value
        /// </summary>
        /// <param name="punid">identify push notification</param>
        /// <param name="newState">new state for push notification</param>
        static void CloseNotification(int punid, PushNotificationStates newState)
        {
            TblPushNotifications pun = db.TblPushNotifications.SingleOrDefault(target => target.PunID == punid);
            if (pun == null)
            {
                log.WriteLine("ERROR: PunID #" + punid + " not found in table during update");
                return;
            }
            pun.PnsID = (int)newState;
            pun.PunSendTS = DateTime.Now;
            db.SubmitChanges();
        }

        public static string NormalizeDeviceID(string raw)
        {
            return Regex.Replace(raw.ToLower(), "^[0-9a-f]", "");
        }


        // event handlers for pushsharp broker

        static void DeviceSubscriptionChanged(object sender, string oldSubscriptionId, string newSubscriptionId, INotification notification)
        {
            if (tracing)
            {
                //Currently this event will only ever happen for Android GCM
                log.WriteLine("* Device Registration Changed:  Old-> " + oldSubscriptionId + "  New-> " + newSubscriptionId + " -> " + notification);
            }
        }
        static void NotificationSent(object sender, INotification notification)
        {
            if (tracing)
            {
                log.WriteLine("* Sent: " + sender + " -> " + notification);
            }
        }

        static void NotificationFailed(object sender, INotification notification, Exception notificationFailureException)
        {
            if (tracing)
            {
                log.WriteLine("* Failure: " + sender + " -> " + notificationFailureException.Message + " -> " + notification);
            }
        }

        static void ChannelException(object sender, IPushChannel channel, Exception exception)
        {
            if (tracing)
            {
                log.WriteLine("* Channel Exception: " + sender + " -> " + exception);
            }
        }

        static void ServiceException(object sender, Exception exception)
        {
            if (tracing)
            {
                log.WriteLine("* Channel Exception: " + sender + " -> " + exception);
            }
        }

        static void DeviceSubscriptionExpired(object sender, string expiredDeviceSubscriptionId, DateTime timestamp, INotification notification)
        {
            if (tracing)
            {
                log.WriteLine("* Device Subscription Expired: " + sender + " -> " + expiredDeviceSubscriptionId);
            }
            string expToken = NormalizeDeviceID(expiredDeviceSubscriptionId);
            // mark matching devices as unsubscribed if the supplied time is newer than modify time
            IEnumerable<TblPushTokens> rsPut = db.TblPushTokens.Where(target => target.PutToken == expToken);
            foreach (TblPushTokens put in rsPut)
            {
                if (put.PutModifyTS <= timestamp)
                {
                    put.PutEnabled = false;
                    put.PutModifyTS = timestamp;
                    log.WriteLine("Subscription expiration for '" + expToken + "' processed");
                }
                else
                {
                    log.WriteLine("Subscription expiration for '" + expToken + "' ignored - older than modified date");
                }
            }
            db.SubmitChanges();
        }

        static void ChannelDestroyed(object sender)
        {
            if (tracing)
            {
                log.WriteLine("* Channel Destroyed for: " + sender);
            }
        }

        static void ChannelCreated(object sender, IPushChannel pushChannel)
        {
            if (tracing)
            {
                log.WriteLine("* Channel Created for: " + sender);
            }
        }
    }
}
