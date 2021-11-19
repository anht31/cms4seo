using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace cms4seo.Model.Entities
{
    public class ArticleTag
    {
        /// <summary>
        ///     Gets or sets the Id
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        ///     Gets or sets the Slug
        /// </summary>
        public string Slug { get; set; }

        /// <summary>
        ///     Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the Articles
        /// </summary>
        public virtual ICollection<Article> Articles { get; set; } = new HashSet<Article>();
    }
}