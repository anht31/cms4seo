using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.WebPages;
using Admin.Resources;
using cms4seo.Admin.Services;
using cms4seo.Common.Attribute;
using cms4seo.Common.BootstrapHelpers;
using cms4seo.Common.Helpers;
using cms4seo.Data;
using cms4seo.Model.Entities;
using cms4seo.Model.LekimaxType;
using cms4seo.Service.Provider;
using cms4seo.Service.Sitemap;

namespace cms4seo.Admin.Controllers
{

    [CustomAuthorize(Roles = "Medium.SearchOptimize")]
    public class SearchOptimizeController : BaseController
    {
        // GET: Admin/SearchOptimize
        public ActionResult Index()
        {
            return View();
        }
        

        public SiteMapResult SiteMap()
        {
            string url = Request.Url?.AbsoluteUri;
            var service = new SiteMapService(url);
            var siteMap = service.GetSiteMap();
            return new SiteMapResult(siteMap);
        }


        public ActionResult ReindexSiteMap()
        {
            string url = Request.Url?.AbsoluteUri;
            var service = new SiteMapService(url);
            var siteMap = service.GetSiteMap();

            service.SaveSiteMap(siteMap);

            TempData[MessageType.Warning] = 
                string.Format(AdminResources.SearchOptimizeControllerReindexSiteMap_Reindex_Sitemap_successfull____0___items_submited, siteMap.Items.Count);

            LogHelper.Write(@"Admin/SearchOptimize",
                $"User: {User.Identity.Name} re-indexed sitemap.xml, total items: [{siteMap.Items.Count}]");

            return RedirectToAction("Index");
        }



        public ActionResult ReseedPermalink()
        {
            if (!Setting.WebSettings[WebSettingType.IsAutoEditPermalink].AsBool())
            {
                TempData[MessageType.Warning] = 
                    AdminResources.SearchOptimizeController_ReseedPermalink_Auto_Edit_Permalink_is_off_please_turn_on;

                return RedirectToAction("Index");
            }

            using (var db = new ApplicationDbContext())
            {
                var count = db.Permalinks.Count();

                if (count > 3000)
                {
                    TempData[MessageType.Warning] =
                        string.Format(AdminResources.SearchOptimizeController_ReseedPermalink_Just_support_3000_Record, count);

                    return RedirectToAction("Index");
                }
            }


            


            var permalinkService = new PermalinkService();
            var permalinkList = permalinkService.Rebuild();


            TempData[MessageType.Warning] = 
                string.Format(AdminResources.SearchOptimizeController_ReseedPermalink_Reseed_Permalink_successfull____0___items_submited, permalinkList.Count);

            LogHelper.Write(@"Admin/SearchOptimize",
                $"User: {User.Identity.Name} Reseed Permalink, total items: [{permalinkList.Count}]");

            return RedirectToAction("Index");

        }


        public ActionResult ReindexPermalink()
        {
            var permalinkService = new PermalinkService();
            var permalinkList = permalinkService.Reindex();



            TempData[MessageType.Warning] =
                string.Format(AdminResources.SearchOptimizeControllerReindexPermalink_Reindex_Permalink_successfull____0___items_submited, permalinkList.Count);

            LogHelper.Write(@"Admin/SearchOptimize",
                $"User: {User.Identity.Name} Reindex Permalink, total items: [{permalinkList.Count}]");

            return RedirectToAction("Index");

        }

    }
}