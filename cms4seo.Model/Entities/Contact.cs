using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Admin.Resources;

namespace cms4seo.Model.Entities
{
    public class Contact
    {
        [Key]
        [ScaffoldColumn(false)]
        public int Id { get; set; }

        [Display(Name = "ContactModelFullName", ResourceType = typeof(AdminResources))]
        public string FullName { get; set; }

        [Display(Name = "ContactModelPhone", ResourceType = typeof(AdminResources))]
        [Required(ErrorMessageResourceType = typeof(AdminResources),
            ErrorMessageResourceName = "ContactModelPhoneRequired")]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }

        //============= is read ===============
        [ScaffoldColumn(false)]
        [Display(Name = "ContactModelPostDate", ResourceType = typeof(AdminResources))]
        public DateTime PostDate { get; set; }

        [ScaffoldColumn(false)]
        [Display(Name = "ContactModelReadBy", ResourceType = typeof(AdminResources))]
        public string ReadBy { get; set; }

        //============= content for send mail ===============        
        [Display(Name = "ContactModelEmail", ResourceType = typeof(AdminResources))]
        public string Email { get; set; }


        [Display(Name = "ContactModelMessage", ResourceType = typeof(AdminResources))]
        public string Message { get; set; }


        #region pawn

        public string HowMuch { get; set; }

        public string HowLong { get; set; }

        public string Asset { get; set; }

        public string Province { get; set; }

        #endregion
    }
}