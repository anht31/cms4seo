using System;
using System.Collections.Generic;
using System.Linq;
using cms4seo.Model.Entities;

namespace cms4seo.Service.TopicHelper
{
    public static class TopicHelpers
    {
        // zero time 0:00:00
        public static DateTime Tomorrow => DateTime.Today.AddDays(1);

        public static List<Article> AsArticlesValidation(this List<Article> articles) =>
            articles.Where( x=>
                !(x.Name == null || x.Name.Trim() == String.Empty)
                & x.PostedDate < Tomorrow
                && x.IsPublish).ToList();

        public static List<Article> GetAllChildArticle(this Topic topic)
        {
            var articles = new List<Article>();

            if (topic.Articles.Count > 0)
            {
                articles.AddRange(topic.Articles);
            }

            if (topic.Children.Count > 0)
            {
                foreach (var childTopic in topic.Children)
                {
                    var articleLv2 = GetAllChildArticle(childTopic);

                    articles.AddRange(articleLv2);
                }
            }

            return articles;
        }


        public static Topic GetRootTopic(this Topic topic)
        {
            if (topic == null)
                return null;

            if (topic.ParentId != null)
            {
                topic = GetRootTopic(topic.Parent);
            }
            return topic;
        }

        public static int HowDeepTopicSiteArchitecture(this Topic topic)
        {
            if (topic == null)
                return 0;

            int deep = 1;

            if (topic.ParentId != null)
            {
                deep = HowDeepTopicSiteArchitecture(topic.Parent) + 1;
            }
            return deep;
        }
    }
}
