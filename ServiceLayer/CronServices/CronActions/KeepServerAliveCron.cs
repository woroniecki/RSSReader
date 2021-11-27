using Microsoft.Extensions.Logging;
using ServiceLayer.SmtpService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceLayer.CronServices
{
    public class KeepServerAliveCron : CronJobService
    {
        const string TASK_NAME = "[Keep Server Alive]";
        const string URL_HEALTH_CHECK = "/api/blog/search?value=https://dev";

        private readonly ISmtpService _smtpService;
        private readonly ILogger<KeepServerAliveCron> _logger;
        private readonly IEnumerable<string> _urls;

        public KeepServerAliveCron(
            IScheduleConfig<KeepServerAliveCron> config,
            ISmtpService smtpService,
            ILogger<KeepServerAliveCron> logger,
            ICronConfig cron_config)
            : base(config.CronExpression, config.TimeZoneInfo)
        {
            _smtpService = smtpService;
            _logger = logger;
            _urls = cron_config.GetBaseUrl();
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{TASK_NAME} job starts.");

            _smtpService.SendEmailToAdministration($"{TASK_NAME} StartAsync", $"Server is working");

            return base.StartAsync(cancellationToken);
        }

        public override Task DoWork(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{DateTime.Now:hh:mm:ss} {TASK_NAME} is working.");

            foreach (var urlBase in _urls)
            {
                using (var client = new HttpClient())
                {
                    try
                    {
                        string url = urlBase + URL_HEALTH_CHECK;

                        var request = WebRequest.Create(url);
                        request.Method = "GET";

                        using var webResponse = request.GetResponse();
                        using var webStream = webResponse.GetResponseStream();

                        using (var reader = new StreamReader(webStream))
                        {
                            var data = reader.ReadToEnd();

                            _logger.LogInformation($"{TASK_NAME} success.");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"{TASK_NAME} failed to ping {urlBase}.");
                        //_smtpService.SendEmailToAdministration($"{TASK_NAME} server refresh failer", $"{ex}");
                    }
                }
            }

            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{TASK_NAME} is stopping.");

            _smtpService.SendEmailToAdministration($"{TASK_NAME} StopAsync", $"{Environment.StackTrace}");

            return base.StopAsync(cancellationToken);
        }
    }
}
