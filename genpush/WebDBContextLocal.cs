﻿/******************************************************************
 *  Filename : WEBDBCONTEXTLOCAL.CS
 *  Project  : GENPUSH.EXE
 *  
 *  )|( Sanctuary Software Studio
 *  Copyright (c) 2013 - All rights reserved.
 *  
 *  Description :
 *  Access the database configured in app.config using SQLMETAL
 ******************************************************************/
using System.Configuration;

namespace com.unitethiscity
{
    /// <summary>
    /// Local implementation of WebDBContext class additions not generated by SQLMETAL
    /// </summary>
    public partial class WebDBContext : System.Data.Linq.DataContext
    {
        public WebDBContext()
            : this(ConfigurationManager.AppSettings["WEBDB_CONNECTIONSTRING"])
        {
        }
    }
}