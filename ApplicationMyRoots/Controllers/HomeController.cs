using ApplicationMyRoots.Common;
using ApplicationMyRoots.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ApplicationMyRoots.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult MyTree()
        {
            if(ResourceManager.LoggedUser != null)
            {
                ViewBag.Title = "My Tree";
                return View();
            }
            else
            {
                return RedirectToAction("LogIn", "Login");
            }



        }
    }
}
