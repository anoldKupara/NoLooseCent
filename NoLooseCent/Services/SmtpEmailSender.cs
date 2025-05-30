using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace NoLooseCent.Services
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public SmtpEmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var smtp = _configuration.GetSection("SmtpSettings");

            var client = new SmtpClient(smtp["Host"], int.Parse(smtp["Port"]))
            {
                Credentials = new NetworkCredential(smtp["Username"], smtp["Password"]),
                EnableSsl = bool.Parse(smtp["EnableSSL"])
            };

            var mail = new MailMessage
            {
                From = new MailAddress(smtp["SenderEmail"], smtp["SenderName"]),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true
            };

            mail.To.Add(email);
            return client.SendMailAsync(mail);
        }
    }
}