using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;
using Admin.Resources;

namespace cms4seo.Model.Entities
{
    public class Category : Seo
    {
        // Old Modal - Migration to WebSetting Option ====================================
        [ScaffoldColumn(false)]
        public Dictionary<int, string> LocationRule = new Dictionary<int, string>
        {
            {0, "Default"},
            {1, "Services"},
            {2, "Portfolio"},
            {3, "Timeline"}
        };

        // SEO class has been property Id

        [Display(Name = "CategoryModelName", ResourceType = typeof(AdminResources))]
        public string Name { get; set; }


        [Display(Name = "CategoryModelDescription", ResourceType = typeof(AdminResources))]
        public string Description { get; set; }

        [AllowHtml]
        [ScaffoldColumn(false)]
        [DataType(DataType.MultilineText)]
        public string Content { get; set; }

        [Display(Name = "CategoryModelAvatar", ResourceType = typeof(AdminResources))]
        public string Avatar { get; set; }


        [Display(Name = "CategoryModelHref", ResourceType = typeof(AdminResources))]
        public string Href { get; set; }

        // relation - Parent =======================================
        [Display(Name = "CategoryModelParentId", ResourceType = typeof(AdminResources))]
        public int? ParentId { get; set; }

        [ForeignKey("ParentId")]
        [ScaffoldColumn(false)]
        public virtual Category Parent { get; set; }

        // relation - Children =======================================
        [ScaffoldColumn(false)]
        public virtual ICollection<Category> Children { get; set; } = new HashSet<Category>();

        // relation - product =======================================
        [ScaffoldColumn(false)]
        public virtual ICollection<Product> Products { get; set; } = new HashSet<Product>();

        // old-code
        [ScaffoldColumn(false)]
        public string PhotosDelimiter { get; set; }


        // Location =================================================
        [Display(Name = "CategoryModelIsMenu", ResourceType = typeof(AdminResources))]
        public bool IsMenu { get; set; }

        [Display(Name = "CategoryModelIsHome", ResourceType = typeof(AdminResources))]
        public bool IsHome { get; set; }

        [Display(Name = "CategoryModelIsAside", ResourceType = typeof(AdminResources))]
        public bool IsAside { get; set; }

        [Display(Name = "CategoryModelLocation", ResourceType = typeof(AdminResources))]
        [Required(ErrorMessageResourceType = typeof(AdminResources), 
            ErrorMessageResourceName = "CategoryModelLocationRequired")]
        public int Location { get; set; }

        [Display(Name = "CategoryModelSort", ResourceType = typeof(AdminResources))]
        public int Sort { get; set; }


        //mask =============================================================

        public Guid? ScopeId { get; set; }

        public DateTime? DateAmended { get; set; }
        public DateTime? DateCreated { get; set; }


        public bool IsCreate { get; set; }

        [ScaffoldColumn(false)]
        public string PostBy { get; set; }
    }
}