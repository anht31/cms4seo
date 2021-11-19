using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Management;
using System.Web.Mvc;
using System.Web.WebPages;
using System.Xml.Linq;
using Admin.Resources;
using cms4seo.Common.Attribute;
using cms4seo.Common.BootstrapHelpers;
using cms4seo.Data;
using cms4seo.Data.Repositories;
using cms4seo.Model.Entities;
using cms4seo.Model.LekimaxType;
using cms4seo.Model.ViewModel;
using cms4seo.Service.Content;
using cms4seo.Service.Provider;

namespace cms4seo.Admin.Controllers
{
    [CustomAuthorize(Roles = "Medium.Contents")]
    public class ContentsController : BaseController
    {

        ApplicationDbContext db = new ApplicationDbContext();


        // GET: Admin/Contents
        public ActionResult Index()
        {

            return View(db.Contents.ToList());
        }

        public ViewResult Create()
        {
            return View("Edit", new Content());
        }

        
        public ActionResult Edit(string key, string theme, string language, bool? isEdit)
        {

            if (key == null)
            {
                throw new KeyNotFoundException("key is null");
            }


            ViewBag.IsEdit = isEdit;

            var content = db.Contents.Find(key, theme, language);

            if (content == null)
            {
                TempData[MessageType.Warning] = 
                    string.Format(AdminResources.CommonItemNotFound, key);

                return RedirectToAction("Index");
            }

            return View(content);

        }

        [HttpPost]
        public ActionResult Edit(Content content, bool? isEdit)
        {
            if (ModelState.IsValid)
            {

                if (isEdit ?? false)
                {
                    //Setting.Contents.Edit(content.Key, content.Value);
                    db.Entry(content).State = EntityState.Modified;
                    

                    TempData[MessageType.Warning] = 
                        string.Format(AdminResources.CommonItemEditMessage, content.Key);
                }
                else
                {

                    var entry = db.Contents.Find(content.Key, content.Theme, content.Language);

                    if (entry != null)
                    {
                        TempData[MessageType.Danger] = $"Duplicate";
                    }
                    else
                    {
                        db.Contents.Add(content);

                        TempData[MessageType.Warning] =
                            string.Format(AdminResources.CommonItemCreated, content.Key);
                    }
                }

                db.SaveChanges();

                
                return RedirectToAction("Index");
            }


            // there is something wrong with the data values   
            ViewBag.IsEdit = isEdit;

            return View(content);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string deleteId, string theme, string language)
        {

            if (!string.IsNullOrEmpty(deleteId))
            {

                var content = db.Contents.Find(deleteId, theme, language);

                if (content == null)
                    return RedirectToAction("Index");
                
                var key = db.Contents.Remove(content);
                db.SaveChanges();

                TempData[MessageType.Warning] = 
                    string.Format(AdminResources.CommonItemDeleted, $"{key}-{theme}-{language}");
            }
            return RedirectToAction("Index");
        }


        public ActionResult Admin_Restart(string reason)
        {
            HttpRuntime.UnloadAppDomain();
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


        public void ExportXml()
        {
            #region source

            var contents = db.Contents.OrderBy(x => x.Key).ToList();


            #endregion



            #region mapping

            XDocument doc = new XDocument();
            var root = new XElement("AppSettings");
            doc.Add(root);

            foreach (var content in contents)
            {
                var element = new XElement("add");
                element.SetAttributeValue("key", content.Key);
                element.SetAttributeValue("value", content.Value);
                root.Add(element);
            }


            //var today = DateTime.Today;
            //var stringDate = today.ToString("mm-dd-yy");
            //var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
            //    $@"App_Data\export-{stringDate}.xml");
            //doc.Save(path);

            #endregion




            #region destination

            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition",
                "attachment;filename=export.xml");
            Response.ContentType = "text/xml";

            doc.Save(Response.OutputStream);

            #endregion



            TempData[MessageType.Success] = "Export done!";
        }




        public void ExportCurrentLanguage()
        {
            #region source


            var theme = Setting.Theme;
            var language = Setting.Language;

            var contents = db.Contents
                .Where(x => x.Theme == theme && x.Language == language)
                .OrderBy(x => x.Key).ToList();


            #endregion



            #region mapping

            XDocument doc = new XDocument();
            var root = new XElement("AppSettings");
            doc.Add(root);

            foreach (var content in contents)
            {
                var element = new XElement("add");
                element.SetAttributeValue("key", content.Key);
                element.SetAttributeValue("value", content.Value);
                root.Add(element);
            }


            #endregion




            #region destination

            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader($"content-disposition",
                $"attachment;filename={language}.xml");
            Response.ContentType = "text/xml";

            doc.Save(Response.OutputStream);

            #endregion



            TempData[MessageType.Success] = $"Export done for theme: {theme}";
        }


        public ActionResult ClearContents()
        {
            db.Database.ExecuteSqlCommand("DELETE FROM Contents");

            var i = 30;
            while (i-- > 0 && db.Contents.Any())
            {
                Thread.Sleep(100);
            }

            // reload bundle
            DynamicBundles.Clear();

            return RedirectToAction("Index");
        }


        public ActionResult RestartWebsite(string reason)
        {
            HttpRuntime.UnloadAppDomain();
            return RedirectToAction("Index");
        }
    }
}