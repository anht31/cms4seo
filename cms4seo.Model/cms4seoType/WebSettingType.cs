using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cms4seo.Model.LekimaxType
{
    public class WebSettingType
    {
        [ScaffoldColumn(false)]
        public const string Id = "WebSetting";

        public const string AdminLanguages = "Website.AdminLanguages";
        public const string ShopLanguages = "Website.ShopLanguages";
        public const string IsAutoSeoMetaTag = "Website.IsAutoSeoMetaTag";
        public const string IsLockDeleteSettings = "Website.IsLockDeleteSettings";

        public const string RedirectMode = "Website.RedirectMode";

        public const string FlatterSiteArchitecture = "Website.FlatterSiteArchitecture";

        public const string IsAutoEditPermalink = "Website.IsAutoEditPermalink";

        public const string ThemeList = "Website.ThemeList";

        public const string CurrentTheme = "Website.CurrentTheme";

        public const string Bootswatch = "Website.Bootswatch";

        public const string IsAutoThousandSeparator = "Website.IsAutoThousandSeparator";

        public const string OptionCategory = "Website.OptionCategory";

        public const string OptionPage = "Website.OptionPage";

        public const string OptionProperties = "Website.OptionProperties";

        public const string IsProductViewCounter = "Website.IsProductViewCounter";

    }
}
