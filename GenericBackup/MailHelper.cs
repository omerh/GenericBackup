using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace GenericBackup
{
    internal class MailHelper
    {
        private static readonly ILog log = LogManager.GetLogger(typeof (BackupHelper));

        internal static void SendEmail(string subject, string body)
        {
            log.Info("Issuing email for " + subject);
            var message = new MailMessage();
            message.To.Add(ConfigurationManager.AppSettings["To"]);
            message.Body = body;
            message.Subject = subject;
            message.From = new System.Net.Mail.MailAddress(Environment.MachineName + "@conduit.com");
            var smtp = new System.Net.Mail.SmtpClient(ConfigurationManager.AppSettings["smtp"]);
            smtp.Send(message);

            log.Debug("mails was sent" + subject);
        }


    }
}
