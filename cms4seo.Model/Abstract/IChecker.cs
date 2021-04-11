using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cms4seo.Model.Abstract
{
    public interface IChecker
    {
        bool IsInvalidIp(string ipAddress);
    }
}
