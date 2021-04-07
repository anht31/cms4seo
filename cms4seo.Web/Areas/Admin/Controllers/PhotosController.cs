using System;
using System.Data.Entity;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Admin.Resources;
using cms4seo.Common.Attribute;
using cms4seo.Common.BootstrapHelpers;
using cms4seo.Common.Helpers;
using cms4seo.Data;
using cms4seo.Model.Entities;
using cms4seo.Service.Photo;

namespace cms4seo.Admin.Controllers
{
    [CustomAuthorize(Roles = "Medium.Photos")]
    public class PhotosController : BaseController
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        // GET: Admin/Photos
        public async Task<ActionResult> Index()
        {
            return View(await db.Photos.ToListAsync());
        }


        // GET: Admin/Photos/Create
        public ActionResult Create()
        {
            return View("Edit", new Photo());
        }


        // GET: Admin/Photos/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Photo photo = await db.Photos.FindAsync(id);
            if (photo == null)
            {
                return HttpNotFound();
            }
            return View(photo);
        }

        // POST: Admin/Photos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Photo photo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(photo).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(photo);
        }


        // POST: Admin/Photos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Photo photo = await db.Photos.FindAsync(id);

            if (photo == null)
            {
                LogHelper.Write(@"Admin/Photos",
                    $"Photo not found: {User.Identity.Name} for try delete photo");

                return RedirectToAction("Index");
            }


            try
            {
                var photoService = new PhotoService();
                photoService.DeletePhoto(photo);
                db.Photos.Remove(photo);

            }
            catch (Exception e)
            {
                TempData[MessageType.Danger] =
                    string.Format(AdminResources.PhotosController_Delete_photo_Message_0, e.Message);

                LogHelper.Write(@"Admin/Photos",
                    $"Deleted photo fail, user: {User.Identity.Name}, message: {e.Message}");

                return RedirectToAction("Index");
            }


            
            await db.SaveChangesAsync();
            //return View("Index"); // not refresh
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
