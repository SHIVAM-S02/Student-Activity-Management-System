using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Student_Activity_Management_System.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Home()
        {
            return View();
        }

        public ActionResult Login()
        {
            return RedirectToAction("Login", "Login");
        }

        
        public ActionResult About()
        {
            return View();
        }

        
        public ActionResult Notice()
        {
            return View();
        }

        
        public ActionResult Contact()
        {
            return View();
        }



    }
}