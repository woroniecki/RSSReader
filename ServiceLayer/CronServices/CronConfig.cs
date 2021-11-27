using System.Collections.Generic;

namespace ServiceLayer.CronServices
{
    public class CronConfig : ICronConfig
    {
        private List<string> UrlsToPing { get; }

        public CronConfig(List<string> urlsToPing)
        {
            UrlsToPing = urlsToPing;
        }

        public IEnumerable<string> GetBaseUrl()
        {
            return UrlsToPing;
        }
    }

    public interface ICronConfig
    {
        IEnumerable<string> GetBaseUrl();
    }
}
