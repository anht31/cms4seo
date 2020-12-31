using System.Linq;
using System.Xml;
using System.Xml.Linq;
using cms4seo.Model.Entities;

namespace cms4seo.Service.Resolver
{
    public class SiteMapWriter
    {
        private SiteMap _siteMap;

        public SiteMapWriter(SiteMap siteMap)
        {
            _siteMap = siteMap;
        }

        public void WriteTo(XmlWriter writer)
        {
            XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
            var xml = new XDocument(new XDeclaration("1.0", "utf-8", "yes"),
                new XElement(ns + "urlset",
                        _siteMap.Items
                        .Select(item => new XElement(ns + "url",
                            new XElement(ns + "loc", item.Loc),
                                        item.LastMod.HasValue ? new XElement(ns + "lastmod", item.LastMod.Value.ToString("s") + "Z") : null,
                                        item.ChangeFreq != ChangeFrequency.NotSet ? new XElement(ns + "changefreq", item.ChangeFreq.ToString().ToLower()) : null,
                                        item.Priority.HasValue ? new XElement(ns + "priority", item.Priority.Value) : null
                                    )
                                )
                            )
                        );
            xml.Save(writer);
        }

    }
}