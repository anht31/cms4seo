using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Admin.Resources;
using cms4seo.Model.LekimaxType;

namespace cms4seo.Model.ViewModel
{
    public class EmailSettingVm
    {
        [ScaffoldColumn(false)]
        public string Id { get; set; }

        [Display(Name = "EmailSettingVmSubject", ResourceType = typeof(AdminResources))]
        public string Subject { get; set; }

        [Display(Name = "EmailSettingVmMailFromAddress", ResourceType = typeof(AdminResources))]
        public string MailFromAddress { get; set; }

        [Display(Name = "EmailSettingVmMailToAddress", ResourceType = typeof(AdminResources))]
        public string MailToAddress { get; set; }

        [Display(Name = "EmailSettingVmMailToAddress2", ResourceType = typeof(AdminResources))]
        public string MailToAddress2 { get; set; }

        [Display(Name = "EmailSettingVmMailToAddressBcc", ResourceType = typeof(AdminResources))]
        public string MailToAddressBcc { get; set; }


        [Display(Name = "EmailSettingVmServerName", ResourceType = typeof(AdminResources))]
        public string ServerName { get; set; }

        [Display(Name = "EmailSettingVmServerPort", ResourceType = typeof(AdminResources))]
        public int ServerPort { get; set; }

        [Display(Name = "EmailSettingVmUsername", ResourceType = typeof(AdminResources))]
        public string Username { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "EmailSettingVmPassword", ResourceType = typeof(AdminResources))]
        public string Password { get; set; }

        [Display(Name = "EmailSettingVmUseSsl", ResourceType = typeof(AdminResources))]
        public bool UseSsl { get; set; }

        [Display(Name = "EmailSettingVmWriteAsFile", ResourceType = typeof(AdminResources))]
        public bool WriteAsFile { get; set; }

        [Display(Name = "EmailSettingVmFileLocation", ResourceType = typeof(AdminResources))]
        public string FileLocation { get; set; }


        public Dictionary<int, string> SaveModes = new Dictionary<int, string>
        {
            {0, AdminResources.EmailSettingVm_SaveModes_Alway},
            {1, AdminResources.EmailSettingVm_SaveModes_JustSaveSpamMail},
        };

        [Display(Name = "EmailSettingVmSaveMode", ResourceType = typeof(AdminResources))]
        [DataType(cms4seoDataType.CustomEditor)]
        public int SaveMode { get; set; }

    }
}
