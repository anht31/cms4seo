using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using Admin.ViewModel;
using Domain.Entities;
using Lekimax.Concrete;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lekimax.Tests
{
    [TestClass]
    public class SqlQueryTest
    {
        //[TestMethod]
        //public void TestGetAllSpeed()
        //{
        //    ITagRepository tagRepository = new TagRepository();
        //    var result = tagRepository.GetAll();

        //    // Assert
        //    Assert.IsTrue(result.ToString().Length > 0);
        //}

        //[TestMethod]
        //public void GetAllByAdoSpeed()
        //{
        //    TagRepository tagRepository = new TagRepository();
        //    var result = tagRepository.GetAllByAdo();

        //    // Assert
        //    Assert.IsTrue(result.ToString().Length > 0);
        //}


        [TestMethod]
        public void Can_Linq_Test()
        {
            var db = new UserContext();

            var products = db.Products.ToList();

            // Assert
            Assert.IsTrue(products.Count > 0);
        }


        [TestMethod]
        public void Can_SqlAdapter_Test()
        {
            var adapter = new SqlAdapter();

            var products = adapter.ListBySqlCommand<Product>("Select * from dbo.Products");

            // Assert
            Assert.IsTrue(products.Count > 0);
        }


        [TestMethod]
        public void SqlwithAdoReflection_faster_Linq_1Query_Test()
        {
            // timer AdoReflection ======================================================
            var timerAdoReflection = Stopwatch.StartNew();

            var adapter = new SqlAdapter();
            var products = adapter.ListBySqlCommand<Product>("Select * from dbo.Products");
            timerAdoReflection.Stop();

            // timer linq ======================================================
            var timerLinq = Stopwatch.StartNew();

            var db = new UserContext();
            var products2 = db.Products.ToList();
            timerLinq.Stop();


            Trace.WriteLine("\ntimerAdoReflection: " + timerAdoReflection.Elapsed.TotalMilliseconds);
            Trace.WriteLine("\ntimerLinq: " + timerLinq.Elapsed.TotalMilliseconds);

            // Assert
            Assert.IsTrue(timerAdoReflection.Elapsed < timerLinq.Elapsed);
        }


        [TestMethod]
        public void SqlwithAdoReflection_faster_Linq_100Query_Test()
        {
            // timer AdoReflection ======================================================
            var timerAdoReflection = Stopwatch.StartNew();

            for (var i = 0; i < 100; i++)
            {
                var adapter = new SqlAdapter();
                var products = adapter.ListBySqlCommand<Product>("Select * from dbo.Products");
            }

            timerAdoReflection.Stop();

            // timer linq ======================================================
            var timerLinq = Stopwatch.StartNew();

            for (var i = 0; i < 100; i++)
            {
                var db = new UserContext();
                var products2 = db.Products.ToList();
            }

            timerLinq.Stop();

            // output ===============================================================
            Trace.WriteLine("\ntimerAdoReflection: " + timerAdoReflection.Elapsed.TotalMilliseconds);
            Trace.WriteLine("\ntimerLinq: " + timerLinq.Elapsed.TotalMilliseconds);

            // Assert
            Assert.IsTrue(timerAdoReflection.Elapsed < timerLinq.Elapsed);
        }


        [TestMethod]
        public void can_query_30record_in_9000record_groupby_date()
        {
            //Thread.CurrentThread.CurrentCulture = new CultureInfo("vi-VN");


            var db = new UserContext();

            var timer = Stopwatch.StartNew();

            //get data
            var myData = from hcList in db.HitCounters
                group hcList by DbFunctions.TruncateTime(hcList.OpenTime)
                into g
                orderby g.Key
                select new {Date = (DateTime) g.Key, DailySessionCounter = g.Count()};

            var lista = myData.OrderByDescending(x => x.Date).Take(30).ToList();

            var hcVmList = new List<HitCounterVM>();
            foreach (var item in lista.OrderBy(x => x.Date))
            {
                var hcVm = new HitCounterVM();

                hcVm.Date = item.Date.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture);
                hcVm.DailySessionCounter = item.DailySessionCounter;

                hcVmList.Add(hcVm);
            }
            lista.Clear();

            timer.Stop();
            Trace.WriteLine("\ntimerLinq: " + timer.Elapsed.TotalMilliseconds);

            // Assert
            Assert.IsTrue(hcVmList.Count == 30);
        }


        [TestMethod]
        public void query_30record_in_9000record_groupby_date_speedtest_1000time()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("vi-VN");


            var db = new UserContext();

            var timer = Stopwatch.StartNew();

            for (var i = 0; i < 1000; i++)
            {
                //get data
                var myData = from hcList in db.HitCounters
                    group hcList by DbFunctions.TruncateTime(hcList.OpenTime)
                    into g
                    orderby g.Key
                    select new {Date = (DateTime) g.Key, DailySessionCounter = g.Count()};

                var lista = myData.OrderByDescending(x => x.Date).Take(30).ToList();

                var hcVmList = new List<HitCounterVM>();
                foreach (var item in lista.OrderBy(x => x.Date))
                {
                    var hcVm = new HitCounterVM();

                    hcVm.Date = item.Date.ToShortDateString();
                    hcVm.DailySessionCounter = item.DailySessionCounter;

                    hcVmList.Add(hcVm);
                }
                lista.Clear();
            }

            timer.Stop();
            Trace.WriteLine("\ntimerLinq: " + timer.Elapsed.TotalMilliseconds);

            // Assert
            Assert.IsTrue(timer.Elapsed.TotalSeconds < 100);
        }


        [TestMethod]
        public void can_query_userCounter_to_hitcounterVM()
        {
            var db = new UserContext();


            var query = from ucList in db.UserCounters
                group ucList by DbFunctions.TruncateTime(ucList.OpenTime)
                into g
                orderby g.Key
                select new {Date = (DateTime) g.Key, DailyUserCounter = g.Count()};

            var lista = query.OrderByDescending(x => x.Date).Take(30).ToList();

            var hcVmList = new List<HitCounterVM>();
            foreach (var item in lista.OrderBy(x => x.Date))
            {
                var hcVm = new HitCounterVM();

                //hcVm.Date = item.Date.ToShortDateString();
                hcVm.Date = item.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                hcVm.DailyUserCounter = item.DailyUserCounter;

                hcVmList.Add(hcVm);
            }
            lista.Clear();

            Trace.WriteLine("\nTotal query: " + hcVmList.Count);
            // Assert
            Assert.IsTrue(hcVmList.Count > 0);
        }


        [TestMethod]
        public void speed_1000_query_userCounter_to_hitcounterVM()
        {
            var db = new UserContext();


            var timer = Stopwatch.StartNew();

            for (var i = 0; i < 1000; i++)
            {
                var query = from ucList in db.UserCounters
                    group ucList by DbFunctions.TruncateTime(ucList.OpenTime)
                    into g
                    orderby g.Key
                    select new {Date = (DateTime) g.Key, DailyUserCounter = g.Count()};

                var lista = query.OrderByDescending(x => x.Date).Take(30).ToList();

                var hcVmList = new List<HitCounterVM>();
                foreach (var item in lista.OrderBy(x => x.Date))
                {
                    var hcVm = new HitCounterVM();

                    //hcVm.Date = item.Date.ToShortDateString();
                    hcVm.Date = item.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                    hcVm.DailyUserCounter = item.DailyUserCounter;

                    hcVmList.Add(hcVm);
                }
                lista.Clear();
            }
            timer.Stop();

            Trace.WriteLine("\nTime eslapse: " + timer.Elapsed.TotalMilliseconds);
            // Assert
            Assert.IsTrue(timer.Elapsed.TotalSeconds < 100);
        }


        [TestMethod]
        public void speed_test_Max_vs_AdoNet()
        {
            var db = new UserContext();


            // max
            var timer = Stopwatch.StartNew();
            for (var i = 0; i < 1000; i++)
            {
                var counter1 = db.HitCounters.Max(x => x.CounterId);
            }
            timer.Stop();

            var connection =
                new SqlConnection(
                    @"Data Source=.\sqlexpress;Initial Catalog=cartonDb;User ID=anht31; Password=234;Integrated Security=false;");
            var myCommand = connection.CreateCommand();
            // ado
            var time2 = Stopwatch.StartNew();
            for (var i = 0; i < 1000; i++)
            {
                connection.Open();
                myCommand.CommandText = "SELECT MAX(CounterId) FROM dbo.HitCounters";
                var maxId = Convert.ToInt32(myCommand.ExecuteScalar());
                connection.Close();
            }
            time2.Stop();


            Trace.WriteLine("\nMax: " + timer.Elapsed.TotalMilliseconds);
            Trace.WriteLine("\nAdo.Net: " + time2.Elapsed.TotalMilliseconds);

            // Assert
            //Assert.IsTrue(counter1 == counter2);
            Assert.IsTrue(timer.Elapsed > time2.Elapsed);
        }


        [TestMethod]
        public void speed_test_Max_vs_OrderByFirst()
        {
            var db = new UserContext();

            // max
            var timer = Stopwatch.StartNew();
            for (var i = 0; i < 1000; i++)
            {
                var counter1 = db.HitCounters.Max(x => x.CounterId);
            }
            timer.Stop();


            // order by
            var time2 = Stopwatch.StartNew();
            for (var i = 0; i < 1000; i++)
            {
                var counter2 = db.HitCounters.OrderByDescending(x => x.CounterId).FirstOrDefault().CounterId;
            }
            time2.Stop();


            Trace.WriteLine("\nMax: " + timer.Elapsed.TotalMilliseconds);
            Trace.WriteLine("\nOrderByFirst: " + time2.Elapsed.TotalMilliseconds);

            // Assert
            //Assert.IsTrue(counter1 == counter2);
            Assert.IsTrue(timer.Elapsed < time2.Elapsed);
        }

        [TestMethod]
        public void speed_test_Max_vs_AdoNet_with_contructer()
        {
            // max
            var timer = Stopwatch.StartNew();
            for (var i = 0; i < 1000; i++)
            {
                using (var db = new UserContext())
                {
                    var counter1 = db.HitCounters.Max(x => x.CounterId);
                }
            }
            timer.Stop();


            // ado
            var time2 = Stopwatch.StartNew();
            for (var i = 0; i < 1000; i++)
            {
                using (
                    var connection =
                        new SqlConnection(
                            @"Data Source=.\sqlexpress;Initial Catalog=cartonDb;User ID=anht31; Password=234;Integrated Security=false;")
                    )
                {
                    using (var myCommand = connection.CreateCommand())
                    {
                        connection.Open();
                        myCommand.CommandText = "SELECT MAX(CounterId) FROM dbo.HitCounters";
                        var maxId = Convert.ToInt32(myCommand.ExecuteScalar());
                        connection.Close();
                    }
                }
            }
            time2.Stop();


            Trace.WriteLine("\nMax: " + timer.Elapsed.TotalMilliseconds);
            Trace.WriteLine("\nAdo.Net: " + time2.Elapsed.TotalMilliseconds);

            // Assert
            //Assert.IsTrue(counter1 == counter2);
            Assert.IsTrue(timer.Elapsed > time2.Elapsed);
        }

        // Helper =====================================

        public class UserContext : DbContext
        {
            public UserContext()
            {
                Database.Connection.ConnectionString =
                    @"Data Source=.\sqlexpress;Initial Catalog=cartonDb;User ID=anht31; Password=234;" +
                    "Integrated Security=false;";
            }

            public DbSet<Product> Products { get; set; }

            public DbSet<HitCounter> HitCounters { get; set; }

            public DbSet<UserCounter> UserCounters { get; set; }
        }

        //}
        //    return timer.Elapsed;
        //    timer.Stop();
        //    toTime();
        //    var timer = Stopwatch.StartNew();
        //{

        //private TimeSpan Time(Action toTime)
    }
}