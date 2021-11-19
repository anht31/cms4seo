using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using cms4seo.Service.Content;
using cms4seo.Service.Provider;
using Newtonsoft.Json;
using cms4seo.Common.Plugins;

namespace TestConsole
{
    class Program
    {




        static void Main(string[] args)
        {
            Widget widget = new Widget("Index", "Test", "PluginTest"
                , "testzone", string.Empty, "test-zone", "true", "", "", "");

            Widget widget2 = new Widget("Index", "Test2", "PluginTest2"
                , "testzone2", string.Empty, "test-zone-2", "true", "", "", "");

            var widgets = new List<Widget>();
            widgets.Add(widget);
            widgets.Add(widget2);

            Console.WriteLine(widget.Action);


            string path = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory, $"widgets.json");

            //// Write the list of Widget objects to file.
            //WriteToJsonFile<List<Widget>>(path, widgets);


            // way 2 with pretty print json
            SaveWidget(path,widgets);


            // Read the list of Widget objects from the file back into a variable.
            List<Widget> importWidgets = ReadFromJsonFile<List<Widget>>(path);

            Console.WriteLine();
            Console.WriteLine("Output ------------");
            foreach (var item in importWidgets)
            {
                Console.WriteLine(widget.Zone);
            }
            

            Console.ReadLine();




            //var task = Main4();
            //Task.WaitAll(task);


            //DoMain().Wait();
        }




        public static void SaveWidget(string path, List<Widget> widgets)
        {

            StringBuilder stringBuilder = new StringBuilder();
            StringWriter stringWriter = new StringWriter(stringBuilder);

            lock (_locker)
            {
                using (JsonWriter writer = new JsonTextWriter(stringWriter))
                {
                    writer.Formatting = Formatting.Indented;

                    writer.WriteStartArray();

                    foreach (var widget in widgets)
                    {
                        writer.WriteStartObject();

                        writer.WritePropertyName("Action");
                        writer.WriteValue(widget.Action);

                        writer.WritePropertyName("Controller");
                        writer.WriteValue(widget.Controller);

                        writer.WritePropertyName("Area");
                        writer.WriteValue(widget.Area);

                        writer.WritePropertyName("Zone");
                        writer.WriteValue(widget.Zone);

                        writer.WritePropertyName("RouteValues");
                        writer.WriteValue(widget.RouteValues);

                        writer.WritePropertyName("Page");
                        writer.WriteValue(widget.Page);

                        writer.WritePropertyName("Active");
                        writer.WriteValue(widget.Active);

                        writer.WriteEndObject();
                    }

                    writer.WriteEndArray();

                    
                }

                string json = stringWriter.ToString();

                // write to file
                using (StreamWriter streamWriter = new StreamWriter(path, false))
                {
                    streamWriter.Write(json);
                }
            }

        }


        public static void Save(string path, Dictionary<string, string> data)
        {

            StringBuilder stringBuilder = new StringBuilder();
            StringWriter stringWriter = new StringWriter(stringBuilder);

            lock (_locker)
            {
                using (JsonWriter writer = new JsonTextWriter(stringWriter))
                {
                    writer.Formatting = Formatting.Indented;

                    writer.WriteStartObject();

                    foreach (var item in data)
                    {
                        writer.WritePropertyName(item.Key);
                        writer.WriteValue(item.Value);
                    }

                    writer.WriteEndObject();
                }

                string json = stringWriter.ToString();

                // write to file
                using (StreamWriter streamWriter = new StreamWriter(path, false))
                {
                    streamWriter.Write(json);
                }
            }

        }

        /// <summary>
        /// Writes the given object instance to a Json file.
        /// <para>Object type must have a parameterless constructor.</para>
        /// <para>Only Public properties and variables will be written to the file. These can be any type though, even other classes.</para>
        /// <para>If there are public properties/variables that you do not want written to the file, decorate them with the [JsonIgnore] attribute.</para>
        /// </summary>
        /// <typeparam name="T">The type of object being written to the file.</typeparam>
        /// <param name="filePath">The file path to write the object instance to.</param>
        /// <param name="objectToWrite">The object instance to write to the file.</param>
        /// <param name="append">If false the file will be overwritten if it already exists. If true the contents will be appended to the file.</param>
        public static void WriteToJsonFile<T>(string filePath, T objectToWrite, bool append = false) where T : new()
        {
            TextWriter writer = null;
            try
            {
                var contentsToWriteToFile = JsonConvert.SerializeObject(objectToWrite);
                writer = new StreamWriter(filePath, append);
                writer.Write(contentsToWriteToFile);
            }
            finally
            {
                if (writer != null)
                    writer.Close();
            }
        }

        /// <summary>
        /// Reads an object instance from an Json file.
        /// <para>Object type must have a parameterless constructor.</para>
        /// </summary>
        /// <typeparam name="T">The type of object to read from the file.</typeparam>
        /// <param name="filePath">The file path to read the object instance from.</param>
        /// <returns>Returns a new instance of the object read from the Json file.</returns>
        public static T ReadFromJsonFile<T>(string filePath) where T : new()
        {
            TextReader reader = null;
            try
            {
                reader = new StreamReader(filePath);
                var fileContents = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<T>(fileContents);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
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
