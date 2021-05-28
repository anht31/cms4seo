using System;
using System.Linq;
using System.Web.Mvc;
using cms4seo.Data;
using cms4seo.Model.Entities;
using cms4seo.Data.IdentityModels;


namespace cms4seo.Web.Controllers
{
    public class NavController : Controller
    {


        private readonly ApplicationDbContext db = new ApplicationDbContext();


        private IQueryable<Category> CategoriesValidation =>
            db.Categories.Where(x => !(x.Name == null || x.Name.Trim() == String.Empty))
                .OrderByDescending(x => x.Sort);


        [ChildActionOnly]
        public PartialViewResult HeaderMenu(string category = null)
        {
            return PartialView(CategoriesValidation.ToList());
        }

        
        [ChildActionOnly]
        public ActionResult AsideMenu(string id)
        {
            return PartialView(new Category
            {
                Id = 0,
                Name = "Menu",
                Children = CategoriesValidation.Where(x => x.ParentId == null && x.IsAside).ToList()
            });
        }


        [ChildActionOnly]
        public ActionResult AsideMenuTopLevel(string id)
        {
            return PartialView(new Category
            {
                Id = 0,
                Name = "Menu",
                Children = CategoriesValidation.Where(x => x.ParentId == null && x.IsAside).ToList()
            });
        }


        [ChildActionOnly]
        public ActionResult FooterMenu()
        {
            return PartialView(CategoriesValidation.Where(x => x.ParentId == null && x.IsMenu).ToList());
        }



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }



        [ChildActionOnly]
        public ActionResult AsideLink()
        {
            return PartialView(CategoriesValidation.ToList());
        }


        [ChildActionOnly]
        public ActionResult SubMenu(int id = 0)
        {
            ViewBag.CID = id;


            return PartialView(CategoriesValidation.ToList());
        }
    }
}