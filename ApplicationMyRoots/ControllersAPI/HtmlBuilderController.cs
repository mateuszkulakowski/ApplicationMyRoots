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
        // Pobieranie/Tworzenie drzewa głównego użytkownika
        [HttpGet]
        public HtmlTreeModel GetUserMainTree(string id ,int mainuser) //mainuser - 0 - nie chcemy tworzenia drzewa jeżeli nie istnieje 1 - chcemy
        {
            HtmlTreeModel htm = new HtmlTreeModel();

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
                            htm.HtmlTree = userTrees.Where(ut => ut.isMainTree == true).First().TreeHtmlCode;
                            htm.Tid = userTrees.Where(ut => ut.isMainTree == true).First().UserTreeID;

                            return htm;
                        }
                        else //pierwsze zalogowanie - użytkownik nie ma drzewa stworzonego
                        {
                            if (mainuser == 0) return htm;// wartość na 0 nie chcemy tworzyć drzewa tylko pobrać jak jest jak nie to nic -- ExchangeTree

                            //data-have - czy dodany już ojciec/matka/partner
                            //data-mainuser - czy to ten co ma konto na stronce czy dodany jako węzeł 1-main, 0-dodany
                            string ageLabelX;
                            string dateBornX;
                            string dateDeadX;
                            string ageValueX;
                            if (user.LanguageID == 1)
                            {
                                ageLabelX = "147.5";
                                dateBornX = "170";
                                dateDeadX = "174";
                                ageValueX = "162";
                            }
                            else
                            {
                                ageLabelX = "145.5";
                                dateBornX = "170";
                                dateDeadX = "173";
                                ageValueX = "163";
                            }

                            htm.HtmlTree =
                                    "<g class=\"tree-elements\" id=\"" + user.UserID + "\" onmousedown=\"mousedowntreeelement(evt)\" transform=\"matrix(1 0 0 1 0 0)\" data-mainuser=\"1\" data-haveRightLine=\"0\" data-haveLeftLine=\"0\" data-haveUpLine=\"1\">" +
                                        "<rect class=\"tree-element-frames\" width=\"200\" height=\"100\" x=\"100\" y=\"50\" fill=\"white\" stroke=\"black\"/>" +
                                        "<text class=\"tree-element-texts\" x=\"200\" y=\"65\" font-family=\"verdana\" font-size=\"12\" fill=\"blue\" alignment-baseline=\"middle\" text-anchor=\"middle\">" + user.NameSurname + "</text>" +

                                        "<rect class=\"addparents\" width=\"180\" height=\"20\" x=\"110\" y=\"30\" fill=\"white\" stroke=\"#428bca\" visibility=\"hidden\" data-have=\"0\" onclick=\"addNewNodeToTreeClick(this)\"/>" +
                                        "<text class=\"addparents-dbt\" x=\"200\" y=\"40\" font-family=\"verdana\" font-size=\"6\" fill=\"black\" alignment-baseline=\"middle\" text-anchor=\"middle\" visibility=\"hidden\" data-tag=\"20\" data-have=\"0\"></text>" +

                                        "<rect class=\"addpartnerR\" width=\"20\" height=\"80\" x=\"300\" y=\"60\" fill=\"white\" stroke=\"#428bca\" visibility=\"hidden\" data-have=\"0\" onclick=\"addNewNodeToTreeClick(this)\"/>" +
                                        "<text class=\"addpartnerR-dbt\" transform=\"rotate(90)\" x=\"100\" y=\"-310\" font-family=\"verdana\" font-size=\"6\" fill=\"black\" alignment-baseline=\"middle\" text-anchor=\"middle\" visibility=\"hidden\" data-tag=\"22\" data-have=\"0\"></text>" +

                                        "<rect class=\"addpartnerL\" width=\"20\" height=\"80\" x=\"80\" y=\"60\" fill=\"white\" stroke=\"#428bca\" visibility=\"hidden\" data-have=\"0\" onclick=\"addNewNodeToTreeClick(this)\"/>" +
                                        "<text class=\"addpartnerL-dbt\" transform=\"rotate(90)\" x=\"100\" y=\"-90\" font-family=\"verdana\" font-size=\"6\" fill=\"black\" alignment-baseline=\"middle\"text-anchor=\"middle\" visibility=\"hidden\" data-tag=\"23\" data-have=\"0\"></text>" +

                                        "<rect class=\"addchildren\" width=\"180\" height=\"20\" x=\"110\" y=\"150\" fill=\"white\" stroke=\"#428bca\" visibility=\"hidden\" data-have=\"1\" onclick=\"addNewNodeToTreeClick(this)\"/>" +
                                        "<text class=\"addchildren-dbt\" x=\"200\" y=\"160\" font-family=\"verdana\" font-size=\"6\" fill=\"black\" alignment-baseline=\"middle\" text-anchor=\"middle\" visibility=\"hidden\" data-tag=\"24\" data-have=\"1\"></text>" +
                                        
                                        "<image class=\"nodeImage\" xlink:href=\"\" x=\"230\" y=\"80\" height=\"60\" width=\"60\"/>" +

                                        "<text class=\"datebirthLabel-dbt\" x=\"138\" y=\"100\" font-family=\"verdana\" font-size=\"5\" fill=\"grey\" alignment-baseline=\"middle\" text-anchor=\"middle\" data-tag=\"29\"></text>" +
                                        "<text class=\"datedeadLabel-dbt\" x=\"138\" y=\"110\" font-family=\"verdana\" font-size=\"5\" fill=\"grey\" alignment-baseline=\"middle\" text-anchor=\"middle\" data-tag=\"30\"></text>" +
                                        "<text class=\"ageLabel-dbt\" x=\""+ageLabelX+"\" y=\"120\" font-family=\"verdana\" font-size=\"5\" fill=\"grey\" alignment-baseline=\"middle\" text-anchor=\"middle\" data-tag=\"31\"></text>" +
                                        "<text class=\"maininfoLabel-dbt\" x=\"165\" y=\"86\" font-family=\"verdana\" font-size=\"6\" fill=\"black\" alignment-baseline=\"middle\" text-anchor=\"middle\" data-tag=\"32\"></text>" +
                                        
                                        "<text class=\"datebirthValue\" x=\""+dateBornX+"\" y=\"100\" font-family=\"verdana\" font-size=\"5\" fill=\"grey\" alignment-baseline=\"middle\" text-anchor=\"middle\"></text>" +
                                        "<text class=\"datedeadValue\" x=\""+dateDeadX+"\" y=\"110\" font-family=\"verdana\" font-size=\"5\" fill=\"grey\" alignment-baseline=\"middle\" text-anchor=\"middle\"></text>" +
                                        "<text class=\"ageValue\" x=\""+ageValueX+"\" y=\"120\" font-family=\"verdana\" font-size=\"5\" fill=\"grey\" alignment-baseline=\"middle\" text-anchor=\"middle\"></text>" +

                                        "<image class=\"copyImage\" xlink:href=\"\" x=\"102\" y=\"135\" height=\"13\" width=\"13\" visibility=\"hidden\" onclick=\"onClickCopy(this)\"/>" +
                                        "<rect class=\"copyImageBorder\" x=\"102\" y=\"135\" height=\"13\" width=\"13\" fill=\"none\" stroke=\"#428bca\" stroke-width=\"0.2\" visibility=\"hidden\"/>" +

                                        "<image class=\"otherpartnersImage\" xlink:href=\"\" x=\"102\" y=\"135\" height=\"13\" width=\"13\" visibility=\"hidden\" onclick=\"onClickOtherPartners(this)\"/>" +
                                        "<rect class=\"otherpartnersImageBorder\" x=\"102\" y=\"135\" height=\"13\" width=\"13\" fill=\"none\" stroke=\"#428bca\" stroke-width=\"0.2\" visibility=\"hidden\"/>" +
                                    "</g>";

                            UserTree newtree = new UserTree
                            {
                                isMainTree = true,
                                TreeHtmlCode = htm.HtmlTree,
                                UserID = user.UserID,
                                User = user,
                                TransformMatrix = "matrix(2 0 0 2 0 0)"
                            };
                            db.UserTrees.Add(newtree);
                            db.SaveChanges();

                            htm.Tid = newtree.UserTreeID;
                            
                            return htm;
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

                        return htm;
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
                    return htm;
                }
            }
        }

        // tworzy się inne drzewo, na jednym było by to bardzo słabo widoczne -> w przypadku kolejnych partnerów
        [HttpPost]
        public async Task<HtmlTreeModel> BuildUserTree()
        {
            HtmlTreeModel htm = new HtmlTreeModel();

            try
            {
                User user = null;
                UserTreeNode usertreenodecaller = null; // ten węzeł na którym klikneliśmy dodaj kolejnego partnera
                UserTreeNode usertreenodenew = null; //partner obecnie tworzony jako nowy partner danej osoby

                int id = int.Parse(this.Request.Headers.GetValues("id").First()); //nid -> usertreenodeid węzeł dla którego jest stworzone drzewo
                int mainuser = int.Parse(this.Request.Headers.GetValues("mainuser").First());
                string namenew = this.Request.Headers.GetValues("name").First();
                string surnamenew = this.Request.Headers.GetValues("surname").First();
                string datebornnew = this.Request.Headers.GetValues("dateborn").First();
                string datedeadnew = this.Request.Headers.GetValues("datedead").First();
                string additionalinfonew = this.Request.Headers.GetValues("additionalinfo").First();
                int withimage = int.Parse(this.Request.Headers.GetValues("withimage").First());

                using (var db = new DAL.DbContext())
                {
                    //----
                    // wyszukiwanie czy poprawne dane
                    if(mainuser == 1) //tabela users
                    {
                        user = db.Users.Find(id);

                        //korzeń naszego drzewa
                        htm.Rootid = user.UserID;
                        htm.Mainuserroot = 1;
                        htm.NameSurnameCaller = user.NameSurname;

                        if (user == null) // błędne podane user id
                            return new HtmlTreeModel();
                    }
                    else if(mainuser == 0) //mainuser - 0 - szukamy czy istnieje taki node o podanym id
                    {
                        usertreenodecaller = db.UserTreeNodes.Find(id);

                        //korzeń naszego drzewa zapisujemy id
                        htm.Rootid = usertreenodecaller.UserTreeNodeID;
                        htm.Mainuserroot = 0;
                        htm.NameSurnameCaller = usertreenodecaller.NameSurname;

                        if (usertreenodecaller == null) // błędny podany node id
                            return new HtmlTreeModel();

                        user = db.UserTreesUserTreeNodes.Where(x => x.UserTreeNodeID == id).First().UserTree.User;

                        if (user == null) //usera nie ma wychodzimy -> każde drzewo/poddrzewo musi mieć przypisanego usera
                            return new HtmlTreeModel();
                    }
                    //----


                    //------
                    //Wstawianie do bazy nowego noda 
                    byte[] filebytes = null;
                    if (withimage == 1)
                    {
                        var provider = new MultipartMemoryStreamProvider();
                        await this.Request.Content.ReadAsMultipartAsync(provider);
                        filebytes = await provider.Contents[0].ReadAsByteArrayAsync();
                    }

                    DateTime? dateborn = null;
                    DateTime? datedead = null;

                    if (datebornnew != "") //podano date born
                    {
                        var tabDateborn = datebornnew.Split('-');
                        dateborn = new DateTime(int.Parse(tabDateborn[0]), int.Parse(tabDateborn[1]), int.Parse(tabDateborn[2]));
                    }
                    if (datedeadnew != "") //podano date death
                    {
                        var tabDatedead = datedeadnew.Split('-');
                        datedead = new DateTime(int.Parse(tabDatedead[0]), int.Parse(tabDatedead[1]), int.Parse(tabDatedead[2]));
                    }

                    usertreenodenew = new UserTreeNode
                    {
                        Name = namenew,
                        Surname = surnamenew,
                        DateBorn = dateborn,
                        DateDead = datedead,
                        AdditionalInfo = additionalinfonew,
                        Image = filebytes,
                    };

                    db.UserTreeNodes.Add(usertreenodenew);
                    db.SaveChanges();
                    //-----
                

                    string ageLabelX;
                    string dateDeadX;
                    string ageValueX;
                    if (user.LanguageID == 1)
                    {
                        ageLabelX = "147";
                        dateDeadX = "174";
                        ageValueX = "162";
                    }
                    else
                    {
                        ageLabelX = "145";
                        dateDeadX = "173";
                        ageValueX = "163";
                    }

                    //html -- połączenie między węzłami + 2x węzły
                    htm.HtmlTree =
                                "<path  d=\"M300 100 Q360 250 420 100\" fill=\"transparent\" stroke=\"black\" data-left=\"" + id + "\" data-right=\"" + usertreenodenew.UserTreeNodeID + "\"/>" +//przeliczone na stałe z myscripts.js tam dynamicznie
                                "<g class=\"tree-elements\" id=\"" + id + "\" onmousedown=\"mousedowntreeelement(evt)\" transform=\"matrix(1 0 0 1 0 0)\" data-mainuser=\"" + mainuser + "\" data-haveRightLine=\"1\" data-haveLeftLine=\"0\" data-haveUpLine=\"1\">" +
                                    "<rect class=\"tree-element-frames\" width=\"200\" height=\"100\" x=\"100\" y=\"50\" fill=\"white\" stroke=\"black\"/>" +
                                    "<text class=\"tree-element-texts\" x=\"200\" y=\"65\" font-family=\"verdana\" font-size=\"12\" fill=\"blue\" alignment-baseline=\"middle\" text-anchor=\"middle\"></text>" +

                                    "<rect class=\"addparents\" width=\"180\" height=\"20\" x=\"110\" y=\"30\" fill=\"white\" stroke=\"#428bca\" visibility=\"hidden\" data-have=\"0\" onclick=\"addNewNodeToTreeClick(this)\"/>" +
                                    "<text class=\"addparents-dbt\" x=\"200\" y=\"40\" font-family=\"verdana\" font-size=\"6\" fill=\"black\" alignment-baseline=\"middle\" text-anchor=\"middle\" visibility=\"hidden\" data-tag=\"20\" data-have=\"0\"></text>" +

                                    "<rect class=\"addchildren\" width=\"180\" height=\"20\" x=\"110\" y=\"150\" fill=\"white\" stroke=\"#428bca\" visibility=\"hidden\" data-have=\"0\" onclick=\"addNewNodeToTreeClick(this)\"/>" +
                                    "<text class=\"addchildren-dbt\" x=\"200\" y=\"160\" font-family=\"verdana\" font-size=\"6\" fill=\"black\" alignment-baseline=\"middle\" text-anchor=\"middle\" visibility=\"hidden\" data-tag=\"24\" data-have=\"0\"></text>" +

                                    "<image class=\"nodeImage\" xlink:href=\"\" x=\"230\" y=\"80\" height=\"60\" width=\"60\"/>" +

                                    "<text class=\"datebirthLabel-dbt\" x=\"138\" y=\"100\" font-family=\"verdana\" font-size=\"5\" fill=\"grey\" alignment-baseline=\"middle\" text-anchor=\"middle\" data-tag=\"29\"></text>" +
                                    "<text class=\"datedeadLabel-dbt\" x=\"138\" y=\"110\" font-family=\"verdana\" font-size=\"5\" fill=\"grey\" alignment-baseline=\"middle\" text-anchor=\"middle\" data-tag=\"30\"></text>" +
                                    "<text class=\"ageLabel-dbt\" x=\"" + ageLabelX + ".5\" y=\"120\" font-family=\"verdana\" font-size=\"5\" fill=\"grey\" alignment-baseline=\"middle\" text-anchor=\"middle\" data-tag=\"31\"></text>" +
                                    "<text class=\"maininfoLabel-dbt\" x=\"165\" y=\"86\" font-family=\"verdana\" font-size=\"6\" fill=\"black\" alignment-baseline=\"middle\" text-anchor=\"middle\" data-tag=\"32\"></text>" +

                                    "<text class=\"datebirthValue\" x=\"170\" y=\"100\" font-family=\"verdana\" font-size=\"5\" fill=\"grey\" alignment-baseline=\"middle\" text-anchor=\"middle\"></text>" +
                                    "<text class=\"datedeadValue\" x=\"" + dateDeadX + "\" y=\"110\" font-family=\"verdana\" font-size=\"5\" fill=\"grey\" alignment-baseline=\"middle\" text-anchor=\"middle\"></text>" +
                                    "<text class=\"ageValue\" x=\"" + ageValueX + "\" y=\"120\" font-family=\"verdana\" font-size=\"5\" fill=\"grey\" alignment-baseline=\"middle\" text-anchor=\"middle\"></text>";

                    if(mainuser == 0) // gdy nie mainuser -> mają być pola do edycji/usuwania/kopiowania/widoku innych partnerów
                    {
                        htm.HtmlTree +=
                                    "<image class=\"editImage\" xlink:href=\"\" x=\"102\" y=\"135\" height=\"13\" width=\"13\" visibility=\"hidden\" onclick=\"onClickEdit(this)\"/>" +
                                    "<rect class=\"editImageBorder\" x=\"102\" y=\"135\" height=\"13\" width=\"13\" fill=\"none\" stroke=\"#428bca\" stroke-width=\"0.2\" visibility=\"hidden\"/>" +
                                    "<image class=\"trashImage\" xlink:href=\"\" x=\"120\" y=\"135\" height=\"13\" width=\"13\" visibility=\"hidden\" onclick=\"onClickDelete(this)\"/>" +
                                    "<rect class=\"trashImageBorder\" x=\"120\" y=\"135\" height=\"13\" width=\"13\" fill=\"none\" stroke=\"#428bca\" stroke-width=\"0.2\" visibility=\"hidden\"/>" +
                                    "<image class=\"copyImage\" xlink:href=\"\" x=\"102\" y=\"135\" height=\"13\" width=\"13\" visibility=\"hidden\" onclick=\"onClickCopy(this)\"/>" +
                                    "<rect class=\"copyImageBorder\" x=\"102\" y=\"135\" height=\"13\" width=\"13\" fill=\"none\" stroke=\"#428bca\" stroke-width=\"0.2\" visibility=\"hidden\"/>" +
                                    "<image class=\"otherpartnersImage\" xlink:href=\"\" x=\"138\" y=\"135\" height=\"13\" width=\"13\" visibility=\"hidden\" onclick=\"onClickOtherPartners(this)\"/>" +
                                    "<rect class=\"otherpartnersImageBorder\" x=\"138\" y=\"135\" height=\"13\" width=\"13\" fill=\"none\" stroke=\"#428bca\" stroke-width=\"0.2\" visibility=\"hidden\"/>";
                    }
                    else if(mainuser == 1)
                    {
                        htm.HtmlTree +=
                                   "<image class=\"copyImage\" xlink:href=\"\" x=\"102\" y=\"135\" height=\"13\" width=\"13\" visibility=\"hidden\" onclick=\"onClickCopy(this)\"/>" +
                                   "<rect class=\"copyImageBorder\" x=\"102\" y=\"135\" height=\"13\" width=\"13\" fill=\"none\" stroke=\"#428bca\" stroke-width=\"0.2\" visibility=\"hidden\"/>" +
                                   "<image class=\"otherpartnersImage\" xlink:href=\"\" x=\"102\" y=\"135\" height=\"13\" width=\"13\" visibility=\"hidden\" onclick=\"onClickOtherPartners(this)\"/>" +
                                   "<rect class=\"otherpartnersImageBorder\" x=\"102\" y=\"135\" height=\"13\" width=\"13\" fill=\"none\" stroke=\"#428bca\" stroke-width=\"0.2\" visibility=\"hidden\"/>";
                    }

                     htm.HtmlTree+=               
                                "</g>"+

                                //drugi nowy węzeł
                                "<g class=\"tree-elements\" id=\"" + usertreenodenew.UserTreeNodeID + "\" onmousedown=\"mousedowntreeelement(evt)\" transform=\"matrix(1 0 0 1 0 0)\" data-mainuser=\"0\" data-haveRightLine=\"0\" data-haveLeftLine=\"1\" data-haveUpLine=\"1\">" +
                                    "<rect class=\"tree-element-frames\" width=\"200\" height=\"100\" x=\"420\" y=\"50\" fill=\"white\" stroke=\"black\"/>" +
                                    "<text class=\"tree-element-texts\" x=\"520\" y=\"65\" font-family=\"verdana\" font-size=\"12\" fill=\"blue\" alignment-baseline=\"middle\" text-anchor=\"middle\"></text>" +

                                    "<rect class=\"addparents\" width=\"180\" height=\"20\" x=\"430\" y=\"30\" fill=\"white\" stroke=\"#428bca\" visibility=\"hidden\" data-have=\"0\" onclick=\"addNewNodeToTreeClick(this)\"/>" +
                                    "<text class=\"addparents-dbt\" x=\"520\" y=\"40\" font-family=\"verdana\" font-size=\"6\" fill=\"black\" alignment-baseline=\"middle\" text-anchor=\"middle\" visibility=\"hidden\" data-tag=\"20\" data-have=\"0\"></text>" +

                                    "<rect class=\"addpartnerR\" width=\"20\" height=\"80\" x=\"620\" y=\"60\" fill=\"white\" stroke=\"#428bca\" visibility=\"hidden\" data-have=\"1\" onclick=\"addNewNodeToTreeClick(this)\"/>" +
                                    "<text class=\"addpartnerR-dbt\" transform=\"rotate(90)\" x=\"100\" y=\"-630\" font-family=\"verdana\" font-size=\"6\" fill=\"black\" alignment-baseline=\"middle\" text-anchor=\"middle\" visibility=\"hidden\" data-tag=\"22\" data-have=\"1\"></text>" +

                                    "<rect class=\"addpartnerL\" width=\"20\" height=\"80\" x=\"400\" y=\"60\" fill=\"white\" stroke=\"#428bca\" visibility=\"hidden\" data-have=\"1\" onclick=\"addNewNodeToTreeClick(this)\"/>" +
                                    "<text class=\"addpartnerL-dbt\" transform=\"rotate(90)\" x=\"100\" y=\"-410\" font-family=\"verdana\" font-size=\"6\" fill=\"black\" alignment-baseline=\"middle\"text-anchor=\"middle\" visibility=\"hidden\" data-tag=\"23\" data-have=\"1\"></text>" +

                                    "<rect class=\"addchildren\" width=\"180\" height=\"20\" x=\"430\" y=\"150\" fill=\"white\" stroke=\"#428bca\" visibility=\"hidden\" data-have=\"0\" onclick=\"addNewNodeToTreeClick(this)\"/>" +
                                    "<text class=\"addchildren-dbt\" x=\"520\" y=\"160\" font-family=\"verdana\" font-size=\"6\" fill=\"black\" alignment-baseline=\"middle\" text-anchor=\"middle\" visibility=\"hidden\" data-tag=\"24\" data-have=\"0\"></text>" +

                                    "<image class=\"nodeImage\" xlink:href=\"\" x=\"550\" y=\"80\" height=\"60\" width=\"60\"/>" +

                                    "<text class=\"datebirthLabel-dbt\" x=\"458\" y=\"100\" font-family=\"verdana\" font-size=\"5\" fill=\"grey\" alignment-baseline=\"middle\" text-anchor=\"middle\" data-tag=\"29\"></text>" +
                                    "<text class=\"datedeadLabel-dbt\" x=\"458\" y=\"110\" font-family=\"verdana\" font-size=\"5\" fill=\"grey\" alignment-baseline=\"middle\" text-anchor=\"middle\" data-tag=\"30\"></text>" +
                                    "<text class=\"ageLabel-dbt\" x=\"" + (int.Parse(ageLabelX)+320) + ".5\" y=\"120\" font-family=\"verdana\" font-size=\"5\" fill=\"grey\" alignment-baseline=\"middle\" text-anchor=\"middle\" data-tag=\"31\"></text>" +
                                    "<text class=\"maininfoLabel-dbt\" x=\"485\" y=\"86\" font-family=\"verdana\" font-size=\"6\" fill=\"black\" alignment-baseline=\"middle\" text-anchor=\"middle\" data-tag=\"32\"></text>" +

                                    "<text class=\"datebirthValue\" x=\"490\" y=\"100\" font-family=\"verdana\" font-size=\"5\" fill=\"grey\" alignment-baseline=\"middle\" text-anchor=\"middle\"></text>" +
                                    "<text class=\"datedeadValue\" x=\"" + (int.Parse(dateDeadX)+320) + "\" y=\"110\" font-family=\"verdana\" font-size=\"5\" fill=\"grey\" alignment-baseline=\"middle\" text-anchor=\"middle\"></text>" +
                                    "<text class=\"ageValue\" x=\"" + (int.Parse(ageValueX)+320) + "\" y=\"120\" font-family=\"verdana\" font-size=\"5\" fill=\"grey\" alignment-baseline=\"middle\" text-anchor=\"middle\"></text>" +

                                    //opcje edytuj/usuń/kopiuj/inni partnerzy
                                    "<image class=\"editImage\" xlink:href=\"\" x=\"422\" y=\"135\" height=\"13\" width=\"13\" visibility=\"hidden\" onclick=\"onClickEdit(this)\"/>" +
                                    "<rect class=\"editImageBorder\" x=\"422\" y=\"135\" height=\"13\" width=\"13\" fill=\"none\" stroke=\"#428bca\" stroke-width=\"0.2\" visibility=\"hidden\"/>" +
                                    "<image class=\"trashImage\" xlink:href=\"\" x=\"440\" y=\"135\" height=\"13\" width=\"13\" visibility=\"hidden\" onclick=\"onClickDelete(this)\"/>" +
                                    "<rect class=\"trashImageBorder\" x=\"440\" y=\"135\" height=\"13\" width=\"13\" fill=\"none\" stroke=\"#428bca\" stroke-width=\"0.2\" visibility=\"hidden\"/>" +
                                    "<image class=\"copyImage\" xlink:href=\"\" x=\"422\" y=\"135\" height=\"13\" width=\"13\" visibility=\"hidden\" onclick=\"onClickCopy(this)\"/>" +
                                    "<rect class=\"copyImageBorder\" x=\"422\" y=\"455\" height=\"13\" width=\"13\" fill=\"none\" stroke=\"#428bca\" stroke-width=\"0.2\" visibility=\"hidden\"/>" +
                                    "<image class=\"otherpartnersImage\" xlink:href=\"\" x=\"458\" y=\"135\" height=\"13\" width=\"13\" visibility=\"hidden\" onclick=\"onClickOtherPartners(this)\"/>" +
                                    "<rect class=\"otherpartnersImageBorder\" x=\"458\" y=\"135\" height=\"13\" width=\"13\" fill=\"none\" stroke=\"#428bca\" stroke-width=\"0.2\" visibility=\"hidden\"/>" +
                                "</g>";

                    int? usertreenodeid = usertreenodecaller == null ? (int?)null : usertreenodecaller.UserTreeNodeID;

                    //nowe drzewo
                    UserTree newtree = new UserTree
                    {
                        isMainTree = false,
                        TransformMatrix = "matrix(2 0 0 2 0 0)",
                        TreeHtmlCode = htm.HtmlTree,
                        UserID = user.UserID,
                        UserTreeNodeID = usertreenodeid,
                        UserTreeNodePartnerID = usertreenodenew.UserTreeNodeID
                    };

                    db.UserTrees.Add(newtree);
                    db.SaveChanges();

                    //id stworzonego drzewa
                    htm.Tid = newtree.UserTreeID;

                    //łączenie nowego node z tree
                    UserTreeUserTreeNode ututn = new UserTreeUserTreeNode
                    {
                        UserTreeID = newtree.UserTreeID,
                        UserTreeNodeID = usertreenodenew.UserTreeNodeID
                    };
                    db.UserTreesUserTreeNodes.Add(ututn);

                    //łączenie nodecaller z tree jeżeli jest -> jak nie ma caller jest z tabelki user więc jest zapisywany jak zapisujemy do UserTree wyżej
                    if(usertreenodeid != null)
                    {
                        UserTreeUserTreeNode ututn2 = new UserTreeUserTreeNode
                        {
                            UserTreeID = newtree.UserTreeID,
                            UserTreeNodeID = (int)usertreenodeid
                        };
                        db.UserTreesUserTreeNodes.Add(ututn2);
                    }

                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                DAL.DbContext db = new DAL.DbContext();
                db.Errors.Add(new Error
                {
                    Message = "Błąd w generowaniu drzewa kolejnego partnera, HtmlBuilderController metoda:BuildUserTree() - " + e.Message,
                    StackTrace = e.StackTrace,
                    DateThrow = DateTime.Now
                });
                db.SaveChanges();
            }

            return htm;
        }

        [HttpPost]
        public string GetNodeTrees()
        {
            string result = "";

            try
            {
                int id = int.Parse(this.Request.Headers.GetValues("id").First());
                int mainuser = int.Parse(this.Request.Headers.GetValues("mainuser").First());
                int languageid = int.Parse(this.Request.Headers.GetValues("lid").First());

                using (var db = new DAL.DbContext())
                {
                    List<UserTree> nodetrees = null;

                    if(mainuser == 1)
                    {
                        nodetrees = db.UserTrees.Where(x => x.UserID == id && x.UserTreeNodeID == null && x.isMainTree == false).ToList();
                    }else if(mainuser == 0)
                    {
                        nodetrees = db.UserTrees.Where(x => x.UserTreeNodeID == id && x.isMainTree == false).ToList();
                    }

                    if (nodetrees == null) return "";

                    //text przejdź do drzewa
                    string text = db.LanguageTexts.Where(x => x.UniqueElementTag == 195 && x.LanguageID == languageid).First().Text;
                    //text usuń drzewo
                    string text2 = db.LanguageTexts.Where(x => x.UniqueElementTag == 199 && x.LanguageID == languageid).First().Text;

                    //generowanie tbody dla tabelki
                    foreach (var tree in nodetrees)
                    {
                        result += "<tr>";
                        result += "<td>"+tree.UserTreeNodePartner.NameSurname+"</td>";
                        result += "<td style='text-align:right;'><button class='btn btn-danger' data-tid='"+tree.UserTreeID+"' onclick='deletetree(this)'>" + text2 + " <span class='glyphicon glyphicon-remove-circle'></span></button></td>";
                        result += "<td style='text-align:left;'><button class='btn btn-default' data-tid='"+tree.UserTreeID+"' onclick='loadtree(this,2)'>" + text + " <span class='glyphicon glyphicon-circle-arrow-right'></span></button></td>";
                        result += "</tr>";
                    }
                }
            }
            catch(Exception e)
            {
                DAL.DbContext db = new DAL.DbContext();
                db.Errors.Add(new Error
                {
                    Message = "Błąd w pobieraniu html-a do modala otherpartners, HtmlBuilderController metoda:GetNodeTrees() - " + e.Message,
                    StackTrace = e.StackTrace,
                    DateThrow = DateTime.Now
                });
                db.SaveChanges();
            }

            return result;
        }


        [HttpPost]
        public HtmlTreeModel GetUserTree()
        {
            HtmlTreeModel htm = new HtmlTreeModel();

            try
            {
                int id = int.Parse(this.Request.Headers.GetValues("tid").First()); // id drzewa które chcemy pobrać

                using (var db = new DAL.DbContext())
                {
                    UserTree ut = db.UserTrees.Find(id);
                    htm.HtmlTree = ut.TreeHtmlCode;
                    htm.Tid = ut.UserTreeID;
                   
                    if(ut.UserTreeNodeID == null) //korzeń główny to userid
                    {
                        htm.Rootid = ut.UserID;
                        htm.Mainuserroot = 1;
                    }
                    else
                    {
                        htm.Rootid = (int)ut.UserTreeNodeID;
                        htm.Mainuserroot = 0;
                    }

                    htm.Ismaintree = ut.isMainTree == true ? 1 : 0;
                }

            }
            catch(Exception e)
            {
                DAL.DbContext db = new DAL.DbContext();
                db.Errors.Add(new Error
                {
                    Message = "Błąd w pobieraniu drzewa po id -> najprawdopodobniej nie istnieje podany id, HtmlBuilderController metoda:GetUserTree() - " + e.Message,
                    StackTrace = e.StackTrace,
                    DateThrow = DateTime.Now
                });
                db.SaveChanges();
            }

            return htm;
        }

        // Zapisuje drzewo użytkownika // w metdzie unload - mytreescripts.js
        [HttpPost]
        public void SaveUserMainTree([FromBody]ClassWithTreeHtml treeClass)
        {
            int id = -1;

            try
            {
                using (var db = new DAL.DbContext())
                {
                    id = int.Parse(this.Request.Headers.GetValues("id").First());

                    UserTree userTree = db.UserTrees.Find(id);
                    userTree.TreeHtmlCode = treeClass.TreeHtml;

                    db.Entry(userTree).State = EntityState.Modified;
                    db.SaveChanges();
                }

            }catch(Exception e)
            {
                DAL.DbContext db = new DAL.DbContext();
                db.Errors.Add(new Error { DateThrow = DateTime.Now, Message = "Błąd przy zapisie drzewa o ID:" + id + "/ SaveUSerMainTree() HtmlBuilderControllerAPI - " + e.Message, StackTrace = e.StackTrace });
                db.SaveChanges();
            }
        }

        [HttpGet]
        public string GetUserTreeTransformMatrix(int id)
        {
            using (var db = new DAL.DbContext())
            {
                try
                {
                    return db.UserTrees.Find(id).TransformMatrix;

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

        [HttpPost]
        public void SaveUserMainTreeTransformMatrix()
        {
            using (var db = new DAL.DbContext())
            {
                try
                {
                    int id = int.Parse(this.Request.Headers.GetValues("id").First());

                    UserTree userTree = db.UserTrees.Find(id);
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
