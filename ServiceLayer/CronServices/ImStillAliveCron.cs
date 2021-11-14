using Dtos.Jobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServiceLayer.JobServices;
using ServiceLayer.SmtpService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceLayer.CronServices
{
    public class ImStillAliveCron : CronJobService
    {
        const string TASK_NAME = "[I AM STILL ALIVE]";
        private readonly ISmtpService _smtpService;
        private readonly ILogger<ImStillAliveCron> _logger;

        public ImStillAliveCron(
            IScheduleConfig<ImStillAliveCron> config,
            ISmtpService smtpService,
            ILogger<ImStillAliveCron> logger)
            : base(config.CronExpression, config.TimeZoneInfo)
        {
            _smtpService = smtpService;
            _logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{TASK_NAME} job starts.");
            return base.StartAsync(cancellationToken);
        }

        public override Task DoWork(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{DateTime.Now:hh:mm:ss} {TASK_NAME} is working.");

            _smtpService.SendEmailToAdministration($"Server is working", $"Server is still working");

            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{TASK_NAME} is stopping.");

            return base.StopAsync(cancellationToken);
        }
    }
}
