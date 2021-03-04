using System.Collections.Generic;
using System.Web.Mvc;


namespace cms4seo.Service.Themeable
{
    public class ThemeableRazorViewEngine : ThemeableVirtualPathProviderViewEngine
    {
        public ThemeableRazorViewEngine()
        {
            AreaViewLocationFormats = new[]
                                          {

                                              //default
                                              "~/Areas/Admin/Views/{1}/{0}.cshtml",
                                              "~/Areas/Admin/Views/Shared/{0}.cshtml",

                                              

                                          };

            AreaMasterLocationFormats = new[]
                                            {

                                                //default
                                                "~/Areas/Admin/Views/{1}/{0}.cshtml",
                                                "~/Areas/Admin/Views/Shared/{0}.cshtml",

                                                
                                            };

            AreaPartialViewLocationFormats = new[]
                                                 {

                                                     //default
                                                    "~/Areas/Admin/Views/{1}/{0}.cshtml",
                                                    "~/Areas/Admin/Views/Shared/{0}.cshtml",

                                                    
                                                 };




            ViewLocationFormats = new[]
                                      { 
                                            //themes
                                            "~/Themes/{2}/Views/{1}/{0}.cshtml",
                                            "~/Themes/{2}/Views/Shared/{0}.cshtml",

                                            //default
                                            "~/Views/{1}/{0}.cshtml",
                                            "~/Views/Shared/{0}.cshtml",

                                            //plugin demo
                                            //"~/Plugins/{2}/Views/{1}/{0}.cshtml",
                                            //"~/Plugins/{2}/Views/Shared/{0}.cshtml",

                                            //"~/Plugins/PluginTest/Views/{1}/{0}.cshtml",
                                            //"~/Plugins/PluginTest/Views/Shared/{0}.cshtml",
                                      };

            MasterLocationFormats = new[]
                                        {
                                            //themes
                                            "~/Themes/{2}/Views/{1}/{0}.cshtml",
                                            "~/Themes/{2}/Views/Shared/{0}.cshtml",

                                            //default
                                            "~/Views/{1}/{0}.cshtml",
                                            "~/Views/Shared/{0}.cshtml",

                                            //plugin demo
                                            //"~/Plugins/{2}/Views/{1}/{0}.cshtml",
                                            //"~/Plugins/{2}/Views/Shared/{0}.cshtml",

                                            //"~/Plugins/PluginTest/Views/{1}/{0}.cshtml",
                                            //"~/Plugins/PluginTest/Views/Shared/{0}.cshtml",
                                        };

            PartialViewLocationFormats = new[]
                                             {
                                                 //themes
                                                "~/Themes/{2}/Views/{1}/{0}.cshtml",
                                                "~/Themes/{2}/Views/Shared/{0}.cshtml",

                                                //default
                                                "~/Views/{1}/{0}.cshtml",
                                                "~/Views/Shared/{0}.cshtml",

                                                //plugin
                                                //"~/Plugins/{2}/Views/{1}/{0}.cshtml",
                                                //"~/Plugins/{2}/Views/Shared/{0}.cshtml",

                                                //"~/Plugins/PluginTest/Views/{1}/{0}.cshtml",
                                                //"~/Plugins/PluginTest/Views/Shared/{0}.cshtml",
                                             };

            FileExtensions = new[] { "cshtml" };
        }

        protected override IView CreatePartialView(ControllerContext controllerContext, string partialPath)
        {
            IEnumerable<string> fileExtensions = base.FileExtensions;
            return new RazorView(controllerContext, partialPath, null, false, fileExtensions);
            //return new RazorView(controllerContext, partialPath, layoutPath, runViewStartPages, fileExtensions, base.ViewPageActivator);
        }

        protected override IView CreateView(ControllerContext controllerContext, string viewPath, string masterPath)
        {
            IEnumerable<string> fileExtensions = base.FileExtensions;
            return new RazorView(controllerContext, viewPath, masterPath, true, fileExtensions);
        }
    }
}
