using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Configuration;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.WebPages;
using System.Xml.Linq;
using Admin.Resources;
using cms4seo.Common.BootstrapHelpers;
using cms4seo.Common.Helpers;
using cms4seo.Common.Permission;
using cms4seo.Data;
using cms4seo.Data.ConnectionString;
using cms4seo.Data.IdentityModels;
using cms4seo.Data.Repositories;
using cms4seo.Model.Entities;
using cms4seo.Model.LekimaxType;
using cms4seo.Model.ViewModel;
using cms4seo.Service.Provider;
using cms4seo.Service.Sitemap;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.SignalR.Infrastructure;

namespace cms4seo.Admin.Controllers
{
    public class SetupController : BaseSetupController
    {

        static string _projectId = WebConfigurationManager.AppSettings["ProjectId"];

        // Enter domain, setup database
        public ActionResult Index()
        {


            if (!ValidSetup)
            {
                TempData[MessageType.Danger] =
                    $"Please edit Property Setup (to \"Setup\":\"True\") in {_projectId}.txt to Access Setup";

                return RedirectToAction("Unauthorized");
            }


            if (Request.Url?.Host.ToLower() != "localhost"
                && !Request.Url.Host.ToLower().EndsWith(".localhost"))
            {
                ViewBag.Mode = "Release Mode";
            }
            else
            {
                ViewBag.Mode = "Local Mode";
            }

            

            return View(new SetupDatabaseVm() {ProjectId = _projectId, TryCreateDatabase = true, ForceSeedAdmin = true});
        }

        [HttpPost]
        public ActionResult Index(SetupDatabaseVm setupDatabaseVm)
        {

            LogHelper.Write("Admin/Setup/Set", "Test Flush Message");

            if (!ValidSetup)
            {
                TempData[MessageType.Danger] =
                    $"Please edit Property Setup (to \"Setup\":\"True\") in {_projectId}.txt to Access Setup";
                return RedirectToAction("Unauthorized");
            }


            //Build(@".\sqlexpress", "lekimax_net_v3_db", "anht31", "234", false);

            if (!ModelState.IsValid)
                return View();


            // Check connection to SQL SERVER, and Errors catcher
            var testConnectionResult = ConnectionStringProvider.TestConnection(setupDatabaseVm);
            LogHelper.Write("Admin/Setup/Set", $"testConnectionResult: {testConnectionResult}");
            if (testConnectionResult != null && testConnectionResult != "0")
            {
                TempData[MessageType.Warning] = testConnectionResult;
                return View();
            }


            // Try Create New Database
            // testConnectionResult == "0" will Database not yet Create
            if (setupDatabaseVm.TryCreateDatabase && testConnectionResult == "0")
            {
                var createDatabaseResult = ConnectionStringProvider.TryCreateDatabase(setupDatabaseVm);

                if (createDatabaseResult != null)
                {
                    TempData[MessageType.Warning] = createDatabaseResult;
                    return View();
                }
            }
            else if (testConnectionResult == "0")
            {
                TempData[MessageType.Warning] = "Database not exist, if you want create Database" +
                                                ", please checked 'Try Create Database' Checkbox";
                return View();
            }


            // set projectId (local mode only)
            if (Request.Url?.Host.ToLower() == "localhost"
                || Request.Url.Host.ToLower().EndsWith(".localhost"))
            {
                ConnectionStringProvider.SetDomain(setupDatabaseVm.ProjectId);
            }


            string setConnectionStringResult = ConnectionStringProvider.Set(setupDatabaseVm);
            if (setConnectionStringResult == null)
            {
                // wait to get appSetting, try 30 times
                int i = 20;
                while (--i > 0 && String.IsNullOrWhiteSpace(ConnectionStringProvider.Get()))
                {
                    // Delay 100ms
                    Thread.Sleep(100);
                }

                // for purpose first run app
                using (var db = new ApplicationDbContext())
                {
                    if (testConnectionResult == "0" || setupDatabaseVm.ForceSeedAdmin)
                    {

                        return RedirectToAction("SeedAdmin");
                    }
                }

                return RedirectToAction("Permission");

            }
            else
            {
                TempData[MessageType.Danger] = setConnectionStringResult;
            }


            return View(setupDatabaseVm);
        }




        public ActionResult SeedAdmin()
        {
            if (!ValidSetup)
            {
                TempData[MessageType.Danger] =
                    $"Please edit Property Setup (to \"Setup\":\"True\") in {_projectId}.txt to Access Setup";
                return RedirectToAction("Unauthorized");
            }


            // check userManager is ready
            var userManager = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();

            if (userManager == null)
            {
                HttpRuntime.UnloadAppDomain();
            }
            

            //return RedirectToAction("SetupStore");

            return View();
        }



        [HttpPost]
        public ActionResult SeedAdmin(RegisterViewModel registerViewModel)
        {
            if (!ValidSetup)
            {
                TempData[MessageType.Danger] =
                    $"Please edit Property Setup (to \"Setup\":\"True\") in {_projectId}.txt to Access Setup";
                return RedirectToAction("Unauthorized");
            }


            if (!ModelState.IsValid)
                return View();

            var result = InitialAdmin(registerViewModel.Email, registerViewModel.Password);


            // valid step 2 - Identity valid (like complex password)
            if(result == "IsNotValid")
                return View();

            // success
            if (result == null)
                return RedirectToAction("Permission");


            TempData[MessageType.Danger] = result;

            return View();
        }



        public ActionResult Permission()
        {
            if (!ValidSetup)
            {
                TempData[MessageType.Danger] =
                    $"Please edit Property Setup (to \"Setup\":\"True\") in {_projectId}.txt to Access Setup";
                return RedirectToAction("Unauthorized");
            }


            // Create folder or file for check Permission
            TempData[MessageType.Success] = PrepareFoldersAndFiles();




            string ProjectId = WebConfigurationManager.AppSettings["ProjectId"];

            string pathConnectionString = Path.Combine(
                AppDomain.CurrentDomain.GetData("DataDirectory").ToString(), $"{ProjectId}.txt");

            string pathRedirectFile = Path.Combine(
                AppDomain.CurrentDomain.GetData("DataDirectory").ToString(), "301.csv");

            string pathPhotoFolder = HostingEnvironment.MapPath("/Uploads");

            string pathSitemapFile = HostingEnvironment.MapPath("/sitemap.xml");

            string pathPlugins = HostingEnvironment.MapPath("/Plugins");

            //return RedirectToAction("Done");

            var permissionVm = new PermissionVm
            {
                HasWriteConnectionStringFile = pathConnectionString.HasWritePermission(),
                HasWriteRedirectFile = pathRedirectFile.HasWritePermission(),
                HasWritePhotoFolder = pathPhotoFolder.HasWritePermission(),
                HasWriteSitemapFile = pathSitemapFile.HasWritePermission(),
                HasWritePluginsFolder = pathPlugins.HasWritePermission()
            };


            return View(permissionVm);
        }



        private string PrepareFoldersAndFiles()
        {

            string photoPath = Server.MapPath("/Uploads");
            string sitemapPath = Server.MapPath("/sitemap.xml");

            string logPath = Server.MapPath("/Events.log");
            string pathRedirectFile = Path.Combine(
                AppDomain.CurrentDomain.GetData("DataDirectory").ToString(), "301.csv");


            if (!Directory.Exists(photoPath))
                Directory.CreateDirectory(photoPath);


            if (!System.IO.File.Exists(sitemapPath))
            {

                #region reindex sitemap

                string url = Request.Url?.AbsoluteUri;
                var service = new SiteMapService(url);
                var siteMap = service.GetSiteMap();

                service.SaveSiteMap(siteMap);

                LogHelper.Write(@"Admin/Setup",
                    $"User: {User.Identity.Name} re-indexed sitemap.xml, total items: [{siteMap.Items.Count}]");


                #endregion
            }
            

            if (!System.IO.File.Exists(logPath))
                System.IO.File.Create(logPath);


            if (!System.IO.File.Exists(pathRedirectFile))
                System.IO.File.Create(pathRedirectFile);

            // wait to 
            int i = 30;

            // The while loop loops through a block of code as long as a specified condition is True
            while (--i > 0
                   && (!Directory.Exists(photoPath)
                       || !System.IO.File.Exists(sitemapPath)
                       || !System.IO.File.Exists(logPath)
                       || !System.IO.File.Exists(pathRedirectFile)
                       ))
            {

                // Delay 100ms
                Thread.Sleep(100);
            }

            //return $"Done when i:{i}";

            return $"Done";
        }



        private bool ValidSetup
        {
            get
            {
                return ConnectionStringProvider.IsSetup;
            }
            set
            {
                if (!value)
                {
                    ConnectionStringProvider.FinishSetup();
                }
                
            }
        }




        public ActionResult SetupStore()
        {

            if (!ValidSetup)
            {
                TempData[MessageType.Danger] =
                    $"Please edit Property Setup (to \"Setup\":\"True\") in {_projectId}.txt to Access Setup";
                return RedirectToAction("Unauthorized");
            }



            #region setup settingRepositoy

            var settingRepository = new SettingRepository();

            settingRepository.InitializationShop();
            settingRepository.InitializationEmail();
            settingRepository.InitializationPhoto();
            settingRepository.InitializationWebSetting();

            //settingRepository.InitializationEmbed();

            #endregion




            #region Sample

            string samplePath = Server.MapPath("/Sample");

            var directories = Directory.GetDirectories(samplePath).Select(Path.GetFileName);

            //var sampleList = directories.ToDictionary(item => item.Trim(), item => item.Trim());

            var sampleList = new Dictionary<string, string> {{"","-- No --" } };
            var sampleList2 = directories.ToDictionary(item => item.Trim(), item => item.Trim());

            foreach (var item in sampleList2)
                sampleList.Add(item.Key, item.Value);


            // Theme
            string themesPath = Server.MapPath("/Themes");
            var themeList = Directory.GetDirectories(themesPath).Select(Path.GetFileName);

            #endregion


            var setupStoreVm = new SetupStoreVm()
            {
                SampleList = sampleList,
                ThemeList = themeList.ToDictionary(item => item.Trim(), item => item.Trim())
            };
            
            return View(setupStoreVm);
        }

        [HttpPost]
        public ActionResult SetupStore(SetupStoreVm setupStoreVm)
        {
            if (!ValidSetup)
            {
                TempData[MessageType.Danger] =
                    $"Please edit Property Setup (to \"Setup\":\"True\") in {_projectId}.txt to Access Setup";
                return RedirectToAction("Unauthorized");
            }


            #region Reseed Sample
            if (setupStoreVm.SampleList == null)
            {
                string samplePath = Server.MapPath("/Sample");

                var directories = Directory.GetDirectories(samplePath).Select(Path.GetFileName);

                //var sampleList = directories.ToDictionary(item => item.Trim(), item => item.Trim());

                var sampleList = new Dictionary<string, string> { { "", "-- No --" } };
                var sampleList2 = directories.ToDictionary(item => item.Trim(), item => item.Trim());

                foreach (var item in sampleList2)
                    sampleList.Add(item.Key, item.Value);


                setupStoreVm.SampleList = sampleList;

            }
            #endregion


            //return RedirectToAction("Done");
            if (!ModelState.IsValid)
            {
                return View(setupStoreVm);
            }


            var adminLanguage = "en-US";

            var settingRepository = new SettingRepository();

            if (!string.IsNullOrWhiteSpace(setupStoreVm.Country)
                && setupStoreVm.Country.ToLower().Contains("vi"))
                adminLanguage = "vi-VN";

            settingRepository.Set(WebSettingType.AdminLanguages, adminLanguage);
            settingRepository.Set(WebSettingType.ShopLanguages, setupStoreVm.Country);

            settingRepository.Set(WebSettingType.CurrentTheme, setupStoreVm.Theme);



            #region Sample database

            if (!string.IsNullOrWhiteSpace(setupStoreVm.CurrentSample))
            {
                var result = Seed(setupStoreVm.CurrentSample);
                if (result != null)
                {
                    // force secure
                    if (ValidSetup)
                        ValidSetup = false;


                    TempData[MessageType.Danger] += result;


                    #region Sample

                    string samplePath = Server.MapPath("/Sample");

                    var directories = Directory.GetDirectories(samplePath).Select(Path.GetFileName);

                    //var sampleList = directories.ToDictionary(item => item.Trim(), item => item.Trim());

                    var sampleList = new Dictionary<string, string> { { "", "-- No --" } };
                    var sampleList2 = directories.ToDictionary(item => item.Trim(), item => item.Trim());

                    foreach (var item in sampleList2)
                        sampleList.Add(item.Key, item.Value);


                    // Theme
                    string themesPath = Server.MapPath("/Themes");
                    var themeList = Directory.GetDirectories(themesPath).Select(Path.GetFileName);

                    #endregion


                    setupStoreVm.SampleList = sampleList;
                    setupStoreVm.ThemeList = themeList.ToDictionary(item => item.Trim(), item => item.Trim());
                    return View(setupStoreVm);
                }
                    
            }


            #endregion






            //#region setup Contents

            //// source
            //IEnumerable<XElement> xElements = Setting.AppSettings.All;

            //// destination
            //var contentRepository = new ContentRepository();


            //// Mapping
            //List<Content> contents = xElements.Select(x => new Content
            //{
            //    Key = x.Attribute("key")?.Value,
            //    Value = x.Attribute("value")?.Value
            //}).ToList();


            //// save
            //contentRepository.AddRange(contents);

            //#endregion




            //TempData[MessageType.Warning] = "Shop Setup done!";

            return RedirectToAction("Done");
        }


        public ActionResult Done()
        {
            //if (!ValidSetup)
            //{
            //    TempData[MessageType.Danger] =
            //        $"Please edit Property Setup (to \"Setup\":\"True\") in {_projectId}.txt to Access Setup";
            //    return RedirectToAction("Unauthorized");
            //}

            string connectionFilePath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory, $@"App_Data\{_projectId}.txt");



            if (System.IO.File.Exists(connectionFilePath))
                ValidSetup = false; // disable setup


            // force reset app
            HttpRuntime.UnloadAppDomain();

            return View();
        }


        public ActionResult Unauthorized()
        {
            return View();
        }


        /// <summary>
        /// Set domain for Developer
        /// </summary>
        /// <param name="id">domain like example.com</param>
        /// <returns>Redirect to Done ViewResult</returns>
        public ActionResult Domain(string id)
        {
            if (Request.Url?.Host.ToLower() != "localhost"
                && !Request.Url.Host.ToLower().EndsWith(".localhost"))
            {
                TempData[MessageType.Danger] =
                    "This function for Develop Time only " +
                    "(if you want to change domain, please change it in Web.config file, in ProjectId section";

                return View();
            }

            if (ConnectionStringProvider.SetDomain(id))
            {
                // force reset app
                HttpRuntime.UnloadAppDomain();

                TempData[MessageType.Info] = "Domain has been set";
                return View();
            }

            TempData[MessageType.Info] = "Some thing wrong";
            return View();
        }




        private string InitialAdmin(string name, string password)
        {
            using (var db = new ApplicationDbContext())
            {
                // Access Entity to init ApplicationDbInitializer
                if (db.Users.Count() >= 0)
                {
                    // use owin
                    var userManager = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
                    var roleManager = System.Web.HttpContext.Current.GetOwinContext().Get<ApplicationRoleManager>();

                    //const string name = "admin@example.com";
                    //const string password = "Admin@45632161";

                    LogHelper.Write("/Setup/InitialAdmin", "Test InitialAdmin: 2");

                    if (userManager == null || roleManager == null)
                    {
                        return "userManager or roleManager is null";
                    }





                    var user = userManager.FindByName(name);

                    if (user != null)
                        return "This Email (of user) has been in system, please enter another Email.";


                    user = new ApplicationUser { UserName = name, Email = name, EmailConfirmed = true };
                    var result = userManager.Create(user, password);

                    if (result.Succeeded)
                        userManager.SetLockoutEnabled(user.Id, false); // bypass ConfirmEmail
                    else
                    {
                        AddErrors(result);
                        return "IsNotValid";
                    }





                    string[] roleListBasic =
                    {
                        // Basic
                        "Basic.Article",
                        "Basic.ArticleTag",
                        "Basic.Category",
                        "Basic.Contact",
                        "Basic.Common",
                        "Basic.PhotosApi",
                        "Basic.Product",
                        "Basic.ProductTag",
                        "Basic.Slider",
                        "Basic.SystemInfo",
                        "Basic.Tag",
                        "Basic.Topic",
                        "Basic.Upload"
                    };



                    string[] roleListMedium =
                    {

                        // Medium
                        "Medium.Config",
                        "Medium.ExtraSiteMaps",
                        "Medium.GroupsAdmin",
                        "Medium.HitCounters",
                        "Medium.Info",
                        "Medium.InjectionHyperlinks",
                        "Medium.Logs",
                        "Medium.Permalinks",
                        "Medium.Photos",
                        "Medium.Redirects",
                        "Medium.SearchOptimize",
                        "Medium.UsersAdmin",
                        "Medium.Contents"
                    };



                    string[] roleListAdvance =
                    {
                        // Advance
                        "Advance.Migration",
                        "Advance.RolesAdmin",
                        "Advance.Developer"
                    };



                    string[] roleList = roleListBasic
                        .Concat(roleListMedium)
                        .Concat(roleListAdvance)
                        .ToArray();



                    // Add role to Store
                    foreach (var role in roleList)
                    {
                        roleManager.Create(new ApplicationRole(role));
                    }


                    
                        


                    var groupManager = new ApplicationGroupManager();

                    if (!groupManager.Groups.Any(x => x.Name == "Developers"))
                    {
                        var developerGroup = new ApplicationGroup("Developers", "No Limit");
                        groupManager.CreateGroup(developerGroup);
                        // Attach roleList Developer
                        groupManager.SetGroupRoles(developerGroup.Id, roleList);
                    }

                    if (!groupManager.Groups.Any(x => x.Name == "Admins"))
                    {
                        var adminGroup = new ApplicationGroup("Admins", "Full Access to All (except of some Developer Role)");
                        groupManager.CreateGroup(adminGroup);
                        // Attach roleList Admin
                        groupManager.SetGroupRoles(adminGroup.Id, roleListBasic.Concat(roleListMedium).ToArray());
                    }

                    if (!groupManager.Groups.Any(x => x.Name == "Editors"))
                    {
                        var editorGroup = new ApplicationGroup("Editors", "Can Edit Product, Article, Infos");
                        groupManager.CreateGroup(editorGroup);
                        // Attach roleList Editor
                        groupManager.SetGroupRoles(editorGroup.Id, roleListBasic);
                    }


                    // SET USER GROUP
                    var group = groupManager.Groups.FirstOrDefault(x => x.Name == "Developers");
                    if (group != null)
                        groupManager.SetUserGroups(user.Id, new string[] { group.Id });



                    return null;

                }

            }


            return null;

        }


        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }


        private string Seed(string sample)
        {
            
            string uploadPath = Server.MapPath("/Uploads");
            string samplePath = Server.MapPath("/Sample/" + sample);

            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            if (!Directory.Exists(samplePath))
                return $"Folder {samplePath} not exist";


            // seed sql
            try
            {
                ExecuteScript(sample);
                //ExecuteSqlFile(samplePath + "\\en.sql");
            }
            catch (Exception e)
            {
                LogHelper.Write("/SetupStore -> Seed Sql script", e.Message);
                return e.Message;
            }


            // seed photo
            try
            {
                Copy($"{samplePath}\\Photo", $"{uploadPath}\\{sample}\\Photo");
            }
            catch (Exception e)
            {
                LogHelper.Write("/SetupStore -> Seed Photo", e.Message);
                return e.Message;
            }
            

            // success
            return null;

        }

        private void ExecuteScript(string sampleName)
        {
            string samplePath = Server.MapPath("/Sample/" + sampleName +"/en.sql");

            string script = System.IO.File.ReadAllText(samplePath);

            // remove USE database
            var safeScript = Regex.Replace(script, @"USE \[.*\]", "", RegexOptions.Multiline | RegexOptions.IgnoreCase);


            // remove GO command (just GO between two IDENTITY_INSERT on/off)
            var pattern = @"(^INSERT+\s\[dbo\]\.\[(?!_).*\r\n)^GO(\r\n^INSERT+\s\[dbo\]\.\[(?!_))";
            var replacement = "$1$2";
            var statements = Regex.Replace(safeScript, pattern, replacement, RegexOptions.Multiline | RegexOptions.IgnoreCase);


            // remapping Upload path
            var statementsRemap = Regex.Replace(statements, 
                @"\/Uploads\/[a-zA-Z'-]+\/Photo\/", 
                $@"/Uploads/{sampleName}/Photo/", 
                RegexOptions.Multiline | RegexOptions.IgnoreCase);


            // split script on GO command
            IEnumerable<string> commandStrings = Regex.Split(statementsRemap, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase);

            using (var db = new ApplicationDbContext())
            {

                foreach (string commandString in commandStrings)
                {
                    if (!string.IsNullOrWhiteSpace(commandString.Trim()))
                    {
                        db.Database.ExecuteSqlCommand(commandString);
                    }
                }
            }
        }


        //private void ExecuteSqlFile(string path)
        //{
        //    var statements = new List<string>();

        //    using (var stream = System.IO.File.OpenRead(path))
        //    using (var reader = new StreamReader(stream))
        //    {
        //        string statement;
        //        while ((statement = ReadNextStatementFromStream(reader)) != null)
        //            statements.Add(statement);
        //    }

        //    using (var db = new ApplicationDbContext())
        //    {
        //        foreach (string stmt in statements)
        //            db.Database.ExecuteSqlCommand(stmt);
        //    }
        //}


        //private string ReadNextStatementFromStream(StreamReader reader)
        //{
        //    var sb = new StringBuilder();
        //    while (true)
        //    {
        //        var lineOfText = reader.ReadLine();
        //        if (lineOfText == null)
        //        {
        //            if (sb.Length > 0)
        //                return sb.ToString();

        //            return null;
        //        }

        //        if (lineOfText.TrimEnd().ToUpper() == "GO")
        //        {

        //            break;
        //        }
                    

        //        sb.Append(lineOfText + Environment.NewLine);
        //    }

        //    return sb.ToString();
        //}


        void Copy(string sourceDir, string targetDir)
        {
            if(!Directory.Exists(targetDir))
                Directory.CreateDirectory(targetDir);

            foreach (var file in Directory.GetFiles(sourceDir))
                if (!System.IO.File.Exists(Path.Combine(targetDir, Path.GetFileName(file))))
                    System.IO.File.Copy(file, Path.Combine(targetDir, Path.GetFileName(file)));

            foreach (var directory in Directory.GetDirectories(sourceDir))
                if (!Directory.Exists(Path.Combine(targetDir, Path.GetFileName(directory))))
                    Copy(directory, Path.Combine(targetDir, Path.GetFileName(directory)));
        }

    }
}