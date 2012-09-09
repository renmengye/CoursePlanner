using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;

namespace Panta.Fetchers
{
    public class WebpageItemFetcher<T> : IItemFetcher<T>
    {
        public string Url { get; private set; }

        public string Content { get; protected set; }

        public WebpageItemFetcher(string url)
        {
            this.Url = url;
            WebClient client = new WebClient();

            try
            {
                this.Content = client.DownloadString(this.Url);
            }
            catch (WebException ex)
            {
                ex.Source = "Unable to fetch: " + Url;
                Trace.WriteLine(ex.ToString());
            }
        }

        public virtual IEnumerable<T> FetchItems()
        {
            return new List<T>();
        }
    }
}
