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
    public class ProductTagsController : Controller
    {
        
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        // GET: ProductTags
        [ChildActionOnly]
        public ActionResult AllTag()
        {
            return PartialView(db.ProductTags.ToList());
        }

        // GET: ProductTags/Details/5
        public async Task<ActionResult> ListBy(string slug, int? page)
        {
            if (slug == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductTag productTag = await db.ProductTags.FirstOrDefaultAsync(x => x.Slug == "/" + slug);
            if (productTag == null)
            {
                //return HttpNotFound("Không tìm thấy Tag yêu cầu");

                return new HttpStatusCodeResult(HttpStatusCode.NotFound, "Không tìm thấy Tag yêu cầu");
            }

            var products = productTag.Products.ToList();

            var pageNumber = page ?? 1; // if no page was specified in the querystring, default to the first page (1)
            var onePage = products.ToPagedList(pageNumber, 60); // will only contain 25 blog max because of the pageSize

            ViewBag.Tag = productTag.Name;
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
