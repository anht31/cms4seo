using System.ComponentModel.DataAnnotations;

namespace IdentitySample.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}