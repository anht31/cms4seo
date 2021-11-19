using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace cms4seo.Common.Helpers
{
    public static class StringHelpers
    {
        /// <summary>
        ///     convert html string to text-only lowercase
        /// </summary>
        /// <param name="strHtml">html string</param>
        /// <returns>text-only with lowercase, return empty if length below 5</returns>
        public static string UnsignConvert(this string strHtml)
        {
            if (strHtml == null)
            {
                return "";
            }

            if (strHtml.Length < 5)
            {
                return "";
            }

            var dirtyString = WebUtility.HtmlDecode(strHtml.StripHTML());
            //remove new line break, multiple space to one space
            var clearedString = Regex.Replace(dirtyString, @"\s+", " ");

            return " " + clearedString.ToLower().ReplaceDiacritics();
        }

        public static string StripHTML(this string HTMLText)
        {
            if (!string.IsNullOrEmpty(HTMLText))
            {
                var reg = new Regex("<.*?>", RegexOptions.IgnoreCase);
                return reg.Replace(HTMLText, "");
            }
            return "";
        }


        //remove Diacritic of letter-diacritic
        public static string ReplaceDiacritics(this string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                return "";
            }
            var sourceInFormD = source.Normalize(NormalizationForm.FormD);

            var output = new StringBuilder();
            foreach (var c in sourceInFormD)
            {
                var uc = CharUnicodeInfo.GetUnicodeCategory(c);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    // special fix
                    var cc = c;
                    if (c == 'Đ')
                        cc = 'D';
                    if (c == 'đ')
                        cc = 'd';

                    output.Append(cc);
                }
            }

            return output.ToString().Normalize(NormalizationForm.FormC);
        }

        // string ================================================================
        public static string ShortenText50(this string LongText)
        {
            if (LongText == null)
            {
                return LongText;
            }
            if (LongText.Length > 50)
            {
                return LongText.Substring(0, 50) + "...";
            }
            return LongText;
        }

        
        public static string ShortenText100(this string LongText)
        {
            if (LongText == null)
            {
                return LongText;
            }
            if (LongText.Length > 100)
            {
                return LongText.Substring(0, 100) + "...";
            }
            return LongText;
        }

        public static string ShortenText150(this string LongText)
        {
            if (LongText == null)
            {
                return LongText;
            }
            if (LongText.Length > 150)
            {
                return LongText.Substring(0, 150) + "...";
            }
            return LongText;
        }

        public static string ShortenText200(this string LongText)
        {
            if (LongText == null)
            {
                return LongText;
            }
            if (LongText.Length > 200)
            {
                return LongText.Substring(0, 200) + "...";
            }
            return LongText;
        }


        public static string ShortenText320(this string LongText)
        {
            if (LongText == null)
            {
                return LongText;
            }
            if (LongText.Length > 320)
            {
                return LongText.Substring(0, 317) + "...";
            }
            return LongText;
        }


        public static bool SupportPhoto(this string key)
        {
            string[] _support = { "Infos.Banner", "Infos.Decorate" };
            if (!string.IsNullOrWhiteSpace(key) && _support.Contains(key))
            {
                return true;                
            }
            return false;
        }
    }
    

    // string ================================================================
    public static class StringExtensions
    {
        public static string MakeUrlFriendly(this string value)
        {
            value = value.ReplaceDiacritics();
            value = value.ToLowerInvariant().Replace(" ", "-");
            value = Regex.Replace(value, @"[^0-9a-z-]", string.Empty);

            return "/" + value;
        }


        public static string MakeNameFriendly(this string value)
        {
            value = value.Trim(); // Trim white space
            value = value.ReplaceDiacritics();
            value = value.ToLowerInvariant().Replace(" ", "-");
            value = Regex.Replace(value, @"[^0-9a-z-]", string.Empty);

            // match repeat '-' character and pretty it
            var pattern = @"(-)\1{1,}";
            value = Regex.Replace(value, pattern, "-");

            return value;
        }

        public static string RemoveSpace(this string value)
        {
            if (value != null)
                return value.Replace(" ", "");

            return "";
        }


        public static string StripHyperlinks(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return "";

            return Regex.Replace(value, "</?(a|A).*?>", "");
        }


        public static string InjectHyperlink(this string htmlContent, string injectText, string url)
        {
            // StripHyperlinks first  
            var clearHtmlContent = htmlContent.StripHyperlinks();

            string htmlDecode = WebUtility.HtmlDecode(clearHtmlContent);

            //return htmlDecode?.Replace(injectText, $"<a href='{url}' style='color:inherit;'>{injectText}</a>");

            return htmlDecode?.Replace(injectText, $"<a href='{url}'>{injectText}</a>");
        }

        public static bool Validation(this string value)
        {
            return value != null && value.Trim() != string.Empty;
        }



        public static Dictionary<int, string> ToOptionsDictionary(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            var list = value.Split(',').ToList();

            var i = 0;

            Dictionary<int, string> locationRule = new Dictionary<int, string>();

            foreach (var name in list)
            {
                locationRule.Add(i++,name);
            }

            return locationRule;
        }


        public static string GetAlt(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            var imgName = value.Split('/').Last();

            if (string.IsNullOrWhiteSpace(imgName))
                return string.Empty;

            return imgName.Split('.').First();
        }

        /// <summary>
        /// Desire Image Small Size
        /// </summary>
        /// <param name="value">sting or rich text</param>
        /// <returns></returns>
        public static string Small(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            if (value.Contains("-md.jpg"))
                return value.Replace("-md.jpg", "-sm.jpg");

            if (value.Contains("-lg.jpg"))
                return value.Replace("-lg.jpg", "-sm.jpg");

            if (value.Contains("-md.png"))
                return value.Replace("-md.png", "-sm.png");

            if (value.Contains("-lg.png"))
                return value.Replace("-lg.png", "-sm.png");

            return value;
        }


        /// <summary>
        /// Desire Image Medium Size
        /// </summary>
        /// <param name="value">sting or rich text</param>
        /// <returns></returns>
        public static string Medium(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            if (value.Contains("-sm.jpg"))
                return value.Replace("-sm.jpg", "-md.jpg");

            if (value.Contains("-lg.jpg"))
                return value.Replace("-lg.jpg", "-md.jpg");

            if (value.Contains("-sm.png"))
                return value.Replace("-sm.png", "-md.png");

            if (value.Contains("-lg.png"))
                return value.Replace("-lg.png", "-md.png");

            return value;
        }


        /// <summary>
        /// Desire Image Large Size
        /// </summary>
        /// <param name="value">sting or rich text</param>
        /// <returns></returns>
        public static string Large(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            if (value.Contains("-sm.jpg"))
                return value.Replace("-sm.jpg", "-lg.jpg");

            if (value.Contains("-md.jpg"))
                return value.Replace("-md.jpg", "-lg.jpg");

            if (value.Contains("-sm.png"))
                return value.Replace("-sm.png", "-lg.png");

            if (value.Contains("-md.png"))
                return value.Replace("-md.png", "-lg.png");

            return value;
        }


        /// <summary>
        /// Desire Image Huge Size
        /// </summary>
        /// <param name="value">sting or rich text</param>
        /// <returns></returns>
        public static string Huge(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            if (value.Contains("-sm.jpg"))
                return value.Replace("-sm.jpg", "-xl.jpg");

            if (value.Contains("-md.jpg"))
                return value.Replace("-md.jpg", "-xl.jpg");

            if (value.Contains("-lg.jpg"))
                return value.Replace("-lg.jpg", "-xl.jpg");

            if (value.Contains("-sm.png"))
                return value.Replace("-sm.png", "-xl.png");

            if (value.Contains("-md.png"))
                return value.Replace("-md.png", "-xl.png");

            if (value.Contains("-lg.png"))
                return value.Replace("-lg.png", "-xl.png");

            if (value.Contains(".jpg"))
                return value.Replace(".jpg", "-xl.jpg");

            if (value.Contains(".png"))
                return value.Replace(".png", "-xl.png");

            return value;
        }



        /// <summary>
        /// Desire Image Small Size
        /// </summary>
        /// <param name="value">rich text</param>
        /// <returns></returns>
        public static string AllSmall(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            if (value.Contains("-md.jpg"))
                value = value.Replace("-md.jpg", "-sm.jpg");

            if (value.Contains("-lg.jpg"))
                return value.Replace("-lg.jpg", "-sm.jpg");

            return value;
        }


        /// <summary>
        /// Desire Image Medium Size
        /// </summary>
        /// <param name="value">rich text</param>
        /// <returns></returns>
        public static string AllMedium(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            if (value.Contains("-sm.jpg"))
                value = value.Replace("-sm.jpg", "-md.jpg");

            if (value.Contains("-lg.jpg"))
                return value.Replace("-lg.jpg", "-md.jpg");

            return value;
        }


        /// <summary>
        /// Desire Image Large Size
        /// </summary>
        /// <param name="value">rich text</param>
        /// <returns></returns>
        public static string AllLarge(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            if (value.Contains("-sm.jpg"))
                value = value.Replace("-sm.jpg", "-lg.jpg");

            if (value.Contains("-md.jpg"))
                return value.Replace("-md.jpg", "-lg.jpg");


            return value;
        }
    }
}