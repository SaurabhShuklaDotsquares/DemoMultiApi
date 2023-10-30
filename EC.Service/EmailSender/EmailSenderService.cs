using Microsoft.Extensions.Configuration;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace EC.Service
{
    public class EmailSenderService : IEmailsTemplateService
    {
        private readonly IConfiguration configuration;
        public EmailSenderService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public Task SendEmailAsync(string email, string subject, string message, string bcc = null)
        {
            try
            {
            using (var client = new SmtpClient())
            {
                var credential = new NetworkCredential
                {
                    UserName = configuration["Email:SmtpMail"],
                    Password = configuration["Email:SmtpPassword"]
                };
                client.Credentials = credential;
                client.Host = configuration["Email:SmtpHost"];
                client.Port = int.Parse(configuration["Email:SmtpPort"]);
                client.EnableSsl = Convert.ToBoolean(configuration["Email:SmtpEnableSsl"]);

                using (var emailMessage = new MailMessage())
                {
                    emailMessage.To.Add(new MailAddress(email));
                    if (!string.IsNullOrWhiteSpace(bcc))
                    {
                        emailMessage.Bcc.Add(new MailAddress(bcc));
                    }
                    emailMessage.IsBodyHtml = true;
                    emailMessage.From = new MailAddress(configuration["Email:SmtpMail"], configuration["Email:DisplayName"]);
                    emailMessage.Subject = subject;
                    emailMessage.Body = message;
                    emailMessage.BodyEncoding = System.Text.Encoding.UTF8;
                    emailMessage.SubjectEncoding = System.Text.Encoding.Default;
                    client.Send(emailMessage);
                }
            }
            }
            catch (Exception ex)
            {

                throw;
            }
            return Task.CompletedTask;
        }
    }
}
