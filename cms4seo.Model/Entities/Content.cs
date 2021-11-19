using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace cms4seo.Model.Entities
{
    public class Content
    {
        [Key]
        [Column(Order = 0)]
        [DisplayName("Key")]
        [Required(ErrorMessage = "You must enter key")]
        public string Key { get; set; }

        [Key]
        [Column(Order = 1)]
        [DisplayName("Theme")]
        public string Theme { get; set; }

        [Key]
        [Column(Order = 2)]
        [DisplayName("Language")]
        public string Language { get; set; }

        [DisplayName("Unassigned")]
        public bool Unassigned { get; set; }

        [AllowHtml]
        [DisplayName("Value")]
        public string Value { get; set; }
    }
}
