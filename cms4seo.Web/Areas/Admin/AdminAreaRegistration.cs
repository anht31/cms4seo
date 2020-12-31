using System.Web.Mvc;

namespace cms4seo.Admin
{
    public class AdminAreaRegistration : AreaRegistration
    {
        public override string AreaName => "Admin";

        public override void RegisterArea(AreaRegistrationContext context)
        {
            // route Localization =============================================
            //context.MapRoute(
            //    "Admin_route_Localization",
            //    "Admin/Localization/",
            //    new { controller = "Home", action = "Html", id = UrlParameter.Optional },
            //    new string[] { "cms4seo.Controllers" }
            //);

            // Default route =============================================
            context.MapRoute(
                "Admin_route",
                "Admin/{controller}/{action}/{id}",
                new {controller = "Home", action = "Index", id = UrlParameter.Optional},
                new[] { "cms4seo.Admin.Controllers" }
                );

            // Default admin area =============================================
            context.MapRoute(
                "Admin_default",
                "Admin/{controller}/{action}/{id}",
                new {action = "Index", id = UrlParameter.Optional},
                new[] { "cms4seo.Admin.Controllers" }
                );
        }
    }
}