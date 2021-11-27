using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicLayer.Helpers
{
    public static class BlogIconMethods
    {
        const string SMALL_RES = "32x32";
        const string HIGH_RES = "150x150";

        /// <summary>
        /// Checks if exists higher resolution of icon 150x150 instead of 32x32 and returns url to higher res
        /// </summary>
        /// <param name="currentUrl">Url of lower resolution icon</param>
        /// <param name="httpService">Service to verify if something is under url</param>
        /// <returns>currentUrl or new url with higher resultion icon</returns>
        public static async Task<string >GetHigherIconResolution(string currentUrl, IHttpHelperService httpService)
        {
            if(currentUrl == null)
                return null;

            if (!currentUrl.Contains(SMALL_RES))
                return currentUrl;

            string high_res_url = currentUrl.Replace(SMALL_RES, HIGH_RES);

            if (await httpService.GetImageContent(high_res_url) != null)
                return high_res_url;

            return currentUrl;
        }
    }
}
