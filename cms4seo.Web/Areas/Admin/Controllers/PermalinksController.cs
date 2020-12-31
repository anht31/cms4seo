using System.Data.Entity;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using cms4seo.Common.Attribute;
using cms4seo.Data;
using cms4seo.Data.IdentityModels;
using cms4seo.Model.Entities;

namespace cms4seo.Admin.Controllers
{
    [CustomAuthorize(Roles = "Medium.Permalinks")]
    public class PermalinksController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Admin/Permalinks
        public async Task<ActionResult> Index()
        {
            return View(await db.Permalinks.ToListAsync());
        }

        // GET: Admin/Permalinks/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Permalink permalink = await db.Permalinks.FindAsync(id);
            if (permalink == null)
            {
                return HttpNotFound();
            }
            return View(permalink);
        }

        // GET: Admin/Permalinks/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Permalinks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,EntityId,EntityName,Slug,IsActive,LanguageId")] Permalink permalink)
        {
            if (ModelState.IsValid)
            {
                db.Permalinks.Add(permalink);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(permalink);
        }

        // GET: Admin/Permalinks/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Permalink permalink = await db.Permalinks.FindAsync(id);
            if (permalink == null)
            {
                return HttpNotFound();
            }
            return View(permalink);
        }

        // POST: Admin/Permalinks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,EntityId,EntityName,Slug,IsActive,LanguageId")] Permalink permalink)
        {
            if (ModelState.IsValid)
            {
                db.Entry(permalink).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(permalink);
        }

        // GET: Admin/Permalinks/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Permalink permalink = await db.Permalinks.FindAsync(id);
            if (permalink == null)
            {
                return HttpNotFound();
            }
            return View(permalink);
        }

        // POST: Admin/Permalinks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Permalink permalink = await db.Permalinks.FindAsync(id);
            db.Permalinks.Remove(permalink);
            await db.SaveChangesAsync();
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
