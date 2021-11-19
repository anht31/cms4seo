using System.Linq;
using System.Web.Mvc;
using cms4seo.Data;
using cms4seo.Service.Content;
using PagedList;

namespace cms4seo.Web.Controllers
{
    public class HomeController : Controller
    {

        private readonly ApplicationDbContext db = new ApplicationDbContext();


        /// <summary>
        /// Query product for some design like foundation
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        //[DynamicOutputCache(VaryByParam = "none")]
        public ActionResult Index(int? page)
        {
            var products = db.Products.Where(x => x.IsPublish).ToList();

            var pageNumber = page ?? 1; // if no page was specified in the querystring, default to the first page (1)
            var onePage = products.ToPagedList(pageNumber, 60); // will only contain 25 blog max because of the pageSize

            // preload hero slider
            ViewBag.HeroImage = db.Sliders.FirstOrDefault(x => x.Active == "active")?.AvatarMobile;

            return View(onePage);
        }

        [ChildActionOnly]
        public ActionResult Slider()
        {
            return PartialView(db.Sliders.Where(s => s.IsPublish).OrderBy(x => x.Sort));
        }



        public MvcHtmlString CmsSwitcher()
        {

            var permission = User.IsInRole("Medium.Contents");

            if (!permission)
            {
                CMS.Active = false;
                return new MvcHtmlString("<script>window.IsAuthorizeEditContent = false;</script>");
            }

            var html =
                "<div class='se-pre-con'></div>" +
                "<div id='cms-switcher'>" +
                    "<h2>CMS Switcher" +
                        "<a href='#'>" +
                            "<i class='fa fa-cogs'></i>" +
                        "</a>" +
                    "</h2>" +
                    "<div>" +

                    "<h3>Themes</h3>" +
                    "<div class='themes-option-style'>" +
                        "<select id='theme-switcher'>" +
                            CMS.BuildThemesOption() +
                        "</select>" +
                    "</div>" +

                    CMS.RenderBootswatch() +

                    "<h3>Contents Toggle</h3>" +
                    "<label class='toggleSwitch'>" +
                        "<input type='checkbox' id='switchEditContents'>" +
                        "<span class='checkbox-slider round'></span>" +
                    "</label>" +
                    "</div>" +
                "</div>" +
                "<script>window.IsAuthorizeEditContent = true;</script>";



            CMS.Active = true;
            return new MvcHtmlString(html);
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