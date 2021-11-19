using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Admin.Resources;

namespace cms4seo.Model.Entities
{
    public class ExtraSiteMap
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "ExtraSiteMapName", ResourceType = typeof(AdminResources))]
        [Required(ErrorMessageResourceType = typeof(AdminResources),
            ErrorMessageResourceName = "ExtraSiteMapNameRequired")]
        [DataType(DataType.PhoneNumber)]
        public string Name { get; set; }

        [Display(Name = "ExtraSiteMapLastModify", ResourceType = typeof(AdminResources))]
        public DateTime? LastMod { get; set; }

        [Display(Name = "ExtraSiteMapChangeFrequency", ResourceType = typeof(AdminResources))]
        public ChangeFrequency ChangeFreq { get; set; }

        [Display(Name = "ExtraSiteMapPriority", ResourceType = typeof(AdminResources))]
        [Required(ErrorMessageResourceType = typeof(AdminResources),
            ErrorMessageResourceName = "ExtraSiteMapPriorityRequired")]
        [Range(0.1, 1.0, ErrorMessageResourceType = typeof(AdminResources), 
            ErrorMessageResourceName = "ExtraSiteMapPriorityRange")]
        public double? Priority { get; set; }
    }
}