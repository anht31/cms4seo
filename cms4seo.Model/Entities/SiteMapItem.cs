using System;

namespace cms4seo.Model.Entities
{
    public class SiteMapItem
    {
        public Uri Loc { get; set; }
        public DateTime? LastMod { get; set; }
        public ChangeFrequency ChangeFreq { get; set; }
        public double? Priority { get; set; }
    }

    public enum ChangeFrequency
    {
        NotSet,
        Always,
        Hourly,
        Daily,
        Weekly,
        Monthly,
        Yearly,
        Never
    }
}