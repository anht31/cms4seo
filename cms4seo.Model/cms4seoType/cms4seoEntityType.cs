

namespace cms4seo.Model.LekimaxType
{
    /// <summary>
    /// Strong type, use for upload photo Folder, Entity Name for upload Photo, Route for Permalinks,
    /// use Client Controller for Route (except Slider model no need for this)
    /// recheck by LocalizedRouteExtensionMethod
    /// </summary>
    public class cms4seoEntityType
    {
        public const string Category = "Categories";
        public const string Product = "Product";
        public const string Topic = "Topics";
        public const string Article = "Articles";
        public const string Info = "Infos";

        // Exception, non check by LocalizedRouteExtensionMethod
        public const string Slider = "Sliders";

        public const string Settings = "Settings";
        public const string Contents = "Contents";

    }
}