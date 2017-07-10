using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVCBackendIliyan.Models;

namespace MVCBackendIliyan.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                ViewBag.Message = message;
            }
            return View();
        }
    }
}