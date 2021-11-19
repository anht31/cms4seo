using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Admin.Resources;
using cms4seo.Service.TopicHelper;
using cms4seo.Admin.Services;
using cms4seo.Common.Attribute;
using cms4seo.Common.BootstrapHelpers;
using cms4seo.Common.Helpers;
using cms4seo.Common.SettingHelpers;
using cms4seo.Data;
using cms4seo.Data.IdentityModels;
using cms4seo.Model.Entities;
using cms4seo.Model.LekimaxType;
using cms4seo.Service.Photo;
using cms4seo.Service.Provider;
using cms4seo.Service.Seo;
using cms4seo.Service.Sitemap;

namespace cms4seo.Admin.Controllers
{
    [CustomAuthorize(Roles = "Basic.Topic")]
    public class TopicController : BaseController
    {

        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        // ReSharper disable once InconsistentNaming
        private ApplicationDbContext db = new ApplicationDbContext();

        private IQueryable<Topic> TopicsValidation =>
            db.Topics.Where(x => !(x.Name == null || x.Name.Trim() == String.Empty));

        private IQueryable<Topic> TopicsInvalidation =>
            db.Topics.Where(x => x.Name == null || x.Name.Trim() == String.Empty);


        // GET: Admin/Topic
        public ActionResult Index()
        {            
            return View(TopicsValidation.ToList());
        }



        // Create =======================================================================================
        public async Task<ViewResult> Create()
        {

            var topic = await TopicsInvalidation.FirstOrDefaultAsync(x => x.PostBy == User.Identity.Name);

            if (topic == null)
            {
                topic = new Topic()
                {
                    PostBy = User.Identity.Name,
                    IsCreate = true,
                };

                db.Topics.Add(topic);
                await db.SaveChangesAsync();
            }
            

            ViewBag.ParentId = new SelectList(await TopicsValidation.Where(x => 
                x.Id != topic.Id && x.ParentId == null).ToListAsync(), "Id", "Name");

            return View("Edit", topic);
        }




        // Edit ==========================================================================
        public async Task<ActionResult> Edit(int id)
        {

            if (!db.Topics.Any())
            {
                TempData[MessageType.Warning] = 
                    AdminResources.TopicControllerEdit_Not_have_any_topic__please_add_first_topic;

                LogHelper.Write(@"Admin/Topic",
                    $"Topic must be created first, user: {User.Identity.Name}");

                return RedirectToAction("Index");
            }


            // query ==================================================
            var topic = await db.Topics.FindAsync(id);






            ViewBag.ParentId = new SelectList(await TopicsValidation.Where(x => 
                x.Id != topic.Id && x.ParentId == null).ToListAsync(), "Id", "Name", topic?.ParentId);

            return View(topic);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(Topic topic)
        {


            if (String.IsNullOrWhiteSpace(topic.Name))
            {
                ModelState.AddModelError("Name", AdminResources.TopicControllerEditYouMustEnterName);
            }

            var parentTopic = db.Topics.Find(topic.ParentId);

            if (parentTopic.HowDeepTopicSiteArchitecture() >= 3)
            {
                ModelState.AddModelError("ParentId", 
                    AdminResources.Admin_TopicController_YouNotAllowSelect2ndLevelTopic);

                TempData[MessageType.Warning] =
                    AdminResources.Admin_TopicController_YouNotAllowSelect2ndLevelTopic;
            }

            if (ModelState.IsValid)
            {



                #region permalink & slug

                
                var rootTopic = parentTopic.GetRootTopic();

                string secondTopicName = null;

                if (rootTopic?.Id != parentTopic?.Id)
                    secondTopicName = parentTopic?.Name.MakeNameFriendly();

                var permalinkProvider = new PermalinkProvider();

                topic.Slug = permalinkProvider.Set(topic.Id, cms4seoEntityType.Topic,
                    rootTopic?.Name,
                    secondTopicName,
                    topic.Name,
                    topic.IsCreate);
                


                #endregion permalink & slug



                // seo =============================================================
                if (!topic.IsLockSeo || Setting.WebSettings[WebSettingType.IsAutoSeoMetaTag].ToBoolean())
                {
                    topic.MetaTitle = topic.Name;
                    topic.MetaDescription = topic.Description.ShortenText320();
                    topic.MetaKeyWords = topic.MetaDescription.UnsignConvert();
                }
                topic.IsLockSeo = true;
                topic.UnsignContent = topic.Name.UnsignConvert();
                topic.UnsignContent += topic.Description.UnsignConvert();


                if (string.IsNullOrEmpty(topic.Href))
                    topic.Href = "#";

                if (topic.IsCreate)
                {
                    topic.DateCreated = DateTime.Now;
                    topic.Sort = topic.Sort != 0 ? topic.Sort : -topic.Id * 10;
                }
                else
                {
                    topic.DateAmended = DateTime.Now;
                }

                db.Topics.AddOrUpdate(topic);
                await db.SaveChangesAsync();

                LogHelper.Write(@"Admin/Topic",
                    topic.IsCreate
                        ? $"User: {User.Identity.Name} created topic: {topic.Name}"
                        : $"User: {User.Identity.Name} updated topic: {topic.Name}");


                // add sitemap
                #region add sitemap

                Uri uri = new Uri(Request.Url?.AbsoluteUri ?? "");
                var protocolHostPort = uri.Scheme + Uri.SchemeDelimiter + uri.Host + ":" + uri.Port;

                if (topic.IsCreate)
                {
                    var siteMapingProvider = new SiteMappingProvider(Request.Url?.AbsoluteUri);

                    siteMapingProvider.Add(new SiteMapItem()
                    {
                        Loc = new Uri($"{protocolHostPort}{topic.Slug}"),
                        ChangeFreq = ChangeFrequency.Weekly,
                        LastMod = topic.DateCreated,
                        Priority = 0.8
                    });
                }


                #endregion add sitemap









                // update status IsCreate
                topic = db.Topics.Find(topic.Id);
                topic.IsCreate = false;
                db.Entry(topic).State = EntityState.Modified;
                db.SaveChanges();



                TempData[MessageType.Success] = 
                    string.Format(AdminResources.TopicControllerEdit__0___has__been__saved, topic.Name);
                return RedirectToAction("Index");
            }

            // there is something wrong with the data values
            ViewBag.ParentId = new SelectList(await TopicsValidation.Where(x => 
                x.Id != topic.Id && x.ParentId == null).ToListAsync(), "Id", "Name", topic.ParentId);

            return View(topic);
        }


        // Del ==========================================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<RedirectToRouteResult> Delete(int id)
        {

            var topic = await db.Topics.FindAsync(id);


            if (topic == null)
            {
                TempData[MessageType.Danger] = 
                    AdminResources.TopicControllerDelete_topic_not_found;

                LogHelper.Write(@"Admin/Topic",
                    $"Access denied user: {User.Identity.Name} for attempt access delete topic: {id}, topic not found");

                return null;
            }


            if (topic.Children.Count > 0)
            {
                TempData[MessageType.Warning] = 
                    string.Format(AdminResources.TopicControllerDelete_Topic_have____0___child_topic__please_delete_child_topic_first, topic.Children.Count);

                LogHelper.Write(@"Admin/Topic",
                    $"Access denied user: {User.Identity.Name} for attempt access delete topic: {id}, topic has child");

                return null;
            }

            if (topic.Articles != null && topic.Articles.Count > 0)
            {
                TempData[MessageType.Warning] = 
                    string.Format(AdminResources.TopicControllerDelete_Topic_have____0___article__please_delete_article_first_, topic.Articles.Count);

                LogHelper.Write(@"Admin/Topic",
                    $"Access denied user: {User.Identity.Name} for attempt access delete topic: {id}, topic has products");

                return null;
            }




            #region delete permalink

            var permalink = await db.Permalinks.FirstOrDefaultAsync(x =>
                x.EntityName == cms4seoEntityType.Topic && x.EntityId == topic.Id);

            if (permalink != null)
                db.Permalinks.Remove(permalink);

            #endregion


            try
            {
                var photoService = new PhotoService();
                photoService.DeletePhotoList(topic.Photos);
                db.Photos.RemoveRange(topic.Photos);

            }
            catch (Exception e)
            {
                TempData[MessageType.Danger] = 
                    string.Format(AdminResources.TopicControllerDelete_Error_rising_when_try_delete_photo__Message___0_, e.Message);

                LogHelper.Write(@"Admin/Topic",
                    $"Access denied user: {User.Identity.Name} for try delete photo, message: {e.Message}");
                    
                return null;
            }


            db.Topics.Remove(topic);
            await db.SaveChangesAsync();
            LogHelper.Write(@"Admin/Topic",
                $"User: {User.Identity.Name} deleted topic: {topic.Name}");


            // delete sitemap
            #region delete sitemap
            Uri uri = new Uri(Request.Url?.AbsoluteUri ?? "");
            var protocolHostPort = uri.Scheme + Uri.SchemeDelimiter + uri.Host + ":" + uri.Port;
            var siteMapingProvider = new SiteMappingProvider(Request.Url?.AbsoluteUri);
            siteMapingProvider.Delete(new Uri($"{protocolHostPort}/{topic.Slug}").ToString());
            #endregion delete sitemap


            TempData[MessageType.Warning] = 
                string.Format(AdminResources.TopicControllerDelete__0__was_deleted, topic.Name);


            return null;
        }


        // dispose --------------------------------------------     

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