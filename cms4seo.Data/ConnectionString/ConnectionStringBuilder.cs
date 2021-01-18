using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Text;
using System.Web.Configuration;
using System.Web.WebPages;
using cms4seo.Common.Helpers;
using cms4seo.Model.ViewModel;
using Newtonsoft.Json;
// ReSharper disable InconsistentlySynchronizedField

namespace cms4seo.Data.ConnectionString
{
    
    public static  class ConnectionStringProvider
    {
        private static class Item
        {
            public const string Connection = "Connection";
            public const string Setup = "Setup";
        }


        // ReSharper disable once InconsistentNaming
        private static readonly object _locker = new object();


        static string _projectId = WebConfigurationManager.AppSettings["ProjectId"];

        
        private static string _connectionString;

        //private static bool _setup = false;

        public static string Build(string dataSource, string initialCatalog, string user, string password, bool integratedSecurity)
        {
            //return $"Data Source={dataSource};Initial Catalog={initialCatalog};" +
            //       $" User ID={user}; Password={password};Integrated Security={integratedSecurity.ToString()};";

            System.Data.SqlClient.SqlConnectionStringBuilder builder =
                new System.Data.SqlClient.SqlConnectionStringBuilder();

            builder["Data Source"] = dataSource;
            builder["Initial Catalog"] = initialCatalog;
            builder["User ID"] = user;
            builder["Password"] = password;
            builder["integrated Security"] = integratedSecurity;

            return builder.ConnectionString;
        }

        public static string Get()
        {
            if (_connectionString != null)
                return _connectionString;
            
            string path = Path.Combine(
                AppDomain.CurrentDomain.GetData("DataDirectory").ToString(), $"{_projectId}.txt");

            lock (_locker)
            {
                if (File.Exists(path))
                {
                    using (TextReader textReader = new StreamReader(path))
                    {
                        var jsonData = textReader.ReadToEnd();
                        var dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonData);
                        _connectionString = dictionary[Item.Connection];
                        return _connectionString;
                    }
                }
            }
            

            return null;
        }


        public static bool IsSetup
        {
            get
            {
                string path = Path.Combine(
                    AppDomain.CurrentDomain.GetData("DataDirectory").ToString(), $"{_projectId}.txt");

                lock (_locker)
                {
                    if (File.Exists(path))
                    {
                        using (TextReader textReader = new StreamReader(path))
                        {

                            var jsonData = textReader.ReadToEnd();

                            var dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonData);

                            _connectionString = dictionary[Item.Connection];

                            return dictionary[Item.Setup].AsBool();

                        }
                    }
                    else
                    {
                        // for init setup
                        return true;
                    }
                }

            }
        }

        public static string FinishSetup()
        {
            if (!IsSetup)
                return "Setup mode already finished";

            if (string.IsNullOrWhiteSpace(_connectionString))
                return "Current connectionString is null";

            // combine to finish setup;
            var data = new Dictionary<string, string>
            {
                {Item.Connection, _connectionString},
                {Item.Setup, false.ToString()}
            };

            string path = Path.Combine(
                AppDomain.CurrentDomain.GetData("DataDirectory").ToString(), $"{_projectId}.txt");

            try
            {
                Save(path,data);
            }
            catch (Exception exception)
            {
                LogHelper.Write("ConnectionStringProvider/FinishSetup", exception.Message);
                return exception.Message;
            }


            return true.ToString();
        }



        /// <summary>
        /// Test connectString with friendly message, Message if error, null if success, 0 if database not exist
        /// </summary>
        /// <param name="setupDatabaseVm"></param>
        /// <returns>Message if error, null if success, 0 if database not exist</returns>
        public static string TestConnection(SetupDatabaseVm setupDatabaseVm)
        {
            var connectionString = Build(setupDatabaseVm.DataSource, setupDatabaseVm.InitialCatalog,
                setupDatabaseVm.User, setupDatabaseVm.Password, false);

            using (var db = new ApplicationDbContext(connectionString))
            {
                try
                {
                    // not throw detail errors
                    //var databaseExist = db.Database.Exists();

                    // Test Database connect
                    LogHelper.Write("ConnectionStringProvider/TryCreateDatabase", "Before db.Database.Connection.Open()");
                    db.Database.Connection.Open();
                    LogHelper.Write("ConnectionStringProvider/TryCreateDatabase", "After db.Database.Connection.Open()");
                    if (db.Database.Connection.State == ConnectionState.Open)
                    {
                        LogHelper.Write("ConnectionStringProvider/TryCreateDatabase", "Before db.Database.Connection.Close()");
                        db.Database.Connection.Close();
                        LogHelper.Write("ConnectionStringProvider/TryCreateDatabase", "After db.Database.Connection.Close(), return null");
                        return null;
                    }

                    return "ConnectionState not Open";
                }
                catch (Exception exception)
                {
                    LogHelper.Write("ConnectionStringProvider/TestConnection", exception.Message);

                    // Display errors friendly for End-User
                    if (exception.Message.Contains("error: 26"))
                        return "Can't connect to SQL server, please check SQL Server & Instance Name is Correct";

                    //if (exception.Message.Contains("Cannot open database") &&
                    //    exception.Message.Contains("Login failed for user"))
                    //    return "Database not yet Created, Please Create Database first, and try again";

                    if (exception.Message.Contains("Cannot open database") &&
                        exception.Message.Contains("Login failed for user"))
                        return "0";

                    if (!exception.Message.Contains("Cannot open database") &&
                        exception.Message.Contains("Login failed for user"))
                        return "User or Password not correct";

                    return "Connect SQL SERVER error, please check SQL SERVER, USER, PASSWORD is correct!";
                }
            }

        }


        public static string TryCreateDatabase(SetupDatabaseVm setupDatabaseVm)
        {
            var connectionString = Build(setupDatabaseVm.DataSource, setupDatabaseVm.InitialCatalog,
                setupDatabaseVm.User, setupDatabaseVm.Password, false);


            using (var db = new ApplicationDbContext(connectionString))
            {
                try
                {
                    LogHelper.Write("ConnectionStringProvider/TryCreateDatabase", "Before check !db.Database.Exists()");

                    if (!db.Database.Exists())
                    {
                        LogHelper.Write("ConnectionStringProvider/TryCreateDatabase", "After check !db.Database.Exists()");
                        db.Database.Create();
                        LogHelper.Write("ConnectionStringProvider/TryCreateDatabase", "After db.Database.Create()");
                    }

                    return null;
                }
                catch (Exception exception)
                {
                    LogHelper.Write("ConnectionStringProvider/TryCreateDatabase", exception.Message);

                    // Display errors friendly for End-User
                    if (exception.Message.Contains("some thing"))
                        return "Can't connect to SQL server, please check SQL Server & Instance Name is Correct";


                    return "Connect SQL SERVER error, please check SQL SERVER, USER, PASSWORD is correct!";
                }
            }
        }


        /// <summary>
        /// Set projectId (local mode) & save connection String
        /// </summary>
        /// <param name="setupDatabaseVm">database config</param>
        /// <returns>return null if success</returns>
        public static string Set(SetupDatabaseVm setupDatabaseVm)
        {

            // save connection String
            string path = Path.Combine(
                AppDomain.CurrentDomain.GetData("DataDirectory").ToString(), $"{_projectId}.txt");

            // check connection file
            if (File.Exists(path))
                return null;

            var connectionString = Build(setupDatabaseVm.DataSource, setupDatabaseVm.InitialCatalog,
                setupDatabaseVm.User, setupDatabaseVm.Password, false);


            // combine to setup mode;
            var data = new Dictionary<string, string>
            {
                {"Connection", connectionString},
                { "Setup", true.ToString()}
            };



            try
            {
                Save(path, data);
            }
            catch(Exception exception)
            {
                LogHelper.Write("ConnectionStringProvider/Set", exception.Message);
                return exception.Message;
            }

            return null;
        }






        public static bool SetDomain(string domain)
        {
            //// Translate '.' to '-'
            //if (domain.Contains("."))
            //    domain = domain.Translate();

            // check current appSetting ProjectId & set new ProjectId
            if (domain != _projectId)
            {
                try
                {

                    //Helps to open the Root level web.config file.
                    Configuration webConfigApp = WebConfigurationManager.OpenWebConfiguration("~");

                    //Modifying the AppKey from AppValue to AppValue1
                    if (webConfigApp.AppSettings.Settings["ProjectId"] != null)
                        webConfigApp.AppSettings.Settings["ProjectId"].Value = domain;
                    else
                        webConfigApp.AppSettings.Settings.Add("ProjectId", domain);

                    //Save the Modified settings of AppSettings.
                    webConfigApp.Save();

                    // Next read wil be in hard disk
                    //ConfigurationManager.RefreshSection("appSettings"); // not work

                    // Update static _projectId
                    _projectId = domain;

                    return true;
                }
                catch (Exception e)
                {
                    LogHelper.Write("ConnectionStringProvider/SetDomain", e.Message);
                    return false;
                }

            }


            return false;
        }



        public static string Translate(this string domain)
        {
            return domain?.Replace(".", "-");
        }


        public static string Reverse(this string name)
        {
            return name?.Replace("-", ".");
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
    }
}
