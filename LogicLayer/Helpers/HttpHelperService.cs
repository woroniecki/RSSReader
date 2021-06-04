using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Drawing;

namespace LogicLayer.Helpers
{
    public class HttpHelperService : IHttpHelperService
    {
        public class RssHttpResponse
        {
            public string RequestUrl { get; set; }
            public string Content { get; set; }
        }

        /// <summary>
        /// Returns text content under url or null if url can't be reached
        /// </summary>
        /// <param name="url"></param>
        /// <returns>Returns text content under url or null if url can't be reached</returns>
        public async Task<RssHttpResponse> GetRssHttpResponse(string url)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri(url);
                    HttpResponseMessage response = await client.GetAsync("");

                    if (response.IsSuccessStatusCode)
                    {
                        RssHttpResponse result = new RssHttpResponse();
                        result.Content = await response.Content.ReadAsStringAsync();
                        result.RequestUrl = response.RequestMessage.RequestUri.AbsoluteUri;
                        return result;
                    }
                }
                catch (Exception ex)
                {
                    return null;
                }
                return null;
            }
        }

        /// <summary>
        /// Returns text content under url or null if url can't be reached
        /// </summary>
        /// <param name="url"></param>
        /// <returns>Returns text content under url or null if url can't be reached</returns>
        public async Task<string> GetStringContent(string url)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    return await client.GetStringAsync(url);
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Returns image under url or null if url can't be reached
        /// </summary>
        /// <param name="url"></param>
        /// <returns>Returns image under url or null if url can't be reached</returns>
        public async Task<Image> GetImageContent(string url)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var httpClient = new HttpClient();
                    Stream st = await client.GetStreamAsync(url);
                    var memoryStream = new MemoryStream();
                    await st.CopyToAsync(memoryStream);
                    memoryStream.Position = 0;
                    Image img = Image.FromStream(memoryStream);
                    return img;
                }
                catch
                {
                    return null;
                }
            }
        }
    }

    public interface IHttpHelperService
    {
        Task<HttpHelperService.RssHttpResponse> GetRssHttpResponse(string url);
        Task<string> GetStringContent(string url);
        Task<Image> GetImageContent(string url);
    }
}
