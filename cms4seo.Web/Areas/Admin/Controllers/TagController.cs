using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Admin.Resources;
using cms4seo.Common.Attribute;
using cms4seo.Data.Repositories;

namespace cms4seo.Admin.Controllers
{
    [CustomAuthorize(Roles = "Basic.Tag")]
    public class TagController : BaseController
    {
        private readonly ITagRepository _repository;

        public TagController() : this(new TagRepository())
        {
        }

        public TagController(ITagRepository repository)
        {
            _repository = repository;
        }

        // GET: Admin/Tag        
        public ActionResult Index()
        {
            var tags = _repository.GetAll();

            if (Request.AcceptTypes.Contains("application/json"))
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
                var model = _repository.Get(tag);
                return View(model: model);
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
            var tags = _repository.GetAll();

            if (!tags.Contains(tag))
            {
                return HttpNotFound();
            }

            if (tags.Contains(newTag))
            {
                return RedirectToAction("index");
            }

            if (string.IsNullOrWhiteSpace(newTag))
            {
                ModelState.AddModelError("key", AdminResources.TagControllerEditNewTagValueCannotBeEmpty);

                return View(model: tag);
            }

            _repository.Edit(tag, newTag);

            return RedirectToAction("index");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string deleteId)
        {
            var tag = deleteId;
            try
            {
                _repository.Delete(tag);

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