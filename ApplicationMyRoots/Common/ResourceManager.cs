using ApplicationMyRoots.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApplicationMyRoots.Common
{
    public class ResourceManager
    {

        public static User LoggedUser {
            get
            {
                if (HttpContext.Current.Session["LoggedUser"] != null)
                    return HttpContext.Current.Session["LoggedUser"] as User;
                else return null;
            }
            set
            {
                HttpContext.Current.Session["LoggedUser"] = value;
                if(value != null)
                    HttpContext.Current.Session["LoggedUserID"] = value.UserID;
                else
                    HttpContext.Current.Session["LoggedUserID"] = null;
            }

        }
    }
}