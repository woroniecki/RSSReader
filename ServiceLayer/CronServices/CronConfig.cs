namespace ServiceLayer.CronServices
{
    public class CronConfig : ICronConfig
    {
        private string BaseUrl { get; }

        public CronConfig(string baseUrl)
        {
            BaseUrl = baseUrl;
        }

        public string GetBaseUrl()
        {
            return BaseUrl;
        }
    }

    public interface ICronConfig
    {
        string GetBaseUrl();
    }
}
