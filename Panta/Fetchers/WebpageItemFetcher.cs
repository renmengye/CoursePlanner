using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;

namespace Panta.Fetchers
{
    public abstract class WebpageItemFetcher<T> : IItemFetcher<T>
    {
        public string Url { get; private set; }

        public string Content { get; protected set; }

        /// <summary>
        /// Use normal/get data method to fetch webpage data
        /// </summary>
        /// <param name="url">Url to fetch from</param>
        public WebpageItemFetcher(string url)
        {
            this.Url = url;
            using (WebClient client = new WebClient())
            {
                try
                {
                    this.Content = client.DownloadString(this.Url);
                }
                catch (WebException ex)
                {
                    ex.Source = "Unable to fetch: " + this.Url;
                    Trace.WriteLine(ex.ToString());
                }
            }
        }

        /// <summary>
        /// Use post data method to fetch webpage data
        /// </summary>
        /// <param name="url">Url to fetch from</param>
        /// <param name="parameters">Parameters</param>
        public WebpageItemFetcher(string url, string parameters)
        {
            this.Url = url;
            using (WebClient client = new WebClient())
            {
                client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                try
                {
                    this.Content = client.UploadString(this.Url, parameters);
                }
                catch (WebException ex)
                {
                    ex.Source = "Unable to fetch: " + this.Url;
                    Trace.WriteLine(ex.ToString());
                }
            }   
        }

        public abstract IEnumerable<T> FetchItems();
    }
}
