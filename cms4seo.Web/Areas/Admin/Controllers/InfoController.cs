using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Admin.Resources;
using cms4seo.Common.Attribute;
using cms4seo.Common.BootstrapHelpers;
using cms4seo.Common.Helpers;
using cms4seo.Common.SettingHelpers;
using cms4seo.Data;
using cms4seo.Model.Entities;
using cms4seo.Model.LekimaxType;
using cms4seo.Service.Photo;
using cms4seo.Service.Provider;
using cms4seo.Service.Seo;
using cms4seo.Service.Sitemap;

namespace cms4seo.Admin.Controllers
{
    [CustomAuthorize(Roles = "Medium.Info")]
    public class InfoController : BaseController
    {

        private readonly ApplicationDbContext db = new ApplicationDbContext();

        private IQueryable<Info> InfosValidation =>
            db.Infos.Where(x => !(x.Name == null || x.Name.Trim() == String.Empty));

        private IQueryable<Info> InfosInvalidation =>
            db.Infos.Where(x => x.Name == null || x.Name.Trim() == String.Empty);

        // GET: Admin/Info
        public ActionResult Index()
        {            
            return View(InfosValidation.ToList());
        }



        // Create =======================================================================================
        public async Task<ViewResult> Create()
        {

            var info = InfosInvalidation.FirstOrDefault(x => x.PostBy == User.Identity.Name);

            if(info == null)
            {
                info = new Info()
                {
                    PostBy = User.Identity.Name,
                    IsCreate = true,
                };

                db.Infos.Add(info);
                await db.SaveChangesAsync();
            }

            return View("Edit", info);
        }




        // Edit ==========================================================================
        public async Task<ActionResult> Edit(int id)
        {
            var info = await db.Infos.FindAsync(id);

            return View(info);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(Info info)
        {

            if (String.IsNullOrWhiteSpace(info.Name))
            {
                ModelState.AddModelError("Name", AdminResources.InfoControllerEditYouMustEnterName);
            }



            if (ModelState.IsValid)
            {

                #region permalink & slug

                
                var permalinkProvider = new PermalinkProvider();

                info.Slug = permalinkProvider.Set(info.Id, cms4seoEntityType.Info,
                    null,
                    null,
                    info.Name,
                    info.IsCreate);

                


                #endregion permalink & slug


                //#region slug

                //if (info.IsCreate)
                //{
                //    var slug = info.Name.MakeUrlFriendly();
                //    info.Slug = slug;
                //    if (db.Permalinks.Count(x => x.Slug == slug) > 0)
                //    {
                //        var n = 0;
                //        do
                //        {
                //            info.Slug = slug + "-" + ++n;
                //        } while (db.Permalinks.Count(x => x.Slug == info.Slug) > 0);
                //    }

                //}

                //#endregion slug




                #region info


                if (String.IsNullOrEmpty(info.Avatar))
                {
                    var photo = db.Photos.FirstOrDefault(x =>
                        x.Entity == cms4seoEntityType.Info && x.ModelId == info.Id);

                    info.Avatar = photo?.SmPath;
                }


                if (!info.IsLockSeo || Setting.WebSettings[WebSettingType.IsAutoSeoMetaTag].ToBoolean())
                {
                    info.MetaTitle = info.Name;
                    info.MetaDescription = info.Brief.ShortenText320();
                    info.MetaKeyWords = info.MetaDescription.UnsignConvert();
                }
                info.IsLockSeo = true;

                info.UnsignContent = info.Name.UnsignConvert();
                info.UnsignContent += info.Brief.UnsignConvert();
                info.UnsignContent += info.Content.UnsignConvert();


                if (info.IsCreate)
                {
                    info.DateCreated = DateTime.Now;

                }
                else
                    info.DateAmended = DateTime.Now;


                db.Infos.AddOrUpdate(info);
                await db.SaveChangesAsync();

                LogHelper.Write(@"Admin/Infos",
                    info.IsCreate
                        ? $"User: {User.Identity.Name} created Info: '{info.Name}'"
                        : $"User: {User.Identity.Name} updated Info: '{info.Name}'");


                #endregion



                // add sitemap
                #region add sitemap

                Uri uri = new Uri(Request.Url?.AbsoluteUri ?? "");
                var protocolHostPort = uri.Scheme + Uri.SchemeDelimiter + uri.Host + ":" + uri.Port;

                if (info.IsCreate)
                {
                    var siteMapingProvider = new SiteMappingProvider(Request.Url?.AbsoluteUri);

                    siteMapingProvider.Add(new SiteMapItem()
                    {
                        Loc = new Uri($"{protocolHostPort}{info.Slug}"),
                        ChangeFreq = ChangeFrequency.Weekly,
                        LastMod = info.DateCreated,
                        Priority = 0.8
                    });
                }

                #endregion add sitemap





                //#region add permalink 

                //if (info.IsCreate)
                //{
                //    var permalink = new Permalink
                //    {
                //        EntityId = info.Id,
                //        EntityName = LekimaxEntityType.Info,
                //        Slug = info.Slug,
                //        IsActive = true,
                //        LanguageId = 0
                //    };
                //    db.Permalinks.Add(permalink);
                //    db.SaveChanges();
                //}


                //#endregion add permanlink




                // update status IsCreate
                info = db.Infos.Find(info.Id);
                info.IsCreate = false;
                db.Entry(info).State = EntityState.Modified;
                db.SaveChanges();




                TempData[MessageType.Success] = string.Format(AdminResources.InfoControllerEdit__0___has__been__saved, info.Name);
                return RedirectToAction("Index");
            }

            return View(info);
        }



        // Del ==========================================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<RedirectToRouteResult> Delete(int id)
        {
            var info = await db.Infos.FindAsync(id);

            // check exist
            if (info == null)
            {
                TempData[MessageType.Warning] = AdminResources.InfoControllerDelete_Not_found_item_to_delete;
                return null;
            }



            #region delete permalink

            var permalink = await db.Permalinks.FirstOrDefaultAsync(x =>
                x.EntityName == cms4seoEntityType.Info && x.EntityId == info.Id);

            if (permalink != null)
                db.Permalinks.Remove(permalink);

            #endregion



            try
            {
                var photoService = new PhotoService();
                photoService.DeletePhotoList(info.Photos);

                db.Photos.RemoveRange(info.Photos);
            }
            catch (Exception e)
            {
                TempData[MessageType.Danger] = 
                    string.Format(AdminResources.InfoControllerDelete_Error_rising_when_try_delete_photo__Message___0_, e.Message);

                LogHelper.Write(@"Admin/Infos",
                    $"Access denied user: {User.Identity.Name} for try delete photo, message: {e.Message}");

                return null;
            }



            // delete 
            db.Infos.Remove(info);
            await db.SaveChangesAsync();                
            


            // delete site-map
            #region delete sitemap
            Uri uri = new Uri(Request.Url?.AbsoluteUri ?? "");
            var protocolHostPort = uri.Scheme + Uri.SchemeDelimiter + uri.Host + ":" + uri.Port;
            var siteMappingProvider = new SiteMappingProvider(Request.Url?.AbsoluteUri);
            siteMappingProvider.Delete(new Uri($"{protocolHostPort}/{info.Slug}").ToString());
            #endregion delete sitemap


            TempData[MessageType.Warning] = 
                string.Format(AdminResources.InfoControllerDelete__0__was_deleted, info.Name);
            LogHelper.Write(@"Admin/Infos",
                $"User: {User.Identity.Name} deleted Info: '{info.Name}'");

            return null;
        }


        // dispose --------------------------------------------     

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