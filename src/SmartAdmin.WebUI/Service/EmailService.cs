using System;
using System.Net.Mail;

namespace SmartAdmin.WebUI.Service
{
    public class EmailService : IEmailService
    {
        public bool SendEmailAsync(string email, string subject, string htmlMessage)
        {
            SmtpClient client = setValidateCredential();
            if (client != null)
            {
                MailAddress from = new MailAddress("admin.reg@ruya.sch.sa", "Ruya School");
                MailAddress to = new MailAddress(email);
                MailMessage message = new MailMessage(from, to);

                // Set subject of the message, body and sender information
                message.Subject = subject;
                message.IsBodyHtml = true;
                message.Body = htmlMessage;
                try
                {
                    client.Send(message);
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public bool SendEmailAlternativeAsync(string email, string subject, AlternateView htmlMessage)
        {
            SmtpClient client = setValidateCredential();
            if (client != null)
            {
                MailAddress from = new MailAddress("admin.reg@ruya.sch.sa", "Ruya School");
                MailAddress to = new MailAddress(email);
                MailMessage message = new MailMessage(from, to);

                // Set subject of the message, body and sender information
                message.Subject = subject;
                message.IsBodyHtml = true;
                message.AlternateViews.Add(htmlMessage);
                try
                {
                    client.Send(message);
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            else
            {
                return false;
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
