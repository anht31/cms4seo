using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.WebPages;
using cms4seo.Data;
using cms4seo.Model.Entities;
using cms4seo.Data.IdentityModels;
using cms4seo.Model.LekimaxType;
using cms4seo.Model.ViewModel;
using cms4seo.Service;
using cms4seo.Service.Product;
using cms4seo.Service.Provider;
using PagedList;


namespace cms4seo.Web.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();


        private IQueryable<Category> CategoriesValidation =>
            db.Categories.Where(x => !(x.Name == null || x.Name.Trim() == String.Empty));



        public ActionResult Details(int id, int? page)
        {
            
            var category = CategoriesValidation.FirstOrDefault(x => x.Id == id);

            if (category != null)
            {
                ViewBag.CategoryName = category.Name;

                // -> ViewBy -> layout -> AsideMenu
                ViewBag.CategoryId = category.FindRootCategory();
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound, "category not found");
            }


            var products = category.GetAllChildProduct().AsProductsValidation();

            int pageSize = Setting.WebSettings[ShopSettingType.ProductPageSize].AsInt();
            pageSize = pageSize > 0 ? pageSize : 36;
            var pageNumber = page ?? 1; // if no page was specified in the querystring, default to the first page (1)
            var onePage = products.ToPagedList(pageNumber, pageSize); // will only contain 25 blog max because of the pageSize


            // breadcrumb 
            ViewBag.Category = category;

            return View(onePage);
        }



        [ChildActionOnly]
        public PartialViewResult CategorySystem(int id)
        {
            var category = CategoriesValidation.FirstOrDefault(x => x.Id == id);
            return PartialView(category);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
