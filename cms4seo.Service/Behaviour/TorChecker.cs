using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace cms4seo.Service.Behaviour
{
    public class TorProvider
    {
        public const string DefaultTorProjectExitAddressesUrl = "https://check.torproject.org/exit-addresses";

        private int retryLimit = 5;

        public string Name => "TorProvider";

        private string exitAddressesUrl;


        public TorProvider()
        {
            this.exitAddressesUrl = DefaultTorProjectExitAddressesUrl;
        }

#pragma warning disable 1998
        public async Task<HashSet<string>> ListIpAsync()
#pragma warning restore 1998
        {
            Exception lastException = null;
            int retry = 0;
            string data = null;
            // Method 1 - using Flurl.Htlp
            //while (data == null && retry++ < retryLimit)
            //{
            //    try
            //    {
            //        data = await exitAddressesUrl.GetAsync().ReceiveString();
            //    }
            //    catch (Exception exc)
            //    {
            //        lastException = exc;
            //    }
            //}

            // Method 2 - using Crawler
            while (data == null && retry++ < retryLimit)
            {
                try
                {
                    #region Crawler

                    string urlAddress = exitAddressesUrl;

                    // using System.Net;
                    ServicePointManager.Expect100Continue = true;
                    //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    //ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
                    ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                    // To turn off SSL3 without affecting other protocols
                    ServicePointManager.SecurityProtocol &= ~SecurityProtocolType.Ssl3;
                    // Use SecurityProtocolType.Ssl3 if needed for compatibility reasons

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlAddress);
                    //request.UserAgent = "Mozilla/4.0 (compatible; MSIE 5.01; Windows NT 5.0)";
                    request.UserAgent = @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.106 Safari/537.36";

                    request.CookieContainer = new CookieContainer();
                    //request.Credentials = GetCredential(url);
                    //request.AllowAutoRedirect = false;


                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        Stream receiveStream = response.GetResponseStream();
                        StreamReader readStream = null;

                        if (response.CharacterSet == null)
                        {
                            readStream = new StreamReader(receiveStream);
                        }
                        else
                        {
                            readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                        }

                        data = readStream.ReadToEnd();

                        response.Close();
                        readStream.Close();
                    }


                    #endregion Crawler
                }
                catch (Exception exc)
                {
                    lastException = exc;
                }
            }



            if (data == null && lastException != null)
                throw lastException;

            var list = new HashSet<string>();

            using (var reader = new StringReader(data))
            {
                while (true)
                {
                    string line = reader.ReadLine();
                    if (line == null) break;
                    var ip = ExtractIp(line);
                    if (ip != null)
                        list.Add(ip);
                }
            }

            return list;
        }


        // Lines with IP address: "ExitAddress 199.249.223.62 2018-08-25 07:07:23"
        private static Regex ipAddress = new Regex(@"^ExitAddress ([\d.]+)", RegexOptions.Compiled);
        private string ExtractIp(string line)
        {
            var match = ipAddress.Match(line);
            return match.Success ? match.Groups[1].Value : null;
        }


    }


    public class TorChecker
    {
        private object updateLock = new object();
        public DateTime LastUpdate { get; set; }

        private HashSet<string> ipList;

        private TorProvider provider { get; set; }

        public TorChecker()
        {
            provider = new TorProvider();
        }

        public bool IsUsingTor(string ipAddress)
        {
            if (ipList == null)
                lock (updateLock)
                {
                    LoadIplIst();
                }

            return ipList.Contains(ipAddress);
        }

        private void LoadIplIst()
        {
            ipList = new HashSet<string>();

            var tasks = new List<Task<HashSet<string>>>();


            tasks.Add(Task.Run(() => provider.ListIpAsync()));


            var continuation = Task.WhenAll(tasks);

            try
            {
                continuation.Wait();
            }
            catch (AggregateException exc)
            {
                throw exc.GetBaseException();
            }


            if (continuation.Status == TaskStatus.RanToCompletion)
                foreach (var result in continuation.Result)
                    ipList.UnionWith(result);
            else
            {
                // todo: get the provider Name
                var errors = from t in tasks
                             where t.Status != TaskStatus.RanToCompletion
                             select $"Status: {t.Status}, Error: {t.Exception}";

                throw new Exception(string.Join(" ", errors));
            }
        }
    }
}