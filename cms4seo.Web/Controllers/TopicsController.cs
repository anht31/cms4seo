using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.WebPages;
using cms4seo.Service.TopicHelper;
using cms4seo.Common.Helpers;
using cms4seo.Data;
using cms4seo.Data.IdentityModels;
using cms4seo.Model.Entities;
using cms4seo.Model.LekimaxType;
using cms4seo.Service.Provider;
using PagedList;

namespace cms4seo.Web.Controllers
{
    public class TopicsController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        private IQueryable<Topic> TopicsValidation =>
            db.Topics.Where(x => !(x.Name == null || x.Name.Trim() == String.Empty))
                .OrderByDescending(x => x.Sort);


        // GET: Topics
        public ActionResult Index()
        {
            var topicRoot = db.Topics.FirstOrDefault(x => x.Location == 3);
            ViewBag.Title = topicRoot?.Name;

            ViewBag.Description = topicRoot?.MetaDescription;
            ViewBag.Keywords = topicRoot?.MetaKeyWords;

            return View(topicRoot?.Children.ToList());

        }





        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }








        /// <summary>
        /// Url database driven for Topics
        /// </summary>
        /// <param name="id"></param>
        /// <param name="page"></param>
        /// <param name="view"></param>
        /// <returns></returns>
        public ActionResult Details(int id, int? page, string view)
        {
            var topic = db.Topics.Find(id);


            if (topic == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound, "topic not found");
            }


            ViewBag.Id = topic.Children.Any() ? topic.Id : 0;
            ViewBag.Title = topic.Name;
            ViewBag.Description = topic.MetaDescription;
            ViewBag.Keywords = topic.MetaKeyWords;

            ViewBag.Location = topic.FindRootTopic().Location;

            ViewBag.TopicSlug = topic.Slug;

            if (view == "folder")
            {
                ViewBag.View = "folder";
            }

            var articles = topic.GetAllChildArticle().AsArticlesValidation()
                .OrderByDescending(x => x.DateCreated);

            int pageSize = Setting.WebSettings[ShopSettingType.ArticlePageSize].AsInt();
            pageSize = pageSize > 0 ? pageSize : 20;

            return View(articles.ToPagedList(page ?? 1, pageSize));
        }



        [ChildActionOnly]
        public PartialViewResult TopicSystem(int id, string view)
        {
            var topic = TopicsValidation.FirstOrDefault(x => x.Id == id);

            if (view == "folder")
            {
                ViewBag.Title = topic?.Name;
                ViewBag.Description = topic?.MetaDescription;
                ViewBag.Keywords = topic?.MetaKeyWords;

                return PartialView("_FolderView", topic);
            }

            return PartialView(topic);
        }



        //[ChildActionOnly]
        //public ActionResult FolderView(int id)
        //{
        //    var topic = db.Topics.Find(id);


        //    if (topic == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.NotFound, "topic not found");
        //    }


        //    //ViewBag.Slug = slug;
        //    ViewBag.Title = topic.Name;
        //    ViewBag.Description = topic.MetaDescription;
        //    ViewBag.Keywords = topic.MetaKeyWords;

        //    //ViewBag.Location = topic?.FindRootTopic().Location;

        //    return View(topic);
        //}


        [ChildActionOnly]
        public ActionResult AsideAllTopic()
        {
            return PartialView(TopicsValidation.ToList());
        }



        [ChildActionOnly]
        public ActionResult HeaderMenu()
        {
            return PartialView(TopicsValidation.ToList());
        }
    }
}

// hello