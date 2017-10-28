using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace ApplicationMyRoots
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
            name: "SecoundApi",
            routeTemplate: "api/{controller}/{action}/{id}/{mainUser}",
            defaults: new { id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
            name: "ThirdApi",
            routeTemplate: "api/{controller}/{action}/{id}/{mainUser}/{languageID}",
            defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
