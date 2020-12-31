using System.Web.Mvc;
using System.Xml;
using cms4seo.Model.Entities;
using cms4seo.Service.Resolver;

namespace cms4seo.Service.Sitemap
{
    public class SiteMapResult : ActionResult
    {
        private SiteMap _siteMap;

        public SiteMapResult(SiteMap siteMap)
        {
            _siteMap = siteMap;
        }

        public override void ExecuteResult(ControllerContext context)
        {

            context.HttpContext.Response.ContentType = "text/xml";

            var siteMapWriter = new SiteMapWriter(_siteMap);

            using (var output = XmlWriter.Create(context.HttpContext.Response.Output))
            {
                siteMapWriter.WriteTo(output);
            }
        }

    }
}