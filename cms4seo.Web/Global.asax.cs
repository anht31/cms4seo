using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using cms4seo.Common.Culture;
using cms4seo.Common.Helpers;
using cms4seo.Common.Plugins;
using cms4seo.Data;
using cms4seo.Data.ConnectionString;
using cms4seo.Model.Entities;
using cms4seo.Web;
using cms4seo.Model.LekimaxType;
using cms4seo.Service.Binders;
using cms4seo.Service.Provider;
using cms4seo.Service.Seo;
using cms4seo.Service.Themeable;

namespace IdentitySample
{
    // Note: For instructions on enabling IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=301868
    public class MvcApplication : HttpApplication
    {
        private readonly string connectionString = ConnectionStringProvider.Get();

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // register admin
            //AuthDbConfig.RegisterAdmin();


            if (connectionString == null)
                return;

            //remove all view engines
            ViewEngines.Engines.Clear();
            //except the themeable razor view engine we use
            ViewEngines.Engines.Add(new ThemeableRazorViewEngine());



            // binding cart 
            ModelBinders.Binders.Add(typeof(Cart), new CartModelBinder());

            // binding product
            ModelBinders.Binders.Add(typeof(Product), new ProductModelBinder());

            // binding article
            ModelBinders.Binders.Add(typeof(Article), new ArticleModelBinder());

            //ResetOnlineStatus();


            // ApplicationDbInitializer run first (before sql adapter)
            //SeedAdmin.Initialize();


            // See all View on working -> to log
            //ViewEngines.Engines.Clear();
            //ViewEngines.Engines.Add(new LoggingRazorViewEngine());

            // online counter
            Application.Lock();
            Application["visitasOnline"] = 0;
            Application.UnLock();


            // Clear old record

            AutoDeleteOldRecord();

            LogHelper.Write("Application_Start", "try delete old log row");
            try
            {
                AutoDeleteOldEventLog();
            }
            catch (Exception)
            {
                // ignored
            }

            LogHelper.Write("Application_Start", "try turn on Trace");
        }



        // default-culture =======================================================

        protected void Application_BeginRequest()
        {

            // for setup mode
            if (ConnectionStringProvider.Get() == null)
            {

                if (!Request.FilePath.ToLower().Contains("/admin/setup"))
                    Response.Redirect("/Admin/Setup");

                return;
            }


            var shopLanguage = Setting.WebSettings[WebSettingType.ShopLanguages];

            // culture for Shop
            if (!string.IsNullOrWhiteSpace(shopLanguage))
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo(shopLanguage);
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(shopLanguage);
            }

            // Admin areas set culture by BaseController (cookie override WebSettingType.AdminLanguages)




            var url = Request.RawUrl.TrimEnd('/');
            if (!url.ToLower().Contains("/admin"))
            {

                // 301.csv
                if (Setting.WebSettings[WebSettingType.RedirectMode] == "2")
                {
                    var redirect = FlatFileAccess.Read301CSV();

                    if (redirect != null && redirect.Keys.Contains(url))
                    {
                        // log
                        LogHelper.Write("Redirect", $" {url} [->] {redirect[url]}");

                        // Clear the error
                        Response.Clear();
                        Server.ClearError();

                        string topLevelDomain = Request.Url.Host.Replace("www.", "");

                        if (redirect[url].ToLower().Contains("http") ||
                            redirect[url].ToLower().Contains(topLevelDomain))
                        {
                            // 301 it to the new url
                            Response.RedirectPermanent(redirect[url]);
                        }
                        else
                        {
                            Response.RedirectPermanent($"https://{topLevelDomain}{redirect[url]}");
                        }

                        return;
                    }


#if (!DEBUG)

                    // Redirect to https non-www
                    if (Request.Url.Host.ToLower().Contains("www"))
                    {

                        string topLevelDomain = Request.Url.Host.Replace("www.", "");

                        var redirectLink = $"https://{topLevelDomain}{Request.Url.PathAndQuery}";

                        Response.RedirectPermanent(redirectLink);
                        return;
                    }




                    // Redirect to Https
                    if (Request.Url.Scheme == "http")
                    {

                        var redirectLink = $"https://{Request.Url.Host}{Request.Url.PathAndQuery}";

                        Response.RedirectPermanent(redirectLink);
                        return;
                    }

#endif

                }
            }


            
            

        }

        //protected void Application_AuthenticateRequest()
        //{
        //    // cache off when authenticate
        //    if (User != null && User.IsInRole("Medium.Contents"))
        //    {
        //        if (CMS.CacheDuration != 0)
        //        {
        //            CMS.CacheDuration = 0;
        //            HttpResponse.RemoveOutputCacheItem("/");
        //        }
        //    }
        //    else
        //    {
        //        // default cache
        //        CMS.CacheDuration = 3600;
        //    }
        //}

        // error when run iis7.5
        protected void Application_AcquireRequestState()
        {
            try
            {
                if (!IsCrawler())
                {
                    var hitCounter = Session["HitCounter"] as HitCounter ?? new HitCounter();

                    hitCounter.AccessTime = DateTime.Now;
                    hitCounter.Interval = Math.Round(
                        DateTime.Now.Subtract(hitCounter.OpenTime).TotalMinutes, 1);

                    hitCounter.IsOnline = true;
                    UpdateHitCounter(hitCounter);

                    Session["HitCounter"] = hitCounter;
                }
            }
            catch
            {
                // ignored
            }
        }


        protected void Application_EndRequest()
        {
        }


        protected void Session_Start()
        {

            // Check setup
            if (connectionString == null)
            {
                return;
            }


            Session["IsClient"] = false;

            if (!IsCrawler())
            {
                // online counter
                Application.Lock();
                var visitasOnline = (int)Application["visitasOnline"];
                visitasOnline++;
                Application["visitasOnline"] = visitasOnline;
                Application.UnLock();


                var hcid = TrackCookieAndCountUser();

                //Session["HitCounter"] = new HitCounter();
                var hitCounter = Session["HitCounter"] as HitCounter ?? new HitCounter();
                hitCounter.UserGuid = hcid;
                hitCounter.AspNetSessionId = Session.SessionID;
                hitCounter.OpenTime = DateTime.Now;
                hitCounter.IsOnline = true;
                hitCounter.IsClient = true;
                hitCounter.IpAddress = Request.ServerVariables["REMOTE_ADDR"];
                hitCounter.UserAgent = Request.UserAgent;
                hitCounter.IsCrawlerMedium = false;
                hitCounter.IsCrawlerParanoid = false;
                hitCounter.CountryName = "http://whatismyipaddress.com/ip/" + Request.ServerVariables["REMOTE_ADDR"];
                Session["HitCounter"] = hitCounter;

                AddHitCounter(hitCounter);
                hitCounter.CounterId = GetLastCounterId();

                if ((bool)Session["IsNewUser"])
                {
                    // and new user
                    CountUser(hitCounter.UserGuid);
                }

                Session["IsClient"] = true;
            }
        }


        protected void Session_End()
        {



            if ((bool)Session["IsClient"])
            {

                // online counter
                Application.Lock();
                var visitasOnline = (int)Application["visitasOnline"];
                visitasOnline--;
                Application["visitasOnline"] = visitasOnline > 0 ? visitasOnline : 0;
                Application.UnLock();


                var hitCounter = Session["HitCounter"] as HitCounter ?? new HitCounter();
                if (hitCounter.IsClient)
                {
                    hitCounter.CloseTime = DateTime.Now;
                    hitCounter.IsOnline = false;
                    UpdateHitCounter(hitCounter);
                }



                // it is impossible to create or access cookies in the Session_end.  
            }
        }



        protected void Application_End()
        {
            //ResetOnlineStatus();
        }





        #region cheking ===============================================================

        protected void Application_PostAcquireRequestState()
        {
        }

        protected void Application_PreRequestHandlerExecute()
        {
        }

        protected void Application_PostRequestHandlerExecute()
        {
        }

        #endregion

        #region Helper ====================================================================

        private bool IsCrawler()
        {
            return Regex.IsMatch(Request.UserAgent,
                @"bot|crawler|baiduspider|facebookexternalhit|80legs|ia_archiver|voyager|curl|wget|yahoo! slurp|mediapartners-google",
                RegexOptions.IgnoreCase);
        }


        //private bool IsCrawlerMedium()
        //{
        //    List<string> crawlers3 = new List<string>()
        //    {
        //        "bot","crawler","spider","80legs","baidu","yahoo! slurp","ia_archiver","mediapartners-google",
        //        "lwp-trivial","nederland.zoek","ahoy","anthill","appie","arale","araneo","ariadne",
        //        "atn_worldwide","atomz","bjaaland","ukonline","calif","combine","cosmos","cusco",
        //        "cyberspyder","digger","grabber","downloadexpress","ecollector","ebiness","esculapio",
        //        "esther","felix ide","hamahakki","kit-fireball","fouineur","freecrawl","desertrealm",
        //        "gcreep","golem","griffon","gromit","gulliver","gulper","whowhere","havindex","hotwired",
        //        "htdig","ingrid","informant","inspectorwww","iron33","teoma","ask jeeves","jeeves",
        //        "image.kapsi.net","kdd-explorer","label-grabber","larbin","linkidator","linkwalker",
        //        "lockon","marvin","mattie","mediafox","merzscope","nec-meshexplorer","udmsearch","moget",
        //        "motor","muncher","muninn","muscatferret","mwdsearch","sharp-info-agent","webmechanic",
        //        "netscoop","newscan-online","objectssearch","orbsearch","packrat","pageboy","parasite",
        //        "patric","pegasus","phpdig","piltdownman","pimptrain","plumtreewebaccessor","getterrobo-plus",
        //        "raven","roadrunner","robbie","robocrawl","robofox","webbandit","scooter","search-au",
        //        "searchprocess","senrigan","shagseeker","site valet","skymob","slurp","snooper","speedy",
        //        "curl_image_client","suke","www.sygol.com","tach_bw","templeton","titin","topiclink","udmsearch",
        //        "urlck","valkyrie libwww-perl","verticrawl","victoria","webscout","voyager","crawlpaper",
        //        "webcatcher","t-h-u-n-d-e-r-s-t-o-n-e","webmoose","pagesinventory","webquest","webreaper",
        //        "webwalker","winona","occam","robi","fdse","jobo","rhcs","gazz","dwcp","yeti","fido","wlm",
        //        "wolp","wwwc","xget","legs","curl","webs","wget","sift","cmc"
        //    };
        //    string ua = Request.UserAgent.ToLower();
        //    return crawlers3.Exists(x => ua.Contains(x));
        //}


        //private bool IsCrawlerParanoid()
        //{
        //    // crawlers that have 'bot' in their useragent
        //    List<string> crawlers1 = new List<string>()
        //    {
        //        "googlebot","bingbot","yandexbot","ahrefsbot","msnbot","linkedinbot","exabot","compspybot",
        //        "yesupbot","paperlibot","tweetmemebot","semrushbot","gigabot","voilabot","adsbot-google",
        //        "botlink","alkalinebot","araybot","undrip bot","borg-bot","boxseabot","yodaobot","admedia bot",
        //        "ezooms.bot","confuzzledbot","coolbot","internet cruiser robot","yolinkbot","diibot","musobot",
        //        "dragonbot","elfinbot","wikiobot","twitterbot","contextad bot","hambot","iajabot","news bot",
        //        "irobot","socialradarbot","ko_yappo_robot","skimbot","psbot","rixbot","seznambot","careerbot",
        //        "simbot","solbot","mail.ru_bot","spiderbot","blekkobot","bitlybot","techbot","void-bot",
        //        "vwbot_k","diffbot","friendfeedbot","archive.org_bot","woriobot","crystalsemanticsbot","wepbot",
        //        "spbot","tweetedtimes bot","mj12bot","who.is bot","psbot","robot","jbot","bbot","bot"
        //    };

        //    // crawlers that don't have 'bot' in their useragent
        //    List<string> crawlers2 = new List<string>()
        //    {
        //        "baiduspider","80legs","baidu","yahoo! slurp","ia_archiver","mediapartners-google","lwp-trivial",
        //        "nederland.zoek","ahoy","anthill","appie","arale","araneo","ariadne","atn_worldwide","atomz",
        //        "bjaaland","ukonline","bspider","calif","christcrawler","combine","cosmos","cusco","cyberspyder",
        //        "cydralspider","digger","grabber","downloadexpress","ecollector","ebiness","esculapio","esther",
        //        "fastcrawler","felix ide","hamahakki","kit-fireball","fouineur","freecrawl","desertrealm",
        //        "gammaspider","gcreep","golem","griffon","gromit","gulliver","gulper","whowhere","portalbspider",
        //        "havindex","hotwired","htdig","ingrid","informant","infospiders","inspectorwww","iron33",
        //        "jcrawler","teoma","ask jeeves","jeeves","image.kapsi.net","kdd-explorer","label-grabber",
        //        "larbin","linkidator","linkwalker","lockon","logo_gif_crawler","marvin","mattie","mediafox",
        //        "merzscope","nec-meshexplorer","mindcrawler","udmsearch","moget","motor","muncher","muninn",
        //        "muscatferret","mwdsearch","sharp-info-agent","webmechanic","netscoop","newscan-online",
        //        "objectssearch","orbsearch","packrat","pageboy","parasite","patric","pegasus","perlcrawler",
        //        "phpdig","piltdownman","pimptrain","pjspider","plumtreewebaccessor","getterrobo-plus","raven",
        //        "roadrunner","robbie","robocrawl","robofox","webbandit","scooter","search-au","searchprocess",
        //        "senrigan","shagseeker","site valet","skymob","slcrawler","slurp","snooper","speedy",
        //        "spider_monkey","spiderline","curl_image_client","suke","www.sygol.com","tach_bw","templeton",
        //        "titin","topiclink","udmsearch","urlck","valkyrie libwww-perl","verticrawl","victoria",
        //        "webscout","voyager","crawlpaper","wapspider","webcatcher","t-h-u-n-d-e-r-s-t-o-n-e",
        //        "webmoose","pagesinventory","webquest","webreaper","webspider","webwalker","winona","occam",
        //        "robi","fdse","jobo","rhcs","gazz","dwcp","yeti","crawler","fido","wlm","wolp","wwwc","xget",
        //        "legs","curl","webs","wget","sift","cmc"
        //    };

        //    string ua = Request.UserAgent.ToLower();
        //    string match = null;

        //    if (ua.Contains("bot")) match = crawlers1.FirstOrDefault(x => ua.Contains(x));
        //    else match = crawlers2.FirstOrDefault(x => ua.Contains(x));

        //    //if (match != null && match.Length < 5)
        //    //    Log("Possible new crawler found: ", ua);

        //    bool iscrawler = match != null;

        //    return iscrawler;
        //}


        private void UpdateHitCounter(HitCounter hitCounter)
        {
            using (var con = new SqlConnection(connectionString))
            {
                var com = new SqlCommand("update HitCounters set " +
                                         "AccessTime=@AccessTime, " +
                                         "CloseTime=@CloseTime, " +
                                         "Interval=@Interval, " +
                                         "IsOnline=@IsOnline " +
                                         "where CounterId=@CounterId", con);

                com.CommandType = CommandType.Text;
                com.Parameters.AddWithValue("@AccessTime", (object)hitCounter.AccessTime ?? DBNull.Value);
                com.Parameters.AddWithValue("@CloseTime", (object)hitCounter.CloseTime ?? DBNull.Value);
                com.Parameters.AddWithValue("@Interval", hitCounter.Interval);
                com.Parameters.AddWithValue("@IsOnline", hitCounter.IsOnline);
                com.Parameters.AddWithValue("@CounterId", hitCounter.CounterId);

                con.Open();
                com.ExecuteNonQuery();
                con.Close();
            }
        }


        private string GetRootDomain()
        {
            var temp = Request.Url.Host.Split('.').Reverse().ToList();

            if (temp.Count > 1)
                return temp[1] + "." + temp[0];

            return "local.com";
        }


        private void AddHitCounter(HitCounter hitCounter)
        {
            using (var con = new SqlConnection(connectionString))
            {
                var com = new SqlCommand("insert into HitCounters " +
                                         "(UserGuid, AspNetSessionId, OpenTime, Interval, IsOnline, IsClient, IpAddress, UserAgent, IsCrawlerMedium, IsCrawlerParanoid, CountryName) " +
                                         "values (@UserGuid, @AspNetSessionId, @OpenTime, @Interval, @IsOnline, @IsClient, @IpAddress, @UserAgent, @IsCrawlerMedium, @IsCrawlerParanoid, @CountryName)",
                    con);

                com.CommandType = CommandType.Text;
                com.Parameters.AddWithValue("@UserGuid", hitCounter.UserGuid);
                com.Parameters.AddWithValue("@AspNetSessionId", hitCounter.AspNetSessionId);
                com.Parameters.AddWithValue("@OpenTime", hitCounter.OpenTime);
                com.Parameters.AddWithValue("@Interval", hitCounter.Interval);
                com.Parameters.AddWithValue("@IsOnline", hitCounter.IsOnline);
                com.Parameters.AddWithValue("@IsClient", hitCounter.IsClient);
                com.Parameters.AddWithValue("@IpAddress", hitCounter.IpAddress);
                com.Parameters.AddWithValue("@UserAgent", hitCounter.UserAgent);
                com.Parameters.AddWithValue("@IsCrawlerMedium", hitCounter.IsCrawlerMedium);
                com.Parameters.AddWithValue("@IsCrawlerParanoid", hitCounter.IsCrawlerParanoid);
                com.Parameters.AddWithValue("@CountryName", hitCounter.CountryName);

                con.Open();
                com.ExecuteNonQuery();
                con.Close();
            }
        }


        private string TrackCookieAndCountUser()
        {
            Session["IsNewUser"] = false;

            string hcid;
            if (Request.Cookies["_hcid"] != null)
                hcid = Request.Cookies["_hcid"].Value;
            else
            {
                hcid = SetCookie("_hcid", Guid.NewGuid().ToString(), 730);

                Session["IsNewUser"] = true;
            }
            return hcid;
        }


        private void CountUser(string hcid)
        {
            using (var con = new SqlConnection(connectionString))
            {
                var com = new SqlCommand("insert into UserCounters " +
                                         "(UserGuid, OpenTime) " +
                                         "values (@UserGuid, @OpenTime)", con);

                com.CommandType = CommandType.Text;
                com.Parameters.AddWithValue("@UserGuid", hcid);
                com.Parameters.AddWithValue("@OpenTime", DateTime.Now);

                con.Open();
                com.ExecuteNonQuery();
                con.Close();
            }
        }


        private string SetCookie(string name, string value, int expireDate = 30)
        {
            var myCookie = new HttpCookie(name, value);
            myCookie.Expires = DateTime.Now.AddDays(expireDate);
            myCookie.Path = "/";

            //bug (when run on localhost)
            //myCookie.Domain = "." + GetRootDomain();
            HttpContext.Current.Response.Cookies.Set(myCookie);
            return value;
        }


        private void AutoDeleteOldRecord()
        {
            var rowCount = CountTotalRecord();

            if (rowCount > 2500)
                TrimTopRecord(500);
        }

        private void TrimTopRecord(int records)
        {
            using (var con = new SqlConnection(connectionString))
            {
                var com = new SqlCommand("DELETE TOP (" + records + ") FROM HitCounters", con);

                con.Open();
                com.ExecuteNonQuery();
                con.Close();
            }
        }

        private void AutoDeleteOldEventLog()
        {
            var mapPathEventLog = System.Web.Hosting.HostingEnvironment.MapPath("~/Events.log");

            if (mapPathEventLog == null)
                return;

            System.Diagnostics.Trace.Close();
            var lineCount = File.ReadLines(mapPathEventLog).Count();

            // runtime
            if (lineCount > 3000)
            {
                TrimFile(mapPathEventLog, lineCount - 2000, lineCount + 1);
            }
        }

        /// <summary>
        /// Trim file, Keep rows from start to end
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="start">start row segment to begin</param>
        /// <param name="end">end row segment to End</param>
        public static void TrimFile(string fileName, int start, int end)
        {
            //File.WriteAllLines(fileName,
            //    File.ReadAllLines(fileName)
            //        .Skip(start - 1).Take(end - start));

            List<string> lines = new List<string>();

            System.Diagnostics.Trace.Close();
            using (var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (StreamReader streamReader = new StreamReader(fileStream, Encoding.Default))
            {
                string line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    lines.Add(line);
                }
            }

            string linesAfterTrim = string.Join("\n", lines.Skip(start - 1).Take(end - start));

            System.Diagnostics.Trace.Close();
            using (StreamWriter streamWriter = new StreamWriter(fileName, false))
            {
                streamWriter.Write(linesAfterTrim);
            }

        }


        private int CountTotalRecord()
        {
            using (var con = new SqlConnection(connectionString))
            {
                var com = new SqlCommand("SELECT COUNT(*) FROM HitCounters", con);

                con.Open();
                var rowCount = (int)com.ExecuteScalar();
                con.Close();

                return rowCount;
            }
        }


        private int GetLastCounterId()
        {
            using (var con = new SqlConnection(connectionString))
            {
                var com = new SqlCommand("SELECT IDENT_CURRENT('HitCounters')", con);

                con.Open();
                var lastCounterId = (int)(decimal)com.ExecuteScalar();
                con.Close();

                return lastCounterId;
            }
        }


        private void ResetOnlineStatus()
        {
            using (var con = new SqlConnection(connectionString))
            {
                var com = new SqlCommand("update HitCounters set IsOnline=0 where IsOnline=1", con)
                {
                    CommandType = CommandType.Text
                };

                con.Open();
                com.ExecuteNonQuery();
                con.Close();
            }
        }


        public class LoggingRazorViewEngine : RazorViewEngine
        {
            protected override IView CreateView(
                ControllerContext controllerContext,
                string viewPath,
                string masterPath)
            {
                LogHelper.Write("View:", viewPath);
                return base.CreateView(controllerContext, viewPath, masterPath);
            }
        }
        #endregion helper




        #region localize cache

        public override string GetVaryByCustomString(HttpContext context, string custom)
        {
            if (custom == "culture") // culture name (e.g. "en-US") is what should vary caching
            {
                string cultureName = null;

                // Attempt to read the culture cookie from Request
                HttpCookie cultureCookie = Request.Cookies["_culture"];
                if (cultureCookie != null)
                {
                    cultureName = cultureCookie.Value;
                }
                else
                {
                    cultureName = Setting.WebSettings[WebSettingType.AdminLanguages];
                }

                // Validate culture name
                cultureName = CultureHelper.GetImplementedCulture(cultureName);
                return cultureName.ToLower(); // use culture name as the cache key, "es", "en-us", "es-cl", etc.
            }
            return base.GetVaryByCustomString(context, custom);
        }

        #endregion




        #region route debug        
        //public override void Init()
        //{
        //    base.Init();
        //    this.AcquireRequestState += ShowRouteValues;
        //}

        //protected void ShowRouteValues(object sender, EventArgs e)
        //{
        //    var context = HttpContext.Current;
        //    if (context == null)
        //        return;
        //    var routeData = RouteTable.Routes.GetRouteData(new HttpContextWrapper(context));


        //    // turn off trace first
        //    System.Diagnostics.Debug.WriteLine($"name: {GetRouteName(routeData)}");

        //    foreach (var key in routeData.Values.Keys)
        //    {
        //        System.Diagnostics.Debug.WriteLine($"{key}: {routeData.Values[key]}");
        //    }

        //    System.Diagnostics.Debug.WriteLine("==========================");


        //}

        //// quick and dirty way to get route name
        //public string GetRouteName(RouteData routeData)
        //{

        //    foreach (string name in new[] {
        //        // admin
        //        "Admin_route",
        //        "Admin_default",

        //        //api
        //        "AdminApi",
        //        "ControllerAndAction",
        //        "DefaultApi",

        //        //client
        //        "slug-tag",
        //        "slug-loc-gia",
        //        "slug-view-thong-tin",
        //        "slug-list",
        //        "slug-chu-de-va-thu-muc",
        //        "slug-article-listbytopic",
        //        "slug-article-viewby",
        //        "slug-tag-bai-viet",
        //        "Homepage",
        //        "slug-gioi-thieu",
        //        "slug-lien-he",
        //        "article-index2",
        //        "article-index",
        //        "slug-view",


        //        "Default"
        //    })
        //    {
        //        if (routeData.Route == RouteTable.Routes[name])
        //        {
        //            return name;
        //        }
        //    }
        //    return "UNKNOWN-ROUTE";   // or throw exception
        //}


        #endregion route debug

    }
}