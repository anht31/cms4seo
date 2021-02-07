using System.Web.Mvc;

namespace cms4seo.Plugins.Test.Controllers
{
    public class TestController : Controller
    {
        [ChildActionOnly]
        public ActionResult Index()
        {
            return View();
        }

    }
}
