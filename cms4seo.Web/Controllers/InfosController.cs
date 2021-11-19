using System.Linq;
using System.Net;
using System.Web.Mvc;
using cms4seo.Data;
using cms4seo.Data.IdentityModels;

namespace cms4seo.Web.Controllers
{
    public class InfosController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();



        // GET: /slug	
        public ActionResult Details(int id)
        {


            var info = db.Infos.FirstOrDefault(x => x.Id == id);

            if (info == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound, "info not found");
            }


            return View(info);
        }


        [ChildActionOnly]
        public ActionResult SectionGallery()
        {
            var info = db.Infos.FirstOrDefault(x => x.Location == 3);

            if (info == null)
                return null;

            return View(info);
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
