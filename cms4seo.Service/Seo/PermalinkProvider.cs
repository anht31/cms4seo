using System.Data.Entity;
using System.Linq;
using System.Web.WebPages;
using cms4seo.Common.Helpers;
using cms4seo.Data;
using cms4seo.Model.Entities;
using cms4seo.Model.LekimaxType;
using cms4seo.Service.Provider;

namespace cms4seo.Service.Seo
{
    public class PermalinkProvider
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// Helper create permalink and save it to database, included avoid conflict slug
        /// </summary>
        /// <param name="entityId">Id of item</param>
        /// <param name="entityName">Entity Name like 'product'</param>
        /// <param name="segmentRootCategory">set null if not use</param>
        /// <param name="segmentMiddleCategory">set null if not use</param>
        /// <param name="segmentItemName">must be set value</param>
        /// <param name="languageId">bypass if not use</param>
        /// <returns></returns>
        public string Create(int entityId, string entityName,
            string segmentRootCategory, string segmentMiddleCategory, string segmentItemName, 
            int languageId = 0)
        {

            // Permalink
            var permalink = new Permalink
            {
                EntityId = entityId,
                EntityName = entityName,
                IsActive = true,
                LanguageId = 0
            };

            var segmentItemNameFriendly = segmentItemName.MakeNameFriendly();
            var flatterSiteArchitecture = Setting.WebSettings[WebSettingType.FlatterSiteArchitecture].AsInt();

            if (flatterSiteArchitecture == 1)
            {
                permalink.Slug = $"/{segmentItemNameFriendly}";
            }
            else if (flatterSiteArchitecture == 2 && segmentRootCategory != null)
            {
                permalink.Slug = $"/{segmentRootCategory.MakeNameFriendly()}/{segmentItemNameFriendly}";
            }
            else if (flatterSiteArchitecture == 3 && segmentRootCategory != null && segmentMiddleCategory != null)
            {
                permalink.Slug = $"/{segmentRootCategory.MakeNameFriendly()}" +
                                 $"/{segmentMiddleCategory.MakeNameFriendly()}/{segmentItemNameFriendly}";
            }
            else if (flatterSiteArchitecture == 4 && segmentRootCategory != null) // Dynamic 1
            {
                permalink.Slug = $"/{segmentRootCategory.MakeNameFriendly()}-{segmentItemNameFriendly}";
            }
            else if(segmentRootCategory != null)
            {
                permalink.Slug = $"/{segmentRootCategory.MakeNameFriendly()}/{segmentItemNameFriendly}";
            }
            else
            {
                permalink.Slug = $"/{segmentItemNameFriendly}";
            }

            //var permalink = db.Permalinks.FirstOrDefault(x => x.EntityName == entityName && x.EntityId == entityId);

            // slug create mode
            var n = 0;
            var slug = permalink.Slug;

            if (db.Permalinks.Count(x => x.Slug == permalink.Slug) > 0)
            {
                do
                {
                    ++n;
                } while (db.Permalinks.Count(p => p.Slug == permalink.Slug + "-" + n) > 0);

                permalink.Slug = $"{permalink.Slug}-{n}";
                slug = permalink.Slug;
            }


            db.Permalinks.Add(permalink);
            db.SaveChanges();

            return slug;
        }


        /// <summary>
        /// Helper create & edit permalink and save it to database, included avoid conflict slug, support IsAutoEditPermalink
        /// </summary>
        /// <param name="entityId">Id of item</param>
        /// <param name="entityName">Entity Name like 'product'</param>
        /// <param name="segmentRootCategory">set null if not use</param>
        /// <param name="segmentMiddleCategory">set null if not use</param>
        /// <param name="segmentItemName">must be set value</param>
        /// <param name="isCreate"></param>
        /// <param name="languageId">bypass if not use</param>
        /// <returns></returns>
        public string Set(int entityId, string entityName,
            string segmentRootCategory, string segmentMiddleCategory, string segmentItemName,
            bool isCreate = false,
            int languageId = 0)
        {

            string slug = "";

            var segmentItemNameFriendly = segmentItemName.MakeNameFriendly();
            var flatterSiteArchitecture = Setting.WebSettings[WebSettingType.FlatterSiteArchitecture].AsInt();

            if (flatterSiteArchitecture == 1)
            {
                slug = $"/{segmentItemNameFriendly}";
            }
            else if (flatterSiteArchitecture == 2 && segmentRootCategory != null)
            {
                slug = $"/{segmentRootCategory.MakeNameFriendly()}/{segmentItemNameFriendly}";
            }
            else if (flatterSiteArchitecture == 3 && segmentRootCategory != null && segmentMiddleCategory != null)
            {
                slug = $"/{segmentRootCategory.MakeNameFriendly()}" +
                                 $"/{segmentMiddleCategory.MakeNameFriendly()}/{segmentItemNameFriendly}";
            }
            else if (flatterSiteArchitecture == 4 && segmentRootCategory != null)
            {
                slug = segmentMiddleCategory == null
                    ? $"/{segmentRootCategory.MakeNameFriendly()}-{segmentItemNameFriendly.MakeNameFriendly()}"
                    : $"/{segmentRootCategory.MakeNameFriendly()}/{segmentMiddleCategory.MakeNameFriendly()}";
            }
            else if (segmentRootCategory != null)
            {
                slug = $"/{segmentRootCategory.MakeNameFriendly()}/{segmentItemNameFriendly}";
            }
            else
            {
                slug = $"/{segmentItemNameFriendly}";
            }

            //var permalink = db.Permalinks.FirstOrDefault(x => x.EntityName == entityName && x.EntityId == entityId);

            


            if (isCreate)
            {
                // slug create mode
                
                if (db.Permalinks.Any(x => x.Slug == slug))
                {
                    var n = 0;
                    do
                    {
                        ++n;
                    } while (db.Permalinks.Any(p => p.Slug == slug + "-" + n));

                    slug = $"{slug}-{n}";
                }

                // Permalink
                var permalink = new Permalink
                {
                    EntityId = entityId,
                    EntityName = entityName,
                    Slug = slug,
                    IsActive = true,
                    LanguageId = 0
                };

                db.Permalinks.Add(permalink);

            }
            else if (Setting.WebSettings[WebSettingType.IsAutoEditPermalink].AsBool())
            {
                // slug edit mode


                if (db.Permalinks.Any(x =>
                        !(x.EntityName == entityName && x.EntityId == entityId) 
                        && x.Slug == slug))
                {

                    var n = 0;
                    do
                    {
                        ++n;
                    } while (db.Permalinks.Any(x =>
                        !(x.EntityName == entityName && x.EntityId == entityId) 
                        && x.Slug == slug + "-" + n));

                    slug = $"{slug}-{n}";
                }

                var permalink = db.Permalinks.FirstOrDefault(
                    x => x.EntityName == entityName && x.EntityId == entityId);

                if (permalink != null && permalink.Slug != slug)
                {
                    permalink.Slug = slug;
                    db.Entry(permalink).State = EntityState.Modified;
                }
                
            }
            else
            {
                slug = db.Permalinks.FirstOrDefault(
                    x => x.EntityName == entityName && x.EntityId == entityId)?.Slug;
            }

            
            db.SaveChanges();


            return slug;
        }



        //public string Create(int entityId, string entityName, int languageId,
        //    string name, string rootCategoryName, string secondCategoryName)
        //{

        //    // Permalink
        //    var permalink = new Permalink
        //    {
        //        EntityId = entityId,
        //        EntityName = entityName,
        //        IsActive = true,
        //        LanguageId = 0
        //    };

        //    var nameFriendly = name.MakeNameFriendly();
        //    var flatterSiteArchitecture = Setting.WebSettings[WebSettingType.FlatterSiteArchitecture].AsInt();

        //    if (flatterSiteArchitecture == 1)
        //    {
        //        permalink.Slug = $"/{nameFriendly}";
        //    }
        //    else if (flatterSiteArchitecture == 2)
        //    {
        //        permalink.Slug = $"/{rootCategoryName.MakeNameFriendly()}/{nameFriendly}";
        //    }
        //    else if (secondCategoryName != null)
        //    {
        //        permalink.Slug = $"/{rootCategoryName.MakeNameFriendly()}" +
        //                         $"/{secondCategoryName.MakeNameFriendly()}/{nameFriendly}";
        //    }
        //    else
        //    {
        //        permalink.Slug = $"/{rootCategoryName.MakeNameFriendly()}/{nameFriendly}";
        //    }


        //    // slug
        //    var n = 0;
        //    var slug = permalink.Slug;

        //    if (db.Permalinks.Count(x => x.Slug == permalink.Slug) > 0)
        //    {
        //        do
        //        {
        //            ++n;
        //        } while (db.Permalinks.Count(x => x.Slug == $"{permalink.Slug}-{n}") > 0);

        //        permalink.Slug = $"{permalink.Slug}-{n}";
        //        slug = permalink.Slug;
        //    }


        //    db.Permalinks.Add(permalink);
        //    db.SaveChanges();

        //    return slug;
        //}
    }
}
