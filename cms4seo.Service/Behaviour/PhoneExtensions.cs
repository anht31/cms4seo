using System.Text.RegularExpressions;

namespace cms4seo.Service.Behaviour
{
    public static class PhoneExtensions
    {
        /// <summary>
        /// Checks to be sure a phone number contains 10 digits as per American phone numbers.  
        /// If 'isRequired' is true, then an empty string will return False. 
        /// If 'isRequired' is false, then an empty string will return True.
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="isRequired"></param>
        /// <returns></returns>
        public static bool ValidatePhoneNumber(this string phone, bool isRequired)
        {
            if (string.IsNullOrEmpty(phone) & !isRequired)
                return true;

            if (string.IsNullOrEmpty(phone) & isRequired)
                return false;

            var cleaned = phone.RemoveNonNumeric();
            if (isRequired)
            {
                if (cleaned.Length >= 9 & cleaned.Length <= 13)
                    return true;

                return false;
            }


            if (cleaned.Length >= 9 & cleaned.Length <= 13)
                return true;

            return false;
        }

        /// <summary>
        /// Removes all non numeric characters from a string
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        public static string RemoveNonNumeric(this string phone)
        {
            return Regex.Replace(phone, @"[^0-9]+", "");
        }
    }
}