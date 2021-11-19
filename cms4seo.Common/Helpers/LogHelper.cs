using System;
using System.ComponentModel;
using System.Diagnostics;

namespace cms4seo.Common.Helpers
{
    //[Localizable(true)]
    public static class LogHelper
    {
        public static void Write(string category, string message)
        {
            Trace.TraceInformation($"[{DateTime.Now}].[{category}] -> " + message + "<br>");
            // You must close or flush the trace to empty the output buffer.  
            Trace.Flush();
        }
    }
}