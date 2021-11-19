using System;
using System.IO;
using System.Text;
using System.Web.Mvc;
using Admin.Resources;
using cms4seo.Common.Attribute;
using cms4seo.Common.BootstrapHelpers;
using cms4seo.Common.Helpers;

namespace cms4seo.Admin.Controllers
{

    [CustomAuthorize(Roles = "Medium.Redirects")]
    public class RedirectsController : BaseController
    {
        // GET: Admin/Redirects
        public ActionResult Index()
        {
            try
            {
                String line;
                // Open the text file using a stream reader.
                using (var fileStream = new FileStream(Server.MapPath("~/App_Data/301.csv"), FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (StreamReader sr = new StreamReader(fileStream, Encoding.Default))
                {
                    // Read the stream to a string, and write the string to the console.
                    line = sr.ReadToEnd();
                }

                return View(model: line);
            }
            catch (Exception e)
            {
                LogHelper.Write(@"Admin/Logs",
                    $"Fail to open 301.csv, {e.Message}");
            }

            return View(model: AdminResources.RedirectsControllerIndex_Fail_to_open_301_csv);
        }


        [HttpPost]

        public ActionResult Index(string content301)
        {
            if (!string.IsNullOrWhiteSpace(content301))
            {
                using (StreamWriter streamWriter = new StreamWriter(Server.MapPath("~/App_Data/301.csv"), false))
                {
                    streamWriter.Write(content301);
                }
            }

            TempData[MessageType.Success] = "config saved!";

            return View();
        }
    }
}