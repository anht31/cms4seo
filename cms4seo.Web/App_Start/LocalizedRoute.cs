using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using cms4seo.Data;
using cms4seo.Model.LekimaxType;

namespace cms4seo.Web
{
    public static class LocalizedRouteExtensionMethod
    {
        public static Route MapLocalizedRoute(this RouteCollection routes, string name, string url, object defaults, string[] namespaces)
        {
            return MapLocalizedRoute(routes, name, url, defaults, null /* constraints */, namespaces);
        }
        public static Route MapLocalizedRoute(this RouteCollection routes, string name, string url, object defaults, object constraints, string[] namespaces)
        {
            if (routes == null)
            {
                throw new ArgumentNullException("routes");
            }
            if (url == null)
            {
                throw new ArgumentNullException("url");
            }

            var route = new ClsRouteData(url, new MvcRouteHandler())
            {
                Defaults = new RouteValueDictionary(defaults),
                Constraints = new RouteValueDictionary(constraints),
                DataTokens = new RouteValueDictionary()
            };

            if ((namespaces != null) && (namespaces.Length > 0))
            {
                route.DataTokens["Namespaces"] = namespaces;
            }

            routes.Add(name, route);

            return route;
        }
    }


    public class ClsRouteData : Route
    {
        public ClsRouteData(string url, IRouteHandler routeHandler)
            : base(url, routeHandler)
        {
        }
        public override RouteData GetRouteData(HttpContextBase httpContext)
        {
            RouteData data = base.GetRouteData(httpContext);
            if (data != null)
            {
                //var seoFriendlyName = data.Values["SeoFriendlyName"] as string;

                var segment1 = data.Values["segment1"] as string;
                var segment2 = data.Values["segment2"] as string;
                var segment3 = data.Values["segment3"] as string;

                var page = data.Values["page"] as string;


                //var slug = httpContext.Request.Path.TrimEnd('/');

                var slug = $"/{segment1}";

                if (segment2 != null)
                    slug += $"/{segment2}";

                if (segment3 != null)
                    slug += $"/{segment3}";

                //get here from Database;
                var db = new ApplicationDbContext();
                var results = db.Permalinks.FirstOrDefault(x => x.Slug == slug);
                if (results != null && results.Id > 0 && results.IsActive)
                {
                    Type controllerType = null;


                    switch (results.EntityName)
                    {
                        case cms4seoEntityType.Category:
                            controllerType = typeof(cms4seo.Web.Controllers.CategoriesController);
                            break;
                        case cms4seoEntityType.Product:
                            controllerType = typeof(cms4seo.Web.Controllers.ProductController);
                            break;
                        case cms4seoEntityType.Topic:
                            controllerType = typeof(cms4seo.Web.Controllers.TopicsController);
                            break;
                        case cms4seoEntityType.Article:
                            controllerType = typeof(cms4seo.Web.Controllers.ArticlesController);
                            break;
                        case cms4seoEntityType.Info:
                            controllerType = typeof(cms4seo.Web.Controllers.InfosController);
                            break;
                        default:
                            data.Values["controller"] = "Error";
                            break;

                    }

                    if (controllerType == null)
                    {
                        // Add Error page here.
                        data.Values["controller"] = "Error";
                        data.Values["action"] = "Notfound";
                        return data;
                    }


                    data.Values["controller"] = controllerType.Name.Replace("Controller", "");
                    data.Values["action"] = "Details";
                    data.Values["Id"] = results.EntityId;

                    // page
                    if (page != null)
                    {
                        data.Values["Page"] = page;
                    }

                }
                else
                {
                    // Add Error page here.
                    data.Values["controller"] = "Error";
                    data.Values["action"] = "Notfound";
                }
            }
            return data;
        }

    }
}