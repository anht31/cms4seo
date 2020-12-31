using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Admin.Resources;
using cms4seo.Common.Attribute;
using cms4seo.Common.BootstrapHelpers;
using cms4seo.Common.Helpers;
using cms4seo.Common.Slug;
using cms4seo.Data;
using cms4seo.Data.IdentityModels;

namespace cms4seo.Admin.Controllers
{

    [CustomAuthorize(Roles = "Basic.ProductTag")]
    public class ProductTagController : BaseController
    {

        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Admin/ProductTag
        public ActionResult Index()
        {
            var productTags = db.ProductTags.ToList();

            var tags = productTags.Select(x => x.Name).ToList();

            if (Request.AcceptTypes != null && Request.AcceptTypes.Contains("application/json"))
            {
                return Json(tags, JsonRequestBehavior.AllowGet);
            }


            return View(tags);
        }



        [HttpGet]
        public ActionResult Edit(string tag)
        {
            try
            {
                var model = db.ProductTags.FirstOrDefault(x => x.Name == tag);
                return View(model: model?.Name);
            }
            catch (KeyNotFoundException e)
            {
                Debug.Assert(e != null, "result is null");
                return HttpNotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string tag, string newTag)
        {
            SlugComparer slugComparer = new SlugComparer();

            var tags = db.ProductTags.Select(x => x.Name).ToList();


            //var tagAfterExcept = (new List<string> {newTag}).Except(tags, slugComparer);

            if (!tags.Contains(tag, slugComparer))
            {
                TempData[MessageType.Warning] = AdminResources.ProductTagControllerEdit_Tag_not_found;
                return RedirectToAction("index");
                //return HttpNotFound();
            }

            if (tags.Contains(newTag, slugComparer))
            {
                TempData[MessageType.Warning] = 
                    AdminResources.ProductTagControllerEdit_Change_tag_fail___tag_already_in_database;
                
                return View(model: tag);
            }

            if (string.IsNullOrWhiteSpace(newTag))
            {
                ModelState.AddModelError("key", AdminResources.ProductTagControllerEditNewTagValueCannotBeEmpty);

                TempData[MessageType.Warning] = 
                    AdminResources.ProductTagControllerEdit_Change_tag_fail___tag_name_must_least_1_character;

                return View(model: tag);
            }


            var productTag = db.ProductTags.FirstOrDefault(x => x.Name == tag);
            if (productTag == null)
            {
                TempData[MessageType.Warning] = 
                    AdminResources.ProductTagControllerEdit_ProductTag_not_exist__please_check_it_;

                LogHelper.Write(@"Admin/ProductTag",
                    $"Access denied user: {User.Identity.Name} for attempt access edit tag: {tag}");

                return null;
            }

            productTag.Name = newTag;
            productTag.Slug = newTag.MakeUrlFriendly();
            db.Entry(productTag).State = EntityState.Modified;
            db.SaveChanges();
            

            return RedirectToAction("index");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            var productTag = db.ProductTags.FirstOrDefault(x => x.Name == id);

            if (productTag == null)
            {
                TempData[MessageType.Warning] = 
                    AdminResources.ProductTagControllerDelete_ProductTag_not_exist__please_check_it_;

                LogHelper.Write(@"Admin/ProductTag/Delete",
                    $"Access denied user: {User.Identity.Name} for attempt access delete tag: {id}");

                return null;
            }

            try
            {
                db.ProductTags.Remove(productTag);
                db.SaveChanges();

                return RedirectToAction("index");
            }
            catch (KeyNotFoundException e)
            {
                Trace.Write(e.Message, "EndRequest");
                return HttpNotFound();
            }
        }

    }
}
