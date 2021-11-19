using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using Admin.Resources;
using cms4seo.Common.Attribute;
using cms4seo.Common.BootstrapHelpers;
using cms4seo.Common.Helpers;
using cms4seo.Common.Security;
using cms4seo.Common.SettingHelpers;
using cms4seo.Data;
using cms4seo.Data.Repositories;
using cms4seo.Model.cms4seoType;
using cms4seo.Model.LekimaxType;
using cms4seo.Model.ViewModel;
using cms4seo.Service.Content;
using cms4seo.Service.Provider;

namespace cms4seo.Admin.Controllers
{
    [CustomAuthorize(Roles = "Medium.Config")]
    public class ConfigController : BaseController
    {

        private ISettingRepository settingRepository;

        public ConfigController()
        {
            settingRepository = new SettingRepository();
        }

        public ActionResult Index()
        {
            return View();
        }

        // GET: Config/Email
        public ActionResult EmailSettings()
        {
            // check setting already
            settingRepository.InitializationEmail();


            var emailSettingVm = new EmailSettingVm();
            emailSettingVm.Id = EmailSettingType.Id;


            emailSettingVm.Subject = settingRepository.Get(EmailSettingType.Subject);
            emailSettingVm.MailFromAddress = settingRepository.Get(EmailSettingType.MailFromAddress);
            emailSettingVm.MailToAddress = settingRepository.Get(EmailSettingType.MailToAddress);
            emailSettingVm.MailToAddress2 = settingRepository.Get(EmailSettingType.MailToAddress2);
            emailSettingVm.MailToAddressBcc = settingRepository.Get(EmailSettingType.MailToAddressBcc);

            emailSettingVm.ServerName = settingRepository.Get(EmailSettingType.ServerName);
            emailSettingVm.ServerPort = Convert.ToInt16(settingRepository.Get(EmailSettingType.ServerPort));
            emailSettingVm.Username = settingRepository.Get(EmailSettingType.Username);

            var hashcode = settingRepository.Get(EmailSettingType.Password);
            emailSettingVm.Password = AesOperation.Decrypt(hashcode);
            emailSettingVm.UseSsl = Convert.ToBoolean(settingRepository.Get(EmailSettingType.UseSsl));

            emailSettingVm.WriteAsFile = Convert.ToBoolean(settingRepository.Get(EmailSettingType.WriteAsFile));
            emailSettingVm.FileLocation = settingRepository.Get(EmailSettingType.FileLocation);
            emailSettingVm.SaveMode = settingRepository.Get(EmailSettingType.SaveMode).AsInt();



            TempData[MessageType.Info] = $"{EmailSettingType.MailToAddress2}, {EmailSettingType.MailToAddressBcc}"
                            + AdminResources.ConfigControllerEmailSettings__if_not_use__you_just_leave_Empty;

            return View(emailSettingVm);
        }


        [HttpPost]
        public ActionResult EmailSettings(EmailSettingVm emailSettingVm)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }



            settingRepository.Set(EmailSettingType.Subject, emailSettingVm.Subject);
            settingRepository.Set(EmailSettingType.MailFromAddress, emailSettingVm.MailFromAddress);
            settingRepository.Set(EmailSettingType.MailToAddress, emailSettingVm.MailToAddress);
            settingRepository.Set(EmailSettingType.MailToAddress2, emailSettingVm.MailToAddress2);
            settingRepository.Set(EmailSettingType.MailToAddressBcc, emailSettingVm.MailToAddressBcc);

            settingRepository.Set(EmailSettingType.ServerName, emailSettingVm.ServerName);
            settingRepository.Set(EmailSettingType.ServerPort, emailSettingVm.ServerPort.ToString());
            settingRepository.Set(EmailSettingType.Username, emailSettingVm.Username);

            var hashcode = AesOperation.Encrypt(emailSettingVm.Password);
            settingRepository.Set(EmailSettingType.Password, hashcode);
            settingRepository.Set(EmailSettingType.UseSsl, emailSettingVm.UseSsl.ToString());

            settingRepository.Set(EmailSettingType.WriteAsFile, emailSettingVm.WriteAsFile.ToString());
            settingRepository.Set(EmailSettingType.FileLocation, emailSettingVm.FileLocation);

            settingRepository.Set(EmailSettingType.SaveMode, emailSettingVm.SaveMode.ToString());


            TempData[MessageType.Warning] =
                string.Format(AdminResources.ConfigControllerEmailSettings__0___has__been__saved, emailSettingVm.Id);


            return RedirectToAction("Index");
        }



        public ActionResult WebSettings()
        {

            // check setting already
            settingRepository.InitializationWebSetting();


            string themesPath = Server.MapPath("/Themes");

            string bootswatchPath = Server.MapPath("/Bootswatch");

            var directories = Directory.GetDirectories(themesPath).Select(Path.GetFileName);

            var bootswatch = Directory.GetFiles(bootswatchPath).Select(Path.GetFileName);


            // Bootswatch Dictionary
            Dictionary<string, string> bootswatchList2 =
                bootswatch.ToDictionary(item => item.Trim(), item => item.Trim());

            Dictionary<string, string> bootswatchList = new Dictionary<string, string>
            {
                {String.Empty, "Default"}
            };

            foreach (var item in bootswatchList2)
                bootswatchList.Add(item.Key, item.Value);


            var webSettingVm = new WebSettingVm
            {
                Id = WebSettingType.Id,

                AdminLanguages = settingRepository.Get(WebSettingType.AdminLanguages),
                ShopLanguages = settingRepository.Get(WebSettingType.ShopLanguages),
                OptionCategory = settingRepository.Get(WebSettingType.OptionCategory),
                OptionProperties = settingRepository.Get(WebSettingType.OptionProperties),
                OptionPage = settingRepository.Get(WebSettingType.OptionPage),
                IsAutoSeoMetaTag = settingRepository.Get(WebSettingType.IsAutoSeoMetaTag).ToBoolean(),
                IsLockDeleteSettings = settingRepository.Get(WebSettingType.IsLockDeleteSettings).AsBool(),
                RedirectMode = settingRepository.Get(WebSettingType.RedirectMode).AsInt(),
                FlatterSiteArchitecture = settingRepository.Get(WebSettingType.FlatterSiteArchitecture).AsInt(),
                IsAutoEditPermalink = settingRepository.Get(WebSettingType.IsAutoEditPermalink).AsBool(),
                CurrentTheme = settingRepository.Get(WebSettingType.CurrentTheme),
                ThemeList = directories.ToDictionary(item => item.Trim(), item => item.Trim()),
                IsAutoThousandSeparator = settingRepository.Get(WebSettingType.IsAutoThousandSeparator).AsBool(),
                IsProductViewCounter = settingRepository.Get(WebSettingType.IsProductViewCounter).AsBool(),
                //Bootswatch = settingRepository.Get(WebSettingType.Bootswatch), // for simple edit bootswatch client side only
                //BootswatchList = bootswatchList // for simple edit bootswatch client side only

                //ThemeList = settingRepository.Get(WebSettingType.ThemeList)
                //    .Split(',').ToDictionary(item => item.Trim(), item => item.Trim())

            };

            return View(webSettingVm);
        }




        [HttpPost]
        public ActionResult WebSettings(WebSettingVm webSettingVm)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var lastTheme = Setting.WebSettings[WebSettingType.CurrentTheme];
            var lastShopLanguage = Setting.WebSettings[WebSettingType.ShopLanguages];

            //var lastBootswatch = Setting.WebSettings[WebSettingType.Bootswatch];


            settingRepository.Set(WebSettingType.AdminLanguages, webSettingVm.AdminLanguages);
            settingRepository.Set(WebSettingType.ShopLanguages, webSettingVm.ShopLanguages);
            settingRepository.Set(WebSettingType.OptionCategory, webSettingVm.OptionCategory);
            settingRepository.Set(WebSettingType.OptionProperties, webSettingVm.OptionProperties);
            settingRepository.Set(WebSettingType.OptionPage, webSettingVm.OptionPage);
            settingRepository.Set(WebSettingType.IsAutoSeoMetaTag, webSettingVm.IsAutoSeoMetaTag.ToString());
            settingRepository.Set(WebSettingType.IsLockDeleteSettings, webSettingVm.IsLockDeleteSettings.ToString());
            settingRepository.Set(WebSettingType.RedirectMode, webSettingVm.RedirectMode.ToString());
            settingRepository.Set(WebSettingType.FlatterSiteArchitecture, webSettingVm.FlatterSiteArchitecture.ToString());
            settingRepository.Set(WebSettingType.IsAutoEditPermalink, webSettingVm.IsAutoEditPermalink.ToString());
            settingRepository.Set(WebSettingType.CurrentTheme, webSettingVm.CurrentTheme);
            settingRepository.Set(WebSettingType.IsAutoThousandSeparator, webSettingVm.IsAutoThousandSeparator.ToString());
            settingRepository.Set(WebSettingType.IsProductViewCounter, webSettingVm.IsProductViewCounter.ToString());
            //settingRepository.Set(WebSettingType.Bootswatch, webSettingVm.Bootswatch);


            TempData[MessageType.Warning] =
                string.Format(AdminResources.ConfigControllerAdminSettings__0___has__been__saved, webSettingVm.Id);


            // reset webapp when change theme
            if (webSettingVm.CurrentTheme != lastTheme || webSettingVm.ShopLanguages != lastShopLanguage)
                DynamicBundles.Clear();

            //if (webSettingVm.CurrentTheme != lastTheme || webSettingVm.Bootswatch != lastBootswatch)
            //    DynamicBundles.Clear();

            return RedirectToAction("Index");
        }



        public ActionResult EmbedSettings()
        {
            // check setting already
            //settingRepository.InitializationEmbed();


            var embedSetting = new EmbedSettingVm
            {
                Id = EmbedSettingType.Id,

                FacebookId = settingRepository.Get(EmbedSettingType.FacebookId),
                FacebookScript = settingRepository.Get(EmbedSettingType.FacebookScript),
                FacebookPage = settingRepository.Get(EmbedSettingType.FacebookPage),
                GoogleAnalytics = settingRepository.Get(EmbedSettingType.GoogleAnalytics),
                Map = settingRepository.Get(EmbedSettingType.Map),
                Styles = settingRepository.Get(EmbedSettingType.Styles),
                Scripts = settingRepository.Get(EmbedSettingType.Scripts),
                Comment = settingRepository.Get(EmbedSettingType.Comment),
                Header = settingRepository.Get(EmbedSettingType.Header),
                Aside = settingRepository.Get(EmbedSettingType.Aside)
            };

            return View(embedSetting);
        }


        [HttpPost]
        public ActionResult EmbedSettings(EmbedSettingVm embedSettingVm)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }


            settingRepository.Set(EmbedSettingType.FacebookId, embedSettingVm.FacebookId);
            settingRepository.Set(EmbedSettingType.FacebookScript, embedSettingVm.FacebookScript);
            settingRepository.Set(EmbedSettingType.FacebookPage, embedSettingVm.FacebookPage);
            settingRepository.Set(EmbedSettingType.GoogleAnalytics, embedSettingVm.GoogleAnalytics);
            settingRepository.Set(EmbedSettingType.Map, embedSettingVm.Map);
            settingRepository.Set(EmbedSettingType.Styles, embedSettingVm.Styles);
            settingRepository.Set(EmbedSettingType.Scripts, embedSettingVm.Scripts);

            settingRepository.Set(EmbedSettingType.Comment, embedSettingVm.Comment);
            settingRepository.Set(EmbedSettingType.Header, embedSettingVm.Header);
            settingRepository.Set(EmbedSettingType.Aside, embedSettingVm.Aside);


            TempData[MessageType.Warning] =
                string.Format(AdminResources.ConfigControllerEmbedSettings__0___has__been__saved, embedSettingVm.Id);

            return RedirectToAction("Index");
        }





        public ActionResult PhotoSettings()
        {
            // check setting is already
            settingRepository.InitializationPhoto();


            PhotoSettingVm photoSettingVm = new PhotoSettingVm
            {
                Id = PhotoSettingType.Id,

                ImageQuality = Convert.ToInt16(settingRepository.Get(PhotoSettingType.ImageQuality)),
                SmallSize = Convert.ToInt16(settingRepository.Get(PhotoSettingType.SmallSize)),
                MediumSize = Convert.ToInt16(settingRepository.Get(PhotoSettingType.MediumSize)),
                LargeSize = Convert.ToInt16(settingRepository.Get(PhotoSettingType.LargeSize)),
                MaxHeight = Convert.ToInt16(settingRepository.Get(PhotoSettingType.MaxHeight)),
                IsAutoConvertPngToJpg = settingRepository.Get(PhotoSettingType.IsAutoConvertPngToJpg).AsBool()
            };


            return View(photoSettingVm);
        }



        [HttpPost]
        public ActionResult PhotoSettings(PhotoSettingVm photoSettingVm)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }


            settingRepository.Set(PhotoSettingType.ImageQuality, photoSettingVm.ImageQuality.ToString());
            settingRepository.Set(PhotoSettingType.SmallSize, photoSettingVm.SmallSize.ToString());
            settingRepository.Set(PhotoSettingType.MediumSize, photoSettingVm.MediumSize.ToString());
            settingRepository.Set(PhotoSettingType.LargeSize, photoSettingVm.LargeSize.ToString());
            settingRepository.Set(PhotoSettingType.MaxHeight, photoSettingVm.MaxHeight.ToString());

            settingRepository.Set(PhotoSettingType.IsAutoConvertPngToJpg, photoSettingVm.IsAutoConvertPngToJpg.ToString());


            TempData[MessageType.Warning] =
                string.Format(AdminResources.ConfigControllerPhotoSettings__0___has__been__saved, photoSettingVm.Id);

            return RedirectToAction("Index");
        }




        public ActionResult ShopSettings()
        {

            // check setting already
            settingRepository.InitializationShop();


            var shopSettingVm = new ShopSettingVm
            {
                Id = ShopSettingType.Id,

                HomepageMetaTitle = settingRepository.Get(ShopSettingType.HomepageMetaTitle),
                HomepageMetaDescription = settingRepository.Get(ShopSettingType.HomepageMetaDescription),
                HomepageMetaKeywords = settingRepository.Get(ShopSettingType.HomepageMetaKeywords),
                MetaAuthor = settingRepository.Get(ShopSettingType.MetaAuthor),
                OgImage = settingRepository.Get(ShopSettingType.OgImage),
                Favicon = settingRepository.Get(ShopSettingType.Favicon),

                CardDecorate = settingRepository.Get(ShopSettingType.CardDecorate),
                IsShowBrief = settingRepository.Get(ShopSettingType.IsShowBrief).AsBool(),
                MenuFormatClass = settingRepository.Get(ShopSettingType.MenuFormatClass),
                IsCardCoverObject = settingRepository.Get(ShopSettingType.IsCardCoverObject).AsBool(),
                ProductDetailSummary = settingRepository.Get(ShopSettingType.ProductDetailSummary),

                IsUnderConstruction = settingRepository.Get(ShopSettingType.IsUnderConstruction).AsBool(),

                IsHiddenChildMenu = settingRepository.Get(ShopSettingType.IsHiddenChildMenu).AsBool(),

                IsHiddenProductSummary = settingRepository.Get(ShopSettingType.IsHiddenProductSummary).AsBool(),
                IsDisallowCopy = settingRepository.Get(ShopSettingType.IsDisallowCopy).AsBool(),
                IsShowContact = settingRepository.Get(ShopSettingType.IsShowContact).AsBool(),

                MobileContact = settingRepository.Get(ShopSettingType.MobileContact),
                AspectRatioImage = settingRepository.Get(ShopSettingType.AspectRatioImage),
                AsideMenu = settingRepository.Get(ShopSettingType.AsideMenu),
                IsShowProductComment = settingRepository.Get(ShopSettingType.IsShowProductComment).AsBool(),
                IsShowArticleComment = settingRepository.Get(ShopSettingType.IsShowArticleComment).AsBool(),

                ProductPageSize = settingRepository.Get(ShopSettingType.ProductPageSize).AsInt(),
                ArticlePageSize = settingRepository.Get(ShopSettingType.ArticlePageSize).AsInt()
            };

            return View(shopSettingVm);
        }


        [HttpPost]
        public ActionResult ShopSettings(ShopSettingVm shopSettingVm)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }


            settingRepository.Set(ShopSettingType.HomepageMetaTitle, shopSettingVm.HomepageMetaTitle);
            settingRepository.Set(ShopSettingType.HomepageMetaDescription, shopSettingVm.HomepageMetaDescription);
            settingRepository.Set(ShopSettingType.HomepageMetaKeywords, shopSettingVm.HomepageMetaKeywords);
            settingRepository.Set(ShopSettingType.MetaAuthor, shopSettingVm.MetaAuthor);
            settingRepository.Set(ShopSettingType.OgImage, shopSettingVm.OgImage);
            settingRepository.Set(ShopSettingType.Favicon, shopSettingVm.Favicon);

            settingRepository.Set(ShopSettingType.CardDecorate, shopSettingVm.CardDecorate);
            settingRepository.Set(ShopSettingType.IsShowBrief, shopSettingVm.IsShowBrief.ToString());
            settingRepository.Set(ShopSettingType.MenuFormatClass, shopSettingVm.MenuFormatClass);
            settingRepository.Set(ShopSettingType.IsCardCoverObject, shopSettingVm.IsCardCoverObject.ToString());
            settingRepository.Set(ShopSettingType.ProductDetailSummary, shopSettingVm.ProductDetailSummary);
            settingRepository.Set(ShopSettingType.IsUnderConstruction, shopSettingVm.IsUnderConstruction.ToString());
            settingRepository.Set(ShopSettingType.IsHiddenChildMenu, shopSettingVm.IsHiddenChildMenu.ToString());

            settingRepository.Set(ShopSettingType.IsHiddenProductSummary, shopSettingVm.IsHiddenProductSummary.ToString());
            settingRepository.Set(ShopSettingType.IsDisallowCopy, shopSettingVm.IsDisallowCopy.ToString());
            settingRepository.Set(ShopSettingType.IsShowContact, shopSettingVm.IsShowContact.ToString());

            settingRepository.Set(ShopSettingType.MobileContact, shopSettingVm.MobileContact);
            settingRepository.Set(ShopSettingType.AspectRatioImage, shopSettingVm.AspectRatioImage);
            settingRepository.Set(ShopSettingType.AsideMenu, shopSettingVm.AsideMenu);
            settingRepository.Set(ShopSettingType.IsShowProductComment, shopSettingVm.IsShowProductComment.ToString());
            settingRepository.Set(ShopSettingType.IsShowArticleComment, shopSettingVm.IsShowArticleComment.ToString());

            settingRepository.Set(ShopSettingType.ProductPageSize, shopSettingVm.ProductPageSize.ToString());
            settingRepository.Set(ShopSettingType.ArticlePageSize, shopSettingVm.ArticlePageSize.ToString());



            TempData[MessageType.Warning] =
                string.Format(AdminResources.ConfigControllerShopSettings__0___has__been__saved, AdminResources.CommonShopSettings);

            return RedirectToAction("Index");
        }



        public ActionResult TocSettings()
        {
            // check setting already
            settingRepository.InitializationToc();


            var tocVm = new TocVm()
            {
                Id = TocSettingType.Id,
                IsAutoProductTOC = settingRepository.Get(TocSettingType.IsAutoProductTOC).AsBool(),
                IsAutoArticleTOC = settingRepository.Get(TocSettingType.IsAutoArticleTOC).AsBool(),
                TocTitle = settingRepository.Get(TocSettingType.TocTitle),
                IsTocReturn = settingRepository.Get(TocSettingType.IsTocReturn).AsBool()
            };

            return View(tocVm);
        }


        [HttpPost]
        public ActionResult TocSettings(TocVm tocVm)
        {
            if (!ModelState.IsValid)
            {
                TempData[MessageType.Danger] = "some thing wrong";
                return View();
            }

            settingRepository.Set(TocSettingType.IsAutoProductTOC, tocVm.IsAutoProductTOC.ToString());
            settingRepository.Set(TocSettingType.IsAutoArticleTOC, tocVm.IsAutoArticleTOC.ToString());
            settingRepository.Set(TocSettingType.TocTitle, tocVm.TocTitle);
            settingRepository.Set(TocSettingType.IsTocReturn, tocVm.IsTocReturn.ToString());


            TempData[MessageType.Success] = "config saved!";

            return View();
        }




        public ActionResult RefreshTheme()
        {
            DynamicBundles.Clear();

            TempData[MessageType.Success] = "Done";

            return RedirectToAction("Index");
        }


        public ActionResult RestartWebsite(string reason)
        {
            HttpRuntime.UnloadAppDomain();
            return RedirectToAction("Index");
        }
    }

    //public class CustomSearcher
    //{
    //    public static List<string> GetDirectories(string path, string searchPattern = "*",
    //        SearchOption searchOption = SearchOption.AllDirectories)
    //    {
    //        if (searchOption == SearchOption.TopDirectoryOnly)
    //            return Directory.GetDirectories(path, searchPattern).ToList();

    //        var directories = new List<string>(GetDirectories(path, searchPattern));

    //        for (var i = 0; i < directories.Count; i++)
    //            directories.AddRange(GetDirectories(directories[i], searchPattern));

    //        return directories;
    //    }

    //    private static List<string> GetDirectories(string path, string searchPattern)
    //    {
    //        try
    //        {
    //            return Directory.GetDirectories(path, searchPattern).ToList();
    //        }
    //        catch (UnauthorizedAccessException)
    //        {
    //            return new List<string>();
    //        }
    //    }
    //}
}