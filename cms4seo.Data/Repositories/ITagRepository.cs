using System.Collections.Generic;

namespace cms4seo.Data.Repositories
{
    public interface ITagRepository
    {
        /// <summary>
        ///     Get all tag of Product
        /// </summary>
        /// <returns>list of string tag</returns>
        IEnumerable<string> GetAll();

        /// <summary>
        ///     check tag is esit or not
        /// </summary>
        /// <param name="tag">tag</param>
        /// <returns>tag</returns>
        string Get(string tag);

        /// <summary>
        ///     Edit current tag
        /// </summary>
        /// <param name="existingTag">exit tag</param>
        /// <param name="newTag">new tag</param>
        void Edit(string existingTag, string newTag);

        /// <summary>
        ///     Delete tag all in products
        /// </summary>
        /// <param name="tag">tag name</param>
        void Delete(string tag);
    }
}