using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using System.Web.WebPages;
using cms4seo.Common.Helpers;
using cms4seo.Data;
using cms4seo.Model.Entities;
using cms4seo.Model.LekimaxType;
using cms4seo.Service.Provider;

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


            if (propertyDescriptor.Name == "MockProductProperties")
            {
                if (!string.IsNullOrWhiteSpace(Setting.WebSettings[WebSettingType.OptionProperties]))
                {
                    var propertyNames = Setting.WebSettings[WebSettingType.OptionProperties].Split(',').Select(t => t.Trim()).ToList();

                    var id = bindingContext.ValueProvider.GetValue("Id").AttemptedValue.AsInt();

                    using (var db = new ApplicationDbContext())
                    {
                        foreach (var propertyName in propertyNames)
                        {
                            
                            var property = db.ProductProperties
                                .Where(x => x.ProductId == id)
                                .Select(x => x.Property)
                                .FirstOrDefault(x => x.Name == propertyName);

                            if(property == null)
                                continue;

                            property.Value = bindingContext.ValueProvider.GetValue(propertyName.MakeNameFriendly())
                                .AttemptedValue; ;

                            db.Entry(property).State = EntityState.Modified;
                        }

                        db.SaveChanges();
                    }

                }
            }


            // let the default model binder do it's thing  ==============================================

            return base.GetPropertyValue(controllerContext, bindingContext, propertyDescriptor, propertyBinder);
        }
    }
}