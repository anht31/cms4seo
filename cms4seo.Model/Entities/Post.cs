using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Admin.Resources;

namespace cms4seo.Model.Entities
{
    public class Post
    {
        public Post()
        {
            Comments = new HashSet<Comment>();
        }

        [Key]
        [ScaffoldColumn(false)]
        public int Id { get; set; }

        [Display(Name = "PostModelMessage", ResourceType = typeof(AdminResources))]
        public string Message { get; set; }

        [ScaffoldColumn(false)]
        [Display(Name = "PostModelSearchContent", ResourceType = typeof(AdminResources))]
        public string UnsignContent { get; set; }

        [ScaffoldColumn(false)]
        [Display(Name = "PostModelPostBy", ResourceType = typeof(AdminResources))]
        public int PostedBy { get; set; }

        [ScaffoldColumn(false)]
        [Display(Name = "PostModelPostDate", ResourceType = typeof(AdminResources))]
        public DateTime PostedDate { get; set; }

        // relation - Comments =========================================
        public virtual ICollection<Comment> Comments { get; set; }

        // relation - article  =============================================
        [Display(Name = "PostModelArticle", ResourceType = typeof(AdminResources))]
        public int Article_Id { get; set; }

        [ForeignKey("Article_Id")]
        public virtual Article Article { get; set; }
    }
}