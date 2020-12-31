using System.ComponentModel.DataAnnotations;

namespace cms4seo.Model.Entities
{
    /// <summary>
    ///     abstract class to common entity
    /// </summary>
    public abstract class BaseEntity
    {
        [Key]
        [ScaffoldColumn(false)]
        public virtual int Id { get; set; }
    }
}