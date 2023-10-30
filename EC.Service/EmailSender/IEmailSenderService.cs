using System.Threading.Tasks;

namespace EC.Service
{
    public interface IEmailsTemplateService
    {
        Task SendEmailAsync(string email, string subject, string message, string bcc=null);
    }
}
