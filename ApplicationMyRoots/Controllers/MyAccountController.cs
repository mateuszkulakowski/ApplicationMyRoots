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

            }
            catch (Exception e)
            {
                DbContext db = new DbContext();
                db.Errors.Add(new Error { DateThrow = DateTime.Now, Message = "Błąd przy wstawianiu miniaturki obrazka -"+e.Message, StackTrace = e.StackTrace });
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
                if (file != null)
                {
                    MemoryStream ms = new MemoryStream();
                    file.InputStream.CopyTo(ms);
                    byte[] photoBytes = ms.ToArray();
                    if (photoBytes.Count() == 0) return Content("ok3");
                    try
                    {
                        DbContext db = new DbContext();
                        User user = db.Users.Where(u => u.UserID == ResourceManager.LoggedUser.UserID).First();
                        user.Image = photoBytes;
                        db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    } catch(Exception e)
                    {
                        DbContext db = new DbContext();
                        db.Errors.Add(new Error { DateThrow = DateTime.Now, Message = e.Message, StackTrace = e.StackTrace });
                        db.SaveChanges();
                    }
                }
            }
            return Content("ok");
        }
    }
}