using System.Web.Mvc;

namespace cms4seo.Plugins.Test
{
    public class PluginTestAreaRegistration : AreaRegistration
    {
        public override string AreaName => "PluginTest";

        public override void RegisterArea(AreaRegistrationContext context)
        {
            // route Localization =============================================
            //context.MapRoute(
            //    "Test_route_Localization",
            //    "pluginTest/Localization/",
            //    new { controller = "Home", action = "Html", id = UrlParameter.Optional },
            //    new string[] { "cms4seo.Plugins.Test.Controllers" }
            //);

            // Default route =============================================
            context.MapRoute(
                "Test_route",
                "PluginTest/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                new[] { "cms4seo.Plugins.Test.Controllers" }
            );

            // Default Test area =============================================
            context.MapRoute(
                "Test_default",
                "PluginTest/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                new[] { "cms4seo.Plugins.Test.Controllers" }
            );
        }
    }
}