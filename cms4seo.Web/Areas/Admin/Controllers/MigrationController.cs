using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Configuration;
using System.Web.Mvc;
using Admin.Resources;
using cms4seo.Common.Attribute;
using cms4seo.Common.BootstrapHelpers;
using cms4seo.Common.Helpers;
using cms4seo.Data;
using cms4seo.Data.ConnectionString;
using cms4seo.Data.IdentityModels;
using cms4seo.Model.Entities;
using cms4seo.Model.LekimaxType;
using cms4seo.Service.Provider;
using System.Text.RegularExpressions;
using cms4seo.Data.Repositories;
using System.Drawing;
using System.IO;
using System.Web.Hosting;
using System.Web.WebPages;
using cms4seo.Common.TurboJpeg;
using WebGrease.Css.Extensions;

namespace cms4seo.Admin.Controllers
{

    [CustomAuthorize(Roles = "Advance.Migration")]
    public class MigrationController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private int count;

        //static string _projectId = WebConfigurationManager.AppSettings["ProjectId"];

        // GET: Admin/Migration
        public ActionResult Index()
        {
            string themesPath = Server.MapPath("/Themes");
            var directories = Directory.GetDirectories(themesPath).Select(Path.GetFileName);
            var dictionaries = directories.ToDictionary(item => item.Trim(), item => item.Trim());

            Dictionary<string, string> dictionaryThemes = new Dictionary<string, string>
            {
                { "", "" }
            };

            foreach (var item in dictionaries)
                dictionaryThemes.Add(item.Key, item.Value);

            ViewBag.ThemeList = dictionaryThemes;

            return View();
        }



        public ActionResult AddSlashBeforeAllLink()
        {
            count = 0;


            // category
            var categories = db.Categories.ToList();
            foreach (var category in categories)
            {
                if (!category.Slug.Contains("/"))
                {
                    category.Slug = "/" + category.Slug;
                    count++;
                }

                db.Entry(category).State = EntityState.Modified;
            }

            db.SaveChanges();

            // product
            var products = db.Products.ToList();
            foreach (var product in products)
            {
                if (!product.Slug.Contains("/"))
                {
                    product.Slug = "/" + product.Slug;
                    count++;
                }

                db.Entry(product).State = EntityState.Modified;
            }

            db.SaveChanges();


            // topic
            var topics = db.Topics.ToList();
            foreach (var topic in topics)
            {
                if (!topic.Slug.Contains("/"))
                {
                    topic.Slug = "/" + topic.Slug;
                    count++;
                }

                db.Entry(topic).State = EntityState.Modified;
            }

            db.SaveChanges();



            // article
            var articles = db.Articles.ToList();
            foreach (var article in articles)
            {
                if (!article.Slug.Contains("/"))
                {
                    article.Slug = "/" + article.Slug;
                    count++;
                }

                db.Entry(article).State = EntityState.Modified;
            }

            db.SaveChanges();


            // info
            var infoes = db.Infos.ToList();
            foreach (var info in infoes)
            {
                if (!info.Slug.Contains("/"))
                {
                    info.Slug = "/" + info.Slug;
                    count++;
                }

                db.Entry(info).State = EntityState.Modified;
            }

            db.SaveChanges();



            // productTag
            var productTags = db.ProductTags.ToList();
            foreach (var productTag in productTags)
            {
                if (!productTag.Slug.Contains("/"))
                {
                    productTag.Slug = "/" + productTag.Slug;
                    count++;
                }

                db.Entry(productTag).State = EntityState.Modified;
            }

            db.SaveChanges();



            // articleTag
            var articleTags = db.ArticleTags.ToList();
            foreach (var articleTag in articleTags)
            {
                if (!articleTag.Slug.Contains("/"))
                {
                    articleTag.Slug = "/" + articleTag.Slug;
                    count++;
                }

                db.Entry(articleTag).State = EntityState.Modified;
            }

            db.SaveChanges();

            TempData[MessageType.Warning] = 
                string.Format(AdminResources.MigrationControllerAddSlashBeforeAllLink_AddSlashBeforeAllLink_successfull____0___items_submited, count);

            LogHelper.Write(@"Admin/SearchOptimize",
                $"User: {User.Identity.Name} AddSlashBeforeAllLink, total items: [{count}]");

            return RedirectToAction("Index");
        }





        public ActionResult GuidToProductFirst()
        {
            count = 0;



            // category
            var categories = db.Categories.ToList();
            foreach (var category in categories)
                Update(category, cms4seoEntityType.Category);

            var products = db.Products.ToList();
            foreach (var product in products)
                Update(product, cms4seoEntityType.Product);

            var topics = db.Topics.ToList();
            foreach (var topic in topics)
                Update(topic, cms4seoEntityType.Topic);

            var articles = db.Articles.ToList();
            foreach (var article in articles)
                Update(article, cms4seoEntityType.Article);

            var infos = db.Infos.ToList();
            foreach (var info in infos)
                Update(info, cms4seoEntityType.Info);


            // slider (exception because slider not inherit from Seo class
            var sliders = db.Sliders.ToList();
            foreach (var slider in sliders)
            {
                foreach (var photo in slider.Photos)
                {
                    photo.Entity = cms4seoEntityType.Slider;
                    photo.ModelId = slider.Id;
                    count++;
                    db.Entry(photo).State = EntityState.Modified;
                }
            }

            db.SaveChanges();

            TempData[MessageType.Success] = 
                string.Format(AdminResources.MigrationControllerGuidToProductFirst_GuidToProductFirst_successful____0___items_submitted, count);
            LogHelper.Write(@"Admin/SearchOptimize",
                $"User: {User.Identity.Name} GuidToProductFirst, total items: [{count}]");

            return RedirectToAction("Index");
        }





        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }



        private void Update<T>(T item, string lekimaxEntityType) where T : Seo
        {

            foreach (var itemPhoto in item.Photos)
            {
                var photo = db.Photos.Find(itemPhoto.Id);

                if (photo == null)
                    return;

                photo.Entity = lekimaxEntityType;
                photo.ModelId = item.Id;


                //db.Entry(photo).CurrentValues.SetValues(item);
                db.Entry(photo).State = EntityState.Modified;

                count++;
            }


            db.SaveChanges();
        }





        public ActionResult PhotoPath()
        {
            count = 0;


            var photos = db.Photos.ToList();
            foreach (var photo in photos)
            {
                photo.LgPath = photo.LgPath.Replace("/Photo/category/", $"/Photo/{cms4seoEntityType.Category}/");
                photo.LgPath = photo.LgPath.Replace("/Photo/product/", $"/Photo/{cms4seoEntityType.Product}/");
                photo.LgPath = photo.LgPath.Replace("/Photo/topic/", $"/Photo/{cms4seoEntityType.Topic}/");
                photo.LgPath = photo.LgPath.Replace("/Photo/article/", $"/Photo/{cms4seoEntityType.Article}/");
                photo.LgPath = photo.LgPath.Replace("/Photo/info/", $"/Photo/{cms4seoEntityType.Info}/");
                photo.LgPath = photo.LgPath.Replace("/Photo/slider/", $"/Photo/{cms4seoEntityType.Slider}/");



                photo.MdPath = photo.MdPath.Replace("/Photo/category/", $"/Photo/{cms4seoEntityType.Category}/");
                photo.MdPath = photo.MdPath.Replace("/Photo/product/", $"/Photo/{cms4seoEntityType.Product}/");
                photo.MdPath = photo.MdPath.Replace("/Photo/topic/", $"/Photo/{cms4seoEntityType.Topic}/");
                photo.MdPath = photo.MdPath.Replace("/Photo/article/", $"/Photo/{cms4seoEntityType.Article}/");
                photo.MdPath = photo.MdPath.Replace("/Photo/info/", $"/Photo/{cms4seoEntityType.Info}/");
                photo.MdPath = photo.MdPath.Replace("/Photo/slider/", $"/Photo/{cms4seoEntityType.Slider}/");



                photo.SmPath = photo.SmPath.Replace("/Photo/category/", $"/Photo/{cms4seoEntityType.Category}/");
                photo.SmPath = photo.SmPath.Replace("/Photo/product/", $"/Photo/{cms4seoEntityType.Product}/");
                photo.SmPath = photo.SmPath.Replace("/Photo/topic/", $"/Photo/{cms4seoEntityType.Topic}/");
                photo.SmPath = photo.SmPath.Replace("/Photo/article/", $"/Photo/{cms4seoEntityType.Article}/");
                photo.SmPath = photo.SmPath.Replace("/Photo/info/", $"/Photo/{cms4seoEntityType.Info}/");
                photo.SmPath = photo.SmPath.Replace("/Photo/slider/", $"/Photo/{cms4seoEntityType.Slider}/");

                count++;
                count++;
                count++;
            }


            var categories = db.Categories.ToList();
            foreach (var category in categories)
            {
                category.Avatar = category.Avatar?.Replace("/Photo/category/", $"/Photo/{cms4seoEntityType.Category}/");
                db.Entry(category).State = EntityState.Modified;
                count++;
            }
            db.SaveChanges();


            var products = db.Products.ToList();
            foreach (var product in products)
            {
                product.Avatar = product.Avatar?.Replace("/Photo/product/", $"/Photo/{cms4seoEntityType.Product}/");
                db.Entry(product).State = EntityState.Modified;
                count++;
            }
            db.SaveChanges();


            var topics = db.Topics.ToList();
            foreach (var topic in topics)
            {
                topic.Avatar = topic.Avatar?.Replace("/Photo/topic/", $"/Photo/{cms4seoEntityType.Topic}/");
                db.Entry(topic).State = EntityState.Modified;
                count++;
            }
            db.SaveChanges();


            var articles = db.Articles.ToList();
            foreach (var article in articles)
            {
                article.Avatar = article.Avatar?.Replace("/Photo/article/", $"/Photo/{cms4seoEntityType.Article}/");
                db.Entry(article).State = EntityState.Modified;
                count++;
            }
            db.SaveChanges();


            var infos = db.Infos.ToList();
            foreach (var info in infos)
            {
                info.Avatar = info.Avatar?.Replace("/Photo/info/", $"/Photo/{cms4seoEntityType.Info}/");
                db.Entry(info).State = EntityState.Modified;
                count++;
            }
            db.SaveChanges();


            var sliders = db.Sliders.ToList();
            foreach (var slider in sliders)
            {
                slider.Avatar = slider.Avatar?.Replace("/Photo/slider/", $"/Photo/{cms4seoEntityType.Slider}/");
                slider.AvatarMobile = slider.AvatarMobile?.Replace("/Photo/slider/", $"/Photo/{cms4seoEntityType.Slider}/");
                db.Entry(slider).State = EntityState.Modified;
                count++;
            }
            db.SaveChanges();


            // photo in product content
            foreach (var product in products)
            {
                product.Description = product.Description?.Replace($"/Photo/product/", $"/Photo/{cms4seoEntityType.Product}/");
                product.UnsignContent = product.UnsignContent?.Replace($"/Photo/product/", $"/Photo/{cms4seoEntityType.Product}/");
                db.Entry(product).State = EntityState.Modified;
                count++;
            }
            db.SaveChanges();


            // photo in article content
            foreach (var article in articles)
            {
                article.Content = article.Content?.Replace("/Photo/article/", $"/Photo/{cms4seoEntityType.Article}/");
                article.UnsignContent = article.UnsignContent?.Replace("/Photo/article/", $"/Photo/{cms4seoEntityType.Article}/");
                db.Entry(article).State = EntityState.Modified;
                count++;
            }
            db.SaveChanges();


            TempData[MessageType.Success] =
                string.Format(AdminResources.MigrationControllerPhotoPath_PhotoPath_successful____0___items_submitted, count);

            LogHelper.Write(@"Admin/SearchOptimize",
                $"User: {User.Identity.Name} PhotoPath, total items: [{count}]");

            return RedirectToAction("Index");

        }





        public ActionResult PhotoPathToProjectId()
        {

            string projectId = WebConfigurationManager.AppSettings["ProjectId"];

            count = 0;


            var photos = db.Photos.ToList();
            foreach (var photo in photos)
            {
                photo.LgPath = photo.LgPath.Replace($"/Photo/{cms4seoEntityType.Category}/", $"/Photo/{projectId}/{cms4seoEntityType.Category}/");
                photo.LgPath = photo.LgPath.Replace($"/Photo/{cms4seoEntityType.Product}/", $"/Photo/{projectId}/{cms4seoEntityType.Product}/");
                photo.LgPath = photo.LgPath.Replace($"/Photo/{cms4seoEntityType.Topic}/", $"/Photo/{projectId}/{cms4seoEntityType.Topic}/");
                photo.LgPath = photo.LgPath.Replace($"/Photo/{cms4seoEntityType.Article}/", $"/Photo/{projectId}/{cms4seoEntityType.Article}/");
                photo.LgPath = photo.LgPath.Replace($"/Photo/{cms4seoEntityType.Info}/", $"/Photo/{projectId}/{cms4seoEntityType.Info}/");
                photo.LgPath = photo.LgPath.Replace($"/Photo/{cms4seoEntityType.Slider}/", $"/Photo/{projectId}/{cms4seoEntityType.Slider}/");



                photo.MdPath = photo.MdPath.Replace($"/Photo/{cms4seoEntityType.Category}/", $"/Photo/{projectId}/{cms4seoEntityType.Category}/");
                photo.MdPath = photo.MdPath.Replace($"/Photo/{cms4seoEntityType.Product}/", $"/Photo/{projectId}/{cms4seoEntityType.Product}/");
                photo.MdPath = photo.MdPath.Replace($"/Photo/{cms4seoEntityType.Topic}/", $"/Photo/{projectId}/{cms4seoEntityType.Topic}/");
                photo.MdPath = photo.MdPath.Replace($"/Photo/{cms4seoEntityType.Article}/", $"/Photo/{projectId}/{cms4seoEntityType.Article}/");
                photo.MdPath = photo.MdPath.Replace($"/Photo/{cms4seoEntityType.Info}/", $"/Photo/{projectId}/{cms4seoEntityType.Info}/");
                photo.MdPath = photo.MdPath.Replace($"/Photo/{cms4seoEntityType.Slider}/", $"/Photo/{projectId}/{cms4seoEntityType.Slider}/");



                photo.SmPath = photo.SmPath.Replace($"/Photo/{cms4seoEntityType.Category}/", $"/Photo/{projectId}/{cms4seoEntityType.Category}/");
                photo.SmPath = photo.SmPath.Replace($"/Photo/{cms4seoEntityType.Product}/", $"/Photo/{projectId}/{cms4seoEntityType.Product}/");
                photo.SmPath = photo.SmPath.Replace($"/Photo/{cms4seoEntityType.Topic}/", $"/Photo/{projectId}/{cms4seoEntityType.Topic}/");
                photo.SmPath = photo.SmPath.Replace($"/Photo/{cms4seoEntityType.Article}/", $"/Photo/{projectId}/{cms4seoEntityType.Article}/");
                photo.SmPath = photo.SmPath.Replace($"/Photo/{cms4seoEntityType.Info}/", $"/Photo/{projectId}/{cms4seoEntityType.Info}/");
                photo.SmPath = photo.SmPath.Replace($"/Photo/{cms4seoEntityType.Slider}/", $"/Photo/{projectId}/{cms4seoEntityType.Slider}/");

                count++;
                count++;
                count++;

                db.Entry(photo).State = EntityState.Modified;
            }

            db.SaveChanges();


            var categories = db.Categories.ToList();
            foreach (var category in categories)
            {
                category.Avatar = category.Avatar?.Replace($"/Photo/{cms4seoEntityType.Category}/", $"/Photo/{projectId}/{cms4seoEntityType.Category}/");
                db.Entry(category).State = EntityState.Modified;
                count++;
            }
            db.SaveChanges();


            var products = db.Products.ToList();
            foreach (var product in products)
            {
                product.Avatar = product.Avatar?.Replace($"/Photo/{cms4seoEntityType.Product}/", $"/Photo/{projectId}/{cms4seoEntityType.Product}/");
                db.Entry(product).State = EntityState.Modified;
                count++;
            }
            db.SaveChanges();


            var topics = db.Topics.ToList();
            foreach (var topic in topics)
            {
                topic.Avatar = topic.Avatar?.Replace($"/Photo/{cms4seoEntityType.Topic}/", $"/Photo/{projectId}/{cms4seoEntityType.Topic}/");
                db.Entry(topic).State = EntityState.Modified;
                count++;
            }
            db.SaveChanges();


            var articles = db.Articles.ToList();
            foreach (var article in articles)
            {
                article.Avatar = article.Avatar?.Replace($"/Photo/{cms4seoEntityType.Article}/", $"/Photo/{projectId}/{cms4seoEntityType.Article}/");
                db.Entry(article).State = EntityState.Modified;
                count++;
            }
            db.SaveChanges();


            var infos = db.Infos.ToList();
            foreach (var info in infos)
            {
                info.Avatar = info.Avatar?.Replace($"/Photo/{cms4seoEntityType.Info}/", $"/Photo/{projectId}/{cms4seoEntityType.Info}/");
                db.Entry(info).State = EntityState.Modified;
                count++;
            }
            db.SaveChanges();


            var sliders = db.Sliders.ToList();
            foreach (var slider in sliders)
            {
                slider.Avatar = slider.Avatar?.Replace($"/Photo/{cms4seoEntityType.Slider}/", $"/Photo/{projectId}/{cms4seoEntityType.Slider}/");
                slider.AvatarMobile = slider.AvatarMobile?.Replace($"/Photo/{cms4seoEntityType.Slider}/", $"/Photo/{projectId}/{cms4seoEntityType.Slider}/");
                db.Entry(slider).State = EntityState.Modified;
                count++;
            }
            db.SaveChanges();

            // photo in product content
            foreach (var product in products)
            {
                product.Description = product.Description?.Replace($"/Photo/{cms4seoEntityType.Product}/", $"/Photo/{projectId}/{cms4seoEntityType.Product}/");
                product.UnsignContent = product.UnsignContent?.Replace($"/Photo/{cms4seoEntityType.Product}/", $"/Photo/{projectId}/{cms4seoEntityType.Product}/");
                db.Entry(product).State = EntityState.Modified;
                count++;
            }
            db.SaveChanges();

            // photo in article content
            foreach (var article in articles)
            {
                article.Content = article.Content?.Replace($"/Photo/{cms4seoEntityType.Article}/", $"/Photo/{projectId}/{cms4seoEntityType.Article}/");
                article.UnsignContent = article.UnsignContent?.Replace($"/Photo/{cms4seoEntityType.Article}/", $"/Photo/{projectId}/{cms4seoEntityType.Article}/");
                db.Entry(article).State = EntityState.Modified;
                count++;
            }
            db.SaveChanges();


            TempData[MessageType.Success] =
                string.Format(AdminResources.MigrationControllerPhotoPath_PhotoPath_successful____0___items_submitted, count);

            LogHelper.Write(@"Admin/SearchOptimize",
                $"User {User.Identity.Name} PhotoPathToProjectId, total items {count}");

            return RedirectToAction("Index");

        }



        public ActionResult ProjectIdToUploads()
        {

            string projectId1 = WebConfigurationManager.AppSettings["ProjectId"].Translate();
            string projectId2 = projectId1;

            count = 0;


            var photos = db.Photos.ToList();
            foreach (var photo in photos)
            {
                photo.LgPath = photo.LgPath.Replace($"/Photo/{projectId1}/{cms4seoEntityType.Category}/", $"/Uploads/{projectId2}/Photo/{cms4seoEntityType.Category}/");
                photo.LgPath = photo.LgPath.Replace($"/Photo/{projectId1}/{cms4seoEntityType.Product}/", $"/Uploads/{projectId2}/Photo/{cms4seoEntityType.Product}/");
                photo.LgPath = photo.LgPath.Replace($"/Photo/{projectId1}/{cms4seoEntityType.Topic}/", $"/Uploads/{projectId2}/Photo/{cms4seoEntityType.Topic}/");
                photo.LgPath = photo.LgPath.Replace($"/Photo/{projectId1}/{cms4seoEntityType.Article}/", $"/Uploads/{projectId2}/Photo/{cms4seoEntityType.Article}/");
                photo.LgPath = photo.LgPath.Replace($"/Photo/{projectId1}/{cms4seoEntityType.Info}/", $"/Uploads/{projectId2}/Photo/{cms4seoEntityType.Info}/");
                photo.LgPath = photo.LgPath.Replace($"/Photo/{projectId1}/{cms4seoEntityType.Slider}/", $"/Uploads/{projectId2}/Photo/{cms4seoEntityType.Slider}/");



                photo.MdPath = photo.MdPath.Replace($"/Photo/{projectId1}/{cms4seoEntityType.Category}/", $"/Uploads/{projectId2}/Photo/{cms4seoEntityType.Category}/");
                photo.MdPath = photo.MdPath.Replace($"/Photo/{projectId1}/{cms4seoEntityType.Product}/", $"/Uploads/{projectId2}/Photo/{cms4seoEntityType.Product}/");
                photo.MdPath = photo.MdPath.Replace($"/Photo/{projectId1}/{cms4seoEntityType.Topic}/", $"/Uploads/{projectId2}/Photo/{cms4seoEntityType.Topic}/");
                photo.MdPath = photo.MdPath.Replace($"/Photo/{projectId1}/{cms4seoEntityType.Article}/", $"/Uploads/{projectId2}/Photo/{cms4seoEntityType.Article}/");
                photo.MdPath = photo.MdPath.Replace($"/Photo/{projectId1}/{cms4seoEntityType.Info}/", $"/Uploads/{projectId2}/Photo/{cms4seoEntityType.Info}/");
                photo.MdPath = photo.MdPath.Replace($"/Photo/{projectId1}/{cms4seoEntityType.Slider}/", $"/Uploads/{projectId2}/Photo/{cms4seoEntityType.Slider}/");



                photo.SmPath = photo.SmPath.Replace($"/Photo/{projectId1}/{cms4seoEntityType.Category}/", $"/Uploads/{projectId2}/Photo/{cms4seoEntityType.Category}/");
                photo.SmPath = photo.SmPath.Replace($"/Photo/{projectId1}/{cms4seoEntityType.Product}/", $"/Uploads/{projectId2}/Photo/{cms4seoEntityType.Product}/");
                photo.SmPath = photo.SmPath.Replace($"/Photo/{projectId1}/{cms4seoEntityType.Topic}/", $"/Uploads/{projectId2}/Photo/{cms4seoEntityType.Topic}/");
                photo.SmPath = photo.SmPath.Replace($"/Photo/{projectId1}/{cms4seoEntityType.Article}/", $"/Uploads/{projectId2}/Photo/{cms4seoEntityType.Article}/");
                photo.SmPath = photo.SmPath.Replace($"/Photo/{projectId1}/{cms4seoEntityType.Info}/", $"/Uploads/{projectId2}/Photo/{cms4seoEntityType.Info}/");
                photo.SmPath = photo.SmPath.Replace($"/Photo/{projectId1}/{cms4seoEntityType.Slider}/", $"/Uploads/{projectId2}/Photo/{cms4seoEntityType.Slider}/");

                count++;
                count++;
                count++;

                db.Entry(photo).State = EntityState.Modified;
            }

            db.SaveChanges();


            var categories = db.Categories.ToList();
            foreach (var category in categories)
            {
                category.Avatar = category.Avatar?.Replace($"/Photo/{projectId1}/{cms4seoEntityType.Category}/", $"/Uploads/{projectId2}/Photo/{cms4seoEntityType.Category}/");
                db.Entry(category).State = EntityState.Modified;
                count++;
            }
            db.SaveChanges();


            var products = db.Products.ToList();
            foreach (var product in products)
            {
                product.Avatar = product.Avatar?.Replace($"/Photo/{projectId1}/{cms4seoEntityType.Product}/", $"/Uploads/{projectId2}/Photo/{cms4seoEntityType.Product}/");
                db.Entry(product).State = EntityState.Modified;
                count++;
            }
            db.SaveChanges();


            var topics = db.Topics.ToList();
            foreach (var topic in topics)
            {
                topic.Avatar = topic.Avatar?.Replace($"/Photo/{projectId1}/{cms4seoEntityType.Topic}/", $"/Uploads/{projectId2}/Photo/{cms4seoEntityType.Topic}/");
                db.Entry(topic).State = EntityState.Modified;
                count++;
            }
            db.SaveChanges();


            var articles = db.Articles.ToList();
            foreach (var article in articles)
            {
                article.Avatar = article.Avatar?.Replace($"/Photo/{projectId1}/{cms4seoEntityType.Article}/", $"/Uploads/{projectId2}/Photo/{cms4seoEntityType.Article}/");
                db.Entry(article).State = EntityState.Modified;
                count++;
            }
            db.SaveChanges();


            var infos = db.Infos.ToList();
            foreach (var info in infos)
            {
                info.Avatar = info.Avatar?.Replace($"/Photo/{projectId1}/{cms4seoEntityType.Info}/", $"/Uploads/{projectId2}/Photo/{cms4seoEntityType.Info}/");
                db.Entry(info).State = EntityState.Modified;
                count++;
            }
            db.SaveChanges();


            var sliders = db.Sliders.ToList();
            foreach (var slider in sliders)
            {
                slider.Avatar = slider.Avatar?.Replace($"/Photo/{projectId1}/{cms4seoEntityType.Slider}/", $"/Uploads/{projectId2}/Photo/{cms4seoEntityType.Slider}/");
                slider.AvatarMobile = slider.AvatarMobile?.Replace($"/Photo/{projectId1}/{cms4seoEntityType.Slider}/", $"/Uploads/{projectId2}/Photo/{cms4seoEntityType.Slider}/");
                db.Entry(slider).State = EntityState.Modified;
                count++;
            }
            db.SaveChanges();


            // photo in product content
            foreach (var product in products)
            {
                product.Description = product.Description?.Replace($"/Photo/{projectId1}/{cms4seoEntityType.Product}/", $"/Uploads/{projectId2}/Photo/{cms4seoEntityType.Product}/");
                product.UnsignContent = product.UnsignContent?.Replace($"/Photo/{projectId1}/{cms4seoEntityType.Product}/", $"/Uploads/{projectId2}/Photo/{cms4seoEntityType.Product}/");
                db.Entry(product).State = EntityState.Modified;
                count++;
            }
            db.SaveChanges();


            // photo in article content
            foreach (var article in articles)
            {
                article.Content = article.Content?.Replace($"/Photo/{projectId1}/{cms4seoEntityType.Article}/", $"/Uploads/{projectId2}/Photo/{cms4seoEntityType.Article}/");
                article.UnsignContent = article.UnsignContent?.Replace($"/Photo/{projectId1}/{cms4seoEntityType.Article}/", $"/Uploads/{projectId2}/Photo/{cms4seoEntityType.Article}/");
                db.Entry(article).State = EntityState.Modified;
                count++;
            }
            db.SaveChanges();


            TempData[MessageType.Success] =
                string.Format(AdminResources.MigrationControllerPhotoPath_PhotoPath_successful____0___items_submitted, count);

            LogHelper.Write(@"Admin/SearchOptimize",
                $"User {User.Identity.Name} ProjectIdToUploads, total items {count}");

            return RedirectToAction("Index");

        }


        public ActionResult UpdateCurrentKey()
        {
            count = 0;


            var settingRepository = new SettingRepository();

            var contents = db.Contents.Where(x => x.Theme == Setting.Theme && x.Language == Setting.Language);

            foreach (var content in contents)
            {
                if(string.IsNullOrWhiteSpace(content.Key))
                    continue;

                if (content.Key.StartsWith("Common.Shop."))
                {
                    settingRepository.Set(content.Key, content.Value);
                }
                else if (content.Key.StartsWith("Common."))
                {
                    Regex regex = new Regex("^Common.");
                    var key = regex.Replace(content.Key, "", 1);
                    Setting.Contents.Set(key, content.Value);
                }
                else if(content.Key.StartsWith("Private."))
                {
                    Regex regex = new Regex(@"^Private.\w+.");
                    var key = regex.Replace(content.Key, "Section.", 1);
                    Setting.Contents.Set(key, content.Value);
                }

                count++;

                db.Contents.Remove(content);
            }


            // remove all
            db.SaveChanges();


            TempData[MessageType.Success] =
                string.Format("Update Current keys done, total: {0}", count);

            LogHelper.Write(@"Admin/SearchOptimize",
                $"User {User.Identity.Name} UpdateCurrentKey, total items {count}");

            return RedirectToAction("Index");
        }



        public ActionResult UpdateSmallAndMediumImageByLargeSize()
        {
            bool onlyResizeIfWider = true;
            string[] _lossyImageExtension = { ".jpg", ".jpe", ".jpeg", ".webp" }; // save webp as jpeg

            ISettingRepository settingRepository = new SettingRepository();
            int large = Convert.ToInt16(settingRepository.Get(PhotoSettingType.LargeSize));
            int medium = Convert.ToInt16(settingRepository.Get(PhotoSettingType.MediumSize));
            int small = Convert.ToInt16(settingRepository.Get(PhotoSettingType.SmallSize));
            int imageQuality = Convert.ToInt16(settingRepository.Get(PhotoSettingType.ImageQuality));
            int maxHeight = Convert.ToInt16(settingRepository.Get(PhotoSettingType.MaxHeight));

            count = 0;
            var message = "";

            var photos = db.Photos.Where(x => x.Entity == "Product" || x.Entity == "Articles").ToList();

            MozJpeg mozJpeg = new MozJpeg();

            foreach (var photo in photos)
            {
                if (System.IO.File.Exists(HostingEnvironment.MapPath(photo.LgPath)))
                {

                    var image = Image.FromFile(HostingEnvironment.MapPath(photo.LgPath));
                    var smPath = HostingEnvironment.MapPath(photo.SmPath);
                    var mdPath = HostingEnvironment.MapPath(photo.MdPath);

                    var mimeType = image.GetMimeType();

                    bool isUseMozJpeg = mimeType == "image/jpeg"
                                        || Setting.WebSettings[PhotoSettingType.IsAutoConvertPngToJpg].AsBool();

                    var smallImage = Imager.Resize(image, small, maxHeight, onlyResizeIfWider);
                    var mediumImage = Imager.Resize(image, medium, maxHeight, onlyResizeIfWider);

                    try
                    {
                        if (isUseMozJpeg)
                        {
                            mozJpeg.Save(smallImage, smPath, imageQuality);
                            mozJpeg.Save(mediumImage, mdPath, imageQuality);
                        }
                        else
                        {
                            if (photo.LgPath.Contains("-lg."))
                            {
                                smallImage.Save(smPath);
                                mediumImage.Save(mdPath);
                            }
                        }

                        smallImage.Dispose();
                        mediumImage.Dispose();
                        image.Dispose();

                        count++;
                    }
                    catch (Exception e)
                    {
                        message += $"\n{e.Message}";
                    }
                    

                    
                }
            }

            TempData[MessageType.Success] = $"Update Small And Medium Image Done, total: {count}\n{message}";

            LogHelper.Write(@"Admin/Migration",
                $"User {User.Identity.Name} UpdateSmallAndMediumImageByLargeSize, total items {count}");

            return RedirectToAction("Index");
        }



        public ActionResult UpdateAllSizeImageByHugeSize()
        {
            bool onlyResizeIfWider = true;
            string[] _lossyImageExtension = { ".jpg", ".jpe", ".jpeg", ".webp" }; // save webp as jpeg

            ISettingRepository settingRepository = new SettingRepository();
            int large = Convert.ToInt16(settingRepository.Get(PhotoSettingType.LargeSize));
            int medium = Convert.ToInt16(settingRepository.Get(PhotoSettingType.MediumSize));
            int small = Convert.ToInt16(settingRepository.Get(PhotoSettingType.SmallSize));
            int imageQuality = Convert.ToInt16(settingRepository.Get(PhotoSettingType.ImageQuality));
            int maxHeight = Convert.ToInt16(settingRepository.Get(PhotoSettingType.MaxHeight));

            count = 0;
            var message = "";

            var photos = db.Photos.Where(x => x.Entity == "Product" || x.Entity == "Articles").ToList();

            MozJpeg mozJpeg = new MozJpeg();

            foreach (var photo in photos)
            {
                if (System.IO.File.Exists(HostingEnvironment.MapPath(photo.LgPath.Huge())))
                {

                    var image = Image.FromFile(HostingEnvironment.MapPath(photo.LgPath.Huge()));
                    var smPath = HostingEnvironment.MapPath(photo.SmPath);
                    var mdPath = HostingEnvironment.MapPath(photo.MdPath);
                    var lgPath = HostingEnvironment.MapPath(photo.LgPath);

                    var mimeType = image.GetMimeType();

                    bool isUseMozJpeg = mimeType == "image/jpeg"
                                        || Setting.WebSettings[PhotoSettingType.IsAutoConvertPngToJpg].AsBool();

                    var smallImage = Imager.Resize(image, small, maxHeight, onlyResizeIfWider);
                    var mediumImage = Imager.Resize(image, medium, maxHeight, onlyResizeIfWider);
                    var largeImage = Imager.Resize(image, large, maxHeight, onlyResizeIfWider);

                    try
                    {
                        if (isUseMozJpeg)
                        {
                            mozJpeg.Save(smallImage, smPath, imageQuality);
                            mozJpeg.Save(mediumImage, mdPath, imageQuality);
                            mozJpeg.Save(largeImage, lgPath, imageQuality);
                        }
                        else
                        {
                            smallImage.Save(smPath);
                            mediumImage.Save(mdPath);
                            largeImage.Save(lgPath);
                        }

                        smallImage.Dispose();
                        mediumImage.Dispose();
                        largeImage.Dispose();
                        image.Dispose();

                        count++;
                    }
                    catch (Exception e)
                    {
                        message += $"\n{e.Message}";
                    }



                }
            }

            TempData[MessageType.Success] = $"Update Small, Medium, Large Image Done, total: {count}\n{message}";

            LogHelper.Write(@"Admin/Migration",
                $"User {User.Identity.Name} UpdateAllSizeImageByHugeSize, total items {count}");

            return RedirectToAction("Index");
        }


        [HttpPost]
        public ActionResult UpdateTheme(string source, string destination)
        {
            count = 0;

            if (String.IsNullOrWhiteSpace(source) || String.IsNullOrWhiteSpace(destination))
            {
                TempData[MessageType.Warning] = $"UpdateTheme fail, source or destination not chosen";

                LogHelper.Write(@"Admin/Migration",
                    $"User {User.Identity.Name} UpdateTheme fail, source or destination not chosen");

                return RedirectToAction("Index");
            }


            // Delete Destination
            var destinations = db.Contents.Where(x => x.Theme == destination);
            db.Contents.RemoveRange(destinations);
            db.SaveChanges();

            // Remove Themes key range (key can not modify)
            var sources = db.Contents.Where(x => x.Theme == source).ToList();
            count = sources.Count;
            db.Contents.RemoveRange(sources);
            db.SaveChanges();

            // Re AddRange
            var newSources = new List<Content>(sources);
            foreach (var item in newSources)
            {
                item.Theme = destination;
            }
            db.Contents.AddRange(newSources);
            db.SaveChanges();

            TempData[MessageType.Success] = $"UpdateTheme Done, total: {count}";

            LogHelper.Write(@"Admin/Migration",
                $"User {User.Identity.Name} UpdateTheme, total items {count}");

            return RedirectToAction("Index");
        }
    }
}
