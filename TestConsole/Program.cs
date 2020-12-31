using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using cms4seo.Service.Content;
using cms4seo.Service.Provider;

namespace TestConsole
{
    class Program
    {




        static void Main(string[] args)
        {
            //var task = Main4();
            //Task.WaitAll(task);


            DoMain().Wait();

            Console.ReadLine();
        }




        #region await test

        public static async Task DoMain()
        {
            Task<int> downloading = DownloadDocsMainPageAsync();
            Console.WriteLine($"{nameof(DoMain)}: Launched downloading.");

            int bytesLoaded = await downloading;
            Console.WriteLine($"{nameof(DoMain)}: Downloaded {bytesLoaded} bytes.");
        }

        private static async Task<int> DownloadDocsMainPageAsync()
        {
            Console.WriteLine($"{nameof(DownloadDocsMainPageAsync)}: About to start downloading.");

            var client = new HttpClient();
            byte[] content = await client.GetByteArrayAsync("https://cms4seo.com/");

            Console.WriteLine($"{nameof(DownloadDocsMainPageAsync)}: Finished downloading.");
            return content.Length;
        }

        #endregion



        #region test async C# 2

        public static async Task DoTheJob()
        {
            Console.WriteLine("Begin DoTheJob");

            //int a = await doSomethingAsync(10);

            Console.WriteLine("Some code");

            Console.WriteLine("Result a: " + await doSomethingAsync(10));

            Console.WriteLine("End DoTheJob");

            return;
        }

        static async Task<int> doSomethingAsync(int a)
        {
            Thread.Sleep(3000);
            Console.WriteLine("doSomethingAsync");
            return a * 2;
        }


        #endregion



        #region test async C#
        public static async Task MainAsync()
        {
            Task task1 = Task1();
            Task task2 = Task2();

            await Task.WhenAll(task1, task2);

            Console.WriteLine("Finished main method");
        }

        public static async Task Task1()
        {
            await Task.Delay(5000);
            Console.WriteLine("Finished Task1");
        }

        public static async Task Task2()
        {
            await Task.Delay(10000);
            Console.WriteLine("Finished Task2");
        }



        #endregion







        #region Test XmlSetting

        static void Main2()
        {
            Stopwatch stopwatch = new Stopwatch();

            //var test = (true).RenderToggleEditButton();

            //var xmlSettingProvider = new XmlSettingProvider();

            // Generate a random number  
            Random random = new Random();


            // fixed key
            var key = "Common.Home.Section1.Column1.Header";



            var n = 0;

            // Begin timing.
            stopwatch.Start();

            Parallel.For(0, 1000, i =>
            {
                //WriteLine("");
                //WriteLine($"[{i}]");

                ////var keyRandom = $"{i}.Contents.{random.Next(9999)}";
                //var keyRandom = $"{i}.Contents.{++n}";

                //// access Test
                //WriteLine(Setting.Contents[key]);

                //// key
                //WriteLine($"Key: {keyRandom}");

                //// cms.attribute
                //WriteLine($"Attribute: {CMS.Attribute(keyRandom)}");

                //// try edit value new keyRandom
                //xmlSettingProvider.UpdateXml(keyRandom, $"value for - {keyRandom}");

                //// value
                //WriteLine($"Value: {Setting.Contents[keyRandom]}");
            });


            // try i times
            //var n = 200;

            //while (n-- > 0)
            //{
            //    WriteLine("");
            //    WriteLine($"[{i}]");

            //    var keyRandom = $"Contents.{random.Next(9999)}";

            //    // access Test
            //    WriteLine(Setting.AppSettings[key]);

            //    // key
            //    WriteLine($"Key: {keyRandom}");

            //    // cms.attribute
            //    WriteLine($"Attribute: {CMS.Attribute(keyRandom)}");

            //    // try edit value new keyRandom
            //    xmlSettingProvider.UpdateXml(keyRandom, $"value for - {keyRandom}");

            //    // value
            //    WriteLine($"Value: {Setting.AppSettings[keyRandom]}");
            //}

            // Stop timing.
            stopwatch.Stop();

            Console.WriteLine();
            Console.WriteLine(">>>>>> DONE <<<<<<");
            Console.WriteLine("Time elapsed: {0}", stopwatch.Elapsed);
            Console.ReadKey();
        }

        public static void WriteLine(string text)
        {
            //Console.WriteLine(text);
        }



        #endregion


        #region Simple example of lock


        private static int _counter;
        private static readonly object _locker = new object();

        private static void Main3()
        {
            Console.WriteLine("**********************************************************");
            Console.WriteLine("Starting counting in parallel 3 times to 10, NO sync lock.");
            Console.WriteLine("**********************************************************");

            Parallel.For(0, 3, i =>
            {
                CountNoLock();
            });

            Console.WriteLine("Hit any key to start locked version.");
            Console.ReadLine();
            Console.WriteLine();
            Console.WriteLine("************************************************************");
            Console.WriteLine("Starting counting in parallel 3 times to 10, WITH sync lock.");
            Console.WriteLine("************************************************************");

            Parallel.For(0, 3, i =>
            {
                CountLock();
            });

            Console.WriteLine();
            Console.WriteLine("*****************************************************");
            Console.WriteLine("End test, hit key to exit.");
            Console.WriteLine("*****************************************************");
            Console.ReadLine();
        }

        private static void CountLock()
        {
            Console.WriteLine("Start counting sync with lock...");
            lock (_locker)
            {
                StartCounting();
            }
        }

        private static void CountNoLock()
        {
            Console.WriteLine("Start counting sync with no lock...");
            StartCounting();
        }

        private static void StartCounting()
        {
            _counter = 0;
            Console.WriteLine("Let's count...");
            for (int i = 0; i < 10; i++)
            {
                _counter++;
                Console.WriteLine("Current counter is: " + _counter);
            }

            Console.WriteLine("Finished...");
        }

        #endregion



        #region Simple example of async/await lock.

        private static readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);

        private static async Task Main4()
        {
            Console.WriteLine("***********************************************************");
            Console.WriteLine("Starting counting in parallel 3 times to 10, NO async lock.");
            Console.WriteLine("***********************************************************");

            var executingTasks = new List<Task>();
            for (int i = 0; i < 3; i++)
            {
                var executingTask = CountAsyncNoLock();
                executingTasks.Add(executingTask);
                await Task.Delay(2000); // wait a bit for next task to start -> better see counter reset
            }

            Task.WaitAll(executingTasks.ToArray());

            Console.WriteLine("Hit any key to start locked version.");
            Console.ReadLine();
            Console.WriteLine();
            Console.WriteLine("*************************************************************");
            Console.WriteLine("Starting counting in parallel 3 times to 10, WITH async lock.");
            Console.WriteLine("*************************************************************");

            executingTasks.Clear();

            for (int i = 0; i < 3; i++)
            {
                var executingTask = CountAsyncWithLock();
                executingTasks.Add(executingTask);
                await Task.Delay(2000); // wait a bit for next task to start -> better see counter reset
            }

            Task.WaitAll(executingTasks.ToArray());

            Console.WriteLine();
            Console.WriteLine("*****************************************************");
            Console.WriteLine("End test, hit key to exit.");
            Console.WriteLine("*****************************************************");
            Console.ReadLine();
        }

        private static async Task CountAsyncNoLock()
        {
            Console.WriteLine("Start counting async no lock...");
            await StartCountingSync();
        }

        private static async Task CountAsyncWithLock()
        {
            Console.WriteLine("Start counting async with lock...");

            await _semaphoreSlim.WaitAsync();
            await StartCountingSync();
            _semaphoreSlim.Release();
        }

        private static async Task StartCountingSync()
        {
            _counter = 0;
            Console.WriteLine("Let's count async...");
            for (int i = 0; i < 10; i++)
            {
                await Task.Delay(500); // faking doing something else async
                _counter++;
                Console.WriteLine("Current counter is: " + _counter);
            }

            Console.WriteLine("Finished...");
        }




        #endregion
    }
}
