using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using cms4seo.Common.Helpers;
using cms4seo.Model.Entities;

namespace cms4seo.Service.Sitemap
{
    public class SiteMappingProvider
    {
        // File path        
        private static string _filePath;

        string _url;

        public SiteMappingProvider(string url)
        {
            _url = url;

            _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"sitemap.xml");            

            if (!File.Exists(_filePath))
                throw new FileNotFoundException($"XML Resource file {_filePath} was not found");
        }



        public XDocument Document => XDocument.Parse(File.ReadAllText(_filePath));

        public XElement Root => Document.Root;

        public IEnumerable<XElement> Urls => Root.Elements();

        public XNamespace XmlNameSpace => Root.Name.NamespaceName;

        public bool Add(SiteMapItem item)
        {            

            try
            {
                var element =
                    new XElement(XmlNameSpace + "url",
                        new XElement(XmlNameSpace + "loc", item.Loc ),
                        item.LastMod.HasValue ? new XElement(XmlNameSpace + "lastmod", item.LastMod.Value.ToString("s") + "Z") : null,
                        item.ChangeFreq != ChangeFrequency.NotSet
                            ? new XElement(XmlNameSpace + "changefreq", item.ChangeFreq.ToString().ToLower())
                            : null,
                        item.Priority.HasValue ? new XElement(XmlNameSpace + "priority", item.Priority.Value) : null
                        );

                
                var doc = new XDocument(Root);                

                doc.Root?.Add(element);

                doc.Save(_filePath);
            }
            catch
            {
                return false;
            }


            return true;
        }

        public string Delete(string url)
        {

            //debug
            //LogHelper.Write("Admin/SiteMapProvider", $"Check url: {url ?? "null"}");

            if (!LinkIsNull(url))
            {
                var doc = new XDocument(Root);

                doc.Root?.Elements(XmlNameSpace + "url").
                    Where(x => x.Element(XmlNameSpace + "loc")?.Value == $"{url}" ).Remove();

                doc.Save(_filePath);

                //LogHelper.Write("Admin/SiteMapProvider", $"debug delete success");

                return url;
                
            }

            return null;
        }


        public bool LinkIsNull(string url)
        {

            try
            {
                if (Root != null 
                    && Urls != null 
                    && Urls.Any() 
                    && Urls.Any(e => e.Element(XmlNameSpace + "loc")?.Value == url))
                        return false;            
            }
            catch
            {
                if (!File.Exists(_filePath))
                {
                    // resubmit sitemap           
                    var service = new SiteMapService(_url);
                    var siteMap = service.GetSiteMap();
                    service.SaveSiteMap(siteMap);

                    LogHelper.Write("SiteMapProvider", $"Reindex site for not found sitemap.xml");
                }
                    
                return true;
            }

            if (!File.Exists(_filePath))
            {
                // resubmit sitemap
                var service = new SiteMapService(_url);
                var siteMap = service.GetSiteMap();
                service.SaveSiteMap(siteMap);

                LogHelper.Write("SiteMapProvider", $"Reindex site for not found sitemap.xml");
            }

            return true;
        }

    }
}