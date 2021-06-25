using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RSSReader.Config
{
    public static class AppSettings
    {
        public static IConfigurationBuilder ConfigureJsonFile(this IConfigurationBuilder builder)
        {
            builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
#if DEBUG
            builder.AddJsonFile("appsettings.Debug.json", optional: true, reloadOnChange: true);
#elif DEVELOPMENT
            builder.AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true);
#endif
            return builder;
        }
    }
}
