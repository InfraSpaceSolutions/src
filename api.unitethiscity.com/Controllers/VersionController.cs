/******************************************************************************
 * Filename: VersionController.cs
 * Project:  UTC WebAPI
 * 
 * Description:
 * Controller for support identifying the api version
******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using com.unitethiscity.api.Models;

namespace com.unitethiscity.api.Controllers
{
    /// <summary>
    /// Controller for identifying the api version
    /// </summary>
    public class VersionController : ApiController
    {
        /// <summary>
        /// Read the version of the api to check for compatiblity
        /// </summary>
        /// <returns>api version model (major, minor, patch)</returns>
        public APIVersion Get()
        {
            Logger.LogAction("Version");
            return new APIVersion();
        }

    }
}
