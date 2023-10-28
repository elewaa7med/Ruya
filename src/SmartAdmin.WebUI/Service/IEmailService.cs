using System.Net.Mail;

namespace SmartAdmin.WebUI.Service
{
    public interface IEmailService
    {
        bool SendEmailAsync(string email, string subject, string htmlMessage);
        bool SendEmailAlternativeAsync(string email, string subject, AlternateView htmlMessage);
    }
}
