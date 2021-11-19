using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml;
using cms4seo.Data;
using cms4seo.Model.Entities;
using cms4seo.Service.Resolver;

namespace cms4seo.Service.Sitemap
{
    public class SiteMapService
    {
        string _protocolHostPort;
        public SiteMapService(string url)
        {            
            Uri uri = new Uri(url);
            _protocolHostPort = uri.Scheme + Uri.SchemeDelimiter + uri.Host + ":" + uri.Port;
        }

        public SiteMap GetSiteMap()
        {

            CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;


            using (ApplicationDbContext context = new ApplicationDbContext())
            {

                var items = new List<SiteMapItem>();

                // Product
                var siteMapProducts = context.Products
                    .Where(x => !(x.Name == null || x.Name.Trim() == String.Empty))
                    .Select(a => new
                {
                    a.Id,
                    a.Slug,
                    LastModified = a.DateAmended,
                    DateCreated = a.Created
                }).ToList().Select(i => new SiteMapItem
                {
                    Loc = new Uri($"{_protocolHostPort}{i.Slug}"),
                    ChangeFreq = ChangeFrequency.Daily,
                    LastMod = i.LastModified ?? i.DateCreated,
                    Priority = 0.9
                }).ToList();


                // Category 
                var siteMapCategories = context.Categories
                    .Where(x => !(x.Name == null || x.Name.Trim() == String.Empty))
                    .Select(a => new
                {
                    a.Id,
                    a.Slug,
                    LastModified = a.DateAmended,
                    DateCreated = a.DateCreated
                }).ToList().Select(i => new SiteMapItem
                {
                    Loc = new Uri($"{_protocolHostPort}{i.Slug}"),
                    ChangeFreq = ChangeFrequency.Daily,
                    LastMod = i.LastModified ?? i.DateCreated,
                    Priority = 0.9
                }).ToList();


                // Extra
                var siteMapExtras = context.ExtraSiteMaps
                    .Where(x => !(x.Name == null || x.Name.Trim() == String.Empty))
                    .Select(a => new
                {
                    a.Id,
                    a.Name,
                    LastModified = a.LastMod,
                    DateCreated = a.LastMod
                }).ToList().Select(i => new SiteMapItem
                {
                    Loc = new Uri($"{_protocolHostPort}{i.Name}"),
                    ChangeFreq = ChangeFrequency.Daily,
                    LastMod = i.LastModified ?? i.DateCreated,
                    Priority = 0.9
                }).ToList();


                // Topics 
                var siteMapTopics = context.Topics
                    .Where(x => !(x.Name == null || x.Name.Trim() == String.Empty))
                    .Select(a => new
                {
                    a.Id,
                    a.Slug,
                    LastModified = a.DateAmended,
                    DateCreated = a.DateCreated
                }).ToList().Select(i => new SiteMapItem
                {
                    Loc = new Uri($"{_protocolHostPort}{i.Slug}"),
                    ChangeFreq = ChangeFrequency.Daily,
                    LastMod = i.LastModified ?? i.DateCreated,
                    Priority = 0.8
                }).ToList();


                // Article 
                var siteMapArticles = context.Articles
                    .Where(x => !(x.Name == null || x.Name.Trim() == String.Empty))
                    .Where(x => x.IsPublish).Select(a => new
                {
                    a.Id,
                    a.Slug,
                    LastModified = a.PostedDate,
                    DateCreated = a.DateCreated
                }).ToList().Select(i => new SiteMapItem
                {
                    Loc = new Uri($"{_protocolHostPort}{i.Slug}"),
                    ChangeFreq = ChangeFrequency.Daily,
                    LastMod = i.LastModified,
                    Priority = 0.8
                }).ToList();

                // Infos 
                var siteMapInfos = context.Infos
                    .Where(x => !(x.Name == null || x.Name.Trim() == String.Empty))
                    .Select(a => new
                {
                    a.Id,
                    a.Slug,
                    LastModified = a.DateAmended,
                    DateCreated = a.DateCreated
                }).ToList().Select(i => new SiteMapItem
                {
                    Loc = new Uri($"{_protocolHostPort}{i.Slug}"),
                    ChangeFreq = ChangeFrequency.Daily,
                    LastMod = i.LastModified ?? i.DateCreated,
                    Priority = 0.8
                }).ToList();



                // Add root domain to site map
                items.Add(new SiteMapItem
                {
                    Loc = new Uri(_protocolHostPort),
                    ChangeFreq = ChangeFrequency.Weekly,
                    Priority = 1.00
                });


                items.AddRange(siteMapProducts);
                items.AddRange(siteMapCategories);
                items.AddRange(siteMapExtras);
                items.AddRange(siteMapTopics);
                items.AddRange(siteMapArticles);
                items.AddRange(siteMapInfos);
                

                return new SiteMap { Items = items };
            }
        }


        public void SaveSiteMap(SiteMap siteMap)
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"sitemap.xml");
            var siteMapWriter = new SiteMapWriter(siteMap);

            using (var output = XmlWriter.Create(path))
            {
                siteMapWriter.WriteTo(output);
            }
        }


        public void SubmitSiteMap()
        {                        
            var siteMap = GetSiteMap();
            SaveSiteMap(siteMap);
        }

    }
}