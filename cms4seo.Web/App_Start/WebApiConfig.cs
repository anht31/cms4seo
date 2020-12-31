using System.Net.Http.Headers;
using System.Web.Http;

namespace cms4seo.Web
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();


            // API admin
            config.Routes.MapHttpRoute(
                name: "AdminApi",
                routeTemplate: "api/admin/{controller}/{id}",
                defaults: new {
                    id = RouteParameter.Optional,
                    namespaces = new string[] { "cms4seo.Admin.Controllers" }
                }
            );


            config.Routes.MapHttpRoute(
                name: "ControllerAndAction",
                routeTemplate: "api/admin/{controller}/{id}/{scopeId}/{action}",
                defaults: new
                {
                    id = RouteParameter.Optional,
                    scopeId = RouteParameter.Optional,
                    namespaces = new string[] { "cms4seo.Admin.Controllers" }
                }
            );



            // API client
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new
                {
                    id = RouteParameter.Optional,
                    namespaces = new string[] { "cms4seo.Controllers" }                    
                }
            );



            // default json
            config.Formatters.JsonFormatter.SupportedMediaTypes
                .Add(new MediaTypeHeaderValue("text/html"));
        }
    }
}
