using System;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;


namespace SmartAdmin.WebUI.Models
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }

    public class EmailSender : IEmailSender
    {
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            SmtpClient client = setValidateCredential();
            if (client != null)
            {

                MailAddress from = new MailAddress("admin.reg@ruya.sch.sa", "Ruya School");
                MailAddress to = new MailAddress(email);
                MailMessage message = new MailMessage(from, to);


                // Set subject of the message, body and sender information
                message.Subject = subject;
                message.Body = htmlMessage;
                message.IsBodyHtml = true;
                client.Send(message);
            }
        }

        public SmtpClient setValidateCredential()
        {
            try
            {
                SmtpClient client = new SmtpClient("ruya-sch-sa.mail.protection.outlook.com");
                client.Port = 25;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                System.Net.NetworkCredential credential =
                    new System.Net.NetworkCredential("admin.reg@ruya.sch.sa", "RuyaReg123456");
                client.Credentials = credential;
                client.EnableSsl = true;
                return client;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
