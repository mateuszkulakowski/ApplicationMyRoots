using ApplicationMyRoots.Common;
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
        public ActionResult LogIn()
        {
            if (ResourceManager.LoggedUser != null) return RedirectToAction("MyTree", "Home");
            else return View();
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
                    catch(Exception e) // uzupełnić wpis o niepoprawnym logoaniu w bazie!
                    {
                        ViewBag.Error = "Nie ma takiego użytkownika!";
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

                    db.Users.Add(user);
                    db.SaveChanges();
                }

                return RedirectToAction("MyTree", "Home");
            }
            return View(registryUser);
        }

        public ActionResult LogOut()
        {
            ResourceManager.LoggedUser = null;

            return View();
        }
    }
}