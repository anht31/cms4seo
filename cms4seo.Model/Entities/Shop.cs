using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Admin.Resources;

namespace cms4seo.Model.Entities
{
    public class Shop
    {
        // GetLocationRule ===============================================
        public Dictionary<int, string> Provinces = new Dictionary<int, string>
        {
            {1, "Province 1"},
            {2, "Province 2"},
            {3, "Province 3"},
            {4, "Province 4"},
            {5, "Province 5"},
     
        };

        public int Id { get; set; }

        [Display(Name = "ShopModelProvince", ResourceType = typeof(AdminResources))]
        [Required(ErrorMessageResourceType = typeof(AdminResources),
            ErrorMessageResourceName = "ShopModelProvinceRequired")]
        public string Province { get; set; }

        [Display(Name = "ShopModelName", ResourceType = typeof(AdminResources))]
        public string Name { get; set; }

        public string Address { get; set; }

        public string Phone { get; set; }

        public string ActiveTime { get; set; }

        public string AllContent { get; set; }
    }
}