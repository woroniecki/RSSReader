using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;


namespace ServiceLayer.SmtpService
{
    public static class SmtpExtensions
    {
        public static IServiceCollection AddSmtpConfig(this IServiceCollection services, IConfiguration config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config), @"Please provide Configuration.");
            }

            services.AddSingleton<ISmtpConfig>(sp => new SmtpConfig(
                config.GetSection("Smtp:Enabled").Get<bool>(),
                config.GetSection("Smtp:UserName").Get<string>(),
                config.GetSection("Smtp:Password").Get<string>(),
                config.GetSection("Smtp:Host").Get<string>(),
                config.GetSection("Smtp:Port").Get<int>(),
                config.GetSection("Smtp:Sender").Get<string>(),
                config.GetSection("Smtp:Recipients").Get<string[]>().ToList()
                ));

            return services;
        }
    }
}
