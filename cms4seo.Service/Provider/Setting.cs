using cms4seo.Data.Repositories;
using cms4seo.Model.LekimaxType;

namespace cms4seo.Service.Provider
{
    public static class Setting
    {
        static string _theme = WebSettings[WebSettingType.CurrentTheme];
        static string _language = WebSettings[WebSettingType.ShopLanguages];

        public static string Theme => 
            _theme ?? (_theme = WebSettings[WebSettingType.CurrentTheme]);

        public static string Language =>
            _language ?? (_language = WebSettings[WebSettingType.ShopLanguages]);


        /// <summary>
        /// Access sql SettingModel
        /// </summary>
        public static SettingRepository WebSettings => new SettingRepository();


        /// <summary>
        /// Access Sql Contents
        /// </summary>
        public static ContentRepository Contents => new ContentRepository(Theme, Language);



        /// <summary>
        /// Access Sql Contents For Image
        /// </summary>
        public static string ImageSource(string key)
        {
            var value = Contents[key];

            if (string.IsNullOrWhiteSpace(value))
                return
                    "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mNkYAAAAAYAAjCB0C8AAAAASUVORK5CYII=";

            return value;
        }


        public static void Clear()
        {
            _theme = null;
            _language = null;
        }
    }
} 