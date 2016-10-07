/******************************************************************************
 * Filename: APIVersion.cs
 * Project:  UTC WebAPI
 * 
 * Description:
 * POCO for representing API version
******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.unitethiscity.api.Models
{
    public class APIVersion
    {
        public APIVersion()
        {
            Major = 1; Minor = 15; Patch = 0;
        }
        public int Major { get; set; }
        public int Minor { get; set; }
        public int Patch { get; set; }
    }
}