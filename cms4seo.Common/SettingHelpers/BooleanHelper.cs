using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc.Html;
using cms4seo.Model.LekimaxType;

namespace cms4seo.Common.SettingHelpers
{
    public static class BooleanHelper
    {
        public static bool ToBoolean(this string value)
        {
            bool output;

            var result = Boolean.TryParse(value, out output);

            if (result)
                return output;

            return false;
        }

    }
}
