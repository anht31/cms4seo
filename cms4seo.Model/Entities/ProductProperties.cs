using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cms4seo.Model.Entities
{
    /// <summary>
    ///     Represents a Product Properties relation
    /// </summary>
    public class ProductProperties
    {
        [Key, Column(Order = 0)]
        public int ProductId { get; set; }
        [Key, Column(Order = 1)]
        public int PropertyId { get; set; }

        public virtual Product Product { get; set; }
        public virtual Property Property { get; set; }

    }
}