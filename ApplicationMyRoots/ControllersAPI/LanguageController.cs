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

    }
}