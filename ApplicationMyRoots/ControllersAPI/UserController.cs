using ApplicationMyRoots.DAL;
using ApplicationMyRoots.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Hosting;
using System.Web.Http;

namespace ApplicationMyRoots.ControllersAPI
{
    public class UserController : ApiController
    {
        //mainuser decyduje czy bierzemy z tabeli user czy usertreenodes
       [HttpGet]
       public string getUserImage(string id, string mainUser)
        {
            string img = null;
            bool correct = false;
            bool correctConvert = false;
            //wartości skonwertowane na int
            int? idc = null, mainUserc = null;

            try
            {
                try
                {
                    idc = int.Parse(id);
                    mainUserc = int.Parse(mainUser);
                    correctConvert = true;

                }catch(Exception e)
                {
                    DbContext db = new DbContext();
                    db.Errors.Add(new Error { DateThrow = DateTime.Now, Message = "Błąd przy konwersji dla mainuser(" + mainUser + ") id(" + id + ") - " + e.Message, StackTrace = e.StackTrace });
                    db.SaveChanges();

                    correct = true;
                }

                if (correctConvert)
                {
                    if (mainUserc == 0) // bierzemy img z tabeli usertreenodes
                    {
                        correct = true;

                        DbContext db = new DbContext();
                        UserTreeNode user = db.UserTreeNodes.Where(u => u.UserTreeNodeID == idc).First();
                        byte[] imgByteData = user.Image;
                        if (imgByteData != null)
                        {
                            string imageBase64Data = Convert.ToBase64String(imgByteData);
                            img = string.Format("data:image/png;base64,{0}", imageBase64Data);
                        }
                    }
                    else if (mainUserc == 1) // bierzemy z tabelki user
                    {
                        correct = true;

                        DbContext db = new DbContext();
                        User user = db.Users.Where(u => u.UserID == idc).First();
                        byte[] imgByteData = user.Image;
                        if (imgByteData != null)
                        {
                            string imageBase64Data = Convert.ToBase64String(imgByteData);
                            img = string.Format("data:image/png;base64,{0}", imageBase64Data);
                        }
                    }
                    else // error mainuser nie moze byc różny od 0/1
                    {
                        using (var database = new DbContext())
                        {
                            database.Errors.Add(new Error { DateThrow = DateTime.Now, Message = "Flaga mainUser nie może być różna od 0/1 (UserController - getUserImage())" });
                            database.SaveChanges();
                        }
                    }
                }

                if(correct && img == null) // obrazek domyślny
                {
                    string path = HostingEnvironment.MapPath("~/images/no_foto.png");
                    byte[] imageByteData = System.IO.File.ReadAllBytes(path);
                    string imageBase64Data = Convert.ToBase64String(imageByteData);
                    img = string.Format("data:image/png;base64,{0}", imageBase64Data);
                }
            }
            catch (Exception e)
            {
                DbContext db = new DbContext();
                db.Errors.Add(new Error { DateThrow = DateTime.Now, Message = "Błąd przy pobieraniu obrazka dla mainuser("+mainUser+") id("+id+") - " + e.Message, StackTrace = e.StackTrace });
                db.SaveChanges();

                //domyślny obrazek
                string path = HostingEnvironment.MapPath("~/images/no_foto.png");
                byte[] imageByteData = System.IO.File.ReadAllBytes(path);
                string imageBase64Data = Convert.ToBase64String(imageByteData);
                img = string.Format("data:image/png;base64,{0}", imageBase64Data);
            }

            return img;
        }
    }
}