﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;

namespace ServiceLayer.CronServices
{
    public static class ScheduledServiceExtensions
    {
        public static IServiceCollection AddCronJobs(
            this IServiceCollection services, 
            IConfiguration config)
        {
            AddAppConfig(services, config);

            services.AddCronJob<UpdateBlogsCron>(c =>
            {
                c.TimeZoneInfo = TimeZoneInfo.Local;
                c.CronExpression = @"0 0 * * *";
            });

            services.AddCronJob<KeepServerAliveCron>(c =>
            {
                c.TimeZoneInfo = TimeZoneInfo.Local;
                c.CronExpression = @"*/3 * * * *";
            });

            return services;
        }

        private static void AddAppConfig(IServiceCollection services, IConfiguration config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config), @"Please provide Configuration.");
            }

            services.AddSingleton<IAppConfig>(sp => new AppConfig(
                    config.GetSection("PingUrls").Get<string[]>().ToList(),
                    config.GetValue<string>("ConfigName"),
                    config.GetValue<string>("AppVersion")
                ));
        }

        private static IServiceCollection AddCronJob<T>(this IServiceCollection services, Action<IScheduleConfig<T>> options) where T : CronJobService
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options), @"Please provide Schedule Configurations.");
            }
            var config = new ScheduleConfig<T>();
            options.Invoke(config);
            if (string.IsNullOrWhiteSpace(config.CronExpression))
            {
                throw new ArgumentNullException(nameof(ScheduleConfig<T>.CronExpression), @"Empty Cron Expression is not allowed.");
            }

            services.AddSingleton<IScheduleConfig<T>>(config);
            services.AddHostedService<T>();
            return services;
        }
    }
}
