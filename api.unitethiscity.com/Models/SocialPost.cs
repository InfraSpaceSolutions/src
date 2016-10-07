/******************************************************************************
 * Filename: SocialPost.cs
 * Project:  UTC WebAPI
 * 
 * Description:
 * POCO for representing model of a social networking post
******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.unitethiscity.api.Models
{
    public class SocialPost
    {
        public int AccId { get; set; }
        public int SptId { get; set; }
        public int BusId { get; set; }
    }
}