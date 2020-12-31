using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using cms4seo.Data;
using cms4seo.Model.Entities;
using cms4seo.Data.IdentityModels;
using PagedList;

namespace cms4seo.Web.Controllers
{
    public class ArticleTagsController : Controller
    {
        
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        // GET: ArticleTags
        [ChildActionOnly]
        public ActionResult AllTag()
        {
            return PartialView(db.ArticleTags.ToList());
        }

        // GET: ArticleTags/Details/5
        public async Task<ActionResult> ListBy(string slug, int? page)
        {
            if (slug == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ArticleTag articleTag = await db.ArticleTags.FirstOrDefaultAsync(x => x.Slug == "/" + slug);
            if (articleTag == null)
            {
                return HttpNotFound();
            }

            var articles = articleTag.Articles.Where(x => x.IsPublish).ToList();

            var pageNumber = page ?? 1; // if no page was specified in the querystring, default to the first page (1)
            var onePage = articles.ToPagedList(pageNumber, 60); // will only contain 25 blog max because of the pageSize

            ViewBag.Tag = articleTag.Name;
            ViewBag.Slug = slug;
            return View(onePage);
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
