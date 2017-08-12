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
        public string getElementTextInLanguage(int? id)
        {
            return ResourceManager.getElementTextInLanguage((int)id, int.Parse(this.Request.Headers.GetValues("languageID").First()));
        }

    }
}