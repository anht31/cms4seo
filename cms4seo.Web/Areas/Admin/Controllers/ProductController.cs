using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.WebPages;
using Admin.Resources;
using cms4seo.Common.Attribute;
using cms4seo.Common.BootstrapHelpers;
using cms4seo.Common.Helpers;
using cms4seo.Common.SettingHelpers;
using cms4seo.Common.Slug;
using cms4seo.Data;
using cms4seo.Model.cms4seoType;
using cms4seo.Model.Entities;
using cms4seo.Model.LekimaxType;
using cms4seo.Service.Photo;
using cms4seo.Service.Product;
using cms4seo.Service.Provider;
using cms4seo.Service.Seo;
using cms4seo.Service.Sitemap;

using CONST = HTMLTOCGenerator.Constants;
using HTML_TOC = HTMLTOCGenerator.HTMLTOCGenerator;
using SHORTEN_PATH = HTMLTOCGenerator.ShortenPath;
using TRY_OPEN = HTMLTOCGenerator.TryOpen;

namespace cms4seo.Admin.Controllers
{
    [CustomAuthorize(Roles = "Basic.Product")]
    public class ProductController : BaseController
    {
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        // ReSharper disable once InconsistentNaming
        private ApplicationDbContext db = new ApplicationDbContext();

        private IQueryable<Product> ProductsValidation =>
            db.Products.Where(x => x.Name != null && x.Name.Trim() != String.Empty);

        private IQueryable<Product> ProductsInvalidation =>
            db.Products.Where(x => x.Name == null || x.Name.Trim() == String.Empty);

        
        private IQueryable<Category> CategoriesValidation => 
            db.Categories.Where(x => !(x.Name == null || x.Name.Trim() == String.Empty));

        private IQueryable<Category> CategoriesInvalidation =>
            db.Categories.Where(x => x.Name == null || x.Name.Trim() == String.Empty);


        public ViewResult Index()
        {
            return View(ProductsValidation.ToList());
        }



        public async Task<ActionResult> Create()
        {
            if (!CategoriesValidation.Any())
            {
                TempData[MessageType.Warning] = AdminResources.ProductControllerCreate_You_not_yet_create_any_Category__please_create_category_first_
                        + $" <a href='{Url.Action("Create", "Category")}' class='btn btn-success'>" +
                        AdminResources.ProductControllerCreate_Click_to_create_category +
                        $"</a>";

                LogHelper.Write(@"Admin/Product", $"Category must be created first, User: {User.Identity.Name}");

                return RedirectToAction("Index");
            }


            var product = ProductsInvalidation.FirstOrDefault(x => x.PostBy == User.Identity.Name);

            if (product == null)
            {
                product = new Product
                {
                    PostBy = User.Identity.Name,
                    IsCreate = true,
                    IsPublish = true,
                    IsHome = true,
                };

                // add product
                db.Products.Add(product);


                if (!string.IsNullOrWhiteSpace(Setting.WebSettings[WebSettingType.OptionProperties]))
                {
                    var propertyNames = Setting.WebSettings[WebSettingType.OptionProperties].Split(',')
                        .Select(t => t.Trim()).ToList();

                    var properties = propertyNames.Select(i => new Property { Name = i, Value = ""}).ToList();
                    var productProperties = properties.Select(i => new ProductProperties { Product = product, Property = i }).ToList();

                    // add property
                    db.ProductProperties.AddRange(productProperties);
                }


                db.SaveChanges();

                
            }

            //ViewBag.CategoryId = new SelectList(await db.Categories.ToListAsync(), "Id", "Name");

            // select with group
            ViewBag.Categories = await CategoriesValidation.ToListAsync();
            ViewBag.SelectedCategory = product.CategoryId;

            return View("Edit", product);
        }



        public async Task<ActionResult> Edit(int id)
        {

            // query-product ==================================================
            var product = await db.Products.FindAsync(id);


            if (product == null)
            {
                LogHelper.Write(@"Admin/Product", 
                    $"User: {User.Identity.Name} not found product with Id: {id}");

                return RedirectToAction("Index");
            }




            #region productTag            

            product.SelectedProductTag = product.ProductTags.Select(x => x.Name).ToList();
            #endregion productTag


            if (!string.IsNullOrWhiteSpace(Setting.WebSettings[WebSettingType.OptionProperties]) && !product.ProductProperties.Any())
            {
                var propertyNames = Setting.WebSettings[WebSettingType.OptionProperties].Split(',')
                    .Select(t => t.Trim()).ToList();

                var properties = propertyNames.Select(i => new Property { Name = i, Value = "" }).ToList();
                var productProperties = properties.Select(i => new ProductProperties { Product = product, Property = i }).ToList();

                // add property
                db.ProductProperties.AddRange(productProperties);
                db.SaveChanges();
            }


            //ViewBag.CategoryId = new SelectList(await db.Categories.ToListAsync(), "Id", "Name", product.CategoryId);

            // select with group
            ViewBag.Categories = await CategoriesValidation.ToListAsync();
            ViewBag.SelectedCategory = product.CategoryId;

            return View(product);
        }


        [HttpPost]
        //[ValidateInput(false)]
        public async Task<ActionResult> Edit(Product product)
        {

            if(String.IsNullOrWhiteSpace(product.Name)) {
                ModelState.AddModelError("Name", AdminResources.YouMustEnterName);
            }


            if (ModelState.IsValid)
            {

                


                //#region slug

                //if (product.IsCreate)
                //{
                    

                //}


                //#endregion slug





                #region add permalink & Slug

                

                var category = db.Categories.Find(product.CategoryId);

                string secondCategoryName = null;

                if (category.GetRootCategory().Id != product.CategoryId)
                    secondCategoryName = category?.Name.MakeNameFriendly();

                var permalinkProvider = new PermalinkProvider();

                product.Slug = permalinkProvider.Set(product.Id, cms4seoEntityType.Product,
                    category.GetRootCategory().Name,
                    secondCategoryName,
                    product.Name,
                    product.IsCreate);





                #endregion add permanlink






                #region productTag       


                #region add tag

                SlugComparer slugComparer = new SlugComparer();

                var tags = db.ProductTags.Select(x => x.Name);

                var listNewTag = product.SelectedProductTag.Except(tags, slugComparer);

                foreach (var newTag in listNewTag)
                {
                    if (String.IsNullOrWhiteSpace(newTag))
                        continue;

                    var productTag = new ProductTag() {Name = newTag, Slug = newTag.MakeUrlFriendly()};

                    db.ProductTags.Add(productTag);
                }

                db.SaveChanges();

                #endregion



                if (!product.IsCreate) // edit mode
                {
                    #region delete relation

                    var productToEdit = db.Products.FirstOrDefault(x => x.Id == product.Id);
                    if (productToEdit != null)
                    {
                        var productTags = productToEdit.ProductTags.ToList();

                        if (productTags.Any())
                        {
                            foreach (var productTag in productTags)
                            {
                                productToEdit.ProductTags.Remove(productTag);
                            }
                            db.SaveChanges();
                        }
                    }

                    db.Entry(productToEdit).State = EntityState.Detached; // fix error already has the same primary key

                    #endregion

                }





                #region add relation

                db.Products.Attach(product);

                product.ProductTags = db.ProductTags.
                    Where(x => product.SelectedProductTag.Contains(x.Name)).ToList();

                if (product.ProductTags.Any())
                {

                    foreach (var productTag in product.ProductTags)
                        db.ProductTags.Attach(productTag);

                }

                #endregion







                #endregion productTag




                #region product ============================================================

                // copy html text Description to Summary
                //product.Summary = product.Description;

                // set avatar

                if (String.IsNullOrEmpty(product.Avatar))
                {
                    var photo = db.Photos.FirstOrDefault(x => 
                        x.Entity == cms4seoEntityType.Product && x.ModelId == product.Id);

                    product.Avatar = photo?.SmPath;
                }
                    

                if (!product.IsLockSeo || Setting.WebSettings[WebSettingType.IsAutoSeoMetaTag].ToBoolean())
                {
                    product.MetaTitle = product.Name;
                    product.MetaDescription = product.Brief.ShortenText320();
                    product.MetaKeyWords = product.MetaDescription.UnsignConvert();
                }
                product.IsLockSeo = true;

                product.UnsignContent = product.Name.UnsignConvert();
                product.UnsignContent += product.Summary.UnsignConvert();
                product.UnsignContent += product.Description.UnsignConvert();
                product.UnsignContent += product.Specification.UnsignConvert();

                if (product.IsCreate)
                {
                    product.IsPublish = true;
                    product.IsHome = true;
                    product.Published = product.Published ?? DateTime.Now;
                    product.Created = DateTime.Now;
                    product.Location = 2;
                    product.Sort = product.Sort != 0 ? product.Sort : product.Id * 10;
                }
                else
                {
                    product.DateAmended = DateTime.Now;
                }

                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();


                LogHelper.Write(@"Admin/Product",
                    product.IsCreate
                        ? $"User: {User.Identity.Name} created Product: '{product.Name}'"
                        : $"User: {User.Identity.Name} updated Product: '{product.Name}'");
                        
                #endregion product



                // add sitemap
                #region add sitemap

                Uri uri = new Uri(Request.Url?.AbsoluteUri ?? "");
                var protocolHostPort = uri.Scheme + Uri.SchemeDelimiter + uri.Host + ":" + uri.Port;

                if (product.IsCreate)
                {
                    var siteMapingProvider = new SiteMappingProvider(Request.Url?.AbsoluteUri);

                    siteMapingProvider.Add(new SiteMapItem()
                    {
                        Loc = new Uri($"{protocolHostPort}{product.Slug}"),
                        ChangeFreq = ChangeFrequency.Daily,
                        LastMod = product.Created,
                        Priority = 0.9
                    });
                }

                #endregion add sitemap






                // update status IsCreate
                product = db.Products.Find(product.Id);
                if (product != null)
                {
                    product.IsCreate = false;
                    db.Entry(product).State = EntityState.Modified;
                    db.SaveChanges();
                }



                #region TOC Generator

                if (Setting.WebSettings[TocSettingType.IsAutoProductTOC].AsBool())
                {
                    product = db.Products.Find(product.Id);

                    HTML_TOC htmlToc = new HTML_TOC();

                    var style = $@"toc-headers:h2,h3,h4,h5,h6;
                    toc-return:{Setting.WebSettings[TocSettingType.IsTocReturn].ToLower()};
                    toc-title:{Setting.WebSettings[TocSettingType.TocTitle]};
                    toc-return-image:/Assets/icon/gototop16.png;
                    toc-image-width:16;
                    toc-image-height:16;
                    toc-header-level:h2;";

                    var tocContainer = "<div class='toc' style='" + style + "'></div>";

                    // check least one element h2 -> h6
                    var regex = new Regex(@"<\/h[2-6]>");
                    var match = regex.Matches(product.Description ?? String.Empty);
                    if (match.Count > 0)
                    {
                        product.ContentWithTOC = htmlToc.add_TOC_to_html(tocContainer + product.Description);

                        db.Entry(product).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    
                }


                #endregion




                TempData[MessageType.Success] = 
                    string.Format(AdminResources.ProductControllerEdit__0___has__been__saved, product?.Name);

                return RedirectToAction("Index");
            }
            // there is something wrong with the data values
            //ViewBag.CategoryId = new SelectList(await db.Categories.ToListAsync(), "Id", "Name", product?.CategoryId);

            // select with group
            ViewBag.Categories = await CategoriesValidation.ToListAsync();
            ViewBag.SelectedCategory = product.CategoryId;

            return View(product);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id)
        {
            var product = await db.Products.FindAsync(id);

            if (product == null)
            {
                TempData[MessageType.Danger] = AdminResources.ProductControllerDelete_Product_not_found;
                return null;
            }




            #region delete permalink

            var permalink = await db.Permalinks.FirstOrDefaultAsync(x =>
                x.EntityName == cms4seoEntityType.Product && x.EntityId == product.Id);

            if(permalink != null)
                db.Permalinks.Remove(permalink);

            #endregion



            try
            {
                var photoService = new PhotoService();
                photoService.DeletePhotoList(product.Photos);
                db.Photos.RemoveRange(product.Photos);

            }
            catch (Exception e)
            {
                TempData[MessageType.Danger] = 
                    string.Format(AdminResources.ProductControllerDelete_Delete_photo_is_fail__Message___0_, e.Message);

                LogHelper.Write(@"Admin/Product",
                    $"Access denied user: {User.Identity.Name} for try delete photo, message: {e.Message}");

                return null;
            }



            db.Products.Remove(product);
            await db.SaveChangesAsync();


            



            // delete sitemap
            #region delete sitemap

            Uri uri = new Uri(Request.Url?.AbsoluteUri ?? "");
            var protocolHostPort = uri.Scheme + Uri.SchemeDelimiter + uri.Host + ":" + uri.Port;
            var siteMappingProvider = new SiteMappingProvider(Request.Url?.AbsoluteUri);
            siteMappingProvider.Delete(new Uri($"{protocolHostPort}/{product.Slug}").ToString());

            #endregion delete sitemap



            TempData[MessageType.Warning] = 
                string.Format(AdminResources.ProductControllerDelete_Product__0__deleted_success__by_User___1_, product.Name, User.Identity.Name);

            LogHelper.Write(@"Admin/Product",
                $"Product {product.Name} deleted by user {User.Identity.Name}");
                
            //return RedirectToAction("Index");
            return null;
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