/******************************************************************************
 * Filename: InboxMessage.cs
 * Project:  UTC WebAPI
 * 
 * Description:
 * POCO for representing model of a message in an account inbox.
******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.unitethiscity.api.Models
{
    public class InboxMessage
    {
        public int Id { get; set; }
        public int MsgId { get; set; }
        public int ToAccId { get; set; }
        public bool InbRead { get; set; }
        public int FromAccID { get; set; }
        public string FromName { get; set; }
        public string Summary { get; set; }
        public string Body { get; set; }
        public DateTime MsgTS { get; set; }
        public string MsgTSAsStr { get; set; }
        public DateTime MsgExpires { get; set; }
        public string MsgExpiresAsStr { get; set; }
        public Guid BusGuid { get; set; }
    }
}