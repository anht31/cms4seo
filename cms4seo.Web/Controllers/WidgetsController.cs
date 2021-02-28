using System.Linq;
using System.Web.Mvc;
using cms4seo.Common.Helpers;

namespace cms4seo.Web.Controllers
{
    public class WidgetsController : Controller
    {
        public ActionResult Index(string zone, string page)
        {
            if (page == null)
                return View(PluginHelpers.Widgets
                    .Where(x => x.Zone == zone).ToList());

            return View(PluginHelpers.Widgets
                .Where(x => x.Zone == zone && x.Page == page).ToList());
        }
    }
}