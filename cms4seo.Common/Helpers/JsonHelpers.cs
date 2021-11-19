using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cms4seo.Common.Plugins;
using Newtonsoft.Json;

namespace cms4seo.Common.Helpers
{
    public static class JsonHelpers
    {

        private static readonly object _locker = new object();

        public static void SaveWidget(string path, List<Widget> widgets)
        {

            StringBuilder stringBuilder = new StringBuilder();
            StringWriter stringWriter = new StringWriter(stringBuilder);

            lock (_locker)
            {
                using (JsonWriter writer = new JsonTextWriter(stringWriter))
                {
                    writer.Formatting = Formatting.Indented;

                    writer.WriteStartArray();

                    foreach (var widget in widgets)
                    {
                        writer.WriteStartObject();

                        writer.WritePropertyName("Action");
                        writer.WriteValue(widget.Action);

                        writer.WritePropertyName("Controller");
                        writer.WriteValue(widget.Controller);

                        writer.WritePropertyName("Area");
                        writer.WriteValue(widget.Area);

                        writer.WritePropertyName("Zone");
                        writer.WriteValue(widget.Zone);

                        writer.WritePropertyName("RouteValues");
                        writer.WriteValue(widget.RouteValues);

                        writer.WritePropertyName("Page");
                        writer.WriteValue(widget.Page);

                        writer.WritePropertyName("Active");
                        writer.WriteValue(widget.Active);

                        writer.WritePropertyName("AssemblyName");
                        writer.WriteValue(widget.AssemblyName);

                        writer.WritePropertyName("TypeName");
                        writer.WriteValue(widget.TypeName);

                        writer.WritePropertyName("Path");
                        writer.WriteValue(widget.Path);

                        writer.WriteEndObject();
                    }

                    writer.WriteEndArray();


                }

                string json = stringWriter.ToString();

                // write to file
                using (StreamWriter streamWriter = new StreamWriter(path, false))
                {
                    streamWriter.Write(json);
                }
            }

        }

        /// <summary>
        /// Writes the given object instance to a Json file.
        /// <para>Object type must have a parameterless constructor.</para>
        /// <para>Only Public properties and variables will be written to the file. These can be any type though, even other classes.</para>
        /// <para>If there are public properties/variables that you do not want written to the file, decorate them with the [JsonIgnore] attribute.</para>
        /// </summary>
        /// <typeparam name="T">The type of object being written to the file.</typeparam>
        /// <param name="filePath">The file path to write the object instance to.</param>
        /// <param name="objectToWrite">The object instance to write to the file.</param>
        /// <param name="append">If false the file will be overwritten if it already exists. If true the contents will be appended to the file.</param>
        public static void WriteToJsonFile<T>(string filePath, T objectToWrite, bool append = false) where T : new()
        {
            TextWriter writer = null;
            try
            {
                var contentsToWriteToFile = JsonConvert.SerializeObject(objectToWrite);
                writer = new StreamWriter(filePath, append);
                writer.Write(contentsToWriteToFile);
            }
            finally
            {
                if (writer != null)
                    writer.Close();
            }
        }

        /// <summary>
        /// Reads an object instance from an Json file.
        /// <para>Object type must have a parameterless constructor.</para>
        /// </summary>
        /// <typeparam name="T">The type of object to read from the file.</typeparam>
        /// <param name="filePath">The file path to read the object instance from.</param>
        /// <returns>Returns a new instance of the object read from the Json file.</returns>
        public static T ReadFromJsonFile<T>(string filePath) where T : new()
        {
            TextReader reader = null;
            try
            {
                reader = new StreamReader(filePath);
                var fileContents = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<T>(fileContents);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }
    }
}
