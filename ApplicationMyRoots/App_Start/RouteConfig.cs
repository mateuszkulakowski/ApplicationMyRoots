using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ApplicationMyRoots
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "First",
                url: "{controller}/{action}",
                defaults: new { controller = "Home", action = "Home" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Home", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Second",
                url: "{controller}/{action}/{id}/{id2}",
                defaults: new { controller = "Home", action = "Home", id = UrlParameter.Optional, id2 = UrlParameter.Optional }
            );
        }
    }
}
