using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Admin.Resources;
using cms4seo.Common.Attribute;
using cms4seo.Common.BootstrapHelpers;
using cms4seo.Common.Helpers;
using cms4seo.Data;
using cms4seo.Data.IdentityModels;

namespace cms4seo.Admin.Controllers
{
    [CustomAuthorize(Roles = "Medium.InjectionHyperlinks")]
    public class InjectionHyperlinksController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Admin/InjectionHyperlinks
        public ActionResult Index()
        {
            return View(db.Articles.ToList());
        }

        public ActionResult Inject(int id)
        {

            Uri uri = Request.Url;
            string urlHomePage = uri.Scheme + Uri.SchemeDelimiter + uri.Host;

            var artilce = db.Articles.Find(id);

            if (artilce == null)
            {
                TempData[MessageType.Warning] = 
                    string.Format(AdminResources.InjectionHyperlinksControllerInject_Id___0__not_found, id);
                return RedirectToAction("Index");
            }

            artilce.Content = artilce.Content.InjectHyperlink("keyword need inject link", $"{urlHomePage}");
            db.Entry(artilce).State = EntityState.Modified;
            db.SaveChanges();

            TempData[MessageType.Warning] = 
                string.Format(AdminResources.InjectionHyperlinksControllerInject_Artilce___0__has_been_inject_hyperlink_success_, artilce.Name);
            return RedirectToAction("Index");
        }

        public ActionResult ClearAllLink(int id)
        {
            var artilce = db.Articles.Find(id);

            if (artilce == null)
            {
                TempData[MessageType.Warning] = 
                    string.Format(AdminResources.InjectionHyperlinksControllerClearAllLink_Id___0__not_found, id);
                return RedirectToAction("Index");
            }

            artilce.Content = artilce.Content.StripHyperlinks();
            db.Entry(artilce).State = EntityState.Modified;
            db.SaveChanges();


            return RedirectToAction("Index");
        }



        public ActionResult ClearAllLinkAllArticle(string id)
        {
            if (id == null || id != DateTime.Now.ToString("yyyyMMdd"))
            {
                TempData[MessageType.Warning] = 
                    AdminResources.InjectionHyperlinksControllerClearAllLinkAllArticle__Unauthor___Verity_code_not_valid_;

                return RedirectToAction("Index");
            }


            var articles = db.Articles.ToList();

            foreach (var article in articles)
            {
                try
                {
                    article.Content = article.Content.StripHyperlinks();
                    db.Entry(article).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (Exception exception)
                {
                    TempData[MessageType.Warning] = 
                        string.Format(AdminResources.InjectionHyperlinksControllerClearAllLinkAllArticle__Error____0____1__, article.Name, exception.Message) +
                        $"</br>";
                }
                
            }
           

            TempData[MessageType.Warning] = 
                string.Format(AdminResources.InjectionHyperlinksControllerClearAllLinkAllArticle_Deleted_done__total___0__articles, articles.Count);
            return RedirectToAction("Index");
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