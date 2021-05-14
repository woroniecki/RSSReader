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
                catch
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
                    Image img = Image.FromStream(st);
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
        Task<string> GetStringContent(string url);
        Task<Image> GetImageContent(string url);
    }
}
