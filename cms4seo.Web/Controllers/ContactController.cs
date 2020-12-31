using System;
using System.Web.Mvc;
using cms4seo.Data;
using cms4seo.Model.Entities;
using cms4seo.Service.Resolver;
using cms4seo.Data.IdentityModels;
using cms4seo.Service.Email;
using cms4seo.Service.Provider;

namespace cms4seo.Web.Controllers
{
    public class ContactController : Controller
    {

        private readonly ApplicationDbContext db = new ApplicationDbContext();

 
        private readonly IOrderProcessor mailProcessor;


        public ContactController()
        {
            mailProcessor = new EmailProcessor();            
        }
        


        [HttpPost]
        public JsonResult ContactForm(Contact contact)
        {
            if (ModelState.IsValid)
            {
                //contact.Slug = contact.Name.MakeUrlFriendly();
                contact.PostDate = DateTime.Now;

                db.Contacts.Add(contact);
                db.SaveChanges();

                // send mail
                var message = mailProcessor.ProcessContact(contact.FullName, contact.Email, contact.Phone, contact.Message);

                if(message != null)
                    return Json(new { success = false, responseText = message },
                        JsonRequestBehavior.AllowGet);

                //  Success
                return Json(new {success = true}, JsonRequestBehavior.AllowGet);
            }

            //  Error
            return Json(new {success = false,
                    responseText = Setting.Contents["Emails.ContactForm.ServerBusyPleaseTryLater"]
                }, JsonRequestBehavior.AllowGet);
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