using System;
using System.Collections.Generic;
using System.Linq;

namespace cms4seo.Data.Repositories
{
    public class TagRepository : ITagRepository
    {
        public IEnumerable<string> GetAll()
        {
            using (var db = new ApplicationDbContext())
            {
                // code1-default
                var tagsCollection = db.Products.Select(p => p.CombinedTags).ToList();
                tagsCollection.RemoveAll(string.IsNullOrEmpty);
                return string.Join(",", tagsCollection).Split(',').Distinct();

                //code2 not use by performance
                //return db.Products.ToList().SelectMany(product => product.Tags).Distinct();
            }
        }


        public string Get(string tag)
        {
            using (var db = new ApplicationDbContext())
            {
                // query instance
                var products = db.Products.Where(product => product.CombinedTags.Contains(tag)).ToList();

                // Distinct for "another-tag" and "another-tag-yet"
                products = products.Where(product =>
                    product.Tags.Contains(tag, StringComparer.CurrentCultureIgnoreCase)).ToList();

                if (!products.Any())
                {
                    throw new KeyNotFoundException("The tag " + tag + " does not exist.");
                }

                return tag.ToLower();
            }
        }

        public void Edit(string existingTag, string newTag)
        {
            using (var db = new ApplicationDbContext())
            {
                var products = db.Products.Where(product => product.CombinedTags.Contains(existingTag)).ToList();

                products = products.Where(product =>
                    product.Tags.Contains(existingTag, StringComparer.CurrentCultureIgnoreCase)).ToList();

                if (!products.Any())
                {
                    throw new KeyNotFoundException("The tag " + existingTag + " does not exist.");
                }

                foreach (var product in products)
                {
                    product.Tags.Remove(existingTag);
                    product.Tags.Add(newTag);
                }

                db.SaveChanges();
            }
        }

        public void Delete(string tag)
        {
            using (var db = new ApplicationDbContext())
            {
                var products = db.Products.Where(product => product.CombinedTags.Contains(tag)).ToList();

                products = products.Where(product =>
                    product.Tags.Contains(tag, StringComparer.CurrentCultureIgnoreCase)).ToList();

                if (!products.Any())
                {
                    throw new KeyNotFoundException("The tag " + tag + " does not exist.");
                }

                foreach (var product in products)
                {
                    product.Tags.Remove(tag);
                }

                db.SaveChanges();
            }
        }
    }
}