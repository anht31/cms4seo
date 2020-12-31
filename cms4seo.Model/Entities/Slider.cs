using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Admin.Resources;
using cms4seo.Model.LekimaxType;
using Newtonsoft.Json;

namespace cms4seo.Model.Entities
{
    public class Slider
    {
        [Key]
        [ScaffoldColumn(false)]
        public int Id { get; set; }

        [Display(Name = "SliderModelIsCaption", ResourceType = typeof(AdminResources))]
        public bool IsCaption { get; set; }


        [Display(Name = "SliderModelName", ResourceType = typeof(AdminResources))]
        public string Name { get; set; }

        [ScaffoldColumn(false)]
        [Display(Name = "SliderModelLogo", ResourceType = typeof(AdminResources))]
        public string Logo { get; set; }


        [Display(Name = "SliderModelText1", ResourceType = typeof(AdminResources))]
        public string Text1 { get; set; }

        [Display(Name = "SliderModelText2", ResourceType = typeof(AdminResources))]
        public string Text2 { get; set; }

        [Display(Name = "SliderModelText3", ResourceType = typeof(AdminResources))]
        public string Text3 { get; set; }

        //========= photo =============

        [Display(Name = "SliderModelHref", ResourceType = typeof(AdminResources))]
        public string Href { get; set; }


        [Display(Name = "SliderModelAvatar", ResourceType = typeof(AdminResources))]
        public string Avatar { get; set; }


        [Display(Name = "SliderModelAvatarMobile", ResourceType = typeof(AdminResources))]
        public string AvatarMobile { get; set; }

        [JsonIgnore]
        [ScaffoldColumn(false)]
        public virtual ICollection<Photo> Photos { get; set; } = new HashSet<Photo>();

        [Display(Name = "SliderModelAlt", ResourceType = typeof(AdminResources))]
        public string Alt { get; set; }

        //========= manage =============

        [Display(Name = "SliderModelSort", ResourceType = typeof(AdminResources))]
        [DataType(cms4seoDataType.CustomEditor)]
        public int Sort { get; set; }

        [ScaffoldColumn(false)]
        public bool IsPublish { get; set; }

        [ScaffoldColumn(false)]
        public string Active { get; set; }


        //mask =============================================================

        [ScaffoldColumn(false)]
        public Guid? ScopeId { get; set; }


        [Display(Name = "SliderModelPostBy", ResourceType = typeof(AdminResources))]
        [ScaffoldColumn(false)]
        public string PostBy { get; set; }


        [ScaffoldColumn(false)]
        public bool IsCreate { get; set; }


        #region slider one-to-one

        [ScaffoldColumn(false)]
        // ReSharper disable once InconsistentNaming
        public string Wp1_3 { get; set; }

        [ScaffoldColumn(false)]
        public string Txt1 { get; set; }

        [ScaffoldColumn(false)]
        public string Txt2 { get; set; }

        [ScaffoldColumn(false)]
        public string Txt3 { get; set; }


        [ScaffoldColumn(false)]
        public string Wp3 { get; set; }

        [ScaffoldColumn(false)]
        public string Slide2Txt1 { get; set; }

        [ScaffoldColumn(false)]
        public string Slide2Txt2 { get; set; }

        [ScaffoldColumn(false)]
        public string Slide2Txt3 { get; set; }


        [ScaffoldColumn(false)]
        public string ImgWp2 { get; set; }

        [ScaffoldColumn(false)]
        public string Wp1 { get; set; }

        [ScaffoldColumn(false)]
        public string Wp2 { get; set; }

        #endregion
    }
}