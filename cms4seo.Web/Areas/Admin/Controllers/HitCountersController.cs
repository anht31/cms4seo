using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Admin.Resources;
using cms4seo.Common.Attribute;
using cms4seo.Common.BootstrapHelpers;
using cms4seo.Data;
using cms4seo.Data.IdentityModels;
using cms4seo.Model.Entities;
using PagedList;

namespace cms4seo.Admin.Controllers
{
    [CustomAuthorize(Roles = "Medium.HitCounters")]
    public class HitCountersController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Admin/HitCounters
		public async Task<ActionResult> Index(int? page)
        {
            var hitCounters = await db.HitCounters.ToListAsync();            
            var onePage = hitCounters.ToPagedList(page ?? 1, 500);
            return View(onePage);
        }
        

		// GET: Admin/HitCounters/Search
		public async Task<ActionResult> Search(string searchItem, int? page)

		{

		    var searchDateTime = Convert.ToDateTime(searchItem);

            return View("Index", (
                await db.HitCounters.Where(x => DbFunctions.TruncateTime(x.OpenTime) == DbFunctions.TruncateTime(searchDateTime)).
					ToListAsync()).ToPagedList(page ?? 1, 500));
        }


        // GET: Admin/HitCounters/Create
        public ActionResult Create()
        {
            return View("Edit", new HitCounter());
        }
        

        // GET: Admin/HitCounters/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HitCounter hitCounter = await db.HitCounters.FindAsync(id);
            if (hitCounter == null)
            {
                return HttpNotFound();
            }
            return View(hitCounter);
        }

        // POST: Admin/HitCounters/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(HitCounter hitCounter)
        {
            if (ModelState.IsValid)
            {
				if(hitCounter.CounterId == 0)
				{
					db.HitCounters.Add(hitCounter);
				}
				else
				{
					db.Entry(hitCounter).State = EntityState.Modified;
				}
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(hitCounter);
        }
        

        // POST: Admin/HitCounters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<PartialViewResult> DeleteConfirmed(int id)
        {
            HitCounter hitCounter = await db.HitCounters.FindAsync(id);
            db.HitCounters.Remove(hitCounter);
            await db.SaveChangesAsync();
			TempData[MessageType.Warning] = 
                string.Format(AdminResources.HitCountersControllerDeleteConfirmed__0__was_deleted, hitCounter.CounterId);
            return new PartialViewResult();
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
