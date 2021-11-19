using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;
using Admin.Resources;
using cms4seo.Model.LekimaxType;

namespace cms4seo.Model.Entities
{
    public class Topic : Seo
    {
        // GetLocationRule ===============================================
        public Dictionary<int, string> LocationRule = new Dictionary<int, string>
        {
            {0, "-- Unset --"},
            {1, "Location 1"},
            {2, "Location 2"},
            {3, "Location 3"}
        };


        [Display(Name = "TopicModelName", ResourceType = typeof(AdminResources))]
        //[Required(ErrorMessage = "Please enter topic name")]
        public string Name { get; set; }

        [Display(Name = "TopicModelDescription", ResourceType = typeof(AdminResources))]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [AllowHtml]
        [ScaffoldColumn(false)]
        [DataType(DataType.MultilineText)]
        public string Content { get; set; }

        [Display(Name = "TopicModelAvatar", ResourceType = typeof(AdminResources))]
        public string Avatar { get; set; }


        //mask ======================================================

        [ScaffoldColumn(false)]
        public Guid? ScopeId { get; set; }

        // Location =================================================
        [Display(Name = "TopicModelIsHome", ResourceType = typeof(AdminResources))]
        [DataType(cms4seoDataType.CustomEditor)]
        public bool IsHome { get; set; }

        [Display(Name = "TopicModelLocation", ResourceType = typeof(AdminResources))]
        [DataType(cms4seoDataType.CustomEditor)]
        public int Location { get; set; }

        [Display(Name = "TopicModelSort", ResourceType = typeof(AdminResources))]
        public int Sort { get; set; }


        // relation - Parent =======================================
        [Display(Name = "TopicModelParentId", ResourceType = typeof(AdminResources))]
        [DataType(cms4seoDataType.CustomEditor)]
        public int? ParentId { get; set; }

        [ForeignKey("ParentId")]
        public virtual Topic Parent { get; set; }

        // relation - Children =======================================
        public virtual ICollection<Topic> Children { get; set; } = new HashSet<Topic>();

        // relation-Article =======================================
        public virtual ICollection<Article> Articles { get; set; } = new HashSet<Article>();


        // old-code
        [ScaffoldColumn(false)]
        public string PhotosDelimiter { get; set; }


        [Display(Name = "TopicModelIsMenu", ResourceType = typeof(AdminResources))]
        [DataType(cms4seoDataType.CustomEditor)]
        public bool IsMenu { get; set; }

        [Display(Name = "TopicModelHref", ResourceType = typeof(AdminResources))]
        public string Href { get; set; }

        [ScaffoldColumn(false)]
        public DateTime? DateAmended { get; set; }

        [ScaffoldColumn(false)]
        public DateTime? DateCreated { get; set; }

        [ScaffoldColumn(false)]
        public bool IsCreate { get; set; }

        [ScaffoldColumn(false)]
        public string PostBy { get; set; }

        // SEO class has been property Id
    }
}