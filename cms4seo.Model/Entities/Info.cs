using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Admin.Resources;
using cms4seo.Model.LekimaxType;

namespace cms4seo.Model.Entities
{
    public class Info : Seo
    {
        // GetLocationRule ===============================================
        public Dictionary<int, string> LocationRule = new Dictionary<int, string>
        {
            {0, AdminResources.InfoModelLocationRuleDefault},
            {1, AdminResources.InfoModelLocationRuleContactForm},
            {2, "Download Page"},
            {3, "SectionGallery"}
        };

        // SEO class has been property Id

        [Display(Name = "InfoModelName", ResourceType = typeof(AdminResources))]
        public string Name { get; set; }

        [Display(Name = "InfoModelBrief", ResourceType = typeof(AdminResources))]
        [DataType(DataType.MultilineText)]
        public string Brief { get; set; }

        [Display(Name = "InfoModelAvatar", ResourceType = typeof(AdminResources))]
        public string Avatar { get; set; }

        [AllowHtml]
        [Display(Name = "InfoModelContent", ResourceType = typeof(AdminResources))]
        [DataType(DataType.MultilineText)]
        public string Content { get; set; }


        [Display(Name = "InfoModelHref", ResourceType = typeof(AdminResources))]
        [ScaffoldColumn(false)]
        public string Href { get; set; }


        // old-code
        [ScaffoldColumn(false)]
        public string PhotosDelimiter { get; set; }


        // Location =================================================

        [Display(Name = "InfoModelLocation", ResourceType = typeof(AdminResources))]
        [DataType(cms4seoDataType.CustomEditor)]
        public int Location { get; set; }

        [Display(Name = "InfoModelSort", ResourceType = typeof(AdminResources))]
        public int Sort { get; set; }

        [ScaffoldColumn(false)]
        public Guid? ScopeId { get; set; }

        [ScaffoldColumn(false)]
        public DateTime? DateAmended { get; set; }

        [ScaffoldColumn(false)]
        public DateTime? DateCreated { get; set; }

        [ScaffoldColumn(false)]
        public bool IsCreate { get; set; }

        [ScaffoldColumn(false)]
        public string PostBy { get; set; }
    }
}