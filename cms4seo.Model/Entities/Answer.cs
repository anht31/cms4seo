using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cms4seo.Model.Entities
{
    public class Answer
    {
        [Key]
        [ScaffoldColumn(false)]
        public int Id { get; set; }

        [DisplayName("Message")]
        public string Message { get; set; }

        [DisplayName("Unsigned Content")]
        [ScaffoldColumn(false)]
        public string UnsignContent { get; set; }

        [ScaffoldColumn(false)]
        [DisplayName("Answer By")]
        public int AnswerBy { get; set; }

        [ScaffoldColumn(false)]
        [DisplayName("Answer Date")]
        public DateTime AnswerDate { get; set; }

        // relation-Question ===========================   
        [DisplayName("Question")]
        public int Question_Id { get; set; }

        [ForeignKey("Question_Id")]
        public virtual Question Question { get; set; }
    }
}