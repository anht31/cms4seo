using System;
using System.Collections.Generic;
using System.Linq;
using cms4seo.Model.Entities;

namespace cms4seo.Service.Product
{


    public static class ProductHelper
    {

        // zero time 0:00:00
        public static DateTime Tomorrow => DateTime.Today.AddDays(1);

        /// <summary>
        /// Validation with Order Descending with Sort
        /// </summary>
        /// <param name="products"></param>
        /// <returns></returns>
        public static List<Model.Entities.Product> AsProductsValidation(this List<Model.Entities.Product> products) =>
            products.Where(x =>
                !(x.Name == null || x.Name.Trim() == String.Empty)
                && x.Published < Tomorrow
                && x.IsPublish)
                .OrderByDescending(x => x.Sort)
                .ToList();


        private static List<Model.Entities.Product> AllChildProduct(this Category category)
        {
            var products = new List<Model.Entities.Product>();
            if (category.Products.Count > 0)
            {
                products.AddRange(category.Products);
            }
            if (category.Children.Count > 0)
            {
                foreach (var childCategory in category.Children)
                {
                    var productsLv2 = AllChildProduct(childCategory);

                    products.AddRange(productsLv2);

                }
            }

            return products;
        }

        /// <summary>
        /// Get all child Product with Validation
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public static List<Model.Entities.Product> GetAllChildProduct(this Category category)
        {
            return category.AllChildProduct().AsProductsValidation();
        }



        public static int GetAllNumOfProduct(this Category category)
        {
            var num = category.Products.Count;
            if (category.Children.Count > 0)
            {
                foreach (var childCategory in category.Children)
                {
                    num += GetAllNumOfProduct(childCategory);
                }
            }
            return num;
        }



        public static int FindRootCategory(this Category category)
        {
            var rootId = category.Id;
            if (category.ParentId != null)
            {
                rootId = FindRootCategory(category.Parent);
            }
            return rootId;
        }


        public static Category GetRootCategory(this Category category)
        {
            if (category == null)
                return null;

            if (category.ParentId != null)
            {
                category = GetRootCategory(category.Parent);
            }
            return category;
        }

        public static int HowDeepCategorySiteArchitecture(this Category category)
        {
            if (category == null)
                return 0;

            int deep = 1;

            if (category.ParentId != null)
            {
                deep = HowDeepCategorySiteArchitecture(category.Parent) + 1;
            }
            return deep;
        }


        public static List<Model.Entities.Property> ToProperties(this Model.Entities.Product product)
        {
            if (product == null)
                return null;

            if(product.ProductProperties == null)
                return new List<Property>();

            return product.ProductProperties.Select(x => x.Property)
                .ToList();
        }

        public static string GetProperty(this List<Model.Entities.Property> properties, string key)
        {
            if (properties == null || properties.Count == 0)
                return null;

            return properties.FirstOrDefault(x => x.Name == key)?.Value;
        }
    }


}