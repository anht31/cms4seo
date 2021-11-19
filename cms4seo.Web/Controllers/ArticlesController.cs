using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using cms4seo.Common.Helpers;
using cms4seo.Data;
using cms4seo.Model.Entities;
using cms4seo.Data.IdentityModels;
using PagedList;

namespace cms4seo.Web.Controllers
{
    public class ArticlesController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        // zero time 0:00:00
        public DateTime Tomorrow => DateTime.Today.AddDays(1);

        private IQueryable<Article> ArticlesValidation =>
            db.Articles
                .Where(x =>
                    !(x.Name == null || x.Name.Trim() == String.Empty)
                    & x.PostedDate <= Tomorrow
                    & x.IsPublish)
                .OrderByDescending(x => x.Sort);

        private IQueryable<Topic> TopicsValidation =>
            db.Topics.Where(x => !(x.Name == null || x.Name.Trim() == String.Empty));




        #region new route


        public ActionResult Index(int? page, int size = 20)
        {

            List<Article> articles = ArticlesValidation.ToList();

            var pageNumber = page ?? 1; // if no page was specified in the querystring, default to the first page (1)
            var pageArticles = articles.ToPagedList(pageNumber, size); // will only contain 20 article max because of the pageSize

            return View(pageArticles);

        }



        [ChildActionOnly]
        public ActionResult OrderView(int? page, int size = 20)
        {
            // override by ensure is Newest (bypass Pin mode)
            List<Article> articles = ArticlesValidation.OrderByDescending(x => x.Sort).ToList();

            var pageNumber = page ?? 1; // if no page was specified in the querystring, default to the first page (1)
            var pageArticles = articles.ToPagedList(pageNumber, size); // will only contain 20 article max because of the pageSize

            return PartialView(pageArticles);
        }


        [ChildActionOnly]
        public ActionResult TopView(int? page, int size = 20)
        {
            // override by ensure is Newest (bypass Pin mode)
            List<Article> articles = ArticlesValidation.OrderByDescending(x => x.ViewCounter).ToList();

            var pageNumber = page ?? 1; // if no page was specified in the querystring, default to the first page (1)
            var pageArticles = articles.ToPagedList(pageNumber, size); // will only contain 20 article max because of the pageSize

            return PartialView(pageArticles);
        }




        [ChildActionOnly]
        public ActionResult Newest(int? page, int size = 20)
        {
            // override by ensure is Newest (bypass Pin mode)
            List<Article> articles = ArticlesValidation.OrderByDescending(x => x.DateCreated).ToList();

            var pageNumber = page ?? 1; // if no page was specified in the querystring, default to the first page (1)
            var pageArticles = articles.ToPagedList(pageNumber, size); // will only contain 20 article max because of the pageSize

            return PartialView(pageArticles);

        }


        [ChildActionOnly]
        public ActionResult TrendInMonth(int size = 5)
        {
            var today = DateTime.Today;
            var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);

            List<Article> articles =
                ArticlesValidation.Where(x => x.DateCreated >= firstDayOfMonth)
                    .OrderByDescending(x => x.ViewCounter).Take(size).ToList();

            return PartialView(articles);

        }


        public ActionResult Details(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "slug is null");
            }

            Article article = ArticlesValidation.FirstOrDefault(x => x.Id == id && x.IsPublish);

            if (article == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound, "article not found");
            }

            article.ViewCounter++;
            db.SaveChanges();

            return View(article);
        }




        public ActionResult Search(string strSearch, int? page)
        {
            //return PartialView();

            ViewBag.strSearch = strSearch;

            if (strSearch == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "strSearch is null");

            }

            string unsignStringSearch = strSearch.UnsignConvert();

            List<Article> articles = ArticlesValidation.
                Where(x => x.UnsignContent.Contains(unsignStringSearch)).ToList();

            ViewBag.NumResult = articles.Count;
            var pageNumber = page ?? 1; // if no page was specified in the querystring, default to the first page (1)
            var pageArticles = articles.ToPagedList(pageNumber, 20); // will only contain 20 article max because of the pageSize

            return View(pageArticles);
        }




        [ChildActionOnly]
        public ActionResult AsideTopView(int size = 5)
        {
            return PartialView(ArticlesValidation
                .OrderByDescending(x => x.Sort)
                .ThenByDescending(x => x.ViewCounter).Take(size).ToList());
        }



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }







        [ChildActionOnly]
        public ActionResult RelateArticleByTag(int articleId)
        {

            var article = ArticlesValidation.FirstOrDefault(x => x.Id == articleId);

            if (article == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound, "article not found");
            }

            var relateArticles = article.ArtilceTags.SelectMany(x => x.Articles).Distinct().ToList();

            relateArticles.Remove(article);


            return PartialView(relateArticles
                .Where(x =>
                    !(x.Name == null || x.Name.Trim() == String.Empty)
                    & x.PostedDate <= Tomorrow
                    & x.IsPublish)
                .OrderByDescending(x => x.ViewCounter)
                .Take(8).ToPagedList(1, 8)
            );
        }


        [ChildActionOnly]
        public ActionResult RelateArticle(int articleId)
        {
            var article = ArticlesValidation.FirstOrDefault(x => x.Id == articleId);

            if (article?.Topic == null)
                return null;

            ViewBag.TopicName = article.Topic?.Name;

            return PartialView(ArticlesValidation
                .Where(x =>
                    !(x.Name == null || x.Name.Trim() == String.Empty)
                    & x.PostedDate <= Tomorrow
                    & x.IsPublish
                    && x.Id != articleId && x.TopicId == article.TopicId)
                .OrderByDescending(x => x.ViewCounter).Take(8).ToPagedList(1, 8)
            );
        }


        [ChildActionOnly]
        public ActionResult AllAuthors()
        {
            var authors = ArticlesValidation.Select(x => x.PostBy).Distinct();

            return PartialView(authors);
        }


        public ActionResult Author(string author, int? page)
        {

            List<Article> articles = ArticlesValidation.Where(x => x.PostBy.Contains(author)).ToList();

            var pageNumber = page ?? 1; // if no page was specified in the querystring, default to the first page (1)
            var pageArticles = articles.ToPagedList(pageNumber, 20); // will only contain 20 article max because of the pageSize

            ViewBag.Author = author;

            return View(pageArticles);

        }




        [ChildActionOnly]
        public ActionResult SectionArticle(int size = 5)
        {
            return PartialView(ArticlesValidation.Take(size).ToList());
        }


        #endregion

    }
}

