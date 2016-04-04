using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;

namespace HaikuAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {

            // Web API routes
            config.MapHttpAttributeRoutes();

            //delete all haikus from user
            config.Routes.MapHttpRoute(
                name: "NestedApi",
                routeTemplate: "api/{controller}/{username}/haikus",
                defaults: new { action = "DeleteAll" }
            );

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{username}",
                defaults: new { username = RouteParameter.Optional },
                constraints: new { httpMethod = 
                    new System.Web.Http.Routing.HttpMethodConstraint(HttpMethod.Get, HttpMethod.Post) }
            );
        }
    }
}
