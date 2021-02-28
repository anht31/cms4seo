using System.Web.Mvc;
using System.Web;

namespace cms4seo.Plugins.Test
{
    public class ShoppingCartAreaRegistration : AreaRegistration
    {
        public override string AreaName => "ShoppingCart";

        public override void RegisterArea(AreaRegistrationContext context)
        {
            // route Localization =============================================
            //context.MapRoute(
            //    "Test_route_Localization",
            //    "ShoppingCart/Localization/",
            //    new { controller = "Home", action = "Html", id = UrlParameter.Optional },
            //    new string[] { "cms4seo.Plugins.ShoppingCart.Controllers" }
            //);

            // Default route =============================================
            context.MapRoute(
                "ShoppingCart",
                "ShoppingCart/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                new[] { "cms4seo.Plugins.ShoppingCart.Controllers" }
            );

            // Default Test area =============================================
            context.MapRoute(
                "ShoppingCart_Default",
                "ShoppingCart/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                new[] { "cms4seo.Plugins.ShoppingCart.Controllers" }
            );
        }
    }
}