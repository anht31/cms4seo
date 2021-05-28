using System;
using System.Linq;
using System.Reflection;
using System.Web.Compilation;
using System.Web.Mvc;
using System.Web.WebPages;
using cms4seo.Common.Helpers;
using cms4seo.Data;
using cms4seo.Model.Entities;
using cms4seo.Service.Resolver;
using cms4seo.Service.Behaviour;
using cms4seo.Service.Email;
using cms4seo.Service.Provider;
using cms4seo.Model.Abstract;
using cms4seo.Model.LekimaxType;

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
                
                contact.PostDate = DateTime.Now;
                bool isInvalidIp = false;

                try
                {
                    if (PluginHelpers.Widgets.Any())
                    {
                        var widget = PluginHelpers.Widgets.FirstOrDefault(x => x.Zone == "mail-filter");

                        if (widget != null && widget.Active.AsBool())
                        {
                            IChecker checker = (IChecker)Activator
                                .CreateInstance(widget.AssemblyName, widget.TypeName)
                                .Unwrap();

                            isInvalidIp = checker.IsInvalidIp(Request.ServerVariables["REMOTE_ADDR"]);
                        }
                    }
                }
                catch (Exception e)
                {
                    LogHelper.Write("ContactController.ContactForm().Widget", $"Filter fail, Message: {e.Message}");
                }
                


                bool isValidPhone = contact.Phone.ValidatePhoneNumber(true);

                // send mail & save message
                if (!isInvalidIp && isValidPhone)
                {
                    var message = "";
                    try
                    {
                        // send mail
                        message = mailProcessor.ProcessContact(contact.FullName, contact.Email, contact.Phone, contact.Message);
                    }
                    catch (Exception exception)
                    {
                        contact.Message += $"\nException: {exception.Message}";

                        // save even SaveMode not allow
                        if (Setting.WebSettings[EmailSettingType.SaveMode] != "0")
                        {
                            // save when not spam
                            db.Contacts.Add(contact);
                            db.SaveChanges();
                        }
                    }

                    if (Setting.WebSettings[EmailSettingType.SaveMode] == "0")
                    {
                        // save when not spam
                        db.Contacts.Add(contact);
                        db.SaveChanges();
                    }
                    

                    if (message != null)
                        return Json(new { success = false, responseText = message },
                            JsonRequestBehavior.AllowGet);
                }
                else
                {
                    // spam mode
                    if (Setting.WebSettings[EmailSettingType.SaveMode] == "1")
                    {
                        // just save spam
                        db.Contacts.Add(contact);
                        db.SaveChanges();
                    }
                }


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