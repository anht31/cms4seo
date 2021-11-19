using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cms4seo.Common.Plugins
{
    public class Widget
    {
        public string Action { get; private set; }
        public string Controller { get; private set; }
        public string Area { get; private set; }
        public string Zone { get; private set; }
        public object RouteValues { get; private set; }

        /// <summary>
        /// page slug need for Model job
        /// </summary>
        public string Page { get; private set; }
        public string Active { get; set; }

        public string AssemblyName { get; private set; }
        public string TypeName { get; private set; }
        public string Path { get; private set; }


        public Widget(string action, string controller, string area, 
            string zone, object routeValues, string page, string active,
            string assemblyName, string typeName, string path)
        {
            Action = action;
            Controller = controller;
            Area = area;
            Zone = zone;
            RouteValues = RouteValues;
            Page = page;
            Active = active;
            AssemblyName = assemblyName;
            TypeName = typeName;
            Path = path;
        }
    }


    

}
