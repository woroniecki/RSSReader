using Microsoft.Extensions.Logging;
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
    public class UpdateBlogsCron : CronJobService
    {
        private readonly ILogger<UpdateBlogsCron> _logger;

        public UpdateBlogsCron(IScheduleConfig<UpdateBlogsCron> config, ILogger<UpdateBlogsCron> logger)
            : base(config.CronExpression, config.TimeZoneInfo)
        {
            _logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("UpdateBlogsCron job starts.");
            return base.StartAsync(cancellationToken);
        }

        public override Task DoWork(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{DateTime.Now:hh:mm:ss} UpdateBlogsCron is working.");
            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("UpdateBlogsCron is stopping.");
            return base.StopAsync(cancellationToken);
        }
    }
}
