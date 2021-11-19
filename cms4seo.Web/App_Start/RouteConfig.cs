using System.Web.Mvc;
using System.Web.Routing;
using cms4seo.Web;

namespace IdentitySample
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {


            // friendly product ===============================================================

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.ico(/.*)?" });





            routes.MapRoute(
                "slug-tag",
                "tag/{slug}",
                new { controller = "ProductTags", action = "ListBy", slug = UrlParameter.Optional },
                new[] { "cms4seo.Web.Controllers" }
                );



            routes.MapRoute(
                "product-search",
                "search/{productSearch}",
                new { controller = "Product", action = "Search", productSearch = UrlParameter.Optional },
                new[] { "cms4seo.Web.Controllers" }
                );


            routes.MapRoute(
                "product-filter",
                "filter/{district}/{maxPrice}",
                new { controller = "Product", action = "Filter", district = UrlParameter.Optional, maxPrice = UrlParameter.Optional },
                new[] { "cms4seo.Web.Controllers" }
            );


            routes.MapRoute(
                "article-search",
                "article-search/{articleSearch}",
                new { controller = "Articles", action = "Search", articleSearch = UrlParameter.Optional },
                new[] { "cms4seo.Web.Controllers" }
            );





            routes.MapRoute(
                "slug-etag",
                "etag/{slug}",
                new { controller = "ArticleTags", action = "ListBy", slug = UrlParameter.Optional },
                new[] { "cms4seo.Web.Controllers" }
                );






            routes.MapRoute(
                "blog-index",
                "blog",
                new { controller = "Articles", action = "Index" },
                new[] { "cms4seo.Web.Controllers" }
                );


            routes.MapRoute(
                "author",
                "author/{author}",
                new { controller = "Articles", action = "Author", author = UrlParameter.Optional },
                new[] { "cms4seo.Web.Controllers" }
            );



            routes.MapRoute(
                "Error - 400",
                "BadRequest",
                new { controller = "Error", action = "BadRequest" }
                );

            routes.MapRoute(
                "Error - 404",
                "NotFound",
                new { controller = "Error", action = "NotFound" }
                );

            routes.MapRoute(
                "Error - 500",
                "ServerError",
                new { controller = "Error", action = "ServerError" }
                );



            // For Ajax Call
            routes.MapRoute(
                "Contact",
                "Contact/{action}",
                new { controller = "Contact", action = "ContactForm" },
                new[] { "cms4seo.Web.Controllers" }
            );







            routes.MapRoute(
                "Controller",
                "Controller/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                new[] { "cms4seo.Web.Controllers" }
            );



            routes.MapRoute(
                "Plugins",
                "Plugins/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional }
                //, new[] { "cms4seo.Plugins.Controllers" } // better, avoid Multiple matching controllers
            );


            //routes.MapLocalizedRoute(
            //    "SeoFriendlyUrl",
            //    "{SeoFriendlyName}/{page}",
            //    new { controller = "Home", action = "Index", page = UrlParameter.Optional },
            //    new {page = @"\d+"},
            //    new[] { "cms4seo.Web.Controllers" }
            //);


            routes.MapLocalizedRoute(
                "FlatterSiteArchitecture1",
                "{segment1}/{page}",
                new
                {
                    controller = "Home",
                    action = "Index",
                    page = UrlParameter.Optional
                },
                new { page = @"\d+" },
                new[] { "cms4seo.Web.Controllers" }
            );


            routes.MapLocalizedRoute(
                "FlatterSiteArchitecture2",
                "{segment1}/{segment2}/{page}",
                new
                {
                    controller = "Home",
                    action = "Index",
                    segment2 = UrlParameter.Optional,
                    page = UrlParameter.Optional
                },
                new { page = @"\d+" },
                new[] { "cms4seo.Web.Controllers" }
            );


            routes.MapLocalizedRoute(
                "FlatterSiteArchitecture3",
                "{segment1}/{segment2}/{segment3}/{page}",
                new
                {
                    controller = "Home",
                    action = "Index",
                    segment2 = UrlParameter.Optional,
                    segment3 = UrlParameter.Optional,
                    page = UrlParameter.Optional
                },
                new[] { "cms4seo.Web.Controllers" }
            );


            //routes.MapLocalizedRoute(
            //    "FlatterSiteArchitecture4",
            //    "{segment1}/{segment2}/{segment3}/{segment4}/{page}",
            //    new
            //    {
            //        controller = "Home",
            //        action = "Index",
            //        segment2 = UrlParameter.Optional,
            //        segment3 = UrlParameter.Optional,
            //        segment4 = UrlParameter.Optional,
            //        page = UrlParameter.Optional
            //    },
            //    new[] { "cms4seo.Web.Controllers" }
            //);




            routes.MapRoute(
                "Default",
                "{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                new[] { "cms4seo.Web.Controllers" }
                );




        }
    }
}