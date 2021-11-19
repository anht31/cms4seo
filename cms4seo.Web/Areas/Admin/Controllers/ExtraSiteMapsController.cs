using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Admin.Resources;
using cms4seo.Common.Attribute;
using cms4seo.Common.BootstrapHelpers;
using cms4seo.Common.Helpers;
using cms4seo.Data;
using cms4seo.Data.IdentityModels;
using cms4seo.Model.Entities;

namespace cms4seo.Admin.Controllers
{
    [CustomAuthorize(Roles = "Medium.ExtraSiteMaps")]
    public class ExtraSiteMapsController : BaseController
    {
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        // ReSharper disable once InconsistentNaming
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Admin/ExtraSiteMaps
        public async Task<ActionResult> Index()
        {
            return View(await db.ExtraSiteMaps.ToListAsync());
        }

        // GET: Admin/ExtraSiteMaps/Create
        public ActionResult Create()
        {
            return View("Edit", new ExtraSiteMap());
        }


        // GET: Admin/ExtraSiteMaps/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ExtraSiteMap extraSiteMap = await db.ExtraSiteMaps.FindAsync(id);
            if (extraSiteMap == null)
            {
                return HttpNotFound();
            }
            return View(extraSiteMap);
        }

        // POST: Admin/ExtraSiteMaps/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(ExtraSiteMap extraSiteMap)
        {
            if (ModelState.IsValid)
            {
                extraSiteMap.LastMod = DateTime.Now;
                
                db.ExtraSiteMaps.AddOrUpdate(extraSiteMap);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(extraSiteMap);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // GET: Admin/ExtraSiteMaps/Delete/5
        public async Task<RedirectToRouteResult> Delete(int? id)
        {
            if (id == null)
            {
                TempData[MessageType.Warning] = 
                    AdminResources.ExtraSiteMapsControllerDelete_Request_Invalid__id_is_null;

                return RedirectToAction("Index");
            }

            ExtraSiteMap extraSiteMap = await db.ExtraSiteMaps.FindAsync(id);
            

            if (extraSiteMap == null)
            {
                TempData[MessageType.Warning] = 
                    AdminResources.ExtraSiteMapsControllerDelete_Not_found_sitemap_item_to_delete;

                LogHelper.Write(@"Admin/ExtraSiteMaps/Delete",
                    $"Sitemap Item not found by Id: {id}");

                return RedirectToAction("Index");
            }

            db.ExtraSiteMaps.Remove(extraSiteMap);
            await db.SaveChangesAsync();

            TempData[MessageType.Warning] = 
                string.Format(AdminResources.ExtraSiteMapsControllerDelete_Delete_success_SiteMap_Item___0__, extraSiteMap.Name);

            LogHelper.Write(@"Admin/ExtraSiteMaps/Delete",
                $"User {User.Identity.Name} has been delete Item '{extraSiteMap.Name}'");
                
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
