using System.Web.Optimization;
using cms4seo.Common.Culture;

// ReSharper disable once CheckNamespace
namespace IdentitySample
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            #region for satify complier

            bundles.Add(new ScriptBundle("~/bundles/jqueryval"));

            #endregion




            #region admin =====================================================

            //bundles.Add(new StyleBundle("~/bootstrap/css")
            //    .Include("~/Areas/Admin/Assets/sb-admin-2/vendor/fontawesome-free/css/all.min.css", new CssRewriteUrlTransform())
            //    .Include("~/Areas/Admin/Assets/jquery-ui-1.12.1/jquery-ui.css", new CssRewriteUrlTransform())
            //    .Include("~/Areas/Admin/Assets/sb-admin-2/css/sb-admin-2.css", new CssRewriteUrlTransform())
            //    .Include(
            //        "~/Areas/Admin/Assets/sb-admin-2/vendor/datatables/dataTables.bootstrap4.min.css",
            //        "~/Areas/Admin/Assets/lobibox/css/lobibox.css",
            //        "~/Areas/Admin/Assets/uploader/css/uploader.css",
            //        "~/Areas/Admin/Assets/uploader/css/demo.css",
            //        "~/Areas/Admin/Assets/admin/admin.css"
            //    ));

            //bundles.Add(new ScriptBundle("~/bootstrap/js")
            //    .Include(
            //        //"~/Areas/Admin/Assets/jquery/jquery-2.1.3.js",
            //        "~/Areas/Admin/Assets/sb-admin-2/vendor/bootstrap/js/bootstrap.bundle.min.js",
            //        "~/Areas/Admin/Assets/Helpers/helper.js",
            //        "~/Areas/Admin/Assets/sb-admin-2/vendor/jquery-easing/jquery.easing.min.js",
            //        "~/Areas/Admin/Assets/sb-admin-2/js/sb-admin-2.js",
            //        "~/Areas/Admin/Assets/sb-admin-2/vendor/chart.js/Chart.min.js",
            //        "~/Areas/Admin/Assets/sb-admin-2/js/chartjs-plugin-labels/chartjs-plugin-labels.min.js",
            //        "~/Areas/Admin/Assets/sb-admin-2/js/html-highlighter/html-syntax-highligher.js",
            //        "~/Areas/Admin/Assets/lobibox/js/lobibox.js",
            //        "~/Areas/Admin/Assets/sb-admin-2/vendor/datatables/jquery.dataTables.min.js",
            //        "~/Areas/Admin/Assets/sb-admin-2/vendor/datatables/dataTables.bootstrap4.min.js",
            //        //"~/Areas/Admin/Assets/jquery-ui-1.12.1/jquery-ui.js",
            //        //$"~/Areas/Admin/Assets/jquery-ui-1.12.1/i18n/datepicker-{CultureHelper.GetCurrentNeutralCulture()}.js",
            //        "~/Areas/Admin/Assets/admin/bootbox.js",
            //        "~/Areas/Admin/Assets/uploader/js/dmuploader.js",
            //        "~/Areas/Admin/Assets/uploader/js/demo.js",
            //        "~/Areas/Admin/Assets/admin/admin.js"
            //    ));

            #endregion admin




            #region client =====================================================

            // MOVE to /Themes/{theme}/_ViewStart

            //bundles.Add(new StyleBundle("~/Content/css")
            //    .Include("~/Themes/modern-business/slick/slick-theme.css", new CssRewriteUrlTransform())
            //    .Include(
            //        "~/Themes/modern-business/slick/slick.css",
            //        "~/Themes/modern-business/flipclock/flipclock.css",
            //        "~/Themes/modern-business/flipclock/flipclock-size.css",
            //        "~/Themes/modern-business/fancybox/source/helpers/jquery.fancybox-buttons.css",
            //        "~/Themes/modern-business/fancybox/source/helpers/jquery.fancybox-thumbs.css",
            //        "~/Themes/modern-business/vendor/bootstrap/css/bootstrap.css",                
            //        "~/Themes/modern-business/css/PagedList.css",
            //        "~/Themes/modern-business/css/animate.css",
            //        "~/Themes/modern-business/css/custom.css"
            //    )
            //    .Include("~/Themes/modern-business/font-awesome/css/font-awesome.min.css", new CssRewriteUrlTransform())
            //    .Include("~/Themes/modern-business/fancybox/source/jquery.fancybox.css", new CssRewriteUrlTransform())
            //    );

            //bundles.Add(new ScriptBundle("~/Scripts/js")
            //    .Include(
            //        "~/Themes/modern-business/jquery/jquery.js",
            //        "~/Themes/modern-business/slick/slick.min.js",
            //        "~/Themes/modern-business/js/common.js",
            //        "~/Themes/modern-business/flipclock/flipclock.js",
            //        //"~/Themes/modern-business/js/jquery.signalR-2.2.0.min.js",
            //        "~/Themes/modern-business/mail/jqBootstrapValidation.js",
            //        "~/Themes/modern-business/mail/mail.js",
            //        "~/Themes/modern-business/fancybox/lib/jquery.mousewheel-3.0.6.pack.js",
            //        "~/Themes/modern-business/fancybox/source/jquery.fancybox.pack.js",
            //        "~/Themes/modern-business/fancybox/source/helpers/jquery.fancybox-buttons.js",
            //        "~/Themes/modern-business/fancybox/source/helpers/jquery.fancybox-media.js",
            //        "~/Themes/modern-business/fancybox/source/helpers/jquery.fancybox-thumbs.js",
            //        "~/Themes/modern-business/vendor/bootstrap/js/bootstrap.bundle.min.js"
            //    ));

            #endregion client
        }
    }
}