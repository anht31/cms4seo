using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using cms4seo.Common.Attribute;
using cms4seo.Common.Helpers;
using cms4seo.Data;
using cms4seo.Data.IdentityModels;
using cms4seo.Model.Entities;
using cms4seo.Model.LekimaxType;

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
                // prevent conflict slug (some problem like this permalink exist on db) v2
                var slug = String.Copy(permalink.Slug);
                var n = 0;
                while (db.Permalinks.Any(x => 
                        !(x.EntityName == permalink.EntityName && x.EntityId == permalink.EntityId) 
                        && x.Slug == permalink.Slug))
                            permalink.Slug = $"{slug}-{++n}";

                

                db.Entry(permalink).State = EntityState.Modified;
                await db.SaveChangesAsync();

                switch (permalink.EntityName)
                {
                    case cms4seoEntityType.Category:
                        var category = db.Categories.FirstOrDefault(x => x.Id == permalink.EntityId);
                        if (category != null)
                        {
                            category.Slug = permalink.Slug;
                            db.Entry(category).State = EntityState.Modified;
                        }
                        break;
                    case cms4seoEntityType.Product:
                        var product = db.Products.FirstOrDefault(x => x.Id == permalink.EntityId);
                        if (product != null)
                        {
                            product.Slug = permalink.Slug;
                            db.Entry(product).State = EntityState.Modified;
                        }
                        break;
                    case cms4seoEntityType.Topic:
                        var topic = db.Topics.FirstOrDefault(x => x.Id == permalink.EntityId);
                        if (topic != null)
                        {
                            topic.Slug = permalink.Slug;
                            db.Entry(topic).State = EntityState.Modified;
                        }
                        break;
                    case cms4seoEntityType.Article:
                        var article = db.Articles.FirstOrDefault(x => x.Id == permalink.EntityId);
                        if (article != null)
                        {
                            article.Slug = permalink.Slug;
                            db.Entry(article).State = EntityState.Modified;
                        }
                        break;
                    case cms4seoEntityType.Info:
                        var info = db.Infos.FirstOrDefault(x => x.Id == permalink.EntityId);
                        if (info != null)
                        {
                            info.Slug = permalink.Slug;
                            db.Entry(info).State = EntityState.Modified;
                        }
                        break;
                    default:
                        LogHelper.Write("PermalinksController.Edit()", $"permalink.EntityName not found");
                        break;

                }

                db.SaveChanges();

                LogHelper.Write("PermalinksController.Edit()", $"user {User.Identity.Name} update Slug:{permalink.Slug}");

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
