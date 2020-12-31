using System.Collections.Generic;
using System.Deployment.Application;
using System.Reflection;
using System.Web.Configuration;
using System.Web.Mvc;
using cms4seo.Common.Attribute;

namespace cms4seo.Admin.Controllers
{
    [CustomAuthorize(Roles = "Basic.SystemInfo")]
    public class SystemInfoController : BaseController
    {
        //// binding-Config ============================================
        //kernel.Bind<IConfigRepository>().To<ConfigRepository>().InRequestScope();

        //// field --------------------------------------------
        //private IConfigRepository repository;    

        // ctor --------------------------------------------

        // GET: Admin/Config
        public ActionResult Index()
        {
            var configData = new Dictionary<string, string>();


            configData.Add("PowerBy", "cms4seo.com");
            configData.Add("Version", CurrentVersion);


            // compilation section ==================================================================
            var sysWeb =
                (SystemWebSectionGroup) WebConfigurationManager.OpenWebConfiguration("/")
                    .GetSectionGroup("system.web");


            configData.Add("debug", sysWeb?.Compilation.Debug.ToString());
            configData.Add("targetFramework", sysWeb?.Compilation.TargetFramework);


            // Overriding-location ==============================================================
            foreach (var key in WebConfigurationManager.AppSettings.AllKeys)
            {
                configData.Add(key, WebConfigurationManager.AppSettings[key]);
            }

            return View(configData);
        }





        // Overriding-location ==============================================================
        public ActionResult OtherAction()
        {
            var configData = new Dictionary<string, string>();
            foreach (var key in WebConfigurationManager.AppSettings.AllKeys)
            {
                configData.Add(key, WebConfigurationManager.AppSettings[key]);
            }
            return View("Index", configData);
        }


      


        // Del ==========================================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int deleteId)
        {
            //Config config = repository.GetById(deleteId);
            //if (config != null)
            //{
            //    repository.Delete(config);
            //    repository.Save();
            //    TempData[MessageType.Warning] = string.Format("{0} was deleted", config.Name);
            //}
            return RedirectToAction("Index");
        }



        public static string CurrentVersion
        {
            get
            {
                return ApplicationDeployment.IsNetworkDeployed
                    ? ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString()
                    : Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }


        // dispose --------------------------------------------
        protected override void Dispose(bool disposing)
        {
            //repository dispose by "InRequestScope()" Ninject
            base.Dispose(disposing);
        }
    }
}