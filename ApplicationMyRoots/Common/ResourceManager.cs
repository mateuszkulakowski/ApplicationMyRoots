using ApplicationMyRoots.DAL;
using ApplicationMyRoots.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApplicationMyRoots.Common
{
    public class ResourceManager
    {
        private static int defaultLanguageID = 1;


        public static int LoggedUserLanguageID { get {
                if (LoggedUser != null)
                    return LoggedUser.LanguageID == null ? defaultLanguageID : (int)LoggedUser.LanguageID;
                else return defaultLanguageID;
            } }


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
                if (value != null)
                {
                    HttpContext.Current.Session["LoggedUserID"] = value.UserID;
                    HttpContext.Current.Session["LanguageID"] = value.LanguageID;
                }
                else
                {
                    HttpContext.Current.Session["LoggedUserID"] = null;
                    HttpContext.Current.Session["LanguageID"] = null;
                }
            }

        }

        public static string getElementTextInLanguage(int UniqueElementTag, int LanguageID = 1)
        {
            using (var db = new DbContext())
            {
                try
                {
                    return db.LanguageTexts.Where(lt => (lt.LanguageID == LanguageID && lt.UniqueElementTag == UniqueElementTag)).First().Text;
                }
                catch (Exception e)
                {
                    db.Errors.Add(new Error
                    {
                        Message = e.Message,
                        StackTrace = e.StackTrace,
                        DateThrow = DateTime.Now
                    });
                    db.SaveChanges();

                    return "";
                }
            }
        }
    }
}