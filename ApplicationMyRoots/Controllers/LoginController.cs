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


                        ViewBag.Error = "Nie poprawne dane logowania!";
                        return View(logInUser);
                    }
                }

                return RedirectToAction("MyTree", "Home");
            }
            ViewBag.Error = "Niepoprawne dane - upewnij się, że uzupełniłeś wszyskie pola!";
            return View(logInUser);

        }

        public ActionResult Registry()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Registry(RegistryUser registryUser)
        {
            if(ModelState.IsValid)
            {
                User user = Converters.RegistryUserToUserConverter(registryUser);
                user.DateSign = DateTime.Now;
                user.DateBorn = null;

                using (var db = new DbContext())
                {
                    ResourceManager.LoggedUser = user;

                    // Sprawdzić czy nie istnieje już podny login!!
                    db.Users.Add(user);
                    db.SaveChanges();
                }

                return RedirectToAction("MyTree", "Home");
            }
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