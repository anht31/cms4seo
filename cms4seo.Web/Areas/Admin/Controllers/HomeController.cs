using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls.Expressions;
using Admin.Resources;
using cms4seo.Common.Attribute;
using cms4seo.Common.Culture;
using cms4seo.Data;
using cms4seo.Data.IdentityModels;
using cms4seo.Data.Repositories;
using cms4seo.Model.LekimaxType;
using cms4seo.Model.ViewModel;
using cms4seo.Service.Content;
using cms4seo.Service.Provider;

namespace cms4seo.Admin.Controllers
{
    [CustomAuthorize(Roles = "Basic.Common")]
    public class HomeController : BaseController
    {
        private readonly IHitCounterRepository hitCounterRepo;
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        public HomeController()
        {
            hitCounterRepo = new HitCounterRepository();
        }

        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        [ChildActionOnly]
        public ActionResult Statistics()
        {

            ViewBag.CategoryCount = db.Categories.Count();
            ViewBag.ProductCount = db.Products.Count();
            ViewBag.OrderCount = 0;
            ViewBag.ArticleCount = db.Articles.Count(x => x.IsPublish);

            return PartialView();
        }

        public ActionResult LineChart()
        {
            var stats = hitCounterRepo.GetOneMonth();

            return Json(stats, JsonRequestBehavior.AllowGet);
        }


        public ActionResult PieChart()
        {

            var pieChart = new List<PieChartVm>();

            var results = from products in db.Products
                group products by products.Category.Name
                into g
                orderby g.Key
                select new
                {
                    Label = g.Key,
                    Count = g.Select(x => x.CategoryId).Count()
                };

            var pieChartAll = results.Select(
                x => new PieChartVm {
                Label = x.Label,
                Count = x.Count
                }).ToList();


            var top3 = pieChartAll.OrderByDescending(x => x.Count).Take(3).ToList();
            var otherCount = pieChartAll.OrderByDescending(x => x.Count).Skip(3).Sum(x => x.Count);
            var remainPie = new PieChartVm()
            {
                Label = AdminResources.HomePiechartOther,
                Count = otherCount
            };
            pieChart.AddRange(top3);
            pieChart.Add(remainPie);

            return Json(pieChart, JsonRequestBehavior.AllowGet);

        }


        /// <summary>
        /// Set Culture by set 
        /// </summary>
        /// <param name="culture"></param>
        /// <returns></returns>
        public ActionResult SetCulture(string culture)
        {
            // No more working by 
            // Mark cross-site cookies as Secure to allow setting them in cross - site contexts
            //// Validate input
            //culture = CultureHelper.GetImplementedCulture(culture);
            //// Save culture in a cookie
            //HttpCookie cookie = Request.Cookies["_culture"];
            //if (cookie != null)
            //    cookie.Value = culture;   // update cookie value
            //else
            //{
            //    cookie = new HttpCookie("_culture");
            //    cookie.Value = culture;
            //    cookie.Expires = DateTime.Now.AddYears(1);
            //}
            //Response.Cookies.Add(cookie);


            // Alt way
            var settingRepository = new SettingRepository();

            settingRepository.Set(WebSettingType.AdminLanguages, culture);

            return RedirectToAction("Index");
        }


        public ActionResult CacheDuration(int id)
        {
            CMS.CacheDuration = id;
            return RedirectToAction("Index");
        }

        public ActionResult RemoveOutputCache()
        {
            HttpResponse.RemoveOutputCacheItem("/");
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
    }
}