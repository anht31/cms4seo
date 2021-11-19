using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using cms4seo.Model.cms4seoType;
using cms4seo.Model.Entities;
using cms4seo.Model.LekimaxType;

namespace cms4seo.Data.Repositories
{
    public class SettingRepository : ISettingRepository
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        public string this[string key] => Get(key);

        public string Get(string key)
        {
            return db.Settings.Find(key)?.Value;
        }

        public bool? Set(string key, string value)
        {
            bool? result = false;

            var setting = db.Settings.Find(key);

            if (setting == null)
            {
                setting = new SettingModel
                {
                    Key = key,
                    Value = value
                };
                db.Settings.Add(setting);

                result = true;
            }
            else
            {
                setting.Value = value;
                db.Entry(setting).State = EntityState.Modified;

                result = null;
            }

            db.SaveChanges();

            return result;
        }

        public bool InitializationEmail()
        {

            if (!string.IsNullOrWhiteSpace(Get(EmailSettingType.FileLocation)))
                return false;


            Set(EmailSettingType.Subject, "Mail form domain");
            Set(EmailSettingType.MailFromAddress, null);
            Set(EmailSettingType.MailToAddress, null);

            Set(EmailSettingType.MailToAddress2, null);
            Set(EmailSettingType.MailToAddressBcc, null);

            Set(EmailSettingType.ServerName, null);
            Set(EmailSettingType.ServerPort, 25.ToString());
            Set(EmailSettingType.Username, null);
            Set(EmailSettingType.Password, null);
            Set(EmailSettingType.UseSsl, true.ToString());

            Set(EmailSettingType.WriteAsFile, false.ToString());
            Set(EmailSettingType.FileLocation, "/Mail/");
            Set(EmailSettingType.FileLocation, 0.ToString());


            var i = 30;
            while (i-- > 0 && string.IsNullOrWhiteSpace(Get(EmailSettingType.FileLocation)))
            {
                Thread.Sleep(100);
            }


            return true;
        }


        public bool InitializationPhoto()
        {
            if (!string.IsNullOrWhiteSpace(Get(PhotoSettingType.MaxHeight)))
                return false;

            Set(PhotoSettingType.ImageQuality, 85.ToString());
            Set(PhotoSettingType.SmallSize, 400.ToString());
            Set(PhotoSettingType.MediumSize, 600.ToString());
            Set(PhotoSettingType.LargeSize, 960.ToString());
            Set(PhotoSettingType.MaxHeight, 1600.ToString());
            Set(PhotoSettingType.IsAutoConvertPngToJpg, false.ToString());


            var i = 30;
            while (i-- > 0 && string.IsNullOrWhiteSpace(Get(PhotoSettingType.MaxHeight)))
            {
                Thread.Sleep(100);
            }


            return true;
        }


        public bool InitializationWebSetting()
        {
            if (!string.IsNullOrWhiteSpace(Get(WebSettingType.Bootswatch)))
                return false;


            Set(WebSettingType.AdminLanguages, "en-US");
            Set(WebSettingType.ShopLanguages, "en-US");
            Set(WebSettingType.IsAutoSeoMetaTag, true.ToString());
            Set(WebSettingType.IsLockDeleteSettings, false.ToString());
            Set(WebSettingType.RedirectMode, 1.ToString());
            Set(WebSettingType.FlatterSiteArchitecture, 3.ToString());
            Set(WebSettingType.IsAutoEditPermalink, false.ToString());
            Set(WebSettingType.CurrentTheme, "moderniness");
            Set(WebSettingType.Bootswatch, "cms4seo.bootstrap.min.css");
            Set(WebSettingType.IsAutoThousandSeparator, true.ToString());
            Set(WebSettingType.OptionCategory, string.Empty);
            Set(WebSettingType.OptionPage, "Default, ContactForm, Download Page, SectionGallery");
            Set(WebSettingType.OptionProperties, string.Empty);
            Set(WebSettingType.IsProductViewCounter, false.ToString());



            var i = 30;
            while (i-- > 0 && string.IsNullOrWhiteSpace(Get(WebSettingType.Bootswatch)))
            {
                Thread.Sleep(100);
            }


            return true;
        }



        public bool InitializationEmbed()
        {
            if (!string.IsNullOrWhiteSpace(Get(EmbedSettingType.Map)))
                return false;


            Set(EmbedSettingType.GoogleAnalytics, "<!-- Google analytics -->");
            Set(EmbedSettingType.FacebookScript, "<!-- Facebook script -->");
            Set(EmbedSettingType.FacebookId, "<!-- Facebook Id -->");
            Set(EmbedSettingType.Styles, "<!-- header.style -->");
            Set(EmbedSettingType.Scripts, "<!-- footer.script -->");
            Set(EmbedSettingType.Map, "<!-- map -->");

            Set(EmbedSettingType.Comment, "<!-- Comment Facebook embed here -->");
            Set(EmbedSettingType.Header, "<!-- Header of page -->");
            Set(EmbedSettingType.Aside, "<!-- Aside of page -->");

            var i = 30;
            while (i-- > 0 && string.IsNullOrWhiteSpace(Get(EmbedSettingType.Map)))
            {
                Thread.Sleep(100);
            }


            return true;
        }

        public bool InitializationShop()
        {

            if (!string.IsNullOrWhiteSpace(Get(ShopSettingType.MetaAuthor)))
                return false;

            Set(ShopSettingType.HomepageMetaTitle, "title content");
            Set(ShopSettingType.HomepageMetaDescription, "description content");
            Set(ShopSettingType.HomepageMetaKeywords, "keyword content");
            Set(ShopSettingType.MetaAuthor, "cms4seo.com");
            Set(ShopSettingType.OgImage, String.Empty);
            Set(ShopSettingType.Favicon, String.Empty);

            Set(ShopSettingType.CardDecorate, "");
            Set(ShopSettingType.IsShowBrief, false.ToString());
            Set(ShopSettingType.MenuFormatClass, "font-weight-bold text-capitalize");
            Set(ShopSettingType.IsCardCoverObject, false.ToString());
            Set(ShopSettingType.ProductDetailSummary, String.Empty);
            Set(ShopSettingType.IsUnderConstruction, false.ToString());
            Set(ShopSettingType.IsHiddenChildMenu, false.ToString());

            Set(ShopSettingType.IsHiddenProductSummary, false.ToString());
            Set(ShopSettingType.IsDisallowCopy, false.ToString());
            Set(ShopSettingType.IsShowContact, true.ToString());

            Set(ShopSettingType.MobileContact, "_MobileContactAnimateCircle");
            Set(ShopSettingType.AspectRatioImage, "aspect-ratio-landscape");

            Set(ShopSettingType.AsideMenu, "_AsideMenu");
            Set(ShopSettingType.IsShowProductComment, true.ToString());
            Set(ShopSettingType.IsShowArticleComment, true.ToString());

            Set(ShopSettingType.ProductPageSize, 36.ToString());
            Set(ShopSettingType.ArticlePageSize, 20.ToString());



            #region Appshop

            Set(AppShop.HotlineName, "Hotline: ");
            Set(AppShop.Hotline, "0123 456 789");
            Set(AppShop.SupporterLabel, "Sale: ");
            Set(AppShop.SupporterPhone, "1234 567 890");
            Set(AppShop.Address1, "Address: Enter your address");
            Set(AppShop.Address2, "");
            Set(AppShop.CompanyNamePart1, "CMS for SEO");
            Set(AppShop.CompanyNamePart2, "High Performance CMS");
            Set(AppShop.Email, "email@example.com");
            Set(AppShop.FacebookLink, "cms4seo");

            #endregion


            var i = 30;
            while (i-- > 0 && string.IsNullOrWhiteSpace(Get(ShopSettingType.MetaAuthor)))
            {
                Thread.Sleep(100);
            }


            return true;

        }

        public bool InitializationToc()
        {
            if (!string.IsNullOrWhiteSpace(Get(TocSettingType.TocTitle)))
                return false;


            #region Init

            Set(TocSettingType.IsAutoProductTOC, false.ToString());
            Set(TocSettingType.IsAutoArticleTOC, false.ToString());
            Set(TocSettingType.TocTitle, "Table of Contents");

            #endregion



            var i = 30;
            while (i-- > 0 && string.IsNullOrWhiteSpace(Get(TocSettingType.TocTitle)))
            {
                Thread.Sleep(100);
            }

            return true;
        }
    }
}

