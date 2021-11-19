using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Admin.Resources;

namespace cms4seo.Model.ViewModel
{
    public class TocVm
    {
        [ScaffoldColumn(false)]
        public string Id { get; set; }

        [Display(Name = "TocVm_IsAutoProductTOC", ResourceType = typeof(AdminResources))]
        // ReSharper disable once InconsistentNaming
        public bool IsAutoProductTOC { get; set; }


        [Display(Name = "TocVm_IsAutoArticleTOC", ResourceType = typeof(AdminResources))]
        // ReSharper disable once InconsistentNaming
        public bool IsAutoArticleTOC { get; set; }


        [Display(Name = "TocVm_TocTitle", ResourceType = typeof(AdminResources))]
        public string TocTitle { get; set; }


        [Display(Name = "TocVm_IsTocReturn", ResourceType = typeof(AdminResources))]
        // ReSharper disable once InconsistentNaming
        public bool IsTocReturn { get; set; }
    }
}
