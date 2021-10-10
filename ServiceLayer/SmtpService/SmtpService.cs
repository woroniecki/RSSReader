using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.SmtpService
{
    public class SmtpService : ISmtpService
    {
        public ISmtpConfig _config { get; }
        public ILogger<SmtpService> _logger { get; }
        private SmtpClient _smtpClient;

        public SmtpService(ISmtpConfig config, ILogger<SmtpService> logger)
        {
            _config = config;
            _logger = logger;

            CreateClient();
        }

        private void CreateClient()
        {
            NetworkCredential basicCredential = new NetworkCredential(_config.GetUserName(), _config.GetPassword());

            _smtpClient = new SmtpClient();
            _smtpClient.Host = _config.GetHost();
            _smtpClient.Port = _config.GetPort();
            _smtpClient.UseDefaultCredentials = false;
            _smtpClient.Credentials = basicCredential;
        }

        bool CanSendEmail()
        {
            return _config.IsEnabled();
        }

        public bool SendEmailToAdministration(string title, string msg)
        {
            if (!CanSendEmail())
                return false;

            MailMessage message = new MailMessage();
            MailAddress fromAddress = new MailAddress(_config.GetSender());

            message.From = fromAddress;
            message.Subject = title;
            message.Body = msg;
            _config.GetRecipients().ForEach(x => message.To.Add(x));

            try
            {
                _smtpClient.Send(message);
            }
            catch (Exception ex)
            {
                _logger.LogWarning("SendEmailToAdministration failed to send email.");
                return false;
            }

            return true;
        }
    }

    public interface ISmtpService
    {
        /// <summary>
        /// Sends email to all adresses setted in config file
        /// Used to easy communication between server and administrators
        /// </summary>
        /// <param name="title">Email title</param>
        /// <param name="msg">Email message</param>
        /// <returns></returns>
        bool SendEmailToAdministration(string title, string msg);
    }
}
