using System.Text.RegularExpressions;

namespace Lekimax.Helpers
{
    public static class StringExtensions
    {
        public static string MakeUrlFriendly(this string value)
        {
            value = value.ReplaceDiacritics();
            value = value.ToLowerInvariant().Replace(" ", "-");
            value = Regex.Replace(value, @"[^0-9a-z-]", string.Empty);

            return value;
        }
    }
}