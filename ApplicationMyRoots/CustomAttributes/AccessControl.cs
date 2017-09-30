using ApplicationMyRoots.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ApplicationMyRoots.CustomAttributes
{
    public class AccessControl : ActionFilterAttribute
    {

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (ResourceManager.LoggedUser == null)
                filterContext.Result = new RedirectResult("/Login/LogIn");
        }
        
    }
}