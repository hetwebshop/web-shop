using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jobify_azure_function
{
    public interface IEmailService
    {
        Task SendEmailAsync(string recipientEmail, string subject, string body);
        Task SendEmailWithTemplateAsync(string recipientEmail, string subject, string body);
    }
}
