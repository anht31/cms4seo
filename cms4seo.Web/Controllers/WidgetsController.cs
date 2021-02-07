using System.Linq;
using System.Web.Mvc;
using cms4seo.Common.Helpers;

namespace cms4seo.Web.Controllers
{
    public class WidgetsController : Controller
    {
        public ActionResult Index(string zone)
        {
            return View(PluginHelpers.Widgets
                .Where(x => x.Zone == zone).ToList());
        }
    }
}