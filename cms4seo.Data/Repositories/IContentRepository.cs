using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cms4seo.Model.Entities;

namespace cms4seo.Data.Repositories
{
    public interface IContentRepository
    {
        string Get(string key);

        bool? Set(string key, string value);

        bool? Create(string key, string value);

        bool? Edit(string key, string value, bool unassigned);

        bool? AddRange(List<Content> contents);

        bool Initialization();

        bool InitializationTheme();
    }
}
