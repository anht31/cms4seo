using System.Collections.Generic;
using cms4seo.Model.Entities;
using cms4seo.Model.ViewModel;

namespace cms4seo.Data.Repositories
{
    public interface IHitCounterRepository : IGenericRepository<HitCounter>
    {
        /// <summary>
        ///     Get number of section
        /// </summary>
        /// <returns>number of section</returns>
        int SessionCounter();

        List<HitCounterVm> GetOneMonth();

        int DailyCounter();
    }
}