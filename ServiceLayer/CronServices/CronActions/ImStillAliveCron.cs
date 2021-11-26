using Microsoft.Extensions.Logging;
using ServiceLayer.SmtpService;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceLayer.CronServices
{
    public class ImStillAliveCron : CronJobService
    {
        const string TASK_NAME = "[I AM STILL ALIVE]";
        const string URL_HEALTH_CHECK = "/api/blog/search?value=https://dev";

        private readonly ISmtpService _smtpService;
        private readonly ILogger<ImStillAliveCron> _logger;
        private readonly string _urlBase;

        public ImStillAliveCron(
            IScheduleConfig<ImStillAliveCron> config,
            ISmtpService smtpService,
            ILogger<ImStillAliveCron> logger,
            ICronConfig cron_config)
            : base(config.CronExpression, config.TimeZoneInfo)
        {
            _smtpService = smtpService;
            _logger = logger;
            _urlBase = cron_config.GetBaseUrl();
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

            using (var client = new HttpClient())
            {
                try
                {
                    string url = _urlBase + URL_HEALTH_CHECK;

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
                    _smtpService.SendEmailToAdministration($"{TASK_NAME} server refresh failer", $"{ex}");
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
