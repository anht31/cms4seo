using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Admin.Resources;

namespace cms4seo.Model.ViewModel
{
    public class PhotoSettingVm
    {
        [ScaffoldColumn(false)]
        public string Id { get; set; }

        [Display(Name = "PhotoSettingVmImageQuality", ResourceType = typeof(AdminResources))]
        [Range(70, 100, ErrorMessageResourceType = typeof(AdminResources),
            ErrorMessageResourceName = "PhotoSettingVmImageQualityRange")]
        public int ImageQuality { get; set; }

        [Display(Name = "PhotoSettingVmSmallSize", ResourceType = typeof(AdminResources))]
        public int SmallSize { get; set; }

        [Display(Name = "PhotoSettingVmMediumSize", ResourceType = typeof(AdminResources))]
        public int MediumSize { get; set; }

        [Display(Name = "PhotoSettingVmLargeSize", ResourceType = typeof(AdminResources))]
        public int LargeSize { get; set; }

        [Display(Name = "PhotoSettingVmMaxHeight", ResourceType = typeof(AdminResources))]
        public int MaxHeight { get; set; }

        [Display(Name = "PhotoSettingVm_IsAutoConvertImageToJpg", ResourceType = typeof(AdminResources))]
        public bool IsAutoConvertPngToJpg { get; set; }
    }
}
