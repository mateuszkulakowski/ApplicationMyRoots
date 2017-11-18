using ApplicationMyRoots.Common;
using ApplicationMyRoots.CustomAttributes;
using ApplicationMyRoots.DAL;
using ApplicationMyRoots.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ApplicationMyRoots.Controllers
{
    [AccessControl]
    public class MyAccountController : Controller
    {
        [HttpGet]
        public ActionResult SettingsSharingAgreementReceived()
        {
            string sharingfrommetable = "<tbody>";
            string sharingfrommewaitingtable = "<tbody>";

            try
            {
                using (var db = new DbContext())
                {
                    var receivedagreements = db.UserTreeSharingAgreements.Where(x => x.UserRecivingID == ResourceManager.LoggedUser.UserID &&
                                                                              x.Accpeted == true).ToList();

                    var visibletext = db.LanguageTexts.Where(x => x.UniqueElementTag == 137 && x.LanguageID == ResourceManager.LoggedUserLanguageID).First().Text;
                    var invisibletext = db.LanguageTexts.Where(x => x.UniqueElementTag == 138 && x.LanguageID == ResourceManager.LoggedUserLanguageID).First().Text;
                    var deleteagreementtext = db.LanguageTexts.Where(x => x.UniqueElementTag == 142 && x.LanguageID == ResourceManager.LoggedUserLanguageID).First().Text;

                    foreach (var receivedagreement in receivedagreements)
                    {
                        sharingfrommetable += "<tr>";
                        sharingfrommetable +=       "<td>" + receivedagreement.UserTreeSharingAgreementID + "</td>";
                        sharingfrommetable +=       "<td>" + receivedagreement.UserSending.Name + "</td>";
                        sharingfrommetable +=       "<td>" + receivedagreement.UserSending.Surname + "</td>";
                        if (receivedagreement.Visible != null && receivedagreement.Visible == true)
                            sharingfrommetable +=   "<td style=\"text-align:center;\"><button class=\"btn btn-default\">"+visibletext+" <span class=\"glyphicon glyphicon-eye-open\" style=\"color:blue;\"></span></button></td>";
                        else
                            sharingfrommetable +=   "<td style=\"text-align:center;\"><button class=\"btn btn-default\">" + invisibletext + " <span class=\"glyphicon glyphicon-eye-close\" style=\"color:gray;\"></span></button></td>";
                        sharingfrommetable +=       "<td><button class=\"btn btn-danger\">"+deleteagreementtext+" <span class=\"glyphicon glyphicon-remove\" style=\"color:red;\"></span></button></td>";
                        sharingfrommetable += "</tr>";
                    }

                    var receivedagreementswaitings = db.UserTreeSharingAgreements.Where(x => x.UserRecivingID == ResourceManager.LoggedUser.UserID && x.Accpeted == null).OrderByDescending(x => x.Date).ToList();

                    var accepttext = db.LanguageTexts.Where(x => x.UniqueElementTag == 143 && x.LanguageID == ResourceManager.LoggedUserLanguageID).First().Text;
                    var discardtext = db.LanguageTexts.Where(x => x.UniqueElementTag == 144 && x.LanguageID == ResourceManager.LoggedUserLanguageID).First().Text;

                    foreach (var receivedagreementswaiting in receivedagreementswaitings)
                    {
                        sharingfrommewaitingtable += "<tr>";
                        sharingfrommewaitingtable +=        "<td>" + receivedagreementswaiting.UserTreeSharingAgreementID + "</td>";
                        sharingfrommewaitingtable +=        "<td>" + receivedagreementswaiting.UserSending.Name + "</td>";
                        sharingfrommewaitingtable +=        "<td>" + receivedagreementswaiting.UserSending.Surname + "</td>";
                        sharingfrommewaitingtable +=        "<td style=\"text-align:center;\"><button class=\"btn btn-success\">"+accepttext+" <span class=\"glyphicon glyphicon-ok\" style=\"color: green;\"></span></button></td>" +
                                                            "<td style=\"text-align:center;\"><button class=\"btn btn-danger\">"+discardtext+" <span class=\"glyphicon glyphicon-remove\" style=\"color:red;\"></span></button></td>";
                        sharingfrommewaitingtable += "</tr>";
                    }

                    var lista = receivedagreementswaitings.Where(x => x.IsReceivedUserRead == false);

                    foreach (var item in lista)
                    {
                        item.IsReceivedUserRead = true;
                        item.UserSending = db.Users.Find(item.UserSendingID);
                        item.UserReceiving = db.Users.Find(item.UserRecivingID);
                        db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }

                }
            }
            catch (Exception e)
            {
                DbContext db = new DbContext();
                db.Errors.Add(new Error { DateThrow = DateTime.Now, Message = "Błąd metoda SettingsSharingAgreementSended() - MyAccountController - " + e.Message, StackTrace = e.StackTrace });
                db.SaveChanges();
            }


            sharingfrommewaitingtable += "</tbody>";
            sharingfrommetable += "</tbody>";

            ViewBag.sharingfrommetable = sharingfrommetable;
            ViewBag.sharingfrommewaitingtable = sharingfrommewaitingtable;

            return View();
        }

        [HttpGet]
        public ActionResult SettingsSharingAgreementSended()
        {
            string sharingtometable = "<tbody>";
            string sharingtomewaitingtable = "<tbody>";

            try
            {
                using (var db = new DbContext())
                {
                    var sendedagreements = db.UserTreeSharingAgreements.Where(x => x.UserSendingID == ResourceManager.LoggedUser.UserID && 
                                                                              x.Accpeted == true).ToList();

                    var visibletext = db.LanguageTexts.Where(x => x.UniqueElementTag == 137 && x.LanguageID == ResourceManager.LoggedUserLanguageID).First().Text;
                    var invisibletext = db.LanguageTexts.Where(x => x.UniqueElementTag == 138 && x.LanguageID == ResourceManager.LoggedUserLanguageID).First().Text;

                    foreach (var sendedagreement in sendedagreements)
                    {
                        sharingtometable += "<tr>";
                        sharingtometable +=     "<td>"+sendedagreement.UserTreeSharingAgreementID+"</td>";
                        sharingtometable +=     "<td>" + sendedagreement.UserReceiving.Name + "</td>";
                        sharingtometable +=     "<td>" + sendedagreement.UserReceiving.Surname + "</td>";
                        if(sendedagreement.Visible != null && sendedagreement.Visible == true)
                            sharingtometable += "<td style=\"text-align:center;\"><span style=\"color:blue;\">"+visibletext+" <span class=\"glyphicon glyphicon-eye-open\" style=\"color:blue;\"></span></span></td>";
                        else
                            sharingtometable += "<td style=\"text-align:center;\"><span style=\"color:gray;\">" + invisibletext + " <span class=\"glyphicon glyphicon-eye-close\" style=\"color:gray;\"></span></span></td>";
                        sharingtometable += "</tr>";
                    }

                    var sendedagreementswaitings = db.UserTreeSharingAgreements.Where(x => x.UserSendingID == ResourceManager.LoggedUser.UserID).OrderByDescending(x =>x.Date).ToList();

                    var waitingforacceptationtext = db.LanguageTexts.Where(x => x.UniqueElementTag == 139 && x.LanguageID == ResourceManager.LoggedUserLanguageID).First().Text;
                    var accepttext = db.LanguageTexts.Where(x => x.UniqueElementTag == 140 && x.LanguageID == ResourceManager.LoggedUserLanguageID).First().Text;
                    var discardtext = db.LanguageTexts.Where(x => x.UniqueElementTag == 141 && x.LanguageID == ResourceManager.LoggedUserLanguageID).First().Text;

                    foreach (var sendedagreementswaiting in sendedagreementswaitings)
                    {
                        sharingtomewaitingtable += "<tr>";
                        sharingtomewaitingtable +=      "<td>" + sendedagreementswaiting.UserTreeSharingAgreementID + "</td>";
                        sharingtomewaitingtable +=      "<td>" + sendedagreementswaiting.UserReceiving.Name + "</td>";
                        sharingtomewaitingtable +=      "<td>" + sendedagreementswaiting.UserReceiving.Surname + "</td>";
                        if (sendedagreementswaiting.Accpeted == null)
                            sharingtomewaitingtable += "<td style=\"text-align:center;\"><strong style=\"color:gray;\">"+waitingforacceptationtext+"</strong> <span class=\"glyphicon glyphicon-refresh\" style=\"color:gray;\"></span></td>";
                        else if (sendedagreementswaiting.Accpeted == true)
                            sharingtomewaitingtable += "<td style=\"text-align:center;\"><strong style=\"color: green;\">"+accepttext+"</strong> <span class=\"glyphicon glyphicon-ok-circle\" style=\"color:green;\"></span></td>";
                        else
                            sharingtomewaitingtable += "<td style=\"text-align:center;\"><strong style=\"color: red;\">"+discardtext+"</strong> <span class=\"glyphicon glyphicon-remove-circle\" style=\"color:red;\"></span></td>";
                        sharingtomewaitingtable += "</tr>";
                    }

                    var lista = sendedagreementswaitings.Where(x => x.IsSendedUserRead == false);

                    foreach (var item in lista)
                    {
                        item.IsSendedUserRead = true;
                        item.UserSending = db.Users.Find(item.UserSendingID);
                        item.UserReceiving = db.Users.Find(item.UserRecivingID);
                        db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }

                }

            }catch(Exception e)
            {
                DbContext db = new DbContext();
                db.Errors.Add(new Error { DateThrow = DateTime.Now, Message = "Błąd metoda SettingsSharingAgreementSended() - MyAccountController - " + e.Message, StackTrace = e.StackTrace });
                db.SaveChanges();
            }
            sharingtomewaitingtable += "</tbody>";
            sharingtometable += "</tbody>";

            ViewBag.sharingtometable = sharingtometable;
            ViewBag.sharingtomewaitingtable = sharingtomewaitingtable;
            return View();
        }


        [HttpGet]
        public ActionResult SettingsSharing()
        {
            try
            {
                using (var db = new DbContext())
                {
                    User user = db.Users.Find(ResourceManager.LoggedUser.UserID);

                    ViewBag.sharingtree = user.UserTreeSharingStatusID;
                    ViewBag.treesharingInformator = "green"; //kolor informatora sharingtree
                }
            }catch(Exception e)
            {
                DbContext db = new DbContext();
                db.Errors.Add(new Error { DateThrow = DateTime.Now, Message = "Błąd metoda SettingsSharing() - MyAccountController - udostępnianie drzewa prawdopodobnie null? - " + e.Message, StackTrace = e.StackTrace });
                db.SaveChanges();

                ViewBag.treesharingInformator = "red"; //kolor informatora sharingtree
            }

            return View();
        }

        [HttpPost]
        public ActionResult SettingsSharing(FormCollection formCollection)
        {
            try
            {
                int treesharingstatus = int.Parse(formCollection["treesharing"]);

                using (var db = new DbContext())
                {
                    int count = db.UserTreeSharingStatuses.Where(x => x.UserTreeSharingStatusID == treesharingstatus).Count();

                    if(count == 1)
                    {
                        User user = db.Users.Find(ResourceManager.LoggedUser.UserID);
                        user.UserTreeSharingStatusID = treesharingstatus;

                        db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        ViewBag.sharingtree = user.UserTreeSharingStatusID; // zmieniona dana do widoku
                        ViewBag.treesharingInformator = "green";
                    }
                    else // nie ma takiego statusu lub jakaś dziwna sytuacja
                    {
                        ViewBag.sharingtree = db.Users.Find(ResourceManager.LoggedUser.UserID).UserTreeSharingStatusID;
                        ViewBag.treesharingInformator = "red";
                        throw new Exception();
                    }
                }

                return View();

            }catch(Exception e)
            {
                DbContext db = new DbContext();
                db.Errors.Add(new Error { DateThrow = DateTime.Now, Message = "Błąd metoda SettingsSharing() MyAccountController - błąd konwersji value w checkboxie miała być intem! - " + e.Message, StackTrace = e.StackTrace });
                db.SaveChanges();

                ViewBag.sharingtree = db.Users.Find(ResourceManager.LoggedUser.UserID).UserTreeSharingStatusID;
                ViewBag.treesharingInformator = "red";
                return View();
            }
        }

        [HttpGet]
        public ActionResult Settings()
        {
            try
            {
                DbContext db = new DbContext();
                User user = db.Users.Where(u => u.UserID == ResourceManager.LoggedUser.UserID).First();
                byte[] imgByteData = user.Image;
                if (imgByteData != null)
                {
                    string imageBase64Data = Convert.ToBase64String(imgByteData);
                    string imageDataURL = string.Format("data:image/png;base64,{0}", imageBase64Data);
                    ViewBag.ImageData = imageDataURL;
                }
                else //obrazek domyślny
                {
                    string path = Server.MapPath("~/images/no_foto.png");
                    byte[] imageByteData = System.IO.File.ReadAllBytes(path);
                    string imageBase64Data = Convert.ToBase64String(imageByteData);
                    string imageDataURL = string.Format("data:image/png;base64,{0}", imageBase64Data);
                    ViewBag.ImageData = imageDataURL;
                }

                ViewBag.Name = user.Name;
                ViewBag.Surname = user.Surname;

                if (user.DateBorn != null)
                {
                    string month = user.DateBorn.Value.Month.ToString();
                    string day = user.DateBorn.Value.Day.ToString();

                    if (user.DateBorn.Value.Month <= 9)
                        month = "0" + user.DateBorn.Value.Month;
                    if (user.DateBorn.Value.Day <= 9)
                        day = "0" + user.DateBorn.Value.Day;

                    string dateBuild = user.DateBorn.Value.Year + "-" + month + "-" + day;
                    ViewBag.DateBorn = dateBuild;
                }

                ViewBag.LanguageID = user.LanguageID;
            }
            catch (Exception e)
            {
                DbContext db = new DbContext();
                db.Errors.Add(new Error { DateThrow = DateTime.Now, Message = "Błąd metoda get Settings() -"+e.Message, StackTrace = e.StackTrace });
                db.SaveChanges();
            }

            return View();
        }
        
        [HttpPost]
        public ActionResult Settings(FormCollection formCollection)
        {
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];
                bool rightChangeLanguage = false;

                if (file != null)
                {
                    MemoryStream ms = new MemoryStream();
                    file.InputStream.CopyTo(ms);
                    byte[] photoBytes = ms.ToArray();
                    string fileExtension = Path.GetExtension(file.FileName);
                    
                    try
                    {
                        DbContext db = new DbContext();
                        User user = db.Users.Where(u => u.UserID == ResourceManager.LoggedUser.UserID).First();
                        if (photoBytes.Count() != 0) // ustawianie img
                        {
                            user.Image = photoBytes;

                            if (photoBytes != null)
                            {
                                string imageBase64Data = Convert.ToBase64String(photoBytes);
                                string imageDataURL = string.Format("data:image/png;base64,{0}", imageBase64Data);
                                ViewBag.ImageData = imageDataURL;
                            }
                            else //obrazek domyślny
                            {
                                string path = Server.MapPath("~/images/no_foto.png");
                                byte[] imageByteData = System.IO.File.ReadAllBytes(path);
                                string imageBase64Data = Convert.ToBase64String(imageByteData);
                                string imageDataURL = string.Format("data:image/png;base64,{0}", imageBase64Data);
                                ViewBag.ImageData = imageDataURL;
                            }
                        }
                        else //nie chcemy wstawiać zdjęcia więc pobieramy z bazy
                        {
                            byte[] imgByteData = user.Image;
                            if (imgByteData != null)
                            {
                                string imageBase64Data = Convert.ToBase64String(imgByteData);
                                string imageDataURL = string.Format("data:image/png;base64,{0}", imageBase64Data);
                                ViewBag.ImageData = imageDataURL;
                            }
                            else //obrazek domyślny
                            {
                                string path = Server.MapPath("~/images/no_foto.png");
                                byte[] imageByteData = System.IO.File.ReadAllBytes(path);
                                string imageBase64Data = Convert.ToBase64String(imageByteData);
                                string imageDataURL = string.Format("data:image/png;base64,{0}", imageBase64Data);
                                ViewBag.ImageData = imageDataURL;
                            }
                        }
                        user.Name = formCollection["name"];
                        user.Surname = formCollection["surname"];

                        string date = formCollection["date"];

                        if (date != null && !date.Equals("")) //ustawianie daty
                        {
                            var tab = date.Split('-');
                            DateTime dt = new DateTime(int.Parse(tab[0]), int.Parse(tab[1]), int.Parse(tab[2]));
                            user.DateBorn = dt;

                            string dateBuild = tab[0] + "-" + tab[1] + "-" + tab[2];
                            ViewBag.DateBorn = dateBuild;
                        }

                        if(user.LanguageID != int.Parse(formCollection["language"]))
                        {
                            List<Language> languages = db.Languages.ToList();
                            foreach(Language l in languages)
                            {
                                if(l.LanguageID == int.Parse(formCollection["language"]))
                                {
                                    rightChangeLanguage = true;
                                }
                            }
                            if (rightChangeLanguage)
                            {
                                user.LanguageID = int.Parse(formCollection["language"]);
                            }
                                
                        }

                        db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        ViewBag.Name = user.Name;
                        ViewBag.Surname = user.Surname;
                        ViewBag.LanguageID = user.LanguageID;

                        ResourceManager.LoggedUser = user; //aktualizacja sesji
                    }
                    catch (Exception e)
                    {
                        DbContext db = new DbContext();
                        db.Errors.Add(new Error { DateThrow = DateTime.Now, Message = "Metoda Settings() - "+e.Message, StackTrace = e.StackTrace });
                        db.SaveChanges();
                    }

                    return View();
                }
            }
            return Content("Błąd krytyczny");
        }
    }
}