using ApplicationMyRoots.Common;
using ApplicationMyRoots.DAL;
using ApplicationMyRoots.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
        public string GetUserMainTree(string id)
        {
            int id_result = -1;
            try
            {
                id_result = int.Parse(id);
            }catch(Exception e){ }


            using (var db = new DAL.DbContext())
            {
                try
                {
                    User user = db.Users.Find(id_result);
                    if (user != null)
                    {
                        var userTrees = db.UserTrees.Where(ut => ut.UserID == user.UserID);

                        if (userTrees != null && userTrees.Count() > 0) // user ma już drzewo - zwracamy które ma ustawione isMainTree na true
                        {
                            return userTrees.Where(ut => ut.isMainTree == true).First().TreeHtmlCode;
                        }
                        else //pierwsze zalogowanie - użytkownik nie ma drzewa stworzonego
                        {
                            //data-have - czy dodany już ojciec/matka/partner
                            //data-mainuser - czy to ten co ma konto na stronce czy dodany jako węzeł 1-main, 0-dodany

                            string htmlTree =
                                "<g class=\"tree-elements\" id=\""+user.UserID+ "\" onmousedown=\"mousedowntreeelement(evt)\" transform=\"matrix(1 0 0 1 0 0)\" data-mainuser=\"1\">" +
                                    "<rect class=\"tree-element-frames\" width=\"200\" height=\"100\" x=\"100\" y=\"50\" fill=\"white\" stroke=\"black\"/>" +
                                    "<text class=\"tree-element-texts\" x=\"200\" y=\"65\" font-family=\"verdana\" font-size=\"12\" fill=\"blue\" alignment-baseline=\"middle\" text-anchor=\"middle\">" + user.NameSurname+"</text>"+

                                    "<rect class=\"addparents\" width=\"180\" height=\"20\" x=\"110\" y=\"30\" fill=\"white\" stroke=\"red\" visibility=\"hidden\" data-have=\"0\" onclick=\"addNewNodeToTreeClick(this)\"/>" +
                                    "<text class=\"addparents\" x=\"200\" y=\"40\" font-family=\"verdana\" font-size=\"6\" fill=\"black\" alignment-baseline=\"middle\" text-anchor=\"middle\" visibility=\"hidden\" data-tag=\"20\" data-have=\"0\"></text>" +

                                    "<rect class=\"addpartnerR\" width=\"20\" height=\"80\" x=\"300\" y=\"60\" fill=\"white\" stroke=\"red\" visibility=\"hidden\" data-have=\"0\" onclick=\"addNewNodeToTreeClick(this)\"/>" +
                                    "<text class=\"addpartnerR\" transform=\"rotate(90)\" x=\"100\" y=\"-310\" font-family=\"verdana\" font-size=\"6\" fill=\"black\" alignment-baseline=\"middle\" text-anchor=\"middle\" visibility=\"hidden\" data-tag=\"22\" data-have=\"0\"></text>" +

                                    "<rect class=\"addpartnerL\" width=\"20\" height=\"80\" x=\"80\" y=\"60\" fill=\"white\" stroke=\"red\" visibility=\"hidden\" data-have=\"0\" onclick=\"addNewNodeToTreeClick(this)\"/>" +
                                    "<text class=\"addpartnerL\" transform=\"rotate(90)\" x=\"100\" y=\"-90\" font-family=\"verdana\" font-size=\"6\" fill=\"black\" alignment-baseline=\"middle\"text-anchor=\"middle\" visibility=\"hidden\" data-tag=\"23\" data-have=\"0\"></text>" +

                                    "<rect class=\"addchildren\" width=\"180\" height=\"20\" x=\"110\" y=\"150\" fill=\"white\" stroke=\"red\" visibility=\"hidden\" data-have=\"0\" onclick=\"addNewNodeToTreeClick(this)\"/>" +
                                    "<text class=\"addchildren\" x=\"200\" y=\"160\" font-family=\"verdana\" font-size=\"6\" fill=\"black\" alignment-baseline=\"middle\" text-anchor=\"middle\" visibility=\"hidden\" data-tag=\"24\" data-have=\"0\"></text>" +

                                    "<image class=\"nodeImage\" xlink:href=\"\" x=\"230\" y=\"80\" height=\"60\" width=\"60\"/>"+
                                    
                                "</g>";


                            db.UserTrees.Add(new UserTree
                            {
                                isMainTree = true,
                                TreeHtmlCode = htmlTree,
                                UserID = user.UserID,
                                User = user,
                                TransformMatrix = "matrix(2 0 0 2 0 0)"
                            });
                            db.SaveChanges();

                            return htmlTree;
                        }
                    }
                    else // nie znaleziono usera o podanym id
                    {
                        db.Errors.Add(new Error
                        {
                            Message = "HtmlBuilderController - API - (GetUserMainTree) - user is null !! - sprawdź przekazywany id do metody przekazana wartość:"+id,
                            StackTrace = "HtmlBuilderController - API - (GetUserMainTree) - user is null !!",
                            DateThrow = DateTime.Now
                        });
                        db.SaveChanges();

                        return "";
                    }
                }catch(Exception e) // 
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


        [HttpGet]
        public string GetUserMainTreeTransformMatrix(int id)
        {
            using (var db = new DAL.DbContext())
            {
                try
                {
                    if(db.UserTrees.Where(ut => ut.UserID == id && ut.isMainTree == true).Count() > 0)
                    {
                        return db.UserTrees.Where(ut => ut.UserID == id && ut.isMainTree == true).First().TransformMatrix;
                    }
                    else
                    {
                        return null;
                    }
                    

                }catch(Exception e)
                {
                    db.Errors.Add(new Error
                    {
                        Message = e.Message,
                        StackTrace = e.StackTrace,
                        DateThrow = DateTime.Now
                    });
                    db.SaveChanges();

                    return null; 
                }
            }
        }


        [HttpGet]
        public void SaveUserMainTreeTransformMatrix(int id)
        {
            using (var db = new DAL.DbContext())
            {
                try
                {
                    UserTree userTree = db.UserTrees.Where(ut => ut.UserID == id && ut.isMainTree == true).First();
                    userTree.TransformMatrix = this.Request.Headers.GetValues("matrix").First();

                    db.Entry(userTree).State = EntityState.Modified;
                    db.SaveChanges();
                }catch (Exception e)
                {

                    db.Errors.Add(new Error
                    {
                        Message = e.Message,
                        StackTrace = e.StackTrace,
                        DateThrow = DateTime.Now
                    });
                    db.SaveChanges();
                }
            }
        }
    }
}
