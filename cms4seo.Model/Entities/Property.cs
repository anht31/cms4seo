using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace cms4seo.Model.Entities
{
    /// <summary>
    ///     Represents a Property tag
    /// </summary>
    public class Property
    {
        /// <summary>
        ///     Gets or sets the Id
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        ///     Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the name
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        ///     Relation table
        /// </summary>
        public virtual ICollection<ProductProperties> ProductProperties { get; set; }
    }
}