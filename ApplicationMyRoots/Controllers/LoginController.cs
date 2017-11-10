using ApplicationMyRoots.Common;
using ApplicationMyRoots.CustomAttributes;
using ApplicationMyRoots.DAL;
using ApplicationMyRoots.Models;
using ApplicationMyRoots.ViewsModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ApplicationMyRoots.Controllers
{
    public class LoginController : Controller
    {
        [HttpGet]
        public ActionResult Home()
        {
            return View();
        }


        [LogInTwice]
        public ActionResult LogIn()
        {
            return View();
        }
        
        [HttpPost]
        public ActionResult LogIn(LogInUser logInUser)
        {
            if(ModelState.IsValid)
            {
                using (var db = new DbContext())
                {
                    try
                    {
                        ResourceManager.LoggedUser = db.Users.Where(u => u.Login.Equals(logInUser.Login) && u.Password.Equals(logInUser.Password)).First();
                    }
                    catch(Exception e)
                    {
                        try
                        {
                            int UserID = db.Users.Where(u => u.Login.Equals(logInUser.Login)).First().UserID;

                            db.FailedLogins.Add(new FailedLogin
                            {
                                Message = "Niepoprawne dane logowania: Użytkownik o loginie -> \"" + logInUser.Login + "\"",
                                UserID = UserID,
                                DateLogin = DateTime.Now
                            });
                            db.SaveChanges();
                        }
                        catch(Exception ex){} // tu brak takiego loginu więc nic nie odnotowujemy


                        ViewBag.Error = ResourceManager.getElementTextInLanguage(99, 1);
                        return View(logInUser);
                    }
                }

                return RedirectToAction("Home", "Home");
            }
            ViewBag.Error = ResourceManager.getElementTextInLanguage(100, 1);//"Niepoprawne dane - upewnij się, że uzupełniłeś wszyskie pola!";
            return View(logInUser);

        }

        public ActionResult Registry()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Registry(RegistryUser registryUser)
        {
            if (ModelState.IsValid && registryUser.Login != null && registryUser.Name != null && registryUser.Password != null && registryUser.Surname != null
               && registryUser.Login != "" && registryUser.Name != "" && registryUser.Password != "" && registryUser.Surname != "")
            {
                User user = Converters.RegistryUserToUserConverter(registryUser);
                user.LanguageID = 1;
                user.DateSign = DateTime.Now;
                user.DateBorn = null;

                using (var db = new DbContext())
                {
                    int existsUser = db.Users.Where(u => u.Login.ToLower() == registryUser.Login.ToLower()).Count();

                    if (existsUser == 0)
                    {
                        ResourceManager.LoggedUser = user;
                        db.Users.Add(user);
                        db.SaveChanges();

                        return RedirectToAction("MyTree", "Home");
                    }
                    else
                    {
                        ViewBag.Error = ResourceManager.getElementTextInLanguage(102, 1); ;
                        return View(registryUser);
                    }
                }
            }

            ViewBag.Error = ResourceManager.getElementTextInLanguage(101, 1); ;
            return View(registryUser);
        }

        [AccessControl]
        public ActionResult LogOut()
        {
            ResourceManager.LoggedUser = null;

            return View();
        }
    }
}