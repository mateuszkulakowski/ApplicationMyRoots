using ApplicationMyRoots.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ApplicationMyRoots.ControllersAPI
{
    public class LanguageController : ApiController
    {
        [HttpGet]
        public string getElementTextInLanguage(string id)//id to data-tag przy elemencie html
        {
            int id_result;
            try
            {
                id_result = int.Parse(id);
                return ResourceManager.getElementTextInLanguage(id_result, int.Parse(this.Request.Headers.GetValues("languageID").First()));
            }
            catch (Exception e) { return "Error"; }


        }


        [HttpGet]
        public string getMonth(int id)//id to data-tag przy elemencie html
        {
            try
            {
                return ResourceManager.getElementTextInLanguage(87, id)+","+ ResourceManager.getElementTextInLanguage(88, id) + "," + ResourceManager.getElementTextInLanguage(89, id) + ","
                    + ResourceManager.getElementTextInLanguage(90, id) + "," + ResourceManager.getElementTextInLanguage(91, id) + "," + ResourceManager.getElementTextInLanguage(92, id) + ","
                    + ResourceManager.getElementTextInLanguage(93, id) + "," + ResourceManager.getElementTextInLanguage(94, id) + ","+ ResourceManager.getElementTextInLanguage(95, id) + ","
                    + ResourceManager.getElementTextInLanguage(96, id) + "," + ResourceManager.getElementTextInLanguage(97, id) + ","+ ResourceManager.getElementTextInLanguage(98, id);
            }
            catch (Exception e) { return "Error"; }


        }

    }
}