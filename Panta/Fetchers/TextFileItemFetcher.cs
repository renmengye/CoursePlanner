using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Panta.Fetchers
{
    public abstract class TextFileItemFetcher<T> : IItemFetcher<T>
    {
        public string Path { get; private set; }

        public string Content { get; protected set; }

        /// <summary>
        /// Use normal/get data method to fetch webpage data
        /// </summary>
        /// <param name="path">Url to fetch from</param>
        public TextFileItemFetcher(string path)
        {
            this.Path = path;
            this.Content = File.ReadAllText(path);
        }

        public abstract IEnumerable<T> FetchItems();
    }
}
