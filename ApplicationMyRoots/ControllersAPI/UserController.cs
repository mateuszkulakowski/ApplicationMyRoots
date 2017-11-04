using ApplicationMyRoots.DAL;
using ApplicationMyRoots.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
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
                    db.Errors.Add(new Error { DateThrow = DateTime.Now, Message = "Błąd przy konwersji getUserImage() dla mainuser(" + mainUser + ") id(" + id + ") - " + e.Message, StackTrace = e.StackTrace });
                    db.SaveChanges();
                }

                if (correctConvert)
                {
                    if (mainUserc == 0) // bierzemy img z tabeli usertreenodes
                    {
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

                if(img == null) // obrazek domyślny
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

        [HttpGet]
        public string getEditImage()
        {
            string path = HostingEnvironment.MapPath("~/images/edit.svg");
            byte[] imageByteData = System.IO.File.ReadAllBytes(path);
            string imageBase64Data = Convert.ToBase64String(imageByteData);
            string imageDataURL = string.Format("data:image/svg+xml;base64,{0}", imageBase64Data);
            return imageDataURL;
        }

        [HttpGet]
        public string getDefaultImage()
        {
            string path = HostingEnvironment.MapPath("~/images/no_foto.png");
            byte[] imageByteData = System.IO.File.ReadAllBytes(path);
            string imageBase64Data = Convert.ToBase64String(imageByteData);
            string imageDataURL = string.Format("data:image/png;base64,{0}", imageBase64Data);
            return imageDataURL;
        }

        [HttpGet]
        public string getTrashImage()
        {
            string path = HostingEnvironment.MapPath("~/images/trash_bin.svg");
            byte[] imageByteData = System.IO.File.ReadAllBytes(path);
            string imageBase64Data = Convert.ToBase64String(imageByteData);
            string imageDataURL = string.Format("data:image/svg+xml;base64,{0}", imageBase64Data);
            return imageDataURL;
        }

        /*Metoda zwraca imię+nazwisko daty urodzenia smierci i wiek użytkownika*/
        public string getUserDataToNode(string id, string mainUser, string languageID)
        {
            string nodataText = "&lt;Brak danych&gt;";
            int? idc = null, mainUserc = null, languageIDc = null;
            bool correctConvert = false;

            try
            {
                try
                {
                    idc = int.Parse(id);
                    mainUserc = int.Parse(mainUser);
                    languageIDc = int.Parse(languageID);
                    correctConvert = true;
                    using (var db = new DbContext())
                    {
                        nodataText = db.LanguageTexts.Where(lt => lt.LanguageID == languageIDc && lt.UniqueElementTag == 42).First().Text;
                    }

                }
                catch (Exception e)
                {
                    DbContext db = new DbContext();
                    db.Errors.Add(new Error { DateThrow = DateTime.Now, Message = "Błąd przy konwersji string->int getUserDataToNode() - UserController API - mainuser(" + mainUser + ") id(" + id + ") - " + e.Message, StackTrace = e.StackTrace });
                    nodataText = db.LanguageTexts.Where(lt => lt.LanguageID == languageIDc && lt.UniqueElementTag == 42).First().Text;
                    db.SaveChanges();
                }
                string userData = "";
                if (correctConvert)
                {
                    if (mainUserc == 1)
                    {
                        using (var db = new DbContext())
                        {
                            var user = db.Users.Where(u => u.UserID == idc).First();

                            //Format: imie_naziwsko,data_urodzenia,data_śmierci,age;
                            if(user.NameSurname != null || !user.NameSurname.Equals(""))
                            {
                                userData += user.NameSurname + ",";
                            }
                            else
                            {
                                userData += nodataText + ",";
                            }

                            if (user.DateBorn != null)
                            {
                                userData += user.DateBorn.Value.Year + "-";
                                if (user.DateBorn.Value.Month <= 9) userData += "0" + user.DateBorn.Value.Month + "-";
                                else userData += user.DateBorn.Value.Month + "-";

                                if (user.DateBorn.Value.Day <= 9) userData += "0" + user.DateBorn.Value.Day + ",";
                                else userData += user.DateBorn.Value.Day + ",";
                            }
                            else
                            {
                                userData += nodataText + ",";
                            }

                            //data śmierci --- narazie brak danych
                            userData += nodataText + ",";

                            //age -- uzupełnić gdy data smierci jest
                            if(user.DateBorn != null)
                            {
                                DateTime now = DateTime.Now;

                                if (now.Year > user.DateBorn.Value.Year ||
                                   (now.Year == user.DateBorn.Value.Year && now.Month > user.DateBorn.Value.Month) ||
                                   (now.Year == user.DateBorn.Value.Year && now.Month == user.DateBorn.Value.Month && now.Day > user.DateBorn.Value.Day)) // data urodzenia musi byc w przeszłości
                                {
                                    int age = now.Year - user.DateBorn.Value.Year;

                                    if(now.Month == user.DateBorn.Value.Month && now.Day < user.DateBorn.Value.Day ||
                                       now.Month < user.DateBorn.Value.Month)
                                    {
                                        age--;
                                    }
                                    
                                    userData += age + "";
                                }
                                else // user ma date urodzenia w przyszłości
                                {
                                    db.Errors.Add(new Error { DateThrow = DateTime.Now, Message = "Podany user ma datę urodzenia w przyszłości - UserController API - mainuser(" + mainUser + ") id(" + id + ") - languageID("+ languageID +")"});
                                    db.SaveChanges();

                                    userData += nodataText;
                                }
                            }
                            else
                            {
                                userData += nodataText;
                            }
                        }
                    }
                    else if (mainUserc == 0)
                    {
                        using (var db = new DbContext())
                        {
                            var nodeUser = db.UserTreeNodes.Where(u => u.UserTreeNodeID == idc).First();

                            //Format: imie_naziwsko,data_urodzenia,data_śmierci,age;
                            if (nodeUser.NameSurname != null || !nodeUser.NameSurname.Equals(""))
                            {
                                userData += nodeUser.NameSurname + ",";
                            }
                            else
                            {
                                userData += nodataText + ",";
                            }

                            if (nodeUser.DateBorn != null)
                            {
                                userData += nodeUser.DateBorn.Value.Year + "-";
                                if (nodeUser.DateBorn.Value.Month <= 9) userData += "0" + nodeUser.DateBorn.Value.Month + "-";
                                else userData += nodeUser.DateBorn.Value.Month + "-";

                                if (nodeUser.DateBorn.Value.Day <= 9) userData += "0" + nodeUser.DateBorn.Value.Day + ",";
                                else userData += nodeUser.DateBorn.Value.Day + ",";
                            }
                            else
                            {
                                userData += nodataText + ",";
                            }

                            //data śmierci --- 
                            if (nodeUser.DateDead != null)
                            {
                                userData += nodeUser.DateDead.Value.Year + "-";
                                if (nodeUser.DateDead.Value.Month <= 9) userData += "0" + nodeUser.DateDead.Value.Month + "-";
                                else userData += nodeUser.DateDead.Value.Month + "-";

                                if (nodeUser.DateDead.Value.Day <= 9) userData += "0" + nodeUser.DateDead.Value.Day + ",";
                                else userData += nodeUser.DateDead.Value.Day + ",";
                            }
                            else
                            {
                                userData += nodataText + ",";
                            }

                            //age -- uzupełnić gdy data smierci jest 
                            if (nodeUser.DateBorn != null)
                            {
                                DateTime now;
                                if (nodeUser.DateDead == null)
                                {
                                    now = DateTime.Now;
                                }
                                else // od daty śmierci
                                {
                                    now = (DateTime)nodeUser.DateDead;
                                }

                                if (now.Year > nodeUser.DateBorn.Value.Year ||
                                    (now.Year == nodeUser.DateBorn.Value.Year && now.Month > nodeUser.DateBorn.Value.Month) ||
                                    (now.Year == nodeUser.DateBorn.Value.Year && now.Month == nodeUser.DateBorn.Value.Month && now.Day > nodeUser.DateBorn.Value.Day)) // data urodzenia musi byc w przeszłości
                                {
                                    int age = now.Year - nodeUser.DateBorn.Value.Year;

                                    if (now.Month == nodeUser.DateBorn.Value.Month && now.Day < nodeUser.DateBorn.Value.Day ||
                                        now.Month < nodeUser.DateBorn.Value.Month)
                                    {
                                        age--;
                                    }

                                    userData += age + "";
                                }
                                else // user ma date urodzenia w przyszłości
                                {
                                    db.Errors.Add(new Error { DateThrow = DateTime.Now, Message = "Podany user ma datę urodzenia w przyszłości getUserDataToNode() - UserController API - mainuser(" + mainUser + ") id(" + id + ") - languageID(" + languageID + ")" });
                                    db.SaveChanges();

                                    userData += nodataText;
                                }
                                
                            }
                            else
                            {
                                userData += nodataText;
                            }


                        }
                    }

                    return userData;
                    
                }
            }
            catch (Exception e)
            {
                DbContext db = new DbContext();
                db.Errors.Add(new Error { DateThrow = DateTime.Now, Message = "Błąd przy pobieraniu getUserDataToNode() dla mainuser(" + mainUser + ") id(" + id + ") - " + e.Message, StackTrace = e.StackTrace });
                nodataText = db.LanguageTexts.Where(lt => lt.LanguageID == languageIDc && lt.UniqueElementTag == 42).First().Text;
                db.SaveChanges();
            }

            return nodataText + "," + nodataText + "," + nodataText + "," + nodataText;
        }

        [HttpPost]
        //Metoda zwracająca id zapisanego węzła
        public async Task<string> SaveUserNode(int id)
        {
            try
            {
                using (var db = new DbContext())
                {
                    int UserTreesID = db.UserTrees.Where(ut =>ut.UserID == id).First().UserTreeID;

                    DateTime? dateborn = null;
                    DateTime? datedead = null;

                    if (this.Request.Headers.GetValues("dateborn").First() != "") //podano date born
                    {
                        var tabDateborn = this.Request.Headers.GetValues("dateborn").First().Split('-');
                        dateborn = new DateTime(int.Parse(tabDateborn[0]), int.Parse(tabDateborn[1]), int.Parse(tabDateborn[2]));
                    }
                    if (this.Request.Headers.GetValues("datedead").First() != "") //podano date death
                    {
                        var tabDatedead = this.Request.Headers.GetValues("datedead").First().Split('-');
                        datedead = new DateTime(int.Parse(tabDatedead[0]), int.Parse(tabDatedead[1]), int.Parse(tabDatedead[2]));
                    }

                    if (this.Request.Headers.GetValues("withFile").First() == "1") // dodawanie ze zdjęciem
                    {
                        // extract file name and file contents
                        var provider = new MultipartMemoryStreamProvider();
                        await this.Request.Content.ReadAsMultipartAsync(provider);
                        byte[] filebytes = await provider.Contents[0].ReadAsByteArrayAsync();

                        UserTreeNode utn = new UserTreeNode
                        {
                            Name = this.Request.Headers.GetValues("name").First(),
                            Surname = this.Request.Headers.GetValues("surname").First(),
                            DateBorn = dateborn,
                            DateDead = datedead,
                            AdditionalInfo = this.Request.Headers.GetValues("additionalinfo").First(),
                            Image = filebytes,
                            UserTreeID = UserTreesID
                        };

                        db.UserTreeNodes.Add(utn);
                        db.SaveChanges();

                        return utn.UserTreeNodeID+"";
                    }
                    else if (this.Request.Headers.GetValues("withFile").First() == "0") // bez zdjęcia
                    {
                        UserTreeNode utn = new UserTreeNode
                        {
                            Name = this.Request.Headers.GetValues("name").First(),
                            Surname = this.Request.Headers.GetValues("surname").First(),
                            DateBorn = dateborn,
                            DateDead = datedead,
                            AdditionalInfo = this.Request.Headers.GetValues("additionalinfo").First(),
                            UserTreeID = UserTreesID
                        };

                        db.UserTreeNodes.Add(utn);
                        db.SaveChanges();

                        return utn.UserTreeNodeID + "";
                    }
                    return "";
                }
            }catch(Exception e)
            {
                DbContext db = new DbContext();
                db.Errors.Add(new Error { DateThrow = DateTime.Now, Message = "Błąd przy zapisie węzła UserController metoda:saveUserNode() id(" + id + ") - " + e.Message, StackTrace = e.StackTrace });
                db.SaveChanges();
                return "";
            }
        }

        [HttpPost]
        //Metoda zwracająca id zapisanego węzła
        public async Task EditUserNode(int id)
        {
            try
            {
                using (var db = new DbContext())
                {
                    UserTreeNode utn = db.UserTreeNodes.Find(id);

                    DateTime? dateborn = null;
                    DateTime? datedead = null;

                    if (this.Request.Headers.GetValues("dateborn").First() != "") //podano date born
                    {
                        var tabDateborn = this.Request.Headers.GetValues("dateborn").First().Split('-');
                        dateborn = new DateTime(int.Parse(tabDateborn[0]), int.Parse(tabDateborn[1]), int.Parse(tabDateborn[2]));
                    }
                    if (this.Request.Headers.GetValues("datedead").First() != "") //podano date death
                    {
                        var tabDatedead = this.Request.Headers.GetValues("datedead").First().Split('-');
                        datedead = new DateTime(int.Parse(tabDatedead[0]), int.Parse(tabDatedead[1]), int.Parse(tabDatedead[2]));
                    }

                    if (this.Request.Headers.GetValues("withFile").First() == "1") // dodawanie ze zdjęciem
                    {
                        // extract file name and file contents
                        var provider = new MultipartMemoryStreamProvider();
                        await this.Request.Content.ReadAsMultipartAsync(provider);
                        byte[] filebytes = await provider.Contents[0].ReadAsByteArrayAsync();
                        utn.Image = filebytes;
                    }
                    utn.Name = this.Request.Headers.GetValues("name").First();
                    utn.Surname = this.Request.Headers.GetValues("surname").First();
                    utn.DateBorn = dateborn;
                    utn.DateDead = datedead;
                    utn.AdditionalInfo = this.Request.Headers.GetValues("additionalinfo").First();

                    db.Entry(utn).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                DbContext db = new DbContext();
                db.Errors.Add(new Error { DateThrow = DateTime.Now, Message = "Błąd przy zapisie węzła UserController metoda:editUserNode() id(" + id + ") - " + e.Message, StackTrace = e.StackTrace });
                db.SaveChanges();
            }
        }

        [HttpPost]
        //Metoda zwracająca id zapisanego węzła
        public string GetUserNode(int id)
        {
            try
            {
                using (var db = new DbContext())
                {
                    UserTreeNode utn = db.UserTreeNodes.Find(id);

                    string imageDataURL = "";
                    string dateborn = "";
                    string datedead = "";
                    string month = "";
                    string day = "";

                    if (utn.Image != null)
                    {
                        byte[] imageByteData = utn.Image;
                        string imageBase64Data = Convert.ToBase64String(imageByteData);
                        imageDataURL = string.Format("data:image/png;base64,{0}", imageBase64Data);
                    }

                    if(utn.DateBorn != null)
                    {
                        if (utn.DateBorn.Value.Month < 10)
                            month = "0" + utn.DateBorn.Value.Month;
                        else
                            month = utn.DateBorn.Value.Month.ToString();

                        if (utn.DateBorn.Value.Day < 10)
                            day = "0" + utn.DateBorn.Value.Day;
                        else
                            day = utn.DateBorn.Value.Day.ToString();

                        dateborn = utn.DateBorn.Value.Year + "-" + month + "-" + day;
                    }

                    if(utn.DateDead != null)
                    {
                        if (utn.DateDead.Value.Month < 10)
                            month = "0" + utn.DateDead.Value.Month;
                        else
                            month = utn.DateDead.Value.Month.ToString();

                        if (utn.DateDead.Value.Day < 10)
                            day = "0" + utn.DateDead.Value.Day;
                        else
                            day = utn.DateBorn.Value.Day.ToString();

                        datedead = utn.DateDead.Value.Year + "-" + month + "-" + day;
                    }

                    return utn.Name + "\\;;/" + utn.Surname + "\\;;/" + dateborn + "\\;;/" + datedead + "\\;;/" + utn.AdditionalInfo + "\\;;/" + imageDataURL;
                }
            }
            catch (Exception e)
            {
                DbContext db = new DbContext();
                db.Errors.Add(new Error { DateThrow = DateTime.Now, Message = "Błąd przy pobieraniu węzła UserController metoda:getUserNode() id(" + id + ") - " + e.Message, StackTrace = e.StackTrace });
                db.SaveChanges();
                return "";
            }
        }

        [HttpPost]
        public void RemoveUserNode()
        {
            try
            {
                int id1 = int.Parse(this.Request.Headers.GetValues("id1").First());
                int id2 = int.Parse(this.Request.Headers.GetValues("id2").First());

                if (id1 == -1 && id2 == -1) return;

                using (var db = new DbContext())
                {
                    if(id1 != -1)
                        db.UserTreeNodes.Remove(db.UserTreeNodes.Find(id1));
                    
                    if (id2 != -1)
                        db.UserTreeNodes.Remove(db.UserTreeNodes.Find(id2));

                    db.SaveChanges();

                }
            }catch(Exception e)
            {
                DbContext db = new DbContext();
                db.Errors.Add(new Error { DateThrow = DateTime.Now, Message = "Błąd przy usuwaniu węzła UserController metoda:removeUserNode() - " + e.Message, StackTrace = e.StackTrace });
                db.SaveChanges();
            }
        }
    }
}