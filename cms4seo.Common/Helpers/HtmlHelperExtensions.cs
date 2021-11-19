using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Optimization;

namespace cms4seo.Common.Helpers
{
    public static class HtmlHelperExtensions
    {
        public static IHtmlString InlineScripts(this HtmlHelper htmlHelper, string bundleVirtualPath)
        {
            return htmlHelper.InlineBundle(bundleVirtualPath, htmlTagName: "script");
        }

        public static IHtmlString InlineStyles(this HtmlHelper htmlHelper, string bundleVirtualPath)
        {
            return htmlHelper.InlineBundle(bundleVirtualPath, htmlTagName: "style");
        }

        private static IHtmlString InlineBundle(this HtmlHelper htmlHelper, string bundleVirtualPath, string htmlTagName)
        {
            string bundleContent = LoadBundleContent(htmlHelper.ViewContext.HttpContext, bundleVirtualPath);
            string htmlTag = string.Format("<{0}>{1}</{0}>", htmlTagName, bundleContent);

            return new HtmlString(htmlTag);
        }

        private static string LoadBundleContent(HttpContextBase httpContext, string bundleVirtualPath)
        {
            var bundleContext = new BundleContext(httpContext, BundleTable.Bundles, bundleVirtualPath);
            var bundle = BundleTable.Bundles.Single(b => b.Path == bundleVirtualPath);
            var bundleResponse = bundle.GenerateBundleResponse(bundleContext);

            return bundleResponse.Content;
        }


        public static IHtmlString NonCriticalStyle(this HtmlHelper htmlHelper, string bundleVirtualPath)
        {
            return Styles.RenderFormat("<link rel=\"preload\" href=\"{0}\" as=\"style\" onload=\"this.rel='stylesheet'\">", bundleVirtualPath);
        }

        public static IHtmlString DeferScript(this HtmlHelper htmlHelper, string bundleVirtualPath)
        {
            return Scripts.RenderFormat(@"<script src='{0}' defer></script>", bundleVirtualPath);
        }


        public static IHtmlString InlinePlatform(this HtmlHelper htmlHelper, string path)
        {
            var content = File.ReadAllText(HostingEnvironment.MapPath(path));

            string htmlTag = string.Format("<{0}>{1}</{0}>", "style", content);

            return new HtmlString(htmlTag);
        }

        public static IHtmlString InlinePlatformStyle(this HtmlHelper htmlHelper, string path)
        {
            var content = File.ReadAllText(HostingEnvironment.MapPath(path));

            string htmlTag = string.Format("<{0}>{1}</{0}>", "style", content);

            return new HtmlString(htmlTag);
        }

        public static IHtmlString InlinePlatformScript(this HtmlHelper htmlHelper, string path)
        {
            var content = File.ReadAllText(HostingEnvironment.MapPath(path));

            string htmlTag = string.Format("<{0}>{1}</{0}>", "script", content);

            return new HtmlString(htmlTag);
        }



        //public static void Widgets(this HtmlHelper htmlHelper, string zone)
        //{
        //    foreach (var widget in PluginHelpers.Widgets)
        //    {
        //        if(widget.Zone == zone)
        //            htmlHelper.RenderAction(widget.Action, widget.Controller, new { area = widget.Area });
        //    }

        //}

    }

}