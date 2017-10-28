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