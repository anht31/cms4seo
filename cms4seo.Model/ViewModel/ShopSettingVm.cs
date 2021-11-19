using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Admin.Resources;
using cms4seo.Model.LekimaxType;

namespace cms4seo.Model.ViewModel
{
    public class ShopSettingVm
    {
        [ScaffoldColumn(false)]
        public string Id { get; set; }


        [Display(Name = "ShopSettingVmHomepageMetaTitle", ResourceType = typeof(AdminResources))]
        public string HomepageMetaTitle { get; set; }

        [Display(Name = "ShopSettingVmHomepageMetaDescription", ResourceType = typeof(AdminResources))]
        public string HomepageMetaDescription { get; set; }

        [Display(Name = "ShopSettingVmHomepageMetaKeywords", ResourceType = typeof(AdminResources))]
        public string HomepageMetaKeywords { get; set; }

        [Display(Name = "ShopSettingVmMetaAuthor", ResourceType = typeof(AdminResources))]
        public string MetaAuthor { get; set; }

        [Display(Name = "ShopSettingVmOgImage", ResourceType = typeof(AdminResources))]
        public string OgImage { get; set; }

        [Display(Name = "ShopSettingVm_Favicon", ResourceType = typeof(AdminResources))]
        public string Favicon { get; set; }

        [Display(Name = "ShopSettingVmCardDecorate", ResourceType = typeof(AdminResources))]
        public string CardDecorate { get; set; }

        [Display(Name = "ShopSettingVmIsShowBrief", ResourceType = typeof(AdminResources))]
        public bool IsShowBrief { get; set; }

        [Display(Name = "ShopSettingVmMenuFormatClass", ResourceType = typeof(AdminResources))]
        public string MenuFormatClass { get; set; }


        [AllowHtml]
        [DataType(DataType.MultilineText)]
        [Display(Name = "ShopSettingVmProductDetailSummary", ResourceType = typeof(AdminResources))]
        public string ProductDetailSummary { get; set; }

        [Display(Name = "ShopSettingVmIsUnderConstruction", ResourceType = typeof(AdminResources))]
        public bool IsUnderConstruction { get; set; }

        [Display(Name = "ShopSettingVmIsHiddenChildMenu", ResourceType = typeof(AdminResources))]
        public bool IsHiddenChildMenu { get; set; }


        // old code, move to AspectRatioImage
        [ScaffoldColumn(false)]
        [Display(Name = "ShopSettingVmIsCardCoverObject", ResourceType = typeof(AdminResources))]
        public bool IsCardCoverObject { get; set; }


        [Display(Name = "ShopSettingVm_IsHiddenProductSummary", ResourceType = typeof(AdminResources))]
        public bool IsHiddenProductSummary { get; set; }

        [Display(Name = "ShopSettingVm_IsDisallowCopy", ResourceType = typeof(AdminResources))]
        public bool IsDisallowCopy { get; set; }


        [Display(Name = "ShopSettingVm_IsShowContact", ResourceType = typeof(AdminResources))]
        public bool IsShowContact { get; set; }

        public Dictionary<string, string> MobileContacts = new Dictionary<string, string>
        {
            {String.Empty, AdminResources.ShopSettingVm_NoMobileContacts},
            {"_MobileContactLegacy", AdminResources.ShopSettingVm_MobileContactLegacy},
            {"_MobileContactAnimate", AdminResources.ShopSettingVm_MobileContactAnimate},
            {"_MobileContactBar", AdminResources.ShopSettingVm_MobileMobileContactBar},
            {"_MobileContactCircle", AdminResources.ShopSettingVm_MobileContactCircle},
            {"_MobileContactAnimateCircle", AdminResources.ShopSettingVm_MobileContactAnimateCircle},
        };

        [Display(Name = "ShopSettingVm_MobileContact", ResourceType = typeof(AdminResources))]
        [DataType(cms4seoDataType.CustomEditor)]
        public string MobileContact { get; set; }



        public Dictionary<string, string> AspectRatioImages = new Dictionary<string, string>
        {
            {String.Empty, AdminResources.ShopSettingVm_AspectRatioImages_NotUse},
            {"aspect-ratio-16-9", AdminResources.ShopSettingVm_AspectRatioImages_16_9},
            {"aspect-ratio-3-2", AdminResources.ShopSettingVm_AspectRatioImages_3_2},
            {"aspect-ratio-landscape", AdminResources.ShopSettingVm_AspectRatioImages_Landscape},
            {"aspect-ratio-square", AdminResources.ShopSettingVm_AspectRatioImages_Square},
            {"aspect-ratio-portrait", AdminResources.ShopSettingVm_AspectRatioImages_Portrait},
            {"aspect-ratio-2-3", AdminResources.ShopSettingVm_AspectRatioImages_2_3},
            {"aspect-ratio-9-16", AdminResources.ShopSettingVm_AspectRatioImages_9_16},
        };

        [Display(Name = "ShopSettingVm_AspectRatioImage", ResourceType = typeof(AdminResources))]
        [DataType(cms4seoDataType.CustomEditor)]
        public string AspectRatioImage { get; set; }


        public Dictionary<string, string> AsideMenus = new Dictionary<string, string>
        {
            {"_AsideMenu", AdminResources.ShopSettingVm_AsideMenuChildView},
            {"_AsideMenuDropdown", AdminResources.ShopSettingVm_AsideMenuDropdown},
        };

        [Display(Name = "ShopSettingVm_AsideMenu", ResourceType = typeof(AdminResources))]
        [DataType(cms4seoDataType.CustomEditor)]
        public string AsideMenu { get; set; }


        [Display(Name = "ShopSettingVm_IsShowProductComment", ResourceType = typeof(AdminResources))]
        [DataType(cms4seoDataType.CustomEditor)]
        public bool IsShowProductComment { get; set; }


        [Display(Name = "ShopSettingVm_IsShowArticleComment", ResourceType = typeof(AdminResources))]
        [DataType(cms4seoDataType.CustomEditor)]
        public bool IsShowArticleComment { get; set; }


        [Display(Name = "ShopSettingVm_ProductPageSize", ResourceType = typeof(AdminResources))]
        [DataType(cms4seoDataType.CustomEditor)]
        public int ProductPageSize { get; set; }

        [Display(Name = "ShopSettingVm_ArticlePageSize", ResourceType = typeof(AdminResources))]
        [DataType(cms4seoDataType.CustomEditor)]
        public int ArticlePageSize { get; set; }
    }
}
