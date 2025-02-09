using System.Threading.Tasks;

namespace API.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string recipientEmail, string subject, string body);
        Task SendEmailWithTemplateAsync(string recipientEmail, string subject, string body);
    }
}
