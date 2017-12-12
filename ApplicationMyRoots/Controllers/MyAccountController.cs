using ApplicationMyRoots.Common;
using ApplicationMyRoots.CustomAttributes;
using ApplicationMyRoots.DAL;
using ApplicationMyRoots.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
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
            string sharingfrommetable = "";
            string sharingfrommewaitingtable = "";

            try
            {
                using (var db = new DbContext())
                {
                    // -------------------
                    //pobieranie rekordów do tabeli - ODEBRANE I ZAAKCEPTOWANE ZGODY - Ci którzy widzą moje drzewo
                    using (var client = new WebClient())
                    {
                        client.BaseAddress = ResourceManager.getTableReceivedAgreementAPIURL;
                        client.Headers.Add("receiverid", ResourceManager.LoggedUser.UserID.ToString());
                        client.Headers.Add("languageid", ResourceManager.LoggedUser.LanguageID.ToString());

                        sharingfrommetable = client.UploadString(ResourceManager.getTableReceivedAgreementAPIURL, "");
                    }
                    //utf-8
                    byte[] bytes = Encoding.Default.GetBytes(sharingfrommetable);
                    sharingfrommetable = Encoding.UTF8.GetString(bytes);

                    sharingfrommetable = sharingfrommetable.Substring(1, sharingfrommetable.Length - 2); //obcinanie cudzysłowów
                    // -------------------


                    //pobieranie rekordów do tabeli - OTRZYMANE OCZEKUJĄCE ZGODY NA AKCEPTACJE
                    using (var client = new WebClient())
                    {
                        client.BaseAddress = ResourceManager.getTableReceivedAgreementWaitingAPIURL;
                        client.Headers.Add("receiverid", ResourceManager.LoggedUser.UserID.ToString());
                        client.Headers.Add("languageid", ResourceManager.LoggedUser.LanguageID.ToString());

                        sharingfrommewaitingtable = client.UploadString(ResourceManager.getTableReceivedAgreementWaitingAPIURL, "");
                    }
                    //utf-8
                    bytes = Encoding.Default.GetBytes(sharingfrommewaitingtable);
                    sharingfrommewaitingtable = Encoding.UTF8.GetString(bytes);

                    sharingfrommewaitingtable = sharingfrommewaitingtable.Substring(1, sharingfrommewaitingtable.Length - 2); //obcinanie cudzysłowów
                    // -------------------

                    //ozaczanie jako przeczytane
                    var lista = db.UserTreeSharingAgreements.Where(x => x.UserRecivingID == ResourceManager.LoggedUser.UserID && x.Accpeted == null && x.IsReceivedUserRead == false).ToList();

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
                db.Errors.Add(new Error { DateThrow = DateTime.Now, Message = "Błąd metoda SettingsSharingAgreementReceived() - MyAccountController - " + e.Message, StackTrace = e.StackTrace });
                db.SaveChanges();
            }

            ViewBag.sharingfrommetable = sharingfrommetable;
            ViewBag.sharingfrommewaitingtable = sharingfrommewaitingtable;

            return View();
        }

        [HttpGet]
        public ActionResult SettingsSharingAgreementSended()
        {
            string sharingtometable = "<tbody>";
            string sharingtomewaitingtable = "";
            string agreementshistory = "";
            string userlisttable = "<tbody>";

            try
            {
                using (var db = new DbContext())
                {
                    // -------------------
                    //pobieranie rekordów do tabeli - WYSŁANE CZEKAJĄCE NA AKCEPTACJE
                    using (var client = new WebClient())
                    {
                        client.BaseAddress = ResourceManager.getTableSendedgreementAPIURL;
                        client.Headers.Add("senderid", ResourceManager.LoggedUser.UserID.ToString());
                        client.Headers.Add("languageid", ResourceManager.LoggedUser.LanguageID.ToString());

                        sharingtometable = client.UploadString(ResourceManager.getTableSendedgreementAPIURL, "");
                    }
                    //utf-8
                    byte[] bytes = Encoding.Default.GetBytes(sharingtometable);
                    sharingtometable = Encoding.UTF8.GetString(bytes);

                    sharingtometable = sharingtometable.Substring(1, sharingtometable.Length - 2); //ucinanie cudzysłowów
                    // -------------------


                    //pobieranie rekordów do tabeli - WYSŁANE CZEKAJĄCE NA AKCEPTACJE
                    using (var client = new WebClient())
                    {
                        client.BaseAddress = ResourceManager.getTableSendedgreementWaitingAPIURL;
                        client.Headers.Add("senderid", ResourceManager.LoggedUser.UserID.ToString());
                        client.Headers.Add("languageid", ResourceManager.LoggedUser.LanguageID.ToString());
                        client.Headers.Add("count", "10");
                        client.Headers.Add("trhidden", "false");//hidden tr - na false bo pokazujemy 10 w tabelce odkrytych
                        client.Headers.Add("joinnamesurname", "false");//złączyć kolumnę imie/nazwisko w jedną td

                        sharingtomewaitingtable = client.UploadString(ResourceManager.getTableSendedgreementWaitingAPIURL, "");
                    }
                    //utf-8 -- polske znaki nie są dobrze odbierane z webapi
                    bytes = Encoding.Default.GetBytes(sharingtomewaitingtable);
                    sharingtomewaitingtable = Encoding.UTF8.GetString(bytes);

                    sharingtomewaitingtable=sharingtomewaitingtable.Substring(1, sharingtomewaitingtable.Length - 2);
                    // -------------------

                    //odznaczanie jako przeczytane
                    var lista = db.UserTreeSharingAgreements.Where(x => x.UserSendingID == ResourceManager.LoggedUser.UserID && x.IsSendedUserRead == false).ToList();

                    foreach (var item in lista)
                    {
                        item.IsSendedUserRead = true;
                        item.UserSending = db.Users.Find(item.UserSendingID);
                        item.UserReceiving = db.Users.Find(item.UserRecivingID);
                        db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }


                    // lista użytkowników do modala
                    var idsdisableduser = db.UserTreeSharingAgreements.Where(x => x.UserSendingID == ResourceManager.LoggedUser.UserID &&  //użytkownicy do których nie moge wysłąć zgody
                                                                             x.Accpeted != false).Select(x => x.UserRecivingID); //mamy ich akceptację lub oczekuje na akceptację

                    var userlist = db.Users.Where(x => x.UserID != ResourceManager.LoggedUser.UserID).ToList();
                    userlist.RemoveAll(x => idsdisableduser.Any(y => y == x.UserID));

                    var sendtext = db.LanguageTexts.Where(x => x.UniqueElementTag == 157 && x.LanguageID == ResourceManager.LoggedUserLanguageID).First().Text;


                    foreach (var user in userlist)
                    {
                        userlisttable += "<tr hidden>";
                        userlisttable += "<td class=\"idcellmodalsend\">" + user.UserID + "</td>";
                        userlisttable += "<td class=\"namecellmodalsend\">" + user.NameSurname + "</td>";
                        userlisttable += "<td style=\"text-align:center;\"><button class=\"btn btn-default\" onclick=\"sendagreement(" + user.UserID + ",this)\">" + sendtext + " <span class=\"glyphicon glyphicon-share-alt\"></span></button></td>";
                        userlisttable += "</tr>";
                    }

                    //pobieranie rekordów do tabeli - WYSŁANE CZEKAJĄCE NA AKCEPTACJE - pobieranie wszystkich do modala historia ^ na górze jest pobieranie 10 można jakoś zoptymalizować
                    using (var client = new WebClient())
                    {
                        client.BaseAddress = ResourceManager.getTableSendedgreementWaitingAPIURL;
                        client.Headers.Add("senderid", ResourceManager.LoggedUser.UserID.ToString());
                        client.Headers.Add("languageid", ResourceManager.LoggedUser.LanguageID.ToString());
                        client.Headers.Add("count", "-1"); //wszystkie
                        client.Headers.Add("trhidden" ,"true");//hidden tr - na true
                        client.Headers.Add("joinnamesurname", "true");//złączyć kolumnę imie/nazwisko w jedną td

                        agreementshistory = client.UploadString(ResourceManager.getTableSendedgreementWaitingAPIURL, "");
                    }
                    //utf-8 -- polske znaki nie są dobrze odbierane z webapi
                    bytes = Encoding.Default.GetBytes(agreementshistory);
                    agreementshistory = Encoding.UTF8.GetString(bytes);

                    agreementshistory = agreementshistory.Substring(1, agreementshistory.Length - 2); // dodaje jakiś nawias otaczający string
                    // -------------------

                }

            }
            catch (Exception e)
            {
                DbContext db = new DbContext();
                db.Errors.Add(new Error { DateThrow = DateTime.Now, Message = "Błąd metoda SettingsSharingAgreementSended() - MyAccountController - " + e.Message, StackTrace = e.StackTrace });
                db.SaveChanges();
            }
            sharingtometable += "</tbody>";
            userlisttable += "</tbody>";

            ViewBag.sharingtometable = sharingtometable; // tabelka drzewa widoczne dla mnie
            ViewBag.sharingtomewaitingtable = sharingtomewaitingtable; // oczekujące/zaakceptowane/odrzucone zgody
            ViewBag.userlisttable = userlisttable; // modal do wysyłania zgód
            ViewBag.agreementshistory = agreementshistory; //modal historia zgód
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