using CalendarEvents.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace CalendarEvents.Scrapper
{
    public abstract class Scrapper : IScrapper<EventModel>
    {
        public abstract List<EventModel> Scrape();
        public virtual List<EventModel> ScrapeFromDate(DateTime? date)
        {
            // TODO: checc HTTP Last-Modified header for changes
            var res = Scrape();
            if (!date.HasValue)
                return res;

            return res.Where(e => e.Start.ToUniversalTime() > date).ToList();
        }

        protected virtual HttpWebRequest PrepareRequest(string url, string proxy)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Accept = "*/*";
            request.ServicePoint.Expect100Continue = false;
            request.ServicePoint.UseNagleAlgorithm = false;
            request.AllowWriteStreamBuffering = false;
            request.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip,deflate");
            request.ContentType = "application/x-www-form-urlencoded";
            request.AllowAutoRedirect = false;
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/50.0.2661.102 Safari/537.36";
            request.Timeout = 5000;
            request.KeepAlive = true;
            request.Method = "GET";
            if (proxy != null) request.Proxy = new WebProxy(proxy);
            request.ServicePoint.ConnectionLimit = int.MaxValue;

            return request;
        }

        protected virtual string GetPageSource(HttpWebRequest request)
        {
            string pageSource = string.Empty;

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                if (response.ContentEncoding.ToLower().Contains("gzip"))// Deal with Gzip
                {
                    using (GZipStream stream = new GZipStream(response.GetResponseStream(), CompressionMode.Decompress))
                    {
                        using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                        {
                            pageSource = reader.ReadToEnd();
                        }
                    }
                }
                else if (response.ContentEncoding.ToLower().Contains("deflate")) // Deal with Deflate
                {
                    using (DeflateStream stream = new DeflateStream(response.GetResponseStream(), CompressionMode.Decompress))
                    {
                        using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                        {
                            pageSource = reader.ReadToEnd();
                        }

                    }
                }
                else
                {
                    using (Stream stream = response.GetResponseStream())//原始
                    {
                        using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                        {

                            pageSource = reader.ReadToEnd();
                        }
                    }
                }

                request.Abort(); // Abort the request after we got page source

                return pageSource;
            }
        }

    }
}
