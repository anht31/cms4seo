using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Admin.Resources;
using cms4seo.Common.Attribute;
using cms4seo.Common.BootstrapHelpers;
using cms4seo.Data;
using cms4seo.Data.IdentityModels;
using cms4seo.Model.Entities;
using Microsoft.AspNet.Identity;

namespace cms4seo.Admin.Controllers
{
    [CustomAuthorize(Roles = "Basic.Contact")]
    public class ContactController : BaseController
    {
        readonly ApplicationDbContext db = new ApplicationDbContext();


        // GET: Admin/Contact
        public ActionResult Index()
        {
            IEnumerable<Contact> contacts = db.Contacts.ToList();
            return View(contacts);
        }

        public ActionResult MarkAsRead(int id)
        {
            var contact = db.Contacts.Find(id);

            if (contact != null)
            {
                contact.ReadBy = User.Identity.GetUserName();
                db.Entry(contact).State = EntityState.Modified;
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var contact = db.Contacts.Find(id);
            if (contact != null)
            {
                db.Contacts.Remove(contact);
                db.SaveChanges();
                TempData[MessageType.Warning] = 
                    string.Format(AdminResources.ContactControllerDelete__0__was_deleted, contact.FullName);
            }

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