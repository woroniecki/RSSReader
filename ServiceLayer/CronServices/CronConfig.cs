using System.Collections.Generic;

namespace ServiceLayer.CronServices
{
    public class CronConfig : ICronConfig
    {
        private List<string> UrlsToPing { get; }
        private string ConfigName { get; }

        public CronConfig(List<string> urlsToPing, string configName)
        {
            UrlsToPing = urlsToPing;
            ConfigName = configName;
        }

        public IEnumerable<string> GetBaseUrl()
        {
            return UrlsToPing;
        }
        
        public string GetConfigName()
        {
            return ConfigName;
        }
    }

    public interface ICronConfig
    {
        IEnumerable<string> GetBaseUrl();
        string GetConfigName();
    }
}
