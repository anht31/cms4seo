using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cms4seo.Model.LekimaxType
{
    public class ShopSettingType
    {
        public const string Id = "ShopSetting";


        // group for WebSettings
        [DataType(DataType.MultilineText)]
        public const string HomepageMetaTitle = "Shop.HomepageMetaTitle";
        [DataType(DataType.MultilineText)]
        public const string HomepageMetaDescription = "Shop.HomepageMetaDescription";
        public const string HomepageMetaKeywords = "Shop.HomepageMetaKeywords";
        public const string MetaAuthor = "Shop.MetaAuthor";
        public const string OgImage = "Shop.OgImage";
        public const string Favicon = "Shop.Favicon";

        public const string CardDecorate = "Shop.CardDecorate";
        public const string IsShowBrief = "Shop.IsShowBrief";
        public const string MenuFormatClass = "Shop.MenuFormatClass";
        public const string IsCardCoverObject = "Shop.IsCardCoverObject";
        [DataType(DataType.MultilineText)]
        public const string ProductDetailSummary = "Shop.ProductDetailSummary";
        public const string IsUnderConstruction = "Shop.IsUnderConstruction";

        public const string IsHiddenChildMenu = "Shop.IsHiddenChildMenu";

        public const string IsHiddenProductSummary = "Shop.IsHiddenProductSummary";
        public const string IsDisallowCopy = "Shop.IsDisallowCopy";
        public const string IsShowContact = "Shop.IsShowContact";


        public const string MobileContact = "Shop.MobileContact";

        public const string AspectRatioImage = "Shop.AspectRatioImage";

        public const string AsideMenu = "Shop.AsideMenu";

        public const string IsShowProductComment = "Shop.IsShowProductComment";
        public const string IsShowArticleComment = "Shop.IsShowArticleComment";

        public const string ProductPageSize = "Shop.ProductPageSize";
        public const string ArticlePageSize = "Shop.ArticlePageSize";

    }
}
