using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using cms4seo.Common.Helpers;

namespace cms4seo.Service.Provider
{
    public class XmlProvider
    {
        // File path
        private string _filePath;


        // ReSharper disable once InconsistentNaming
        private static readonly object _locker = new object();


        public XmlProvider(string filePath)
        {
            _filePath = filePath;

            lock (_locker)
            {
                if (!File.Exists(filePath))
                {
                    LogHelper.Write("XmlProvider", $"XML Resource file {filePath} was not found");
                    throw new FileNotFoundException($"XML Resource file {filePath} was not found");
                }
            }

        }


        /// <summary>
        /// Access Read file with lock for not conflict access file
        /// </summary>
        public XElement Root
        {
            get
            {
                lock (_locker)
                {
                    return XDocument.Parse(File.ReadAllText(_filePath)).Element("appSettings");
                }

            }
        }

        public IEnumerable<XElement> All => Root.Elements("add");


        public string this[string key] =>
            All.FirstOrDefault(e => e.Attribute("key")?.Value == key)?.Attribute("value")?.Value;

        public IEnumerable<string> AllKeys => All.Select(x => x.Attribute("key")?.Value).ToArray();


        public bool CheckIsNull(string key)
        {
            try
            {
                if (Root == null)
                    return true;

                if (All == null || !All.Any())
                    return true;

                return All.FirstOrDefault(e => e.Attribute("key")?.Value == key) == null;

            }
            catch
            {
                // ignored
            }


            return true;
        }



        /// <summary>
        /// Create new setting key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public string CreateXml(string key, string value)
        {

            if (string.IsNullOrEmpty(key))
            {
                throw new NullReferenceException("key is null or empty");
            }

            if (string.IsNullOrEmpty(value))
            {
                value = String.Empty;
            }

            // Check duplication Key
            if (!CheckIsNull(key))
            {
                return $"{key} is duplicate with exist key, please chose another key";
            }


            lock (_locker)
            {
                XDocument doc = new XDocument(Root);
                var element = new XElement("add");
                element.SetAttributeValue("key", key);
                element.SetAttributeValue("value", value);
                doc.Root?.Add(element);

                doc.Save(_filePath);

                // set XDocument object reference to null and GC will recollect the acquired memory.
                // ReSharper disable once RedundantAssignment
                doc = null;
            }




            return null;
        }



        /// <summary>
        /// Edit Setting Key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void UpdateXml(string key, string value)
        {


            if (string.IsNullOrEmpty(key))
            {
                throw new NullReferenceException("key is null or empty");
            }

            if (string.IsNullOrEmpty(value))
            {
                value = String.Empty;
            }


            if (key == "undefined")
            {
                throw new Exception("Xml Provider try create undefined Key");
            }


            // Check key exist
            if (CheckIsNull(key))
            {
                //CreateXml(key, value);
                return;

                //throw new NullReferenceException($"Key {key} to edit not found");
            }



            lock (_locker)
            {
                XDocument doc = new XDocument(Root);

                var attribute = doc.Element("appSettings")?.Elements("add")
                    .FirstOrDefault(e => e.Attribute("key")?.Value == key)?.Attribute("value");

                if (attribute != null)
                    attribute.Value = value;

                doc.Save(_filePath);

                // set XDocument object reference to null and GC will recollect the acquired memory.
                // ReSharper disable once RedundantAssignment
                doc = null;
            }



        }


        /// <summary>
        /// Delete key & value
        /// </summary>
        /// <param name="key">key name</param>
        /// <returns>key name has been deleted</returns>
        public string Delete(string key)
        {
            if (!CheckIsNull(key))
            {
                lock (_locker)
                {
                    var doc = XDocument.Load(_filePath);
                    doc.Element("appSettings")?.Elements("add").
                        Where(x => (string)x.Attribute("key") == key).Remove();

                    doc.Save(_filePath);

                    // set XDocument object reference to null and GC will recollect the acquired memory.
                    // ReSharper disable once RedundantAssignment
                    doc = null;
                }

            }

            return key;
        }


        public bool Sort()
        {
            lock (_locker)
            {

                XElement root = XElement.Load(_filePath);

                var orderedElements = root.Elements("add")
                    .OrderBy(x => (string)x.Attribute("key"))
                    .ToArray();

                root.RemoveAll();

                foreach (XElement element in orderedElements)
                    root.Add(element);

                root.Save(_filePath);


                return true;
            }
        }




        public XElement Export(List<Model.Entities.Content> contents)
        {
            lock (_locker)
            {

                XElement root = XElement.Load(_filePath);

                root.RemoveAll();

                foreach (var content in contents)
                {
                    var element = new XElement("add");
                    element.SetAttributeValue("key", content.Key);
                    element.SetAttributeValue("value", content.Value);
                    root.Add(element);
                }

                return root;
            }
        }


    }
}