using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using cms4seo.Model.LekimaxType;

namespace cms4seo.Model.ViewModel
{
    public class SetupStoreVm
    {
        [ScaffoldColumn(false)]
        public string Id { get; set; }


        public Dictionary<string, string> Countries = new Dictionary<string, string>
        {
            {"en-US", "English"},
            {"zh-CN", "Chinese"},
            {"es", "Spanish"},
            {"vi-VN", "Vietnamese"},
            {"fr", "French"},
            {"ru", "Russian"},
            {"pt", "Portuguese"},
            {"id", "Indonesian"},
            {"de", "German"}            
        };

        [DataType(cms4seoDataType.CustomEditor)]
        public string Country { get; set; }


        //[Display(Name = "Sample Database")]
        //public bool SampleData { get; set; }


        public Dictionary<string, string> SampleList { get; set; }

        [Display(Name = "Sample Database")]
        [DataType(cms4seoDataType.CustomEditor)]
        public string CurrentSample { get; set; }


        //Holder for themes
        public Dictionary<string, string> ThemeList { get; set; }


        [Display(Name = "Sample Database")]
        [DataType(cms4seoDataType.CustomEditor)]
        public string Theme { get; set; }
    }
}
