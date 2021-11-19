using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Admin.Resources;

namespace cms4seo.Admin.Controllers
{
    [Authorize]
    public class ResourceController : BaseController
    {
        // GET: /Resource/GetResources
        const int durationInSeconds = 2 * 60 * 60;  // 2 hours.
        [OutputCache(VaryByCustom = "culture", Duration = durationInSeconds)]
        public JsonResult GetResources()
        {
            var resources = typeof(AdminResources)
                 .GetProperties()
                 .Where(p => p.Name.Contains("js_")) // Skip the properties you don't need on the client side.
                 .ToDictionary(p => p.Name, p => p.GetValue(null) as string);

            //resources.Add("js_DataTableLanguage", Common.Culture.CultureHelper.GetLanguageWithoutCountry());

            return Json(resources, JsonRequestBehavior.AllowGet);
        }
    }
}