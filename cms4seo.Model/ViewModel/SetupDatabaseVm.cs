using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cms4seo.Model.LekimaxType;

namespace cms4seo.Model.ViewModel
{
    public class SetupDatabaseVm
    {

        [DataType(cms4seoDataType.CustomEditor)]
        public string ProjectId { get; set; }

        [DisplayName("SQL Server")]
        [Required(ErrorMessage = "Please enter SQL Server")]
        public string DataSource { get; set; }

        [DisplayName("Database Name")]
        [Required(ErrorMessage = "Please enter Database Name")]
        public string InitialCatalog { get; set; }

        [DisplayName("SQL User")]
        [Required(ErrorMessage = "Please enter User (Permissible Access Database Name")]
        public string User { get; set; }

        [DisplayName("SQL Password")]
        [Required(ErrorMessage = "Please enter Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool TryCreateDatabase { get; set; }

        [DisplayName("Force Seed Admin (this function for Exist Database only)")]
        public bool ForceSeedAdmin { get; set; }

        [DataType(cms4seoDataType.CustomEditor)]
        public string IntegratedSecurity { get; set; }
    }
}
