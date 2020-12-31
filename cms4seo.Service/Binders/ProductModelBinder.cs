using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.Mvc;

namespace cms4seo.Service.Binders
{
    // dev ===================================================================
    public class ProductModelBinder : DefaultModelBinder
    {
        protected override object GetPropertyValue(
            ControllerContext controllerContext,
            ModelBindingContext bindingContext,
            PropertyDescriptor propertyDescriptor,
            IModelBinder propertyBinder)
        {
            // productTag ===============================================================

            if (propertyDescriptor.Name == "SelectedProductTag")
            {
                var tags = bindingContext.ValueProvider.GetValue("SelectedProductTag").AttemptedValue;

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