using cms4seo.Model.Entities;

namespace cms4seo.Common.Helpers
{
    public static class TopicHelper
    {

        // FindRootCategory ==========================================================================
        public static Topic FindRootTopic(this Topic topic)
        {
            //var name = topic;
            if (topic.ParentId != null)
            {
                topic = topic.Parent.FindRootTopic();
            }
            return topic;
        }
    }
}