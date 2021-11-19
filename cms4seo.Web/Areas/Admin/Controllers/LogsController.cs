using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.Mvc;
using Admin.Resources;
using cms4seo.Common.Attribute;
using cms4seo.Common.Helpers;

namespace cms4seo.Admin.Controllers
{
    [CustomAuthorize(Roles = "Medium.Logs")]
    public class LogsController : BaseController
    {
        // GET: Admin/Logs
        public ActionResult Index()
        {
            try
            {
                //String line;
                //// Open the text file using a stream reader.
                //using (var fileStream = new FileStream(Server.MapPath("~/Events.log"), FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                //using (StreamReader sr = new StreamReader(fileStream, Encoding.Default))
                //{
                //    // Read the stream to a string, and write the string to the console.
                //    line = sr.ReadToEnd();                                       
                //}

                //return View(model: line);

                // solution 3
                List<string> lines = new List<string>();

                using (var fileStream = new FileStream(Server.MapPath("~/Events.log"), FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (StreamReader sr = new StreamReader(fileStream, Encoding.Default))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        lines.Add(line);
                    }
                }

                lines.Reverse();

                return View(lines);

            }
            catch (Exception e)
            {
                LogHelper.Write(@"Admin/Logs",
                    $"Fail to open Events.log, {e.Message}");
            }

            return View(new List<string>() { AdminResources.LogsControllerIndex_Fail_to_open_Events_log, "\n" });
        }
    }
}