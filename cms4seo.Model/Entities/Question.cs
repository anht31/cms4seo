using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cms4seo.Model.Entities
{
    public class Question
    {
        public Question()
        {
            Answers = new HashSet<Answer>();
        }

        [Key]
        [ScaffoldColumn(false)]
        public int Id { get; set; }

        [DisplayName("Message")]
        public string Message { get; set; }

        [ScaffoldColumn(false)]
        [DisplayName("UnsignContent")]
        public string UnsignContent { get; set; }

        [ScaffoldColumn(false)]
        [DisplayName("QuestionBy")]
        public int QuestionBy { get; set; }

        [ScaffoldColumn(false)]
        [DisplayName("QuestionDate")]
        public DateTime QuestionDate { get; set; }

        // relation-Comments =========================================
        public virtual ICollection<Answer> Answers { get; set; }

        // relation-product  =============================================
        [DisplayName("Product")]
        public int Product_Id { get; set; }

        [ForeignKey("Product_Id")]
        public virtual Product Product { get; set; }
    }
}