/******************************************************************************
 * Filename: HomeController.cs
 * Project:  UTC WebAPI
 * 
 * Description:
 * Display the webapi web interface
******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace com.unitethiscity.api.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}
