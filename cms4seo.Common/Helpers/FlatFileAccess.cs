using System;
using System.Collections.Generic;
using System.IO;

namespace cms4seo.Common.Helpers
{
    public class FlatFileAccess
    {
        public static Dictionary<string, string> Read301CSV()
        {
            string file = "301.csv";
            string path = Path.Combine(AppDomain.CurrentDomain.GetData("DataDirectory").ToString(), file);

            if (File.Exists(path))
            {
                using (TextReader sr = new StreamReader(path))
                {
                    Dictionary<string, string> redirect_dict = new Dictionary<string, string>();
                    string line = "";
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] columns = line.Split(',');
                        redirect_dict.Add(columns[0].Trim(), columns[1].Trim());
                    }
                    return redirect_dict;
                }
            }
            else
                return null;

        }

    }
}