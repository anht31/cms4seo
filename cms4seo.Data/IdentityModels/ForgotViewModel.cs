using System.ComponentModel.DataAnnotations;

namespace IdentitySample.Models
{
    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}