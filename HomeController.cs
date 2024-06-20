using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DGHCM.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult HomeIndex()
        {
            return View();
        }

        public ActionResult About()
        {
            

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult Contact1()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}