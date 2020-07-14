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

    [Flags]
    public enum MySecurityProtocolType
    {
        //
        // Summary:
        //     Specifies the Secure Socket Layer (SSL) 3.0 security protocol.
        Ssl3 = 48,
        //
        // Summary:
        //     Specifies the Transport Layer Security (TLS) 1.0 security protocol.
        Tls = 192,
        //
        // Summary:
        //     Specifies the Transport Layer Security (TLS) 1.1 security protocol.
        Tls11 = 768,
        //
        // Summary:
        //     Specifies the Transport Layer Security (TLS) 1.2 security protocol.
        Tls12 = 3072
    }

    public abstract class WebpageItemFetcher<T> : IItemFetcher<T>
    {
        public string Url { get; private set; }

        public string Content { get; protected set; }

        public bool AcceptAllCertifications(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certification, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        /// <summary>
        /// Use normal/get data method to fetch webpage data
        /// </summary>
        /// <param name="url">Url to fetch from</param>
        public WebpageItemFetcher(string url)
        {
            this.Url = url;
            
            // Support a local file.
            if (url.StartsWith("C:\\"))
            {
                this.Content = File.ReadAllText(url);
                return;
            }

            System.Net.ServicePointManager.SecurityProtocol = (SecurityProtocolType)(MySecurityProtocolType.Tls12 | MySecurityProtocolType.Tls11 | MySecurityProtocolType.Tls);
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            var req = HttpWebRequest.Create(url);
            req.Proxy = null;
            req.Method = "GET";
            try
            {
                string source;
                int BYTES_TO_READ = 30000000;
                var buffer = new byte[BYTES_TO_READ];
                int totalBytesRead = 0;
                int bytesRead;
                int retry = 0;
                int MAX_RETRY = 10;

                using (HttpWebResponse resp = (HttpWebResponse)req.GetResponse())
                {
                    using (Stream sm = resp.GetResponseStream())
                    {
                        do
                        {
                            // You have to do this in a loop because there's no guarantee that
                            // all the bytes you need will be ready when you call.
                            bytesRead = sm.Read(buffer, totalBytesRead, BYTES_TO_READ - totalBytesRead);
                            //Console.Out.WriteLine(this.Url);
                            //Console.Out.WriteLine(bytesRead);
                            totalBytesRead += bytesRead;
                            if (bytesRead == 0)
                            {
                                retry++;
                            }
                            else
                            {
                                retry = 0;
                            }
                            if (totalBytesRead >= BYTES_TO_READ)
                            {
                                throw new IndexOutOfRangeException("Exceeding limit");
                            }
                        } while (retry < MAX_RETRY && totalBytesRead < BYTES_TO_READ);

                        // Sometimes WebResponse will hang if you try to close before
                        // you've read the entire stream.  So you can abort the request.
                        req.Abort();
                    }
                }
                buffer = buffer.Take(totalBytesRead).ToArray();
                source = System.Text.Encoding.UTF8.GetString(buffer);
                //using (StreamReader reader = new StreamReader(req.GetResponse().GetResponseStream()))
                //{
                //    source = reader.ReadToEnd();
                //}
                this.Content = source;
                return;
            }
            catch (IOException e)
            {
                Console.Out.WriteLine("Unable to fetch: {0:S}", url);
                Console.Out.WriteLine(e);
            }
            catch (WebException e)
            {
                Console.Out.WriteLine("Unable to fetch: {0:S}", url);
                Console.Out.WriteLine(e);
            }
            //Console.Out.WriteLine("Started 2nd retry.");
            //bool retry2 = false;
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
            //            retry2 = false;
            //            this.Content = client.DownloadString(this.Url);
            //        }
            //        catch (WebException ex)
            //        {
            //            Console.WriteLine("Retry: {0:D}, unable to fetch: {1:G}", retryCount, this.Url);
            //            Console.WriteLine(ex.ToString());
            //            if (retryCount < maxRetryCount)
            //            {
            //                retry2 = true;
            //            }
            //        }
            //    }
            //} while (retry2);
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
