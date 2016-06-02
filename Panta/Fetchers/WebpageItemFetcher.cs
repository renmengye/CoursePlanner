using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Panta.Fetchers
{
    public class TimeoutClient : WebClient
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

            WebRequest req = HttpWebRequest.Create(url);
            req.Proxy = null;
            req.Method = "GET";

            try
            {
                string source;
                using (StreamReader reader = new StreamReader(req.GetResponse().GetResponseStream()))
                {
                    source = reader.ReadToEnd();
                }
                this.Content = source;
            }
            catch
            {
                Console.WriteLine("Unable to fetch: {0:S}", url);
            }
            //bool retry = false;
            //int maxRetryCount = 50;
            //int retryCount = 0;
            //do
            //{
            //    using (var client = new TimeoutClient(10000))
            //    {

            //        client.Proxy = null;
            //        try
            //        {
            //            retryCount++;
            //            retry = false;
            //            this.Content = client.DownloadString(this.Url);
            //        }
            //        catch (WebException ex)
            //        {
            //            Console.WriteLine("Retry: {0:D}, unable to fetch: {1:G}", retryCount, this.Url);
            //            Console.WriteLine(ex.ToString());
            //            if (retryCount < maxRetryCount)
            //            {
            //                retry = true;
            //            }
            //        }
            //    }
            //} while (retry);
        }

        /// <summary>
        /// Use post data method to fetch webpage data
        /// </summary>
        /// <param name="url">Url to fetch from</param>
        /// <param name="parameters">Parameters</param>
        public WebpageItemFetcher(string url, string parameters)
        {
            this.Url = url;
            bool retry = false;
            int maxRetryCount = 50;
            int retryCount = 0;
            do
            {
                using (var client = new TimeoutClient(10000))
                {
                    client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                    try
                    {
                        retryCount++;
                        retry = false;
                        this.Content = client.UploadString(this.Url, parameters);
                    }
                    catch (WebException ex)
                    {
                        Console.WriteLine("Retry: {0:D}, unable to fetch: {1:G}", retryCount, this.Url);
                        Console.WriteLine(ex.ToString());
                        if (retryCount < maxRetryCount)
                        {
                            retry = true;
                        }
                    }
                }
            } while (retry);
        }

        public abstract IEnumerable<T> FetchItems();
    }
}
