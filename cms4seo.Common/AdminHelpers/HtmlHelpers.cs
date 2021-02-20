using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using cms4seo.Model.Entities;

namespace cms4seo.Common.AdminHelpers
{
    // paging ===============================================================
    public static class PagingHelpers
    {
        public static MvcHtmlString PageLinks(
            this HtmlHelper html,
            PagingInfo pagingInfo,
            Func<int, string> pageUrl)
        {
            var result = new StringBuilder();
            for (var i = 1; i <= pagingInfo.TotalPages; i++)
            {
                var tag = new TagBuilder("a");
                tag.MergeAttribute("href", pageUrl(i));
                tag.InnerHtml = i.ToString();
                if (i == pagingInfo.CurrentPage)
                {
                    tag.AddCssClass("selected");
                    tag.AddCssClass("btn-primary");
                }
                tag.AddCssClass("btn btn-outline-secondary");
                result.Append(tag);
            }
            return MvcHtmlString.Create(result.ToString());
        }
    }

    

    public static class ExtensionMethods
    {
        /// <summary>
        /// Returns a single-selection select element containing the options specified in the items parameter.
        /// </summary>
        /// <typeparam name="T">The type of elements in the collection.</typeparam>
        /// <param name="helper">The class being extended.</param>
        /// <param name="name">Name of select list (same name with property for bidding)</param>
        /// <param name="selectedCategory">Id of category for selected='selected'</param>
        /// <param name="items">The collection of items used to populate the drop down list.</param>
        /// <param name="parentItemsPredicate">A function to determine which elements are considered as parents.</param>
        /// <param name="parentChildAssociationPredicate">A function to determine the children of a given parent.</param>
        /// <param name="dataValueField">The value for the element.</param>
        /// <param name="dataTextField">The display text for the value.</param>
        /// <param name="defaultText"></param>
        /// <returns></returns>
        public static MvcHtmlString DropDownGroupList<T>(
            this HtmlHelper helper,
            string name,
            int? selectedCategory,
            IEnumerable<T> items,
            Func<T, bool> parentItemsPredicate,
            Func<T, T, bool> parentChildAssociationPredicate,
            string dataValueField,
            string dataTextField,
            string defaultText,
            int deepLevel = 3)
        {            
            var html = new StringBuilder("<select name='" + name + "' id='" + name + "' class='form-control'>");

            if (defaultText != null)
            {
                var selectRoot = selectedCategory == null ? "selected='selected'" : "";
                html.Append($"<option class='root-item' value {selectRoot}>{defaultText}</option>");
            }

            var enumerable = items as T[] ?? items.ToArray();
            foreach (var item in enumerable.Where(parentItemsPredicate))
            {
                // no-select root category
                //html.Append($"<optgroup label=\"{item.GetType().GetProperty(dataTextField).GetValue(item, null)}\">");                

                string select = selectedCategory == (int) item.GetType().GetProperty(dataValueField).GetValue(item, null)
                    ? "selected='selected'"
                    : "";


                html.Append(
                    $"<option class='root-item' value=\"{item.GetType().GetProperty(dataValueField).GetValue(item, null)}\" " + select + ">" +
                        $"{item.GetType().GetProperty(dataTextField).GetValue(item, null)}" +
                    "</option>");

                foreach (var child in enumerable.Where(x => parentChildAssociationPredicate(x, item)))
                {
                    var childType = child.GetType();
                    select = selectedCategory == (int)childType.GetProperty(dataValueField).GetValue(child, null)
                        ? "selected='selected'"
                        : "";

                    html.Append(
                        $"<option class='child-item' value=\"{childType.GetProperty(dataValueField).GetValue(child, null)}\" " + select + ">" +
                            $"... / {childType.GetProperty(dataTextField).GetValue(child, null)}" + 
                        "</option>");

                    foreach (var child_lv3 in enumerable.Where(x => parentChildAssociationPredicate(x, child)))
                    {
                        var child_lv3Type = child_lv3.GetType();
                        select = selectedCategory == (int)child_lv3Type.GetProperty(dataValueField).GetValue(child_lv3, null)
                            ? "selected='selected'"
                            : "";

                        if(deepLevel == 3)
                            html.Append(
                                $"<option class='child-item' value=\"{child_lv3Type.GetProperty(dataValueField).GetValue(child_lv3, null)}\" " + select + ">" +
                                    $"... / ... / {child_lv3Type.GetProperty(dataTextField).GetValue(child_lv3, null)}" +
                                "</option>");
                    }
                }

                // no-select root category
                //html.Append("</optgroup>");
            }

            html.Append("</select>");

            return new MvcHtmlString(html.ToString());
        }


        /// <summary>
        /// Check Sidebar is active
        /// </summary>
        /// <param name="path">path and query</param>
        /// <param name="items">list of controler, action</param>
        /// <returns></returns>
        public static string IsNavbarActive(this string path, params string[] items)
        {
            if (path != null && items.Any())
            {
                foreach (var item in items)
                {
                    if (path.Contains(item))
                    {                   
                        return "active";
                    }
                }
            }
            return "";
        }
    }



}