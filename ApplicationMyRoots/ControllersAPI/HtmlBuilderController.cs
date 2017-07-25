using ApplicationMyRoots.Common;
using ApplicationMyRoots.DAL;
using ApplicationMyRoots.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace ApplicationMyRoots.Controllers
{
    public class HtmlBuilderController : ApiController
    {
        // Pierwszy raz użytkownik stworzył konto i dostaje na drzewie tylko swoją osobę
        [HttpGet]
        public string GetFirstLoggedUser(int id)
        {
            using (var db = new DbContext())
            {
                try
                {
                    User user = db.Users.Find(id);

                    return "<svg class=\"tree-elements\" id=\"" + user.UserID + "\" name=\"" + user.UserID + "\" width=\"200\" height=\"100\" x=\"0\" y=\"0\">" +
                            "<rect class=\"tree-element-frames\" width=\"200\" height=\"100\" x=\"0\" y=\"0\" fill=\"white\" stroke=\"black\" />" +
                            "<text class=\"tree-element-texts\" x=\"50%\" y=\"10%\" font-family=\"Verdana\" font-size=\"12\" fill=\"blue\" alignment-baseline=\"middle\" text-anchor=\"middle\">" + user.NameSurname + "</text>" +
                    "</svg>";

                }
                catch(Exception e) // wywala gdy nie znajdzie użytkownika o podanym id
                {
                    db.Errors.Add(new Error {
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
