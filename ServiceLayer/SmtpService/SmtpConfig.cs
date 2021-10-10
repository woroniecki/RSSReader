using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.SmtpService
{
    public class SmtpConfig : ISmtpConfig
    {
        private bool Enabled { get; }
        private string UserName { get; }
        private string Password { get; }
        private string Host { get; }
        private int Port { get; }
        private string Sender { get; }
        private List<string> Recipients { get; }

        public SmtpConfig(
            bool enabled,
            string username,
            string password,
            string host,
            int port,
            string sender,
            List<string> recipients
            )
        {
            Enabled = enabled;
            UserName = username;
            Password = password;
            Host = host;
            Port = port;
            Sender = sender;
            Recipients = recipients;
        }

        public bool IsEnabled() => Enabled;
        public string GetUserName() => UserName;
        public string GetPassword() => Password;
        public string GetHost() => Host;
        public int GetPort() => Port;
        public string GetSender() => Sender;
        public List<string> GetRecipients() => Recipients;
    }
}
