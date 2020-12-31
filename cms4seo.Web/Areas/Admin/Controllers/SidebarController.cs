using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using cms4seo.Data;

namespace cms4seo.Admin.Controllers
{
    public class SidebarController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();


        [ChildActionOnly]
        public ActionResult Index()
        {
            int messageUnread;
            using (var applicationDbContext = new ApplicationDbContext())
            {
                messageUnread = applicationDbContext.Contacts.Count(x => x.ReadBy == null);
            }

            ViewBag.MessageUnread = messageUnread;
            return PartialView();
        }
    }
}