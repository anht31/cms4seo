using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace cms4seo.Model.Entities
{
    /// <summary>
    ///     Represents a product tag
    /// </summary>
    public class ProductTag
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
        ///     Gets or sets the products
        /// </summary>
        public virtual ICollection<Product> Products { get; set; } = new HashSet<Product>();
    }
}