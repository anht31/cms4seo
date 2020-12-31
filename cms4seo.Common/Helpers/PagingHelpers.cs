using Lekimax.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Lekimax.Helpers;

namespace Lekimax.Helpers
{
    // paging ===============================================================
    public static class PagingHelpers
    {
        public static MvcHtmlString PageLinks(
            this HtmlHelper html, 
            PagingInfo pagingInfo,
            Func<int, string> pageUrl)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 1; i <= pagingInfo.TotalPages; i++)
            {
                TagBuilder tag = new TagBuilder("a");
                tag.MergeAttribute("href", pageUrl(i));
                tag.InnerHtml = i.ToString();
                if (i == pagingInfo.CurrentPage)
                {
                    tag.AddCssClass("selected");
                    tag.AddCssClass("btn-primary");
                }
                tag.AddCssClass("btn btn-default");
                result.Append(tag.ToString());
            }
            return MvcHtmlString.Create(result.ToString());
        }
    }


    // BeginDiv ==============================================================

    //@using (new MvcDiv(Html.ViewContext))
    //{
    //    // ...
    //}

    // --------------------------------------------------------
}