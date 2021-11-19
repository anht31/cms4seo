using System;
using System.ComponentModel.DataAnnotations;

namespace cms4seo.Model.Entities
{
    public class UserCounter
    {
        [Key]
        public int Id { get; set; }

        public string UserGuid { get; set; }
        public DateTime OpenTime { get; set; }
    }
}