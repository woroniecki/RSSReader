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
    public class UpdateBlogsCron : CronJobService
    {
        const string TASK_NAME = "[BLOGS UPDATE]";
        private readonly ILogger<UpdateBlogsCron> _logger;
        private readonly ISmtpService _smtpService;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public UpdateBlogsCron(
            IScheduleConfig<UpdateBlogsCron> config,
            ISmtpService smtpService,
            IServiceScopeFactory serviceScopeFactory,
            ILogger<UpdateBlogsCron> logger)
            : base(config.CronExpression, config.TimeZoneInfo)
        {
            _logger = logger;
            _smtpService = smtpService;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("UpdateBlogsCron job starts.");
            return base.StartAsync(cancellationToken);
        }

        public override Task DoWork(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{DateTime.Now:hh:mm:ss} UpdateBlogsCron is working.");

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var update_blogs_service = scope.ServiceProvider.GetService<IUpdateBlogsJobService>();

                try
                {
                    _smtpService.SendEmailToAdministration($"{TASK_NAME} Started", $"Started update blogs job");

                    Task<UpdateBlogsJobResponse> task = Task.Run(() =>
                    {
                        return update_blogs_service.UpdateBlogs();
                    });

                    task.Wait();

                    SendSuccessEmail(task.Result);
                }
                catch (Exception e)
                {
                    _smtpService.SendEmailToAdministration($"{TASK_NAME} Failed", $"Failed with exception:\n {e}");
                }
            }

            return Task.CompletedTask;
        }

        private void SendSuccessEmail(Dtos.Jobs.UpdateBlogsJobResponse result)
        {
            StringBuilder sb = new StringBuilder(
                                $"Success:\n\nFailed: {result.Failed.Count()}\nSucceeded: {result.Succeeded.Count()}\nNo update: {result.NoUpdate.Count()}\n\n",
                                3000
                                );

            sb.Append("[FAILED LIST]\n");
            result.Failed.ForEach(x => sb.Append(x).Append("\n"));

            sb.Append("[SUCCESS LIST]\n");
            result.Succeeded.ForEach(x => sb.Append(x).Append("\n"));

            _smtpService.SendEmailToAdministration($"{TASK_NAME} Success", sb.ToString());
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("UpdateBlogsCron is stopping.");
            return base.StopAsync(cancellationToken);
        }
    }
}
