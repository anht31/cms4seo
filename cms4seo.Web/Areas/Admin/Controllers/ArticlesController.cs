using System;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.WebPages;
using Admin.Resources;
using cms4seo.Service.TopicHelper;
using cms4seo.Admin.Controllers;
using cms4seo.Common.Attribute;
using cms4seo.Common.BootstrapHelpers;
using cms4seo.Common.Helpers;
using cms4seo.Common.SettingHelpers;
using cms4seo.Common.Slug;
using cms4seo.Data;
using cms4seo.Model.cms4seoType;
using cms4seo.Model.Entities;
using cms4seo.Model.LekimaxType;
using cms4seo.Service.Photo;
using cms4seo.Service.Provider;
using cms4seo.Service.Seo;
using cms4seo.Service.Sitemap;

using CONST = HTMLTOCGenerator.Constants;
using HTML_TOC = HTMLTOCGenerator.HTMLTOCGenerator;
using SHORTEN_PATH = HTMLTOCGenerator.ShortenPath;
using TRY_OPEN = HTMLTOCGenerator.TryOpen;

namespace cms4seo.Admin.Controllers
{
    [CustomAuthorize(Roles = "Basic.Article")]
    public class ArticlesController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private IQueryable<Article> ArticlesValidation =>
            db.Articles.Where(x => !(x.Name == null || x.Name.Trim() == String.Empty));

        private IQueryable<Article> ArticlesInvalidation =>
            db.Articles.Where(x => x.Name == null || x.Name.Trim() == String.Empty);

        private IQueryable<Topic> TopicsValidation =>
            db.Topics.Where(x => !(x.Name == null || x.Name.Trim() == String.Empty));

        //private IQueryable<Topic> TopicsInvalidation =>
        //    db.Topics.Where(x => x.Name == null || x.Name.Trim() == String.Empty);

        // GET: Admin/Article
        public async Task<ViewResult> Index()
        {
            return View(await ArticlesValidation.ToListAsync());
        }


        // Create =======================================================================================
        public async Task<ActionResult> Create()
        {
            if (!TopicsValidation.Any())
            {
                TempData[MessageType.Warning] = AdminResources.ArticlesControllerCreate_You_not_yet_create_any_Topic__please_create_Topic_first_
                    + $" <a href='{Url.Action("Create", "Topic")}' class='btn btn-success'>" +
                    AdminResources.ArticlesControllerCreate_Enter_link_to_navigate_to_Topic +
                    $"</a>";

                LogHelper.Write(@"Admin/Product",
                    $"Category must be created first, user: {User.Identity.Name}");

                return RedirectToAction("Index");
            }

            var article = await ArticlesInvalidation.FirstOrDefaultAsync(x => x.PostBy == User.Identity.Name);

            if (article == null)
            {

                article = new Article
                {
                    PostBy = User.Identity.Name,
                    IsCreate = true,
                    IsPublish = true
                };

                db.Articles.Add(article);
                db.SaveChanges();
            }



            //ViewBag.TopicId = new SelectList(await TopicsValidation.ToListAsync(), "Id", "Name");

            ViewBag.Topics = await TopicsValidation.ToListAsync();
            ViewBag.SelectedTopic = article.TopicId;


            return View("Edit", article);
        }



        // Edit ==========================================================================
        public async Task<ActionResult> Edit(int id)
        {


            // query ==================================================
            var article = await db.Articles.FindAsync(id);


            if (article == null)
            {
                TempData[MessageType.Danger] = AdminResources.ArticlesControllerEdit_Not_found_any_article_to_edit;

                LogHelper.Write(@"Admin/Article",
                    $"User: {User.Identity.Name} not found article with Id: {id}");

                return RedirectToAction("Index");
            }


            #region articleTag            

            article.SelectedArticleTag = article.ArtilceTags.Select(x => x.Name).ToList();

            #endregion articleTag


            //ViewBag.TopicId = new SelectList(await TopicsValidation.ToListAsync(), 
            //    "Id", "Name", article.TopicId);

            ViewBag.Topics = await TopicsValidation.ToListAsync();
            ViewBag.SelectedTopic = article.TopicId;

            return View(article);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(Article article)
        {
            if (String.IsNullOrWhiteSpace(article.Name))
            {
                ModelState.AddModelError("Name", AdminResources.ArticlesControllerEditYouMustEnterName);
            }


            if (ModelState.IsValid)
            {


                #region permalink & slug


                var topic = db.Topics.Find(article.TopicId);

                string secondTopicName = null;

                if (topic.GetRootTopic().Id != article.TopicId)
                    secondTopicName = topic?.Name.MakeNameFriendly();

                var permalinkProvider = new PermalinkProvider();

                article.Slug = permalinkProvider.Set(article.Id, cms4seoEntityType.Article,
                    topic.GetRootTopic().Name,
                    secondTopicName,
                    article.Name,
                    article.IsCreate);



                #endregion permalink & slug


                #region articleTag            

                SlugComparer slugComparer = new SlugComparer();

                var tags = db.ArticleTags.Select(x => x.Name);

                var listNewTag = article.SelectedArticleTag.Except(tags, slugComparer);

                foreach (var newTag in listNewTag)
                {
                    if (String.IsNullOrWhiteSpace(newTag)) continue;
                    db.ArticleTags.Add(new ArticleTag() { Name = newTag, Slug = newTag.MakeUrlFriendly() });
                }
                db.SaveChanges();


                // delete old =================================================
                if (!article.IsCreate)
                {
                    var articleToEdit = db.Articles.FirstOrDefault(x => x.Id == article.Id);
                    if (articleToEdit != null)
                    {
                        var articleTags = articleToEdit.ArtilceTags.ToList();

                        if (articleTags.Any())
                        {
                            foreach (var articleTag in articleTags)
                            {
                                articleToEdit.ArtilceTags.Remove(articleTag);
                            }
                            db.SaveChanges();
                        }
                    }

                    db.Entry(articleToEdit).State = EntityState.Detached; // fix error already has the same primary key
                    
                }

                // add new ====================================================

                db.Articles.Attach(article);

                article.ArtilceTags = db.ArticleTags.
                        Where(x => article.SelectedArticleTag.Contains(x.Name)).ToList();

                if (!article.IsCreate && article.SelectedArticleTag != null)
                {
                    foreach (var articleTag in article.ArtilceTags)
                        db.ArticleTags.Attach(articleTag);
                }

                #endregion articleTag


                // save ===============================================================

                article.PostedDate = DateTime.Now;
                article.PostBy = User.Identity.Name.Split('@')[0];
                article.Description = article.Description.ShortenText320();


                // set avatar
                if (String.IsNullOrEmpty(article.Avatar))
                {
                    var photo = db.Photos.FirstOrDefault(x =>
                        x.Entity == cms4seoEntityType.Article && x.ModelId == article.Id);

                    article.Avatar = photo?.SmPath;
                }


                // seo =============================================================
                if (!article.IsLockSeo || Setting.WebSettings[WebSettingType.IsAutoSeoMetaTag].ToBoolean())
                {
                    article.MetaTitle = article.Name;
                    article.MetaDescription = article.Description;
                    article.MetaKeyWords = article.MetaDescription.ReplaceDiacritics();
                }
                article.IsLockSeo = true;
                article.UnsignContent = article.Name.UnsignConvert();
                article.UnsignContent += article.Description.UnsignConvert();
                article.UnsignContent += article.Content.UnsignConvert();


                if (article.IsCreate)
                {
                    article.DateCreated = DateTime.Now;
                    article.PostedDate = article.PostedDate ?? DateTime.Now;
                    article.Sort = article.Sort != 0 ? article.Sort : article.Id * 10;
                }


                //article.UrlFriendly = article.Name.MakeUrlFriendly();
                db.Entry(article).State = EntityState.Modified;
                db.SaveChanges();

                LogHelper.Write(@"Admin/Article",
                    article.IsCreate
                        ? $"User: {User.Identity.Name} created article: {article.Name}"
                        : $"User: {User.Identity.Name} updated article: {article.Name}");


                // add sitemap
                #region add sitemap

                Uri uri = new Uri(Request.Url?.AbsoluteUri ?? "");
                var protocolHostPort = uri.Scheme + Uri.SchemeDelimiter + uri.Host + ":" + uri.Port;

                if (article.IsCreate && article.IsPublish)
                {
                    var siteMappingProvider = new SiteMappingProvider(Request.Url?.AbsoluteUri);

                    siteMappingProvider.Add(new SiteMapItem()
                    {
                        Loc = new Uri($"{protocolHostPort}{article.Slug}"),
                        ChangeFreq = ChangeFrequency.Daily,
                        LastMod = article.DateCreated,
                        Priority = 0.8
                    });
                }

                #endregion add sitemap







                // update status IsCreate
                article = db.Articles.Find(article.Id);
                article.IsCreate = false;
                db.Entry(article).State = EntityState.Modified;
                db.SaveChanges();




                #region TOC Generator

                if (Setting.WebSettings[TocSettingType.IsAutoArticleTOC].AsBool())
                {
                    article = db.Articles.Find(article.Id);

                    HTML_TOC htmlToc = new HTML_TOC();

                    var style = $@"toc-headers:h2,h3,h4,h5,h6;
                    toc-return:{Setting.WebSettings[TocSettingType.IsTocReturn].ToLower()};
                    toc-title:{Setting.WebSettings[TocSettingType.TocTitle]};
                    toc-return-image:/Assets/icon/gototop16.png;
                    toc-image-width:16;
                    toc-image-height:16;
                    toc-header-level:h2;";

                    var tocContainer = "<div class='toc' style='" + style + "'></div>";

                    // check least one element h2 -> h6
                    var regex = new Regex(@"<\/h[2-6]>");
                    var match = regex.Matches(article.Content ?? String.Empty);
                    if (match.Count > 0)
                    {
                        article.ContentWithTOC = htmlToc.add_TOC_to_html(tocContainer + article.Content);

                        db.Entry(article).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    
                }
                

                #endregion




                TempData[MessageType.Success] = LocalizeString.Format(AdminResources.ArticlesControllerEdit__0___has__been__saved, article.Name);

                return RedirectToAction("Index");
            }


            //ViewBag.TopicId = new SelectList(await TopicsValidation.ToListAsync()
            //    , "Id", "Name", article.TopicId);

            ViewBag.Topics = await TopicsValidation.ToListAsync();
            ViewBag.SelectedTopic = article.TopicId;

            return View(article);
        }



        // Del ==========================================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public RedirectToRouteResult Delete(int id)
        {
            var article = db.Articles.Find(id);


            if (article == null)
            {
                TempData[MessageType.Warning] = AdminResources.ArticlesControllerDelete_Article_was_t_deleted;
                //return RedirectToAction("Index");
                return null;
            }




            #region delete permalink

            var permalink = db.Permalinks.FirstOrDefault(x =>
                x.EntityName == cms4seoEntityType.Article && x.EntityId == article.Id);

            if (permalink != null)
                db.Permalinks.Remove(permalink);

            #endregion



            try
            {
                var photoService = new PhotoService();
                photoService.DeletePhotoList(article.Photos);
                db.Photos.RemoveRange(article.Photos);

            }
            catch (Exception e)
            {
                TempData[MessageType.Danger] =
                    string.Format(AdminResources.ArticlesControllerDelete_Error_rising_when_try_delete_photo__Message___0_, e.Message);

                LogHelper.Write(@"Admin/Article",
                    $"User: {User.Identity.Name} try deleted article: {article.Name}, "
                    + $"Error rising when try delete photo, Message: {e.Message}");

                //return RedirectToAction("Index");
                return null;
            }




            db.Articles.Remove(article);
            db.SaveChanges();


            // delete sitemap
            #region delete sitemap 
            Uri uri = new Uri(Request.Url?.AbsoluteUri ?? "");
            var protocolHostPort = uri.Scheme + Uri.SchemeDelimiter + uri.Host + ":" + uri.Port;
            var siteMappingProvider = new SiteMappingProvider(Request.Url?.AbsoluteUri);
            siteMappingProvider.Delete(new Uri($"{protocolHostPort}/{article.Slug}").ToString());
            #endregion delete sitemap



            TempData[MessageType.Warning] = string.Format(AdminResources.ArticlesControllerDelete__0__was_deleted, article.Name);

            LogHelper.Write(@"Admin/Article", 
                $"User: {User.Identity.Name} deleted article: {article.Name}");


            return null;
        }



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}