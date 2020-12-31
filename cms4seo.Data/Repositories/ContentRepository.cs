using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.IO;
using System.Xml.Linq;
using cms4seo.Common.Culture;
using cms4seo.Model.Entities;

namespace cms4seo.Data.Repositories
{
    public class ContentRepository : IContentRepository
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        private readonly string theme;
        private readonly string language;

        public ContentRepository(string themeValue, string languageValue)
        {
            theme = themeValue;
            language = languageValue;
        }

        public string this[string key] => Get(key);

        public string Get(string key)
        {
            return db.Contents.Find(key, theme, language)?.Value;
        }



        public bool? Set(string key, string value)
        {
            var content = db.Contents.Find(key, theme, language);

            if (content == null)
            {
                content = new Content
                {
                    Key = key,
                    Value = value,
                    Theme = theme,
                    Language = language
                };
                db.Contents.Add(content);

            }
            else
            {
                content.Value = value;
                content.Theme = theme;
                content.Language = language;

                db.Entry(content).State = EntityState.Modified;
            }

            db.SaveChanges();

            return true;
        }

        /// <summary>
        /// Create if not existed key on database
        /// , with theme & language automatic adding
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool? Create(string key, string value)
        {
            var content = db.Contents.Find(key, theme, language);

            if (content != null)
            {
                return false;
            }

            content = new Content
            {
                Key = key,
                Value = value,
                Theme = theme,
                Language = language
            };
            db.Contents.Add(content);



            db.SaveChanges();

            return true;
        }


        /// <summary>
        /// Edit key, with theme & language automatic adding
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="unassigned">set true to tell this key not assigned</param>
        /// <returns></returns>
        public bool? Edit(string key, string value, bool unassigned = false)
        {

            var content = db.Contents.Find(key, theme, language);

            if (content == null)
            {
                return false;
            }

            content.Value = value;
            content.Theme = theme;
            content.Language = language;
            content.Unassigned = unassigned;

            db.Entry(content).State = EntityState.Modified;

            db.SaveChanges();

            return true;
        }





        public bool? AddRange(List<Content> contents)
        {

            db.Contents.AddRange(contents);
            db.SaveChanges();

            return true;
        }


        public bool Initialization()
        {

            return true;
        }



        public bool InitializationTheme()
        {
            //var neutralCulture = CultureHelper.GetCurrentNeutralCulture();

            string alternateLanguage = string.Copy(language);

            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, 
                $@"Themes\{theme}\Language\{language}.xml");

            if (!File.Exists(path))
            {
                return false;

                //alternateLanguage = "en-US";

                //path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                //    $@"Themes\{theme}\Language\{alternateLanguage}.xml");

                //if (!File.Exists(path))
                //    return false;
            }

            // source
            IEnumerable<XElement> xElements = 
                XDocument.Parse(File.ReadAllText(path))
                    .Element("appSettings")?.Elements("add");

            if (xElements == null)
                return false;


            // Mapping
            List<Content> contents = xElements.Select(x => new Content
            {
                Key = x.Attribute("key")?.Value,
                Value = x.Attribute("value")?.Value,
                Theme = theme,
                Language = alternateLanguage
            }).ToList();


            var contentsAdding = contents.Except(db.Contents, new ContentComparer()).ToList();

            // save
            AddRange(contentsAdding);


            return true;
        }


        private class ContentComparer : IEqualityComparer<Content>
        {
            public bool Equals(Content content1, Content content2)
            {
                return content1?.Key == content2?.Key
                    && content1?.Theme == content2?.Theme
                    && content1?.Language == content2?.Language;
            }

            // Return a hash that reflects the comparison criteria. According to the   
            // rules for IEqualityComparer<T>, if Equals is true, then the hash codes must  
            // also be equal. Because equality as defined here is a simple value equality, not  
            // reference identity, it is possible that two or more objects will produce the same  
            // hash code.  
            public int GetHashCode(Content content)
            {
                string s = $"{content.Key}";
                return s.GetHashCode();
            }
        }
    }
}
