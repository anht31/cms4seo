using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.WebPages;
using cms4seo.Common.Helpers;
using cms4seo.Data;
using cms4seo.Model.Entities;
using cms4seo.Model.LekimaxType;
using cms4seo.Service;
using cms4seo.Service.Product;
using cms4seo.Service.Provider;
using PagedList;

namespace cms4seo.Web.Controllers
{
    public class ProductController : Controller
    {

        private readonly ApplicationDbContext db = new ApplicationDbContext();

        // zero time 0:00:00
        public DateTime Tomorrow => DateTime.Today.AddDays(1);

        private IQueryable<Product> ProductsValidation =>
            db.Products.Where(x =>
                !(x.Name == null || x.Name.Trim() == String.Empty)
                & x.Published < Tomorrow
                & x.IsPublish);

        private IQueryable<Category> CategoriesValidation =>
            db.Categories.Where(x => 
                !(x.Name == null || x.Name.Trim() == String.Empty));





        #region new route


        [ChildActionOnly]
        public PartialViewResult ProductSystem()
        {
            return PartialView(CategoriesValidation.Where(x => x.IsHome).OrderByDescending(x => x.Sort).ToList());
        }


        public ActionResult Details(int id)
        {

            var product = ProductsValidation.FirstOrDefault(x => x.Id == id);
            if (product == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound, "product not found");
            }

            if (Setting.WebSettings[WebSettingType.IsProductViewCounter].AsBool())
            {
                product.ViewCounter++;
                db.SaveChanges();
            }
            

            //ViewBag.Locations = GetLocationRule();

            // View by -> LayoutProduct -> AsideMenu
            ViewBag.CategoryId = product.Category.FindRootCategory();


            return View(product);
        }



        public ActionResult Search(string productSearch, int? page)
        {
            ViewBag.CategoryId = 0;
            productSearch = productSearch.ToLower().ReplaceDiacritics();

            var products = ProductsValidation.Where(x => x.UnsignContent.Contains(productSearch)).ToList();


            var pageNumber = page ?? 1; // if no page was specified in the querystring, default to the first page (1)
            var onePage = products.ToPagedList(pageNumber, 50); // will only contain 25 blog max because of the pageSize

            ViewBag.strSearch = productSearch;

            return View(onePage);
        }




        public ActionResult Filter(string district, string maxPrice, int? page)
        {
            ViewBag.CategoryId = 0;

            int price;
            int.TryParse(maxPrice, NumberStyles.Any, CultureInfo.CurrentCulture, out price);

            var products = new List<Product>();

            if (!String.IsNullOrWhiteSpace(district) && !String.IsNullOrWhiteSpace(maxPrice))
            {
                products = ProductsValidation.Where(x => x.District == district && x.Price < price).ToList();
            }
            else if (!String.IsNullOrWhiteSpace(district))
            {
                products = ProductsValidation.Where(x => x.District == district).ToList();
            }
            else if (!String.IsNullOrWhiteSpace(maxPrice))
            {
                products = ProductsValidation.Where(x => x.Price < price).ToList();
            }


            var pageNumber = page ?? 1; // if no page was specified in the querystring, default to the first page (1)
            var onePage = products.ToPagedList(pageNumber, 50); // will only contain 25 blog max because of the pageSize

            ViewBag.strSearch = $"{district}, Max Price {maxPrice}";

            return View(onePage);
        }



        public ActionResult FilterPrice(long priceFrom, long priceTo, int? page)
        {
            var products = ProductsValidation.Where(x => priceFrom < x.Price && x.Price <= priceTo).ToList();

            var pageNumber = page ?? 1; // if no page was specified in the querystring, default to the first page (1)
            var onePage = products.ToPagedList(pageNumber, 20); // will only contain 20 blog max because of the pageSize


            ViewBag.strSearch = $"{priceFrom} -> {priceTo}";

            ViewBag.PriceFrom = priceFrom;

            return View(onePage);
        }





        [ChildActionOnly]
        public ActionResult RelateProductByTag(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "id is null");
            }
            
            var product = ProductsValidation.FirstOrDefault(x => x.Id == id);

            if (product == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound, "product not found");
            }

            List<Product> relateProducts = product.ProductTags.SelectMany(x => x.Products).Distinct().ToList();

            relateProducts.Remove(product);

            return PartialView(relateProducts.Where(x =>
                !(x.Name == null || x.Name.Trim() == String.Empty)
                & x.Published < Tomorrow
                & x.IsPublish));
        }



        [ChildActionOnly]
        public ActionResult RelateProduct(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "id is null");
            }

            var product = ProductsValidation.FirstOrDefault(x => x.Id == id);

            if (product == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound, "product not found");
            }

            var relateProducts = ProductsValidation.Where(x => x.CategoryId == product.CategoryId).ToList();

            relateProducts.Remove(product);

            return PartialView(relateProducts.Where(x =>
                !(x.Name == null || x.Name.Trim() == String.Empty)
                & x.Published < Tomorrow
                & x.IsPublish));
        }





        [ChildActionOnly]
        public PartialViewResult NewestProducts(int size = 6)
        {
            ViewBag.Size = size;
            return PartialView(ProductsValidation.OrderByDescending(x => x.Sort).Take(size).ToList());
        }


        #endregion







        #region lazy loading

        public JsonResult LoadMore(int size, int count)
        {

            try
            {
                var model = ProductsValidation.OrderBy(x => x.Published).Skip(count).Take(size);
                int modelCount = ProductsValidation.Count();

                if (model.Any())
                {
                    string modelString = RenderRazorViewToString("NewestProducts", model);

                    return Json(new { ModelString = modelString, ModelCount = modelCount }, JsonRequestBehavior.AllowGet);
                }

                return Json(model, JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                return Json(new { success = false, ex = e.Message }, JsonRequestBehavior.AllowGet);
            }

        }

        #endregion









        #region extension

        private string RenderRazorViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext =
                    new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }

        #endregion
    }
}