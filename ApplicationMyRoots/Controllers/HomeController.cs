using ApplicationMyRoots.Common;
using ApplicationMyRoots.CustomAttributes;
using ApplicationMyRoots.DAL;
using ApplicationMyRoots.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace ApplicationMyRoots.Controllers
{
    public class HomeController : Controller
    {
        [AccessControl]
        [HttpPost]
        public int saveCopyNode()
        {
            try
            {
                int id = int.Parse(Request.Headers.GetValues("nodeid").First());
                int mid = int.Parse(Request.Headers.GetValues("mainuser").First());

                using (var db = new DbContext())
                {
                    UserTreeNode utn;

                    if(mid == 1) // użytkownik serwisu
                    {
                        User user = db.Users.Find(id);
                        utn = new UserTreeNode(user.UserID,user.Name, user.Surname, user.DateBorn, null, "", 1); //mainuser 1 - bo pochodzi z tabelki User, extid - tutaj wpisujemy userid
                    }
                    else
                    {
                        utn = db.UserTreeNodes.Find(id);
                        utn.ExtID = utn.UserTreeNodeID; //extid tutaj wpisujemy usertreenodeid -> i co za tym idzie mainuser na 0
                        utn.MainUser = 0;
                    }

                    return ResourceManager.setCopiedNode(utn);
                }

            }catch(Exception e)
            {
                DbContext db = new DbContext();
                db.Errors.Add(new Error() { Message = "Błąd kopiowanie węzła, metoda:saveCopyNode, controller:HomeController - " + e.Message, StackTrace = e.StackTrace, DateThrow = DateTime.Now});
                db.SaveChanges();
            }

            return 2; //nieznany błąd
        }

        [AccessControl]
        [HttpGet]
        public JsonResult getCopiedNode(int id, int id2)
        {
            try
            {
                int extid = id;
                int mainuser = id2;

                var l = ResourceManager.getCopiedNodes();

                List<UserTreeNode> k = new List<UserTreeNode>();

                foreach(var element in l)
                {
                    k.Add(new UserTreeNode(element.ExtID, element.Name, element.Surname, element.DateBorn, element.DateDead, element.AdditionalInfo, element.MainUser));
                }

                return Json(k.Where(x=>x.MainUser == mainuser && x.ExtID == extid), JsonRequestBehavior.AllowGet);

            }catch(Exception e)
            {
                DbContext db = new DbContext();
                db.Errors.Add(new Error() { Message = "Błąd pobierania skopiowanego węzła, metoda:getCopiedNode, controller:HomeController - " + e.Message, StackTrace = e.StackTrace, DateThrow = DateTime.Now });
                db.SaveChanges();
            }

            return null;
        }

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

            path = Server.MapPath("~/images/paste.svg");
            imageByteData = System.IO.File.ReadAllBytes(path);
            imageBase64Data = Convert.ToBase64String(imageByteData);
            ViewBag.PasteImage = string.Format("data:image/svg+xml;base64,{0}", imageBase64Data);


            //pobieranie rekordów do tabeli - skopiowane węzły
            string copiednodestablebody = "";
            var copiednodes = ResourceManager.getCopiedNodes();

            string chooesetext = ResourceManager.getElementTextInLanguage(187, ResourceManager.LoggedUserLanguageID);

            foreach(var copiednode in copiednodes)
            {
                copiednodestablebody += "<tr>";
                copiednodestablebody += "<td>" + copiednode.NameSurname + "</td>";
                copiednodestablebody += "<td style='text-align:center;'><button class='btn btn-default' onclick='onClickChooseNodeToPaste("+copiednode.ExtID+","+copiednode.MainUser+")'>"+ chooesetext + " <span class='glyphicon glyphicon-ok'></span></button></td>";
                copiednodestablebody += "</tr>";
            }

            ViewBag.copiednodes = copiednodestablebody;


            return View();
        }


        [AccessControl]
        public ActionResult Home()
        {
            ViewBag.Title = "Home";

            return View();
        }

        [AccessControl]
        public ActionResult StatisticsBornMonth()
        {
            ViewBag.Title = "StatisticsBorn";

            return View();
        }

        [AccessControl]
        public ActionResult StatisticsAgeRange()
        {
            ViewBag.Title = "StatisticsAge";

            return View();
        }


        [AccessControl]
        public ActionResult ExchangeTrees()
        {
            ViewBag.Title = "ExchangeTrees";
            string userswithagree = "";

            //pobieranie rekordów do tabeli - WYSŁANE CZEKAJĄCE NA AKCEPTACJE
            using (var client = new WebClient())
            {
                client.BaseAddress = ResourceManager.getUsersWithAgreeAPIURL;
                client.Headers.Add("senderid", ResourceManager.LoggedUser.UserID.ToString());

                userswithagree = client.UploadString(ResourceManager.getUsersWithAgreeAPIURL, "");
            }
            //utf-8 -- polske znaki nie są dobrze odbierane z webapi
            byte[] bytes = Encoding.Default.GetBytes(userswithagree);
            userswithagree = Encoding.UTF8.GetString(bytes);

            userswithagree = userswithagree.Substring(1, userswithagree.Length - 2);
            // -------------------

            ViewBag.UsersWithAgree = userswithagree;
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
