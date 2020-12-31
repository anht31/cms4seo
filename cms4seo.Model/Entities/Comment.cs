using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Admin.Resources;

namespace cms4seo.Model.Entities
{
    public class Comment
    {
        [Key]
        [ScaffoldColumn(false)]
        public int Id { get; set; }

        [Display(Name = "CommentModelMessage", ResourceType = typeof(AdminResources))]
        public string Message { get; set; }

        [ScaffoldColumn(false)]
        [Display(Name = "CommentModelSearchContent", ResourceType = typeof(AdminResources))]
        public string UnsignContent { get; set; }

        [ScaffoldColumn(false)]
        [Display(Name = "CommentModelCommentBy", ResourceType = typeof(AdminResources))]
        public int CommentedBy { get; set; }

        [ScaffoldColumn(false)]
        [Display(Name = "CommentModelCommnetDate", ResourceType = typeof(AdminResources))]
        public DateTime CommentedDate { get; set; }

        // ralation - post ===========================   
        [Display(Name = "CommentModelPostId", ResourceType = typeof(AdminResources))]
        public int Post_Id { get; set; }

        [ForeignKey("Post_Id")]
        public virtual Post Post { get; set; }
    }
}