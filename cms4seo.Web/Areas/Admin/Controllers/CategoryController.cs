using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Admin.Resources;
using cms4seo.Admin.Services;
using cms4seo.Common.Attribute;
using cms4seo.Common.BootstrapHelpers;
using cms4seo.Common.Helpers;
using cms4seo.Common.SettingHelpers;
using cms4seo.Data;
using cms4seo.Data.IdentityModels;
using cms4seo.Model.Entities;
using cms4seo.Model.LekimaxType;
using cms4seo.Service.Photo;
using cms4seo.Service.Product;
using cms4seo.Service.Provider;
using cms4seo.Service.Seo;
using cms4seo.Service.Sitemap;
using Microsoft.AspNet.SignalR.Hubs;

namespace cms4seo.Admin.Controllers
{
    [CustomAuthorize(Roles = "Basic.Category")]
    public class CategoryController : BaseController
    {

        private readonly ApplicationDbContext db = new ApplicationDbContext();



        private IQueryable<Category> CategoriesValidation =>
            db.Categories.Where(x => !(x.Name == null || x.Name.Trim() == String.Empty));

        private IQueryable<Category> CategoriesInvalidation =>
            db.Categories.Where(x => x.Name == null || x.Name.Trim() == String.Empty);


        // ActionResult ===============================================

        // GET: Admin/Category        
        public async Task<ActionResult> Index()
        {
            return View(await CategoriesValidation.ToListAsync());
        }



        // Get: Admin/Category/Create
        [HttpGet]
        public async Task<ActionResult> Create()
        {

            var category = CategoriesInvalidation.FirstOrDefault(x => x.PostBy == User.Identity.Name);


            if (category == null)
            {
                category = new Category
                {
                    PostBy = User.Identity.Name,
                    IsCreate = true
                };
                db.Categories.Add(category);
                db.SaveChanges();
            }


            // legacy selector
            //ViewBag.ParentId = new SelectList(await db.Categories.ToListAsync(), "Id", "Name");

            // group selector
            ViewBag.Categories = await CategoriesValidation.Where(x => 
                x.Id != category.Id).ToListAsync();

            ViewBag.SelectedCategory = category.ParentId ?? 0;

            return View("Edit", category);
        }



        // Get: Admin/Category/Edit
        [HttpGet]
        public async Task<ActionResult> Edit(int id)
        {

            // query ==================================================
            var category = await db.Categories.FindAsync(id);
            if (category == null)
            {
                return HttpNotFound();
            }



            //ViewBag.Locations = category.LocationRule;

            // legacy selecter
            //ViewBag.ParentId = new SelectList(await CategoriesValidation.ToListAsync(), "Id", "Name", category.ParentId);

            // group selecter
            ViewBag.Categories = await CategoriesValidation.Where(x => 
                x.Id != category.Id).ToListAsync();
            ViewBag.SelectedCategory = category.ParentId ?? 0;

            return View(category);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(Category category)
        {

            if (String.IsNullOrWhiteSpace(category.Name))
            {
                ModelState.AddModelError("Name", AdminResources.CategoryControllerEditYouMustEnterName);
            }

            var parentCategory = db.Categories.Find(category.ParentId);

            if (parentCategory.HowDeepCategorySiteArchitecture() >= 3)
            {
                ModelState.AddModelError("ParentId", AdminResources.Admin_CategoryController_Edit_YouNotAllowSelectSecondLevelCategory);
                TempData[MessageType.Warning] =
                    AdminResources.Admin_CategoryController_Edit_YouNotAllowSelectSecondLevelCategory;
            }


            if (ModelState.IsValid)
            {



                #region permalink & slug


                var rootCategory = parentCategory.GetRootCategory();

                string secondCategoryName = null;

                if (rootCategory?.Id != parentCategory?.Id)
                    secondCategoryName = parentCategory?.Name.MakeNameFriendly();

                var permalinkProvider = new PermalinkProvider();

                category.Slug = permalinkProvider.Set(category.Id, cms4seoEntityType.Category,
                    rootCategory?.Name,secondCategoryName, category.Name,
                    category.IsCreate);


                //if (category.IsCreate)
                //{
                //    var slug = category.Name.MakeUrlFriendly();
                //    category.Slug = slug;
                //    if (db.Permalinks.Count(x => x.Slug == slug) > 0)
                //    {
                //        var n = 0;
                //        do
                //        {
                //            category.Slug = slug + "-" + ++n;
                //        } while (db.Permalinks.Count(x => x.Slug == category.Slug) > 0);
                //    }

                //}

                #endregion slug




                // save ===============================================================
                //category.PhotosDelimiter = string.Join("|",
                //    photoSession.All.Where(x => !x.IsDelete).Select(p => p.Name.ToString()));

                // set link
                if (string.IsNullOrEmpty(category.Href))
                    category.Href = "#";

                if (!category.IsLockSeo || Setting.WebSettings[WebSettingType.IsAutoSeoMetaTag].ToBoolean())
                {
                    category.MetaTitle = category.Name;
                    category.MetaDescription = category.Description.ShortenText320();
                    category.MetaKeyWords = category.MetaDescription.ReplaceDiacritics();
                }
                category.IsLockSeo = true;

                category.UnsignContent = category.Name.ToLower().ReplaceDiacritics();
                category.UnsignContent += category.Description.UnsignConvert();

                if (category.IsCreate)
                {
                    category.DateCreated = DateTime.Now;
                    category.Sort = category.Sort != 0 ? category.Sort : -category.Id * 10;
                }
                else
                    category.DateAmended = DateTime.Now;


                db.Entry(category).State = EntityState.Modified;
                db.SaveChanges();

                LogHelper.Write(@"Admin/Category",
                    category.IsCreate 
                        ? $"User: {User.Identity.Name} created category: {category.Name}" 
                        : $"User: {User.Identity.Name} updated category: {category.Name}");



                // add sitemap 
                #region add sitemap

                if (category.IsCreate)
                {
                    Uri uri = new Uri(Request.Url?.AbsoluteUri ?? "");
                    var protocolHostPort = uri.Scheme + Uri.SchemeDelimiter + uri.Host + ":" + uri.Port;

                    var siteMappingProvider = new SiteMappingProvider(Request.Url?.AbsoluteUri);

                    siteMappingProvider.Add(new SiteMapItem()
                    {
                        Loc = new Uri($"{protocolHostPort}{category.Slug}"),
                        ChangeFreq = ChangeFrequency.Weekly,
                        LastMod = category.DateCreated,
                        Priority = 0.9
                    });
                }

                #endregion add sitemap



                //#region add permalink 

                //if(category.IsCreate)
                //{
                //    var permalink = new Permalink
                //    {
                //        EntityId = category.Id,
                //        EntityName = LekimaxEntityType.Category,
                //        Slug = category.Slug,
                //        IsActive = true,
                //        LanguageId = 0
                //    };
                //    db.Permalinks.Add(permalink);
                //    db.SaveChanges();
                //}

                //#endregion add permanlink




                // update status IsCreate
                category = db.Categories.Find(category.Id);
                if (category != null)
                {
                    category.IsCreate = false;
                    db.Entry(category).State = EntityState.Modified;
                    db.SaveChanges();

                    TempData[MessageType.Success] = string.Format(AdminResources.CategoryControllerEdit__0___has__been__saved, category.Name);
                }


                return RedirectToAction("Index");
            }
            //ViewBag.Locations = category.LocationRule;

            // legacy selecter
            //ViewBag.ParentId = new SelectList(await db.Categories.ToListAsync(), "Id", "Name", category.ParentId);

            // group selecter
            ViewBag.Categories = await CategoriesValidation.Where(x => 
                x.Id != category.Id).ToListAsync();

            ViewBag.SelectedCategory = category.ParentId ?? 0;

            return View(category);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<RedirectToRouteResult> Delete(int deleteId)
        {
            var category = await db.Categories.FindAsync(deleteId);

            if (category == null)
            {
                TempData[MessageType.Warning] = AdminResources.CategoryControllerDelete_Category_wasn_t_deleted;

                LogHelper.Write(@"Admin/Category",
                    $"Access denied user: {User.Identity.Name} for attempt access delete topic Id: {deleteId}, category not found");

                //return RedirectToAction("Index");
                return null;
            }


            if (category.Children.Count > 0)
            {
                TempData[MessageType.Warning] =
                    string.Format(AdminResources.CategoryControllerDelete_Category_have____0___child_category__please_delete_child_category_first, category.Children.Count);

                LogHelper.Write(@"Admin/Category",
                    $"Access denied user: {User.Identity.Name} for attempt access delete topic: {deleteId}, category has child");

                return null;
            }


            if (category.Products != null && category.Products.Count > 0)
            {
                TempData[MessageType.Warning] =
                    string.Format(AdminResources.CategoryControllerDelete_Category_have____0___product__please_delete_product_first, category.Products.Count);

                LogHelper.Write(@"Admin/Category",
                    $"Access denied user: {User.Identity.Name} for attempt access delete topic Id: {deleteId}, category has products");

                return null;
            }





            #region delete permalink

            var permalink = await db.Permalinks.FirstOrDefaultAsync(x =>
                x.EntityName == cms4seoEntityType.Category && x.EntityId == category.Id);

            if (permalink != null)
                db.Permalinks.Remove(permalink);

            #endregion


            try
            {
                var photoService = new PhotoService();
                photoService.DeletePhotoList(category.Photos);
                db.Photos.RemoveRange(category.Photos);

                TempData[MessageType.Warning] =
                    string.Format(AdminResources.CategoryControllerDelete__0__was_deleted, category.Name);
            }
            catch (Exception e)
            {
                TempData[MessageType.Danger] =
                    string.Format(AdminResources.CategoryControllerDelete_Error_rising_when_try_delete_photo__Message___0_, e.Message);

            }


            db.Categories.Remove(category);
            await db.SaveChangesAsync();
            LogHelper.Write(@"Admin/Category",
                $"User: {User.Identity.Name} deleted topic: {category.Name}");


            // delete site-map
            #region delete sitemap
            Uri uri = new Uri(Request.Url?.AbsoluteUri ?? "");
            var protocolHostPort = uri.Scheme + Uri.SchemeDelimiter + uri.Host + ":" + uri.Port;
            var siteMapingProvider = new SiteMappingProvider(Request.Url?.AbsoluteUri);
            siteMapingProvider.Delete(new Uri($"{protocolHostPort}/{category.Slug}").ToString());

            #endregion delete sitemap




            return null;

            //return RedirectToAction("Index");
        }
    }
}