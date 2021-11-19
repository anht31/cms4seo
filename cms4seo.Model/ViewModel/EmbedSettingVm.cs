using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Admin.Resources;

namespace cms4seo.Model.ViewModel
{
    public class EmbedSettingVm
    {
        [ScaffoldColumn(false)]
        public string Id { get; set; }

        [Display(Name = "EmbedSettingVmName", ResourceType = typeof(AdminResources))]
        [ScaffoldColumn(false)]
        public string Name { get; set; }

        [AllowHtml]
        [Display(Name = "EmbedSettingVmFacebookId", ResourceType = typeof(AdminResources))]
        [DataType(DataType.MultilineText)]
        public string FacebookId { get; set; }

        [AllowHtml]
        [Display(Name = "EmbedSettingVmFacebookScript", ResourceType = typeof(AdminResources))]
        [DataType(DataType.MultilineText)]
        public string FacebookScript { get; set; }

        [AllowHtml]
        [Display(Name = "EmbedSettingVmFacebookPage", ResourceType = typeof(AdminResources))]
        [DataType(DataType.MultilineText)]
        public string FacebookPage { get; set; }

        [AllowHtml]
        [Display(Name = "EmbedSettingVmGoogleAnalytics", ResourceType = typeof(AdminResources))]
        [DataType(DataType.MultilineText)]
        public string GoogleAnalytics { get; set; }

        [AllowHtml]
        [Display(Name = "EmbedSettingVmMap", ResourceType = typeof(AdminResources))]
        [DataType(DataType.MultilineText)]
        public string Map { get; set; }

        [AllowHtml]
        [Display(Name = "EmbedSettingVmStyles", ResourceType = typeof(AdminResources))]
        [DataType(DataType.MultilineText)]
        public string Styles { get; set; }

        [AllowHtml]
        [Display(Name = "EmbedSettingVmScripts", ResourceType = typeof(AdminResources))]
        [DataType(DataType.MultilineText)]
        public string Scripts { get; set; }


        [AllowHtml]
        [Display(Name = "EmbedSettingVm_Comment", ResourceType = typeof(AdminResources))]
        [DataType(DataType.MultilineText)]
        public string Comment { get; set; }

        [AllowHtml]
        [Display(Name = "EmbedSettingVm_Header", ResourceType = typeof(AdminResources))]
        [DataType(DataType.MultilineText)]
        public string Header { get; set; }

        [AllowHtml]
        [Display(Name = "EmbedSettingVm_Aside", ResourceType = typeof(AdminResources))]
        [DataType(DataType.MultilineText)]
        public string Aside { get; set; }
    }
}
