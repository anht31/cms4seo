using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Admin.Resources;
using cms4seo.Model.LekimaxType;

namespace cms4seo.Model.ViewModel
{
    public class WebSettingVm
    {
        [DataType(cms4seoDataType.CustomEditor)]
        public string Id { get; set; }


        [Display(Name = "WebSettingVmAdminLanguages", ResourceType = typeof(AdminResources))]
        [DataType(cms4seoDataType.CustomEditor)]
        public string AdminLanguages { get; set; }

        [Display(Name = "WebSettingVmShopLanguages", ResourceType = typeof(AdminResources))]
        public string ShopLanguages { get; set; }

        

        [Display(Name = "WebSettingVmIsLockDeleteSettings", ResourceType = typeof(AdminResources))]
        public bool IsLockDeleteSettings { get; set; }

        public Dictionary<int, string> RedirectModes = new Dictionary<int, string>
        {
            {0, AdminResources.WebSettingVm_RedirectModes_NoRedirect},
            {1, AdminResources.WebSettingVm_RedirectModes_RedirectWhenNotfound},
            {2, AdminResources.WebSettingVm_RedirectModes_ForceRedirects}
        };

        [Display(Name = "WebSettingVmRedirectMode", ResourceType = typeof(AdminResources))]
        [DataType(cms4seoDataType.CustomEditor)]
        public int RedirectMode { get; set; }



        public Dictionary<int, string> FlatterSiteArchitectures = new Dictionary<int, string>
        {
            {1, AdminResources.WebSettingVm_1_Segment},
            {2, AdminResources.WebSettingVm_2_Segment},
            {3, AdminResources.WebSettingVm_3_Segment},
            {4, "Dynamic 1 - 2 segment"},
        };


        [DataType(cms4seoDataType.CustomEditor)]
        [Display(Name = "WebSettingVmFlatterSiteArchitecture", ResourceType = typeof(AdminResources))]
        public int FlatterSiteArchitecture { get; set; }

        [Display(Name = "WebSettingVmIsAutoSeoMetaTag", ResourceType = typeof(AdminResources))]
        public bool IsAutoSeoMetaTag { get; set; }

        [Display(Name = "WebSettingVm_IsAutoEditPermalink", ResourceType = typeof(AdminResources))]
        public bool IsAutoEditPermalink { get; set; }


        public Dictionary<string, string> ThemeList { get; set; }


        [DataType(cms4seoDataType.CustomEditor)]
        public string CurrentTheme { get; set; }

        [Display(Name = "WebSettingVm_IsAutoThousandSeparator", ResourceType = typeof(AdminResources))]
        public bool IsAutoThousandSeparator { get; set; }

        [Display(Name = "WebSettingVm_IsProductViewCounter", ResourceType = typeof(AdminResources))]
        public bool IsProductViewCounter { get; set; }

        //public Dictionary<string, string> BootswatchList { get; set; }


        //[DataType(LekimaxDataType.CustomEditor)]
        //[Display(Name = "Bootswatch (Platform Theme, support Bootstrap Themes only)")]
        //public string Bootswatch { get; set; }

        [Display(Name = "WebSettingVmOptionCategory", ResourceType = typeof(AdminResources))]
        public string OptionCategory { get; set; }


        [Display(Name = "WebSettingVmOptionPage", ResourceType = typeof(AdminResources))]
        public string OptionPage { get; set; }


        [Display(Name = "WebSettingVmOptionProperties", ResourceType = typeof(AdminResources))]
        public string OptionProperties { get; set; }

    }
}
