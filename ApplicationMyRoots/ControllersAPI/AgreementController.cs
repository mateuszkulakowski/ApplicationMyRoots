using ApplicationMyRoots.Common;
using ApplicationMyRoots.DAL;
using ApplicationMyRoots.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ApplicationMyRoots.ControllersAPI
{
    public class AgreementController : ApiController
    {
        [HttpPost]
        public int SendAgreement() // error 0 - gdy bez błędnie
        {
            int error = 0;

            try
            {
                int receiverid = int.Parse(this.Request.Headers.GetValues("receiverid").First());
                int sendedid = int.Parse(this.Request.Headers.GetValues("senderid").First());

                using (var db = new DbContext())
                {
                    //sprawdzanie czy możemy dodać taką zgodę - możemy gdy tylko jest odrzucona jak do akceptacji lub zaakceptowana to nie możemy dodać.
                    int count = db.UserTreeSharingAgreements.Where(x => x.UserReceiving.UserID == receiverid && x.UserSending.UserID == sendedid && x.Accpeted != false).Count();

                    if (count != 0) return 2;

                    UserTreeSharingAgreement agree = new UserTreeSharingAgreement();
                    agree.Accpeted = null;
                    agree.Date = DateTime.Now;
                    agree.Visible = null;
                    agree.UserSending = db.Users.Find(sendedid);
                    agree.UserReceiving = db.Users.Find(receiverid);
                    agree.IsReceivedUserRead = false;
                    agree.IsSendedUserRead = true;

                    db.UserTreeSharingAgreements.Add(agree);
                    db.SaveChanges();
                }
                
            }
            catch(Exception e)
            {
                DbContext db = new DbContext();
                db.Errors.Add(new Error { DateThrow = DateTime.Now, Message = "Błąd metoda SendAgreement() - AgreementController - " + e.Message, StackTrace = e.StackTrace });
                db.SaveChanges();
                error = 1;
            }

            return error;
        }

        [HttpPost]
        public int AcceptAgreement()
        {
            int error = 0;

            try
            {
                int receivingid = int.Parse(this.Request.Headers.GetValues("receivingid").First());
                int agreementid = int.Parse(this.Request.Headers.GetValues("agreementid").First());

                using (var db = new DbContext())
                {
                    var agreements = db.UserTreeSharingAgreements.Where(x => x.UserRecivingID == receivingid && x.UserTreeSharingAgreementID == agreementid).ToList();

                    if (agreements.Count != 1) return error = 2; //musi być 1 wynik zapytania 

                    var agreement = agreements.First();
                    agreement.Accpeted = true;
                    agreement.IsSendedUserRead = false;
                    agreement.IsReceivedUserRead = true;
                    agreement.UserReceiving = db.Users.Find(receivingid);
                    agreement.UserSending = db.Users.Find(agreement.UserSendingID);
                    agreement.Visible = true;

                    db.Entry(agreement).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }

            }
            catch(Exception e)
            {
                DbContext db = new DbContext();
                db.Errors.Add(new Error { DateThrow = DateTime.Now, Message = "Błąd metoda AcceptAgreement() - AgreementController - " + e.Message, StackTrace = e.StackTrace });
                db.SaveChanges();
                error = 1;
            }

            return error;
        }

        [HttpPost]
        public int DiscardAgreement()
        {
            int error = 0;

            try
            {
                int receivingid = int.Parse(this.Request.Headers.GetValues("receivingid").First());
                int agreementid = int.Parse(this.Request.Headers.GetValues("agreementid").First());

                using (var db = new DbContext())
                {
                    var agreements = db.UserTreeSharingAgreements.Where(x => x.UserRecivingID == receivingid && x.UserTreeSharingAgreementID == agreementid).ToList();

                    if (agreements.Count != 1) return error = 2; //musi być 1 wynik zapytania 

                    var agreement = agreements.First();
                    agreement.Accpeted = false;
                    agreement.IsSendedUserRead = false;
                    agreement.IsReceivedUserRead = true;
                    agreement.UserReceiving = db.Users.Find(receivingid);
                    agreement.UserSending = db.Users.Find(agreement.UserSendingID);
                    agreement.Visible = null;

                    db.Entry(agreement).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }

            }
            catch (Exception e)
            {
                DbContext db = new DbContext();
                db.Errors.Add(new Error { DateThrow = DateTime.Now, Message = "Błąd metoda DiscardAgreement() - AgreementController - " + e.Message, StackTrace = e.StackTrace });
                db.SaveChanges();
                error = 1;
            }

            return error;
        }

        [HttpPost]
        public int DeleteAgreement()
        {
            int error = 0;

            try
            {
                int receivingid = int.Parse(this.Request.Headers.GetValues("receivingid").First());
                int agreementid = int.Parse(this.Request.Headers.GetValues("agreementid").First());

                using (var db = new DbContext())
                {
                    var agreements = db.UserTreeSharingAgreements.Where(x => x.UserRecivingID == receivingid && x.UserTreeSharingAgreementID == agreementid).ToList();

                    if (agreements.Count != 1) return error = 2; //musi być 1 wynik zapytania 

                    UserTreeSharingAgreement agreementtodelete = agreements.First();

                    db.Entry(agreementtodelete).State = System.Data.Entity.EntityState.Deleted;
                    db.SaveChanges();
                }

            }
            catch(Exception e)
            {
                DbContext db = new DbContext();
                db.Errors.Add(new Error { DateThrow = DateTime.Now, Message = "Błąd metoda DeleteAgreement() - AgreementController - " + e.Message, StackTrace = e.StackTrace });
                db.SaveChanges();
                error = 1;
            }

            return error;
        }

        [HttpPost]
        public int ChangeVisibilityAgreement()
        {
            int error = 0;

            try
            {
                int receivingid = int.Parse(this.Request.Headers.GetValues("receivingid").First());
                int agreementid = int.Parse(this.Request.Headers.GetValues("agreementid").First());

                using (var db = new DbContext())
                {
                    var agreements = db.UserTreeSharingAgreements.Where(x => x.UserRecivingID == receivingid && x.UserTreeSharingAgreementID == agreementid).ToList();

                    if (agreements.Count != 1) return error = 2; //musi być 1 wynik zapytania 

                    UserTreeSharingAgreement modifagreement = agreements.First();
                    modifagreement.Visible = !modifagreement.Visible;
                    modifagreement.UserReceiving = db.Users.Find(receivingid);
                    modifagreement.UserSending = db.Users.Find(modifagreement.UserSendingID);

                    db.Entry(modifagreement).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }

            }
            catch (Exception e)
            {
                DbContext db = new DbContext();
                db.Errors.Add(new Error { DateThrow = DateTime.Now, Message = "Błąd metoda DeleteAgreement() - AgreementController - " + e.Message, StackTrace = e.StackTrace });
                db.SaveChanges();
                error = 1;
            }

            return error;
        }

        [HttpPost]
        public string getTableSendedgreement()
        {
            string sharingtometable = "";

            try
            {
                using (var db = new DbContext())
                {
                    int sendedid = int.Parse(this.Request.Headers.GetValues("senderid").First());
                    int languageid = int.Parse(this.Request.Headers.GetValues("languageid").First());

                    var sendedagreements = db.UserTreeSharingAgreements.Where(x => x.UserSendingID == sendedid && x.Accpeted == true).ToList();

                    var visibletext = db.LanguageTexts.Where(x => x.UniqueElementTag == 137 && x.LanguageID == languageid).First().Text;
                    var invisibletext = db.LanguageTexts.Where(x => x.UniqueElementTag == 138 && x.LanguageID == languageid).First().Text;

                    foreach (var sendedagreement in sendedagreements)
                    {
                        sharingtometable += "<tr>";
                        sharingtometable += "<td>" + sendedagreement.UserTreeSharingAgreementID + "</td>";
                        sharingtometable += "<td>" + sendedagreement.UserReceiving.Name + "</td>";
                        sharingtometable += "<td>" + sendedagreement.UserReceiving.Surname + "</td>";
                        if (sendedagreement.Visible != null && sendedagreement.Visible == true)
                            sharingtometable += "<td style='text-align:center;'><span style='color:blue;'>" + visibletext + " <span class='glyphicon glyphicon-eye-open' style='color:blue;'></span></span></td>";
                        else
                            sharingtometable += "<td style='text-align:center;'><span style='color:gray;'>" + invisibletext + " <span class='glyphicon glyphicon-eye-close' style='color:gray;'></span></span></td>";
                        sharingtometable += "</tr>";
                    }
                }
            }catch(Exception e)
            {
                DbContext db = new DbContext();
                db.Errors.Add(new Error { DateThrow = DateTime.Now, Message = "Błąd metoda getTableSendedgreement() - AgreementController - " + e.Message, StackTrace = e.StackTrace });
                db.SaveChanges();
            }

            return sharingtometable;
        }

        [HttpPost]
        public string getTableSendedgreementWaiting()
        {
            string sharingtomewaitingtable = "";

            try
            {
                using (var db = new DbContext())
                {
                    int sendedid = int.Parse(this.Request.Headers.GetValues("senderid").First());
                    int languageid = int.Parse(this.Request.Headers.GetValues("languageid").First());

                    var sendedagreementswaitings = db.UserTreeSharingAgreements.Where(x => x.UserSendingID == sendedid).OrderByDescending(x => x.Date).ToList();

                    var waitingforacceptationtext = db.LanguageTexts.Where(x => x.UniqueElementTag == 139 && x.LanguageID == languageid).First().Text;
                    var accepttext = db.LanguageTexts.Where(x => x.UniqueElementTag == 140 && x.LanguageID == languageid).First().Text;
                    var discardtext = db.LanguageTexts.Where(x => x.UniqueElementTag == 141 && x.LanguageID == languageid).First().Text;

                    foreach (var sendedagreementswaiting in sendedagreementswaitings)
                    {
                        sharingtomewaitingtable += "<tr>";
                        sharingtomewaitingtable +=      "<td>" + sendedagreementswaiting.UserTreeSharingAgreementID + "</td>";
                        sharingtomewaitingtable +=      "<td>" + sendedagreementswaiting.UserReceiving.Name + "</td>";
                        sharingtomewaitingtable +=      "<td>" + sendedagreementswaiting.UserReceiving.Surname + "</td>";
                        if (sendedagreementswaiting.Accpeted == null)
                            sharingtomewaitingtable +=  "<td style='text-align:center;'><strong style='color:gray;'>" + waitingforacceptationtext + "</strong> <span class='glyphicon glyphicon-refresh' style='color:gray;'></span></td>";
                        else if (sendedagreementswaiting.Accpeted == true)
                            sharingtomewaitingtable +=  "<td style='text-align:center;'><strong style='color: green;'>" + accepttext + "</strong> <span class='glyphicon glyphicon-ok-circle' style='color:green;'></span></td>";
                        else
                            sharingtomewaitingtable +=  "<td style='text-align:center;'><strong style='color: red;'>" + discardtext + "</strong> <span class='glyphicon glyphicon-remove-circle' style='color:red;'></span></td>";
                        sharingtomewaitingtable += "</tr>";
                    }
                }
            }catch(Exception e)
            {
                DbContext db = new DbContext();
                db.Errors.Add(new Error { DateThrow = DateTime.Now, Message = "Błąd metoda getTableReceivedAgreementWaiting() - AgreementController - " + e.Message, StackTrace = e.StackTrace });
                db.SaveChanges();
            }

            return sharingtomewaitingtable;
        }

        [HttpPost]
        public string getTableReceivedAgreement()
        {
            string sharingfrommetable = "";

            try
            {
                using (var db = new DbContext())
                {
                    int receiverid = int.Parse(this.Request.Headers.GetValues("receiverid").First());
                    int languageid = int.Parse(this.Request.Headers.GetValues("languageid").First());

                    var receivedagreements = db.UserTreeSharingAgreements.Where(x => x.UserRecivingID == receiverid && x.Accpeted == true).ToList();

                    var visibletext = db.LanguageTexts.Where(x => x.UniqueElementTag == 137 && x.LanguageID == languageid).First().Text;
                    var invisibletext = db.LanguageTexts.Where(x => x.UniqueElementTag == 138 && x.LanguageID == languageid).First().Text;
                    var deleteagreementtext = db.LanguageTexts.Where(x => x.UniqueElementTag == 142 && x.LanguageID == languageid).First().Text;

                    foreach (var receivedagreement in receivedagreements)
                    {
                        sharingfrommetable += "<tr>";
                        sharingfrommetable += "<td>" + receivedagreement.UserTreeSharingAgreementID + "</td>";
                        sharingfrommetable += "<td>" + receivedagreement.UserSending.Name + "</td>";
                        sharingfrommetable += "<td>" + receivedagreement.UserSending.Surname + "</td>";
                        if (receivedagreement.Visible != null && receivedagreement.Visible == true)
                            sharingfrommetable += "<td style='text-align:center;'><button class='btn btn-default' onclick='changeVisibility("+ receivedagreement.UserTreeSharingAgreementID + ")'>" + visibletext + " <span class='glyphicon glyphicon-eye-open' style='color:blue;'></span></button></td>";
                        else
                            sharingfrommetable += "<td style='text-align:center;'><button class='btn btn-default' onclick='changeVisibility(" + receivedagreement.UserTreeSharingAgreementID + ")'>" + invisibletext + " <span class='glyphicon glyphicon-eye-close' style='color:gray;'></span></button></td>";
                        sharingfrommetable += "<td><button class='btn btn-danger' onclick='deleteagreement(" + receivedagreement.UserTreeSharingAgreementID + ")'>" + deleteagreementtext + " <span class='glyphicon glyphicon-remove' style='color:red;'></span></button></td>";
                        sharingfrommetable += "</tr>";
                    }
                }
            }catch(Exception e)
            {
                DbContext db = new DbContext();
                db.Errors.Add(new Error { DateThrow = DateTime.Now, Message = "Błąd metoda getTableReceivedAgreement() - AgreementController - " + e.Message, StackTrace = e.StackTrace });
                db.SaveChanges();
            }

            return sharingfrommetable;
        }

        [HttpPost]
        public string getTableReceivedAgreementWaiting()
        {
            string sharingfrommewaitingtable = "";

            try
            {
                using (var db = new DbContext())
                {
                    int receiverid = int.Parse(this.Request.Headers.GetValues("receiverid").First());
                    int languageid = int.Parse(this.Request.Headers.GetValues("languageid").First());

                    var receivedagreementswaitings = db.UserTreeSharingAgreements.Where(x => x.UserRecivingID == receiverid && x.Accpeted == null).OrderByDescending(x => x.Date).ToList();

                    var accepttext = db.LanguageTexts.Where(x => x.UniqueElementTag == 143 && x.LanguageID == languageid).First().Text;
                    var discardtext = db.LanguageTexts.Where(x => x.UniqueElementTag == 144 && x.LanguageID == languageid).First().Text;

                    foreach (var receivedagreementswaiting in receivedagreementswaitings)
                    {
                        sharingfrommewaitingtable += "<tr>";
                        sharingfrommewaitingtable += "<td>" + receivedagreementswaiting.UserTreeSharingAgreementID + "</td>";
                        sharingfrommewaitingtable += "<td>" + receivedagreementswaiting.UserSending.Name + "</td>";
                        sharingfrommewaitingtable += "<td>" + receivedagreementswaiting.UserSending.Surname + "</td>";
                        sharingfrommewaitingtable += "<td style='text-align:center;'><button class='btn btn-success' onclick='acceptagreement(" + receivedagreementswaiting.UserTreeSharingAgreementID + ")'>" + accepttext + " <span class='glyphicon glyphicon-ok' style='color: green;'></span></button></td>" +
                                                            "<td style='text-align:center;'><button class='btn btn-danger' onclick='discardagreement(" + receivedagreementswaiting.UserTreeSharingAgreementID + ")'>" + discardtext + " <span class='glyphicon glyphicon-remove' style='color:red;'></span></button></td>";
                        sharingfrommewaitingtable += "</tr>";
                    }
                }
            }catch(Exception e)
            {
                DbContext db = new DbContext();
                db.Errors.Add(new Error { DateThrow = DateTime.Now, Message = "Błąd metoda getTableReceivedAgreementWaiting() - AgreementController - " + e.Message, StackTrace = e.StackTrace });
                db.SaveChanges();
            }

            return sharingfrommewaitingtable;
        }
    }
}