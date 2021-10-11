using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Mail;

namespace ServiceLayer.SmtpService
{
    public class SmtpService : ISmtpService
    {
        public ISmtpConfig _config { get; }
        public ILogger<SmtpService> _logger { get; }

        public SmtpService(ISmtpConfig config, ILogger<SmtpService> logger)
        {
            _config = config;
            _logger = logger;
        }

        bool CanSendEmail()
        {
            return _config.IsEnabled();
        }

        public bool SendEmailToAdministration(string title, string msg)
        {
            if (!CanSendEmail())
                return false;

            NetworkCredential basic_credential = new NetworkCredential(_config.GetUserName(), _config.GetPassword());

            SmtpClient smtp_client = new SmtpClient();
            smtp_client.Host = _config.GetHost();
            smtp_client.Port = _config.GetPort();
            smtp_client.UseDefaultCredentials = false;
            smtp_client.Credentials = basic_credential;

            MailMessage message = new MailMessage();
            MailAddress from_address = new MailAddress(_config.GetSender());

            message.From = from_address;
            message.Subject = title;
            message.Body = msg;
            _config.GetRecipients().ForEach(x => message.To.Add(x));

            try
            {
                smtp_client.Send(message);
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
