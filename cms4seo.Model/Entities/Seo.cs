using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;
using Admin.Resources;
using cms4seo.Model.LekimaxType;
using Newtonsoft.Json;

namespace cms4seo.Model.Entities
{
    /// <summary>
    ///     Base Seo class for Meta tag
    /// </summary>
    public class Seo : BaseEntity
    {
        // BaseEntity class has been property Id

        // seo ===========================================================
        [AllowHtml] // fix something containing < or > in a page
        [ScaffoldColumn(false)]
        [Display(Name = "SeoModelSearchContent", ResourceType = typeof(AdminResources))]
        public string UnsignContent { get; set; }

        [Display(Name = "SeoModelMetaKeywords", ResourceType = typeof(AdminResources))]
        [DataType(cms4seoDataType.CustomEditor)]
        public string MetaKeyWords { get; set; }

        [Display(Name = "SeoModelMetaDescription", ResourceType = typeof(AdminResources))]
        [DataType(cms4seoDataType.CustomEditor)]
        public string MetaDescription { get; set; }

        [Display(Name = "SeoModelMetaTitle", ResourceType = typeof(AdminResources))]
        [DataType(cms4seoDataType.CustomEditor)]
        public string MetaTitle { get; set; }

        [ScaffoldColumn(false)]
        [Display(Name = "SeoModelLockSeo", ResourceType = typeof(AdminResources))]
        public bool IsLockSeo { get; set; }


        [ScaffoldColumn(false)]
        public string Slug { get; set; }

        

        [JsonIgnore]
        [ScaffoldColumn(false)]
        public virtual ICollection<Photo> Photos { get; set; } = new HashSet<Photo>();
    }
}