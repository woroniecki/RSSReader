using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.SmtpService
{
    public interface ISmtpConfig
    {
        bool IsEnabled();
        string GetUserName();
        string GetPassword();
        string GetHost();
        int GetPort();
        string GetSender();
        List<string> GetRecipients();
    }
}
