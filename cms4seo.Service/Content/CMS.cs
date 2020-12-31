using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using cms4seo.Model.LekimaxType;
using cms4seo.Service.Provider;

namespace cms4seo.Service.Content
{

    public static class CMS
    {
        private static int _cacheDuration;

        public static int CacheDuration
        {
            get
            {
                return _cacheDuration;
            }
            set
            {
                if (!_cacheDuration.Equals(value))
                    _cacheDuration = value;
            }
        }


        private static bool _active;

        //private static string _host;

        public static bool Active
        {
            get
            {
                return _active;
            }
            set
            {
                if (!value.Equals(_active))
                    _active = value;
            }
        }


        public static string Encode(string value)
        {
            return value?.Replace(".", "-");
        }

        public static string Decode(string value)
        {
            return value?.Replace("-", ".");
        }


        public static string Attribute(params string[] keys)
        {
            if (!Active)
                return $"data-cms={Encode(string.Join(",", keys))}";


            foreach (var key in keys)
            {
                if(string.IsNullOrWhiteSpace(key))
                    continue;

                Setting.Contents.Create(key, "Please enter value");
            }

            return $"data-cms={Encode(string.Join(",", keys))}";
        }


        public static IEnumerable ToOptions(this string value)
        {
            if (value == null)
                return String.Empty;

            return value.Split(',').ToList();
        }



        public static string BuildThemesOption()
        {
            string options = "";

            string themesPath = System.Web.HttpContext.Current.Server.MapPath("/Themes");
            var directories = Directory.GetDirectories(themesPath).Select(Path.GetFileName);
            var themeList = directories.ToDictionary(item => item.Trim(), item => item.Trim());
            var currentTheme = Setting.WebSettings[WebSettingType.CurrentTheme];

            foreach (var theme in themeList)
            {
                var selected = currentTheme == theme.Value ? "selected" : "";
                options += $"<option value='{theme.Value}' {selected}>{theme.Key}</option>";
            }

            return options;
        }


        public static string BuildBootswatchOption()
        {
            string options = "";

            string bootswatchPath = System.Web.HttpContext.Current.Server.MapPath("/Bootswatch");

            var bootSwatchs = Directory.GetFiles(bootswatchPath).Select(Path.GetFileName);

            // Bootswatch Dictionary
            Dictionary<string, string> bootswatchList2 =
                bootSwatchs.ToDictionary(item => item.Trim(), item => item.Trim());

            Dictionary<string, string> bootswatchList = new Dictionary<string, string>
            {
                { "Default", "cms4seo.bootstrap.min.css" }
            };

            foreach (var item in bootswatchList2)
                bootswatchList.Add(item.Key, item.Value);


            var currentBootswatch = Setting.WebSettings[WebSettingType.Bootswatch];

            foreach (var bootswatch in bootswatchList)
            {
                var selected = currentBootswatch == bootswatch.Value ? "selected" : "";
                options += $"<option value='{bootswatch.Value}' {selected}>{bootswatch.Key}</option>";
            }

            return options;
        }


        public static string RenderBootswatch()
        {
            if (!DynamicBundles.SupportBootswatch)
                return string.Empty;

            var html =
                "<h3>Bootswatch</h3>" +
                "<div class='themes-option-style'>" +
                    "<select id='bootswatch-switcher'>" +
                        BuildBootswatchOption() +
                    "</select>" +
                "</div>";

            return html;
        }



        //public static MvcHtmlString RenderToggleEditButton(this bool permission)
        //{
        //    var html =
        //        "<div class='toggleEditContents' onclick='ToggleEditContents()'>" +
        //            "<span class='fa-stack fa-lg'>" +
        //                "<i class='fa fa-circle fa-stack-2x'></i>" +
        //                "<i class='fas fa-cogs fa-stack-1x fa-inverse'></i>" +
        //            "</span>" +
        //        "</div>" +
        //        "<script>window.IsAuthorizeEditContent = true;</script>";

        //    if (!permission)
        //    {
        //        _active = false;
        //        return new MvcHtmlString("<script>window.IsAuthorizeEditContent = false;</script>");
        //    }

        //    _active = true;
        //    return new MvcHtmlString(html);
        //}



        //public static MvcHtmlString RenderCmsSwitcher(this bool permission)
        //{
        //    var html =
        //        "<div class='se-pre-con'></div>" +
        //        "<div id='cms-switcher'>" +
        //            "<h2>CMS Switcher" +
        //                "<a href='#'>" +
        //                    "<i class='fa fa-cogs'></i>" +
        //                "</a>" +
        //            "</h2>" +
        //            "<div>" +
        //            "<h3>Themes</h3>" +
        //            "<div class='themes-option-style'>" +
        //                "<select id='theme-switcher'>" +
        //                    BuildThemesOption() +
        //                "</select>" +
        //            "</div>" +
        //            "<h3>Contents Toggle</h3>" +
        //            "<label class='toggleSwitch'>" +
        //                "<input type='checkbox' id='switchEditContents'>" +
        //                "<span class='checkbox-slider round'></span>" +
        //            "</label>" +
        //            "</div>" +
        //        "</div>" +
        //        "<script>window.IsAuthorizeEditContent = true;</script>";

        //    if (!permission)
        //    {
        //        _active = false;
        //        return new MvcHtmlString("<script>window.IsAuthorizeEditContent = false;</script>");
        //    }

        //    _active = true;
        //    return new MvcHtmlString(html);
        //}

    }

    public class DynamicOutputCacheAttribute : OutputCacheAttribute
    {
        public DynamicOutputCacheAttribute()
        {
            this.Duration = CMS.CacheDuration;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (this.Duration != CMS.CacheDuration)
                this.Duration = CMS.CacheDuration;
        }
    }
}
