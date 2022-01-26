using System.Collections.Generic;

namespace ServiceLayer.CronServices
{
    public class AppConfig : IAppConfig
    {
        private List<string> UrlsToPing { get; }
        private string ConfigName { get; }
        private string AppVersion{ get; }

        public AppConfig(List<string> urlsToPing, string configName, string appVersion)
        {
            UrlsToPing = urlsToPing;
            ConfigName = configName;
            AppVersion = appVersion;
        }

        public IEnumerable<string> GetBaseUrl()
        {
            return UrlsToPing;
        }
        
        public string GetConfigName()
        {
            return ConfigName;
        }

        public string GetAppVersion()
        {
            return AppVersion;
        }
    }

    public interface IAppConfig
    {
        IEnumerable<string> GetBaseUrl();
        string GetConfigName();
        string GetAppVersion();
    }
}
