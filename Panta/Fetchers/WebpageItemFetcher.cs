using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;

namespace Panta.Fetchers
{
    class TimeoutClient : WebClient
    {
        /// <summary>
        /// Time in milliseconds
        /// </summary>
        public int Timeout { get; set; }

        public TimeoutClient() : this(60000) { }

        public TimeoutClient(int timeout)
        {
            this.Timeout = timeout;
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = base.GetWebRequest(address);
            if (request != null)
            {
                request.Timeout = this.Timeout;
            }
            return request;
        }
    }

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
            bool retry = false;
            int maxRetryCount = 50;
            int retryCount = 0;
            do
            {
                using (var client = new TimeoutClient())
                {
                    try
                    {
                        retryCount++;
                        retry = false;
                        this.Content = client.DownloadString(this.Url);
                    }
                    catch (WebException ex)
                    {
                        ex.Source = "Unable to fetch: " + this.Url;
                        Trace.WriteLine(ex.ToString());
                        Console.WriteLine(ex.ToString());
                        if (retryCount < maxRetryCount)
                        {
                            retry = true;
                        }
                    }
                }
            } while (retry);
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
