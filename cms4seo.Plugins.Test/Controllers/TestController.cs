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

        [ChildActionOnly]
        public ActionResult Sub()
        {
            return View();
        }


        public ActionResult Query()
        {
            return View();
        }

    }
}
