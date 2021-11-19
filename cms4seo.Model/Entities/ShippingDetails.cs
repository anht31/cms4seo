using System.ComponentModel.DataAnnotations;

namespace cms4seo.Model.Entities
{
    public class ShippingDetails
    {

        [Display(Name = "Họ Tên")]
        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        public string Name { get; set; }

        [Display(Name = "Địa chỉ")]
        [Required(ErrorMessage = "Vui lòng nhập địa chỉ")]
        public string Address { get; set; }


        [Display(Name = "Số điện thoại")]
        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        public string Phone { get; set; }

        [Display(Name = "Địa chỉ email")]
        [Required(ErrorMessage = "Vui lòng nhập địa chỉ email")]
        public string Email { get; set; }




        
        [Display(Name = "Line 1")]
        public string Line1 { get; set; }

        [Display(Name = "Line 2")]
        public string Line2 { get; set; }

        [Display(Name = "Line 3")]
        public string Line3 { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Zip { get; set; }

        public string Country { get; set; }

        public bool GiftWrap { get; set; }
    }
}