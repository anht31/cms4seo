using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Http.Description;
using cms4seo.Common.Helpers;
using cms4seo.Data.Repositories;
using cms4seo.Model.LekimaxType;
using cms4seo.Service.Provider;
using cms4seo.Service.Content;

namespace cms4seo.Web.Controllers
{
    public class ContentVm
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    //[Authorize(Roles = "Medium.Contents")]
    public class ContentsController : ApiController
    {

        //private static string _host;


        // GET: api/Contents/5
        [ResponseType(typeof(ContentVm))]
        public IHttpActionResult Get(string id)
        {
            if(String.IsNullOrWhiteSpace(id))
                return Json("null");

            // friendly json return
            if (!User.IsInRole("Medium.Contents"))
                return Json("null");


            string[] idArray = id.Split(',')
                .Select(t => t.Trim()).ToArray();

            var keys = new List<String>();

            foreach (var item in idArray)
                keys.Add(CMS.Decode(item));


            var contentVms = new List<ContentVm>();

            // version for share AppShop
            foreach (var key in keys)
            {
                if(string.IsNullOrWhiteSpace(key))
                    continue;

                var contentVm = new ContentVm
                {
                    Key = key,
                    Value = key.ToLower().Contains("common.shop.")
                        ? Setting.WebSettings[key]
                        : Setting.Contents[key]
                };
                contentVms.Add(contentVm);

            }

            // version for content only
            //var contentVms = keys.Select(i => new ContentVm
            //{
            //    Key = i,
            //    Value = Setting.Contents[i]
            //}).ToList();


            return Json(contentVms);
        }


        /// <summary>
        /// for update themes
        /// </summary>
        /// <param name="theme">theme</param>
        /// <param name="bootswatch">bootswatch</param>
        //// POST: api/Contents
        [ResponseType(typeof(string))]
        public string Post(string theme, string bootswatch)
        {
            // friendly json return
            if (!User.IsInRole("Medium.Contents"))
                return null;

            // for themeable
            if(!String.IsNullOrWhiteSpace(theme))
            {
                var setting = new SettingRepository();

                var lastTheme = Setting.WebSettings[WebSettingType.CurrentTheme];

                setting.Set(WebSettingType.CurrentTheme, theme);

                // reset webApp when change theme
                if (theme != lastTheme)
                    DynamicBundles.Clear();

                return theme;
            }

            // for bootstrap swatch
            if (!String.IsNullOrWhiteSpace(bootswatch))
            {
                var setting = new SettingRepository();

                var lastBootswatch = Setting.WebSettings[WebSettingType.Bootswatch];

                setting.Set(WebSettingType.Bootswatch, bootswatch);

                // reset webApp when change bootswatch
                if (bootswatch != lastBootswatch)
                    DynamicBundles.Clear();

                return bootswatch;
            }


            return null;
        }


        ///// <summary>
        ///// for update themes
        ///// </summary>
        ///// <param name="value">theme</param>
        ////// POST: api/Contents
        //[ResponseType(typeof(string))]
        //public string Post([FromBody]string value)
        //{
        //    // friendly json return
        //    if (!User.IsInRole("Medium.Contents"))
        //        return null;

        //    if (String.IsNullOrWhiteSpace(value))
        //        return null;

        //    var setting = new SettingRepository();

        //    var lastTheme = Setting.WebSettings[WebSettingType.CurrentTheme];

        //    setting.Set(WebSettingType.CurrentTheme, value);

        //    // reset webapp when change theme
        //    if (value != lastTheme)
        //        DynamicBundles.Clear();

        //    return value;
        //}

        // PUT: api/Contents/5
        [ResponseType(typeof(string))]
        public string Put(string id, [FromBody]string value)
        {
            // friendly json return
            if (!User.IsInRole("Medium.Contents"))
                return null;


            id = CMS.Decode(id);

            if (!string.IsNullOrWhiteSpace(id) && id.ToLower().Contains("common.shop."))
            {
                Setting.WebSettings.Set(id, value);
            }
            else if (!string.IsNullOrWhiteSpace(id) && id.EndsWith(".Image") && Setting.Contents[id] != value)
            {
                ImageDelete(Setting.Contents[id]);
                var version = DateTime.Now.ToString("mmss");
                Setting.Contents.Edit(id, value + "?v=" + version, true);
            }
            else
            {
                Setting.Contents.Edit(id, value, true);
            }
            


            //// this slowly
            //if (string.IsNullOrWhiteSpace(_host))
            //    _host = System.Web.HttpContext.Current.Request.Url.Host;

            //if (_host == "localhost")
            //{
            //    var theme = Setting.WebSettings[WebSettingType.CurrentTheme];

            //    var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
            //        $@"Themes\{theme}\Language\en.xml");

            //    try
            //    {
            //        var xmlProvider = new XmlProvider(path);
            //        xmlProvider.UpdateXml(id, value);
            //    }
            //    catch
            //    {
            //        // ignored
            //    }
            //}


            return value;
        }
        

        //// DELETE: api/Contents/5
        //public void Delete(int id)
        //{
        //}


        public void ImageDelete(string path)
        {
            try
            {
                if (File.Exists(HostingEnvironment.MapPath(path)))
                    // ReSharper disable once AssignNullToNotNullAttribute
                    File.Delete(HostingEnvironment.MapPath(path));

                if (File.Exists(HostingEnvironment.MapPath(path.Huge())))
                    // ReSharper disable once AssignNullToNotNullAttribute
                    File.Delete(HostingEnvironment.MapPath(path.Huge()));
            }
            catch (Exception e)
            {
                LogHelper.Write(@"ContentsController.ImageDelete()",
                    $"Delete Image Fail, User: {User.Identity.Name} , Message: {e.Message}");
            }

        }
    }
}
