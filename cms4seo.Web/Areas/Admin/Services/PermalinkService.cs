using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Admin.Resources;
using cms4seo.Common.Helpers;
using cms4seo.Data;
using cms4seo.Model.Entities;
using cms4seo.Model.LekimaxType;
using cms4seo.Service.Product;
using cms4seo.Service.Seo;
using cms4seo.Service.TopicHelper;

namespace cms4seo.Admin.Services
{
    public class PermalinkService
    {

        private readonly ApplicationDbContext db = new ApplicationDbContext();

        private IQueryable<Category> CategoriesValidation =>
            db.Categories.Where(x => !(x.Name == null || x.Name.Trim() == String.Empty));

        private IQueryable<Product> ProductsValidation =>
            db.Products.Where(x => x.Name != null && x.Name.Trim() != String.Empty);

        private IQueryable<Topic> TopicsValidation =>
            db.Topics.Where(x => !(x.Name == null || x.Name.Trim() == String.Empty));

        private IQueryable<Article> ArticlesValidation =>
            db.Articles.Where(x => !(x.Name == null || x.Name.Trim() == String.Empty));

        private IQueryable<Info> InfosValidation =>
            db.Infos.Where(x => !(x.Name == null || x.Name.Trim() == String.Empty));


        /// <summary>
        /// Reindex all link (Note: data.Values["action"] = "Details";)
        /// </summary>
        /// <returns></returns>
        public List<Permalink> Reindex()
        {

            var permalinkList = new List<Permalink>();

            // Category
            var permalinkCategory = db.Categories.Select(
                x => new
                {
                    EntityId = x.Id,
                    EntityName = cms4seoEntityType.Category,
                    Slug = x.Slug,
                    IsActive = true,
                    LanguageId = 0
                }).ToList().Select(
                s => new Permalink
                {
                    EntityId = s.EntityId,
                    EntityName = s.EntityName,
                    Slug = s.Slug,
                    IsActive = s.IsActive,
                    LanguageId = s.LanguageId
                }).ToList();


            // Product
            var permalinkProduct = db.Products.Select(
                x => new
                {
                    EntityId = x.Id,
                    EntityName = cms4seoEntityType.Product,
                    Slug = x.Slug,
                    IsActive = true,
                    LanguageId = 0
                }).ToList().Select(
                s => new Permalink
                {
                    EntityId = s.EntityId,
                    EntityName = s.EntityName,
                    Slug = s.Slug,
                    IsActive = s.IsActive,
                    LanguageId = s.LanguageId
                }).ToList();


            // Topic
            var permalinkTopic = db.Topics.Select(
                x => new
                {
                    EntityId = x.Id,
                    EntityName = cms4seoEntityType.Topic,
                    Slug = x.Slug,
                    IsActive = true,
                    LanguageId = 0
                }).ToList().Select(
                s => new Permalink
                {
                    EntityId = s.EntityId,
                    EntityName = s.EntityName,
                    Slug = s.Slug,
                    IsActive = s.IsActive,
                    LanguageId = s.LanguageId
                }).ToList();



            // Artilce
            var permalinkArtile = db.Articles.Select(
                x => new
                {
                    EntityId = x.Id,
                    EntityName = cms4seoEntityType.Article,
                    Slug = x.Slug,
                    IsActive = true,
                    LanguageId = 0
                }).ToList().Select(
                s => new Permalink
                {
                    EntityId = s.EntityId,
                    EntityName = s.EntityName,
                    Slug = s.Slug,
                    IsActive = s.IsActive,
                    LanguageId = s.LanguageId
                }).ToList();



            // Info
            var permalinkInfo = db.Infos.Select(
                x => new
                {
                    EntityId = x.Id,
                    EntityName = cms4seoEntityType.Info,
                    Slug = x.Slug,
                    IsActive = true,
                    LanguageId = 0
                }).ToList().Select(
                s => new Permalink
                {
                    EntityId = s.EntityId,
                    EntityName = s.EntityName,
                    Slug = s.Slug,
                    IsActive = s.IsActive,
                    LanguageId = s.LanguageId
                }).ToList();



            // add list
            permalinkList.AddRange(permalinkCategory);
            permalinkList.AddRange(permalinkProduct);
            permalinkList.AddRange(permalinkTopic);
            permalinkList.AddRange(permalinkArtile);
            permalinkList.AddRange(permalinkInfo);

            // clear table 
            db.Database.ExecuteSqlCommand("TRUNCATE TABLE [Permalinks]");


            // permalink Comparer
            var permalinkListUnique = new List<Permalink>();


            foreach (var permalink in permalinkList)
            {
                if (permalinkListUnique.Select(x => x.Slug).Contains(permalink.Slug))
                {

                    // make unique slug on permalink
                    var n = 0;
                    do
                    {
                        permalink.Slug = permalink.Slug + "-" + ++n;
                    } while (permalinkListUnique.Count(x => x.Slug == permalink.Slug) > 0);



                    // edit unique slug on cateogry, product, topic, artilce, info, etc

                    if (permalink.EntityName == cms4seoEntityType.Category)
                    {
                        var category = db.Categories.Find(permalink.EntityId);
                        category.Slug = permalink.Slug;
                    }
                    else if (permalink.EntityName == cms4seoEntityType.Product)
                    {
                        var product = db.Products.Find(permalink.EntityId);
                        product.Slug = permalink.Slug;
                    }
                    else if (permalink.EntityName == cms4seoEntityType.Topic)
                    {
                        var topic = db.Topics.Find(permalink.EntityId);
                        topic.Slug = permalink.Slug;
                    }
                    else if (permalink.EntityName == cms4seoEntityType.Article)
                    {
                        var artilce = db.Articles.Find(permalink.EntityId);
                        artilce.Slug = permalink.Slug;
                    }
                    else if (permalink.EntityName == cms4seoEntityType.Info)
                    {
                        var info = db.Infos.Find(permalink.EntityId);
                        info.Slug = permalink.Slug;
                    }
                    else
                    {
                        throw new ArgumentException(AdminResources.PermalinkServiceReindex_Entity_not_found_);
                    }




                    db.SaveChanges();
                }

                permalinkListUnique.Add(permalink);
            }

            // save list
            db.Permalinks.AddRange(permalinkListUnique);
            db.SaveChanges();

            return permalinkListUnique;

        }



        public List<Permalink> Rebuild()
        {
            // clear table 
            db.Database.ExecuteSqlCommand("TRUNCATE TABLE [Permalinks]");


            var permalinkProvider = new PermalinkProvider();

            // wait clear data, try 30 times
            int i = 30;
            while (db.Permalinks.Any())
            {
                // Delay 100ms
                Thread.Sleep(100);

                if (--i <= 0)
                    return null;

            }




            #region category

            var categories = CategoriesValidation.ToList();
            foreach (var category in categories)
            {
                string rootCategoryName = null;

                var rootCategory = category.GetRootCategory();

                if (rootCategory.Id != category.Id)
                    rootCategoryName = rootCategory.Name;

                string secondCategoryName = null;

                if (category.Parent != null && rootCategory.Id != category.Parent.Id)
                    secondCategoryName = category.Parent?.Name.MakeNameFriendly();



                category.Slug = permalinkProvider.Set(category.Id, cms4seoEntityType.Category,
                    rootCategoryName, secondCategoryName, category.Name,
                    true);
            }

            db.SaveChanges();

            // Delay 300ms
            Thread.Sleep(300);

            #endregion category




            #region product

            var products = ProductsValidation.ToList();

            foreach (var product in products)
            {
                var category = product.Category;

                var rootCategory = category.GetRootCategory();

                string secondCategoryName = null;

                if (rootCategory.Id != product.CategoryId)
                    secondCategoryName = category?.Name.MakeNameFriendly();

                product.Slug = permalinkProvider.Set(product.Id, cms4seoEntityType.Product,
                    rootCategory.Name,
                    secondCategoryName,
                    product.Name,
                    true);
            }

            db.SaveChanges();

            // Delay 300ms
            Thread.Sleep(300);

            #endregion product




            #region topic

            var topics = TopicsValidation.ToList();

            foreach (var topic in topics)
            {
                string rootTopicName = null;

                var rootTopic = topic.GetRootTopic();

                if (rootTopic.Id != topic.Id)
                    rootTopicName = rootTopic.Name;

                string secondTopicName = null;

                if (topic.Parent != null && rootTopic.Id != topic.Parent.Id)
                    secondTopicName = topic.Parent?.Name.MakeNameFriendly();


                topic.Slug = permalinkProvider.Set(topic.Id, cms4seoEntityType.Topic,
                    rootTopicName,
                    secondTopicName,
                    topic.Name,
                    true);
            }

            db.SaveChanges();

            // Delay 300ms
            Thread.Sleep(300);


            #endregion topic





            #region article

            var articles = ArticlesValidation.ToList();

            foreach (var article in articles)
            {

                string secondTopicName = null;

                if (article.Topic.GetRootTopic().Id != article.TopicId)
                    secondTopicName = article.Topic?.Name.MakeNameFriendly();


                article.Slug = permalinkProvider.Set(article.Id, cms4seoEntityType.Article,
                    article.Topic.GetRootTopic().Name,
                    secondTopicName,
                    article.Name,
                    true);
            }

            db.SaveChanges();

            // Delay 300ms
            Thread.Sleep(300);

            #endregion article





            #region page

            var infos = InfosValidation.ToList();

            foreach (var info in infos)
            {

                info.Slug = permalinkProvider.Set(info.Id, cms4seoEntityType.Info,
                    null,
                    null,
                    info.Name,
                    true);
            }

            db.SaveChanges();

            // Delay 300ms
            Thread.Sleep(300);

            #endregion page



            var permalinks = db.Permalinks.ToList();

            //LogHelper.Write(@"Admin/Test",
            //    $"Test mode: [{permalinks.Count}]");

            return permalinks;
        }
    }
}