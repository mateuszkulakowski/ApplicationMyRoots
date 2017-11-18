using ApplicationMyRoots.Common;
using ApplicationMyRoots.CustomAttributes;
using ApplicationMyRoots.DAL;
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
        [AccessControl]
        public ActionResult MyTree()
        {
            ViewBag.Title = "My Tree";
            ViewBag.LoggedUser = ResourceManager.LoggedUser.UserID;
            ViewBag.LanguageID = ResourceManager.LoggedUser.LanguageID;

            string path = Server.MapPath("~/images/no_foto.png");
            byte[] imageByteData = System.IO.File.ReadAllBytes(path);
            string imageBase64Data = Convert.ToBase64String(imageByteData);
            string imageDataURL = string.Format("data:image/png;base64,{0}", imageBase64Data);
            ViewBag.DefaultImageData = imageDataURL;

            return View();
        }


        [AccessControl]
        public ActionResult Home()
        {
            ViewBag.Title = "Home";

            return View();
        }

        [AccessControl]
        public ActionResult Statistics()
        {
            ViewBag.Title = "Statistics";

            return View();
        }


        [AccessControl]
        public ActionResult Events()
        {
            ViewBag.Title = "Events";

            return View();
        }

        [AccessControl]
        public ActionResult Photos()
        {
            ViewBag.Title = "Photos";

            ViewBag.LID = ResourceManager.LoggedUser.LanguageID;
            ViewBag.UID = ResourceManager.LoggedUser.UserID;
            return View();
        }
    }
}
