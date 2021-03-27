using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Drawing;

namespace RSSReader.Data
{
    public class HttpService : IHttpService
    {
        /// Returns text content under url or null if url can't be reached
        public async Task<string> GetStringContent(string url)
        {
            string feed = null;
            using (var client = new HttpClient())
            {
                try
                {
                    return await client.GetStringAsync(url);
                }
                catch
                {
                    return null;
                }
            }
        }

        /// Returns image under url or null if url can't be reached
        public async Task<Image> GetImageContent(string url)
        {
            Image img = null;
            using (var client = new HttpClient())
            {
                try
                {
                    var httpClient = new HttpClient();
                    Stream st = await client.GetStreamAsync(url);
                    var memoryStream = new MemoryStream();
                    await st.CopyToAsync(memoryStream);
                    memoryStream.Position = 0;
                    img = Image.FromStream(st);
                    return img;
                }
                catch
                {
                    return null;
                }
            }
        }
    }

    public interface IHttpService
    {
        Task<string> GetStringContent(string url);
        Task<Image> GetImageContent(string url);
    }
}
