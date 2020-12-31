using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace cms4seo.Model.Entities
{
    public class SettingModel
    {
        [Key]
        [DisplayName("Key")]
        [Required(ErrorMessage = "You must enter key")]
        public string Key { get; set; }

        [AllowHtml]
        [DisplayName("Value")]
        //[Required(ErrorMessage = "You must enter value")]
        public string Value { get; set; }
    }
}