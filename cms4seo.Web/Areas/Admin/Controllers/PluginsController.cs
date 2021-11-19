using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.WebPages;
using cms4seo.Common.Helpers;
using cms4seo.Model.Abstract;

namespace cms4seo.Admin.Controllers
{
    [Authorize]
    public class PluginsController : BaseController
    {
        // GET: Plugins
        public ActionResult Index()
        {
            return View(PluginHelpers.Widgets);
        }


        public ActionResult Toggle(bool active, string area, string zone)
        {
            // test with realtime mode first
            var widget = PluginHelpers.Widgets.FirstOrDefault(x => x.Area == area && x.Zone == zone);
            widget.Active = active.ToString();

            var path = widget.Path;

            var widgets = PluginHelpers.Widgets.Where(x => x.Path == path).ToList();
            
            JsonHelpers.SaveWidget(HostingEnvironment.MapPath(path), widgets);

            // reload
            HttpRuntime.UnloadAppDomain();

            return RedirectToAction("Index");
        }


    }
}