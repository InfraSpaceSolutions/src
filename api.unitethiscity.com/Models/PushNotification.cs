/******************************************************************************
 * Filename: PushNotification.cs
 * Project:  UTC WebAPI
 * 
 * Description:
 * POCO for representing model of a push notification
******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.unitethiscity.api.Models
{
    public enum PushNotificationStates
    {
        Undefined = 0,
        Queued = 1,
        Sent = 2,
        ErrorAccount = 3,
        ErrorDevice = 4,
        ErrorPush = 5,
        ErrorOther = 6
    }
    public class PushNotification
    {
        public int AccId { get; set; }
        public string PutToken { get; set; }
        public string PunAlert { get; set; }
        public int PunBadgeId { get; set; }
    }
}