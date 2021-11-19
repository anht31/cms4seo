using System.Collections.Generic;
using cms4seo.Common.Helpers;

namespace cms4seo.Common.Slug
{
    public class SlugComparer : IEqualityComparer<string>
    {
        public bool Equals(string tag1, string tag2)
        {
            return tag1.MakeUrlFriendly() == tag2.MakeUrlFriendly();
        }

        /// <summary>
        /// Return a hash that reflects the comparison criteria. According to the
        /// rules for IEqualityComparer<T>, if Equals is true, then the hash codes must
        /// also be equal. Because equality as defined here is a simple value equality, not
        /// reference identity, it is possible that two or more objects will produce the same hash code
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public int GetHashCode(string tag)
        {
            string s = $"{tag.MakeUrlFriendly()}";
            return s.GetHashCode();
        }
    }
}