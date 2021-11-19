using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using cms4seo.Model.Entities;
using cms4seo.Model.ViewModel;

namespace cms4seo.Data.Repositories
{
    public class HitCounterRepository : GenericRepository<ApplicationDbContext, HitCounter>, IHitCounterRepository
    {
        public int SessionCounter()
        {
            if (GetAll().Any())
            {
                return GetAll().Max(x => x.CounterId);
            }
            return 0;
        }


        public List<HitCounterVm> GetOneMonth()
        {
            var fromDate = DateTime.Today.AddDays(-30);            

            var query = from hcList in Context.HitCounters
                where hcList.OpenTime > fromDate && hcList.Interval > 0
                group hcList by DbFunctions.TruncateTime(hcList.OpenTime)
                into g
                orderby g.Key
                select new
                {
                    Date = (DateTime) g.Key,
                    DailySessionCounter = g.Count(),
                    DailyUserCounter = g.Select(x => x.UserGuid).Distinct().Count()
                };


            var hcVmList = new List<HitCounterVm>();
            foreach (var item in query.OrderBy(x => x.Date))
            {
                var hcVm = new HitCounterVm
                {
                    Date = item.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    DailySessionCounter = item.DailySessionCounter,
                    DailyUserCounter = item.DailyUserCounter
                };


                hcVmList.Add(hcVm);
            }


            return hcVmList;
        }


        public int DailyCounter()
        {
            var today = DateTime.Now;
            return GetAll().Count(x => DbFunctions.TruncateTime(x.OpenTime) == today.Date);
        }
    }
}