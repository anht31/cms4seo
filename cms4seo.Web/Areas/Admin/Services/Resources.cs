using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Admin.Resources;
using Newtonsoft.Json;

namespace cms4seo.Admin.Services
{
    public static class Resources
    {
        /// <summary>
        /// Get all string for Localization Scripts (Localization will base of main query page)
        /// </summary>
        /// <returns></returns>
        public static string Gets()
        {
            var resources = typeof(AdminResources)
                 .GetProperties()
                 .Where(p => p.Name.Contains("js_")) // Skip the properties you don't need on the client side.
                 .ToDictionary(p => p.Name, p => p.GetValue(null) as string);

            //resources.Add("js_DataTableLanguage", Common.Culture.CultureHelper.GetLanguageWithoutCountry());

            return JsonConvert.SerializeObject(resources);
        }
    }
}