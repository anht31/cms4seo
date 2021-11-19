using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cms4seo.Data.Repositories
{
    public interface ISettingRepository
    {
        string Get(string key);

        bool? Set(string key, string value);

        bool InitializationEmail();
        bool InitializationPhoto();
        bool InitializationWebSetting();
        bool InitializationEmbed();
        bool InitializationShop();
        bool InitializationToc();
    }
}
