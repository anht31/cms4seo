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

namespace cms4seo.Admin.Controllers
{

    [CustomAuthorize(Roles = "Basic.ArticleTag")]
    public class ArticleTagController : BaseController
    {

        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Admin/ArticleTag
        public ActionResult Index()
        {
            var articleTags = db.ArticleTags.ToList();

            var tags = articleTags.Select(x => x.Name).ToList();

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
                var model = db.ArticleTags.FirstOrDefault(x => x.Name == tag);
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

            var tags = db.ArticleTags.Select(x => x.Name).ToList();


            //var tagAfterExcept = (new List<string> {newTag}).Except(tags, slugComparer);

            if (!tags.Contains(tag, slugComparer))
            {
                TempData[MessageType.Warning] = AdminResources.ArticleTagControllerEdit_Not_found_tag_need_edit;
                return RedirectToAction("index");
                //return HttpNotFound();
            }

            if (tags.Contains(newTag, slugComparer))
            {
                TempData[MessageType.Warning] = AdminResources.ArticleTagControllerEdit_Rename_Tag_fail___tag_name_have_already_in_database;

                return View(model: tag);
            }

            if (string.IsNullOrWhiteSpace(newTag))
            {
                ModelState.AddModelError("key", AdminResources.ArticleTagControllerEditNewTagValueCannotBeEmpty);

                TempData[MessageType.Warning] = AdminResources.ArticleTagControllerEdit_Rename_tag_fail___tag_must_least_1_character;

                return View(model: tag);
            }


            var articleTag = db.ArticleTags.FirstOrDefault(x => x.Name == tag);
            if (articleTag == null)
            {
                TempData[MessageType.Warning] = AdminResources.ArticleTagControllerEditArticleTag_not_exist__please_check_it_;

                LogHelper.Write(@"Admin/ArticleTag/Edit",
                    $"Access denied user: {User.Identity.Name} for attempt access edit tag: {tag}");

                return null;
            }

            articleTag.Name = newTag;
            articleTag.Slug = newTag.MakeUrlFriendly();
            db.Entry(articleTag).State = EntityState.Modified;
            db.SaveChanges();


            return RedirectToAction("index");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            var articleTag = db.ArticleTags.FirstOrDefault(x => x.Name == id);

            if (articleTag == null)
            {
                TempData[MessageType.Warning] = AdminResources.ArticleTagControllerDelete_ArticleTag_not_exist__please_check_it_;

                LogHelper.Write(@"Admin/ArticleTag/Delete",
                    $"Access denied user: {User.Identity.Name} for attempt access delete tag: {id}");

                
                return null;
            }

            try
            {
                db.ArticleTags.Remove(articleTag);
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
