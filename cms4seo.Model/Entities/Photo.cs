using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Admin.Resources;
using Newtonsoft.Json;

namespace cms4seo.Model.Entities
{
    /// <summary>
    ///     Share class
    /// </summary>
    public class Photo
    {
        [Key]
        [ScaffoldColumn(false)]
        public int Id { get; set; }

        [Display(Name = "PhotoModelName", ResourceType = typeof(AdminResources))]
        public string Name { get; set; }

        [Display(Name = "PhotoModelMineType", ResourceType = typeof(AdminResources))]
        public string MimeType { get; set; }

        [Display(Name = "PhotoModelEntity", ResourceType = typeof(AdminResources))]
        public string Entity { get; set; }

        [Display(Name = "PhotoModelModelId", ResourceType = typeof(AdminResources))]
        public int ModelId { get; set; }

        [Display(Name = "PhotoModelAltAttribute", ResourceType = typeof(AdminResources))]
        public string AltAttribute { get; set; }

        [Display(Name = "PhotoModelTitleAttribute", ResourceType = typeof(AdminResources))]
        public string TitleAttribute { get; set; }

        // path =========================================================
        [Display(Name = "PhotoModelLgPath", ResourceType = typeof(AdminResources))]
        public string LgPath { get; set; }

        [Display(Name = "PhotoModelMdPath", ResourceType = typeof(AdminResources))]
        public string MdPath { get; set; }

        [Display(Name = "PhotoModelSmPath", ResourceType = typeof(AdminResources))]
        public string SmPath { get; set; }

        [Display(Name = "PhotoModelPostBy", ResourceType = typeof(AdminResources))]
        public string PostBy { get; set; }

        // manager old-code ========================================================

        [ScaffoldColumn(false)]
        public int Sort { get; set; }


        [ScaffoldColumn(false)]
        public bool IsDelete { get; set; }


        [ScaffoldColumn(false)]
        public bool IsEditMode { get; set; }


        // mask ========================================================
        [ScaffoldColumn(false)]
        public Statuses Status { get; set; }

        [ScaffoldColumn(false)]
        public string ProcessingBy { get; set; }

        [ScaffoldColumn(false)]
        public Guid? ScopeId { get; set; }



        #region relationShip

        [JsonIgnore]
        public virtual ICollection<Category> Categories { get; set; } = new HashSet<Category>();

        [JsonIgnore]
        public virtual ICollection<Product> Products { get; set; } = new HashSet<Product>();

        [JsonIgnore]
        public virtual ICollection<Info> Infos { get; set; } = new HashSet<Info>();

        [JsonIgnore]
        public virtual ICollection<Slider> Sliders { get; set; } = new HashSet<Slider>();

        [JsonIgnore]
        public virtual ICollection<Topic> Topics { get; set; } = new HashSet<Topic>();

        [JsonIgnore]
        public virtual ICollection<Article> Articles { get; set; } = new HashSet<Article>();

        #endregion
    }
}