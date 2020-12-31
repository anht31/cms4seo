using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using cms4seo.Service.Provider;

namespace cms4seo.Web.Controllers
{
    public class DownloadController : Controller
    {
        // GET: Download
        public ActionResult Doc(string name)
        {
            var folder = "/Uploads/config/Documents/";
            var path = folder + name;
            if (!System.IO.File.Exists(Server.MapPath(path)))
                return new HttpStatusCodeResult(HttpStatusCode.NotFound, "doc file not found");
            //return new HttpStatusCodeResult(HttpStatusCode.NotFound);

            var downloadCount =  Setting.WebSettings["Statistic.DownloadFull"];
            if (string.IsNullOrWhiteSpace(downloadCount))
            {
                Setting.WebSettings.Set("Statistic.DownloadFull", 1.ToString());
            }
            else
            {
                var count = downloadCount.AsInt();
                Setting.WebSettings.Set("Statistic.DownloadFull", (++count).ToString());
            }

            return File(path, "application/force-download", name);
        }



        // GET: Download
        public ActionResult Pack(string name, string pack)
        {
            var folder = "/Uploads/config/Documents/";
            var path = folder + name;
            if (!System.IO.File.Exists(Server.MapPath(path)))
                return new HttpStatusCodeResult(HttpStatusCode.NotFound, "Pack file not found");
            //return new HttpStatusCodeResult(HttpStatusCode.NotFound);


            if (!string.IsNullOrWhiteSpace(pack))
            {
                var downloadCount = Setting.WebSettings[pack];
                if (string.IsNullOrWhiteSpace(downloadCount))
                {
                    Setting.WebSettings.Set(pack, 1.ToString());
                }
                else
                {
                    var count = downloadCount.AsInt();
                    Setting.WebSettings.Set(pack, (++count).ToString());
                }
            }
            

            return File(path, "application/force-download", name);
        }
    }
}