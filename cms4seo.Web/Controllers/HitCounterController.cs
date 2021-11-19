using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using cms4seo.Data;
using cms4seo.Data.IdentityModels;


namespace cms4seo.Web.Controllers
{
    public class HitCounterController : Controller
    {

        private readonly ApplicationDbContext db = new ApplicationDbContext();

        // GET: HitCounter
        [ChildActionOnly]
        public ActionResult AsideCounter()
        {

            var today = DateTime.Today;

            if (!db.UserCounters.Any())
            {
                ViewBag.UserCounter = 0;
                ViewBag.DailyCounter = 0;
                return PartialView(0);
            }

            ViewBag.UserCounter = db.UserCounters.Max(x => x.Id);
            ViewBag.DailyCounter = db.HitCounters.Count(x => DbFunctions.TruncateTime(x.OpenTime) == today.Date);
            return PartialView(db.HitCounters.Max(x => x.CounterId));
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