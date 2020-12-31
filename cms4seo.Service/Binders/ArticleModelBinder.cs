using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.Mvc;

namespace cms4seo.Service.Binders
{
    // dev ===================================================================
    public class ArticleModelBinder : DefaultModelBinder
    {
        protected override object GetPropertyValue(
            ControllerContext controllerContext,
            ModelBindingContext bindingContext,
            PropertyDescriptor propertyDescriptor,
            IModelBinder propertyBinder)
        {
            // articleTag ===============================================================

            if (propertyDescriptor.Name == "SelectedArticleTag")
            {
                var tags = bindingContext.ValueProvider.GetValue("SelectedArticleTag").AttemptedValue;

                if (string.IsNullOrWhiteSpace(tags))
                {
                    return new List<string>();
                }

                return tags.Split(',').Select(t => t.Trim()).ToList();
            }


            // tag ===============================================================

            if (propertyDescriptor.Name == "Tags")
            {
                var tags = bindingContext.ValueProvider.GetValue("Tags").AttemptedValue;

                if (string.IsNullOrWhiteSpace(tags))
                {
                    return new List<string>();
                }

                return tags.Split(',').Select(t => t.Trim()).ToList();
            }


            // let the default model binder do it's thing  ==============================================

            return base.GetPropertyValue(controllerContext, bindingContext, propertyDescriptor, propertyBinder);
        }
    }
}