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

        public static int setCopiedNode(UserTreeNode utn) // 0 - bez błędu, 1 - węzeł już był kopiowany, 2 - nieznany błąd 
        {
            try
            {
                List<UserTreeNode> copiednodes;

                if (HttpContext.Current.Session["CopiedNodes"] == null)
                {
                    copiednodes = new List<UserTreeNode>();
                    copiednodes.Add(utn);
                }
                else
                {
                    copiednodes = HttpContext.Current.Session["CopiedNodes"] as List<UserTreeNode>;
                    //extid - może być userid lub usertreenodeid w zależności od mainuser 1 - userid, 0 - usertreenodeid
                    bool alreadyexists = copiednodes.Where(x => x.ExtID == utn.ExtID && x.MainUser == utn.MainUser).Count() == 0 ? false : true;

                    if (alreadyexists)
                        return 1; //istnieje już skopiowany ten węzeł -> wychodzimy

                    copiednodes.Add(utn);
                }

                HttpContext.Current.Session["CopiedNodes"] = copiednodes;
                return 0; //bez błędu

            }
            catch(Exception e)
            {
                DbContext db = new DbContext();
                db.Errors.Add(new Error{ Message = "Błąd ResourceManager, metoda:setCopiedNode - " + e.Message, StackTrace = e.StackTrace, DateThrow = DateTime.Now});
                db.SaveChanges();
            }

            return 2; // nieznany błąd
        }

        public static List<UserTreeNode> getCopiedNodes()
        {
            try
            {
                if (HttpContext.Current.Session["CopiedNodes"] as List<UserTreeNode> != null)
                {
                    return HttpContext.Current.Session["CopiedNodes"] as List<UserTreeNode>;
                } 

            }
            catch(Exception e)
            {
                DbContext db = new DbContext();
                db.Errors.Add(new Error { Message = "Błąd ResourceManager, metoda:getCopiedNodes - " + e.Message, StackTrace = e.StackTrace, DateThrow = DateTime.Now });
                db.SaveChanges();
            }

            return new List<UserTreeNode>();
        }

        public static void clearSessionVariables()
        {
            if (HttpContext.Current.Session["CopiedNodes"] != null)
                HttpContext.Current.Session["CopiedNodes"] = null;

            if (LoggedUser != null)
                LoggedUser = null;
        }

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

                    return "<Error>";
                }
            }
        }


        //ścieżki do api
        public static string getTableSendedgreementWaitingAPIURL = "http://localhost:25450/api/Agreement/getTableSendedgreementWaiting";
        public static string getTableReceivedAgreementAPIURL = "http://localhost:25450/api/Agreement/getTableReceivedAgreement";
        public static string getTableReceivedAgreementWaitingAPIURL = "http://localhost:25450/api/Agreement/getTableReceivedAgreementWaiting";
        public static string getTableSendedgreementAPIURL = "http://localhost:25450/api/Agreement/getTableSendedgreement";
        public static string getUsersWithAgreeAPIURL = "http://localhost:25450/api/Agreement/getUsersWithAgree";



    }
}