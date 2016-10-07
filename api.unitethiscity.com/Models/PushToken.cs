/******************************************************************************
 * Filename: PushToken.cs
 * Project:  UTC WebAPI
 * 
 * Description:
 * POCO for representing model of a push notification token
******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.unitethiscity.api.Models
{
    public class PushToken
    {
        public int PutId { get; set; }
        public int AccId { get; set; }
        public int PdtId { get; set; }
        public string PutToken { get; set; }
        public bool PutEnabled { get; set; }
    }
}