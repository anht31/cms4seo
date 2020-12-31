using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cms4seo.Common.Helpers
{
    [Localizable(true)]
    public static class LocalizeString
    {
        public static string Format(string format, params object[] args)
        {
            return String.Format(format, args);
        }

    }
}
