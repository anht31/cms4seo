using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Admin.Resources;
using cms4seo.Admin.Services;
using cms4seo.Common.Attribute;
using cms4seo.Common.BootstrapHelpers;
using cms4seo.Common.Helpers;
using cms4seo.Data;
using cms4seo.Data.IdentityModels;
using cms4seo.Model.Entities;
using cms4seo.Service.Photo;
using Microsoft.Ajax.Utilities;

namespace cms4seo.Admin.Controllers
{
    [CustomAuthorize(Roles = "Basic.Slider")]
    public class SlidersController : BaseController
    {

        private ApplicationDbContext db = new ApplicationDbContext();


        private IQueryable<Slider> SlidersValidation =>
            db.Sliders.Where(x => !(x.Name == null || x.Name.Trim() == String.Empty));

        private IQueryable<Slider> SlidersInvalidation =>
            db.Sliders.Where(x => x.Name == null || x.Name.Trim() == String.Empty);


        // GET: Admin/Slider
        public ActionResult Index()
        {            
            return View(SlidersValidation.ToList());
        }


        // Create =======================================================================================
        public async Task<ActionResult> Create()
        {

            var slider = await SlidersInvalidation
                .FirstOrDefaultAsync(x => x.PostBy == User.Identity.Name);

            if (slider == null)
            {
                slider = new Slider
                {
                    PostBy = User.Identity.Name,
                    IsCreate = true,
                    Sort = db.Sliders.DefaultIfEmpty().Max(x => x == null ? 0 : x.Sort + 1)
                };

                db.Sliders.Add(slider);
                await db.SaveChangesAsync();
            }


            // Sort helper
            var maxSort = db.Sliders.Max(x => x.Sort) + 1;
            var sortList = db.Sliders.DistinctBy(g => g.Sort).ToDictionary(x => x.Sort, x => x.Sort);
            sortList.Add(-1, -1);
            sortList.Add(maxSort, maxSort);

            // remove non work key
            if (sortList.ContainsKey(slider.Sort - 1))
                sortList.Remove(slider.Sort - 1);

            ViewBag.Sort = new SelectList(sortList.OrderBy(x => x.Value), "Key", "Value", slider.Sort);

            return View("Edit", slider);
        }



        public async Task<ActionResult> Edit(int id)
        {
            var slider = await db.Sliders.FindAsync(id);

            if (slider == null)
            {
                LogHelper.Write(@"Admin/Slider",
                    AdminResources.SlidersControllerEdit_slider_not_found);

                TempData[MessageType.Danger] = 
                    AdminResources.SlidersControllerEdit_slider_not_found;

                return RedirectToAction("Index");
            }

            
            // Sort helper
            var maxSort = db.Sliders.Max(x => x.Sort) + 1;
            var sortList = db.Sliders.DistinctBy(g => g.Sort).ToDictionary(x => x.Sort, x => x.Sort);
            sortList.Add(-1, -1);
            sortList.Add(maxSort, maxSort);

            // remove non work key
            if (sortList.ContainsKey(slider.Sort - 1))
                sortList.Remove(slider.Sort - 1);

            ViewBag.Sort = new SelectList(sortList.OrderBy(x => x.Value), "Key", "Value", slider.Sort);

            return View(slider);
        }



        [HttpPost]
        public async Task<ActionResult> Edit(Slider slider)
        {

            if (String.IsNullOrWhiteSpace(slider.Name))
            {
                ModelState.AddModelError("Name", AdminResources.SlidersControllerEditYouMustEnterName);
            }



            if (ModelState.IsValid)
            {

                // avatar
                if (string.IsNullOrEmpty(slider.Avatar) && slider.Photos.Any())
                    slider.Avatar = slider.Photos.FirstOrDefault()?.LgPath;


                slider.Active = slider.Sort == 0 ? "active" : "";
                slider.IsPublish = true;
                slider.Alt = slider.Name.MakeNameFriendly();

                //// check conflict sort
                //if (slider.IsCreate)
                //{
                //    while (sliders.Exists(x => x.Sort == slider.Sort))
                //    {
                //        ++slider.Sort;
                //    }
                //}
                //else
                //{
                    
                //}


                LogHelper.Write(@"Admin/Slider",
                    slider.IsCreate
                        ? $"User: {User.Identity.Name} created slider: {slider.Alt}"
                        : $"User: {User.Identity.Name} updated slider: {slider.Alt}");


                slider.IsCreate = false;
                db.Entry(slider).State = EntityState.Modified;
                await db.SaveChangesAsync();


                // resort order
                var sliders = await db.Sliders.ToListAsync();
                var i = 0;
                foreach (var item in sliders.OrderBy(x => x.Sort).ThenBy(x => x.Id == slider.Id))
                {
                    item.Active = (i == 0 ? "active" : "");
                    item.Sort = i++;
                }
                await db.SaveChangesAsync();


                TempData[MessageType.Success] = 
                    string.Format(AdminResources.SlidersControllerEdit__0__already_saved, slider.Name);

                return RedirectToAction("Index");
            }


            // Sort helper
            var maxSort = db.Sliders.Max(x => x.Sort) + 1;
            var sortList = db.Sliders.DistinctBy(g => g.Sort).ToDictionary(x => x.Sort, x => x.Sort);
            sortList.Add(-1, -1);
            sortList.Add(maxSort, maxSort);

            // remove non work key
            if (sortList.ContainsKey(slider.Sort - 1))
                sortList.Remove(slider.Sort - 1);

            ViewBag.Sort = new SelectList(sortList.OrderBy(x => x.Value), "Key", "Value", slider.Sort);

            return View(slider);
        }

        

        // Del ==========================================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int deleteId)
        {
            var slider = db.Sliders.Find(deleteId);

            if (slider != null)
            {

                try
                {
                    var photoService = new PhotoService();
                    photoService.DeletePhotoList(slider.Photos);
                    db.Photos.RemoveRange(slider.Photos);
                }
                catch (Exception e)
                {
                    TempData[MessageType.Danger] = 
                        string.Format(AdminResources.SlidersControllerDelete_Error_rising_when_try_delete_photo__Message___0_, e.Message);

                }

                db.Sliders.Remove(slider);
                db.SaveChanges();



                // resort order
                var sliders = db.Sliders.ToList();
                var i = 0;
                foreach (var item in sliders.OrderBy(x => x.Sort).ThenBy(x => x.Id == slider.Id))
                {
                    item.Active = (i == 0 ? "active" : "");
                    item.Sort = i++;
                }
                db.SaveChanges();


                TempData[MessageType.Warning] = 
                    string.Format(AdminResources.SlidersControllerDelete__0__was_deleted, slider.Name);

                LogHelper.Write(@"Admin/Slider",
                    $"User: {User.Identity.Name} deleted slider: {slider.Alt}");
                    
            }
            return RedirectToAction("Index");
        }


        // dispose --------------------------------------------     

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