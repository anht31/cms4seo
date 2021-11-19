using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web.Mvc;
using Admin.Resources;
using cms4seo.Model.LekimaxType;
using Newtonsoft.Json;

namespace cms4seo.Model.Entities
{
    public class Article : Seo
    {
        // Tags ==========================================================

        // relation - Comments ===========================================
        //public virtual ICollection<Comment4article> Comment4articles { get; set; }

        // SEO class has been property Id

        [Display(Name = "ArticleModelName", ResourceType = typeof(AdminResources))]
        public string Name { get; set; }

        [Display(Name = "ArticleModelDescription", ResourceType = typeof(AdminResources))]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }


        [Display(Name = "ArticleModelAvatar", ResourceType = typeof(AdminResources))]
        public string Avatar { get; set; }

        [AllowHtml]
        [Display(Name = "ArticleModelContent", ResourceType = typeof(AdminResources))]
        [DataType(DataType.MultilineText)]
        public string Content { get; set; }

        [AllowHtml]
        [ScaffoldColumn(false)]
        [DataType(DataType.MultilineText)]
        // ReSharper disable once InconsistentNaming
        public string ContentWithTOC { get; set; }

        [Display(Name = "ArticleModelPostDate", ResourceType = typeof(AdminResources))]
        //[ScaffoldColumn(false)]
        [DataType(cms4seoDataType.CustomEditor)]
        public DateTime? PostedDate { get; set; }


        [Display(Name = "ArticleModelDateCreated", ResourceType = typeof(AdminResources))]
        [ScaffoldColumn(false)]
        public DateTime? DateCreated { get; set; }

        [ScaffoldColumn(false)]
        [Display(Name = "ArticleModelPostBy", ResourceType = typeof(AdminResources))]
        public string PostBy { get; set; }

        [ScaffoldColumn(false)]
        [Display(Name = "ArticleModelRating", ResourceType = typeof(AdminResources))]
        public decimal Rating { get; set; }


        [Display(Name = "ArticleModelViewCounter", ResourceType = typeof(AdminResources))]
        [ScaffoldColumn(false)]
        public int ViewCounter { get; set; }

        //========= location =============

        [Display(Name = "ArticleModelLocation", ResourceType = typeof(AdminResources))]
        [DataType(cms4seoDataType.CustomEditor)]
        public int Location { get; set; }

        [Display(Name = "ArticleModelSort", ResourceType = typeof(AdminResources))]
        public int Sort { get; set; }

        [Display(Name = "ArticleModelIsPublish", ResourceType = typeof(AdminResources))]
        [DataType(cms4seoDataType.CustomEditor)]
        public bool IsPublish { get; set; }


        // relation-Posts ==================================================
        public virtual ICollection<Post> Post { get; set; } = new HashSet<Post>();

        // relation-topic ===========================================
        [Display(Name = "ArticleModelTopicId", ResourceType = typeof(AdminResources))]
        [DataType(cms4seoDataType.CustomEditor)]
        public int? TopicId { get; set; }

        [ForeignKey("TopicId")]
        public virtual Topic Topic { get; set; }

        public IList<string> Tags { get; set; } = new List<string>();

        [ScaffoldColumn(false)]
        public string CombinedTags
        {
            get { return string.Join(",", Tags); }
            set
            {
                if (value != null) Tags = value.Split(',').Select(s => s.Trim()).ToList();
            }
        }

        // old-code
        [ScaffoldColumn(false)]
        public string PhotosDelimiter { get; set; }


        [DataType(cms4seoDataType.CustomEditor)]
        public Guid? ScopeId { get; set; }


        // productTag ==================================================

        [NotMapped]
        [Display(Name = "ArticleModelSelectedArticleTag", ResourceType = typeof(AdminResources))]
        [DataType(cms4seoDataType.CustomEditor)]
        public IList<string> SelectedArticleTag { get; set; } = new List<string>();

        [JsonIgnore]
        public virtual ICollection<ArticleTag> ArtilceTags { get; set; } = new HashSet<ArticleTag>();


        [ScaffoldColumn(false)]
        public bool IsCreate { get; set; }
    }
}