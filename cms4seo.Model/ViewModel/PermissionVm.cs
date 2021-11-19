using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cms4seo.Model.ViewModel
{
    public class PermissionVm
    {
        public bool HasWriteSettingFile { get; set; }
        public bool HasWriteConnectionStringFile { get; set; }
        public bool HasWriteRedirectFile { get; set; }
        public bool HasWritePhotoFolder { get; set; }
        public bool HasWriteSitemapFile { get; set; }
        public bool HasWritePluginsFolder { get; set; }
    }
}
