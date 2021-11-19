//using System.Diagnostics;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Admin.Resources;
using cms4seo.Model.LekimaxType;
using Newtonsoft.Json;


namespace cms4seo.Model.Entities
{
    public class Product : Seo
    {
        // GetLocationRule ===============================================
        [JsonIgnore]
        public Dictionary<int, string> LocationRule = new Dictionary<int, string>
        {
            {1, "Default"},
            {2, "Homepage"},
            {3, "Not use"},
            {4, "Not use"}
        };

        // SEO class has been property Id

        [Display(Name = "ProductModelName", ResourceType = typeof(AdminResources))]
        public string Name { get; set; }


        [Display(Name = "ProductModelTitle2", ResourceType = typeof(AdminResources))]
        //[ScaffoldColumn(false)]
        public string Title2 { get; set; }


        [Display(Name = "ProductModelTitle3", ResourceType = typeof(AdminResources))]
        [ScaffoldColumn(false)]
        public string Title3 { get; set; }


        [Display(Name = "ProductModelBrief", ResourceType = typeof(AdminResources))]
        [DataType(DataType.MultilineText)]
        public string Brief { get; set; }

        [AllowHtml]
        [Display(Name = "ProductModelSummary", ResourceType = typeof(AdminResources))]
        [DataType(DataType.MultilineText)]
        public string Summary { get; set; }

        [AllowHtml]
        [Display(Name = "ProductModelDescription", ResourceType = typeof(AdminResources))]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        // begin option
        [AllowHtml]
        [Display(Name = "ProductModelSpecification", ResourceType = typeof(AdminResources))]
        [DataType(DataType.MultilineText)]
        [ScaffoldColumn(false)]
        public string Specification { get; set; }

        [AllowHtml]
        [ScaffoldColumn(false)]
        [DataType(DataType.MultilineText)]
        // ReSharper disable once InconsistentNaming
        public string ContentWithTOC { get; set; }

        [Display(Name = "ProductModelAvatar", ResourceType = typeof(AdminResources))]
        public string Avatar { get; set; }


        [Display(Name = "ProductModelVolume", ResourceType = typeof(AdminResources))]
        [ScaffoldColumn(false)]
        public int Volume { get; set; }

        [Display(Name = "ProductModelDistrict", ResourceType = typeof(AdminResources))]
        [ScaffoldColumn(false)]
        public string District { get; set; }



        [NotMapped]
        [Display(Name = "ProductModelStringOldPrice", ResourceType = typeof(AdminResources))]
        [DataType(cms4seoDataType.CustomEditor)]
        public string StringOldPrice
        {
            get { return $"{OldPrice:N0}"; }
            set
            {
                if (value != null)
                {
                    double oldPrice;
                    Double.TryParse(value, NumberStyles.Any, CultureInfo.CurrentCulture, out oldPrice);
                    OldPrice = oldPrice;
                }
            }
        }

        [NotMapped]
        [Display(Name = "ProductModelStringPrice", ResourceType = typeof(AdminResources))]
        [DataType(cms4seoDataType.CustomEditor)]
        public string StringPrice
        {
            get { return $"{Price:N0}"; }
            set
            {
                if (value != null)
                {
                    double price;
                    Double.TryParse(value, NumberStyles.Any, CultureInfo.CurrentCulture, out price);
                    Price = price;
                }
            }
        }


        [Display(Name = "ProductModelOldPrice", ResourceType = typeof(AdminResources))]
        //[ScaffoldColumn(false)] // Enable this (Uncomment) will be set Hidden Field, Cause of Empty Value
        [DataType(cms4seoDataType.CustomEditor)]
        public double OldPrice { get; set; }


        [Display(Name = "ProductModelPrice", ResourceType = typeof(AdminResources))]
        //[ScaffoldColumn(false)] // Enable this (Uncomment) will be set Hidden Field, Cause of Empty Value
        [DataType(cms4seoDataType.CustomEditor)]
        public double Price { get; set; }


        [Display(Name = "ProductModelIsOutOfStock", ResourceType = typeof(AdminResources))]
        [DataType(cms4seoDataType.CustomEditor)]
        public bool IsOutOfStock { get; set; }


        [Display(Name = "ProductModelCreated", ResourceType = typeof(AdminResources))]
        [ScaffoldColumn(false)]
        public DateTime? Created { get; set; }

        [Display(Name = "ProductModelDateAmended", ResourceType = typeof(AdminResources))]
        [ScaffoldColumn(false)]
        public DateTime? DateAmended { get; set; }

        [Display(Name = "ProductModelPublished", ResourceType = typeof(AdminResources))]
        [DataType(cms4seoDataType.CustomEditor)]
        public DateTime? Published { get; set; }

        [Display(Name = "ProductModelIsPublish", ResourceType = typeof(AdminResources))]
        [DataType(cms4seoDataType.CustomEditor)]
        public bool IsPublish { get; set; }

        [Display(Name = "ProductModelIsHome", ResourceType = typeof(AdminResources))]
        [DataType(cms4seoDataType.CustomEditor)]
        public bool IsHome { get; set; }


        // Tags ==========================================================
        [ScaffoldColumn(false)]
        public IList<string> Tags { get; set; } = new List<string>();

        [ScaffoldColumn(false)]
        public string CombinedTags
        {
            get { return string.Join(",", Tags); } // 
            set
            {
                if (value != null) Tags = value.Split(',').Select(s => s.Trim()).ToList();
            }
        }

        //mask =============================================================


        [ScaffoldColumn(false)]
        [Display(Name = "ProductModelPostBy", ResourceType = typeof(AdminResources))]
        public string PostBy { get; set; }


        [ScaffoldColumn(false)]
        public Guid? ScopeId { get; set; }

        // relationship ====================================================
        [Display(Name = "ProductModelCategoryId", ResourceType = typeof(AdminResources))]
        //[Required(ErrorMessage = "please choose category")]
        [DataType(cms4seoDataType.CustomEditor)]
        public int? CategoryId { get; set; }

        [JsonIgnore]
        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }

        // relation-Question ====================================================

        [JsonIgnore]
        [ScaffoldColumn(false)]
        public virtual ICollection<Question> Questions { get; set; } = new HashSet<Question>();

        // old-code
        [ScaffoldColumn(false)]
        public string PhotosDelimiter { get; set; }


        // productTag ==================================================

        [JsonIgnore]
        [NotMapped]
        [Display(Name = "ProductModelSelectedProductTag", ResourceType = typeof(AdminResources))]
        [ScaffoldColumn(false)]
        public IList<string> SelectedProductTag { get; set; } = new List<string>();

        //[NotMapped]
        //public string CombinedProductTags
        //{
        //    get { return string.Join(",", SelectedProductTag); } // 
        //    set
        //    {
        //        if (value != null)
        //        {
        //            SelectedProductTag = value.Split(',').Select(s => s.Trim()).ToList();
        //        }
        //    }
        //}


        [JsonIgnore]
        [ScaffoldColumn(false)]
        public virtual ICollection<ProductTag> ProductTags { get; set; } = new HashSet<ProductTag>();

        // location ==============================================

        [Display(Name = "ProductModelLocation", ResourceType = typeof(AdminResources))]
        //[Required(ErrorMessage = "Please choose a location")]
        [ScaffoldColumn(false)]
        public int Location { get; set; }

        [Display(Name = "ProductModelSort", ResourceType = typeof(AdminResources))]
        public int Sort { get; set; }

        // adding-option ==================================================
        [ScaffoldColumn(false)]
        public string Text1 { get; set; }

        [ScaffoldColumn(false)]
        public string Text2 { get; set; }

        [ScaffoldColumn(false)]
        public string Text3 { get; set; }

        [ScaffoldColumn(false)]
        public string Href { get; set; }

        [ScaffoldColumn(false)]
        public bool IsCreate { get; set; }


        [Display(Name = "Product_ViewCounter", ResourceType = typeof(AdminResources))]
        [ScaffoldColumn(false)]
        public int ViewCounter { get; set; }

        /// <summary>
        ///     Relation table
        /// </summary>
        public virtual ICollection<ProductProperties> ProductProperties { get; set; }

        // for purpose ModelBinder stop point
        [NotMapped]
        [ScaffoldColumn(false)]
        public int MockProductProperties { get; set; }

        [Display(Name = "Product_IsHiddenProductSummary", ResourceType = typeof(AdminResources))]
        public bool IsHiddenProductSummary { get; set; }
    }
}