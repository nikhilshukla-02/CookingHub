using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace dotnetapp.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendVerificationEmailAsync(string toEmail, string token)
        {
            var smtpClient = new SmtpClient(_config["Email:SmtpServer"])
            {
                Port = int.Parse(_config["Email:Port"]),
                Credentials = new NetworkCredential(_config["Email:Username"], _config["Email:Password"]),
                EnableSsl = true,
            };

            // Use your backend API base URL – adjust port if needed
            var verifyUrl = $"https://8080-aeecccebfeecdabeebedccecabfaedfdcf.premiumproject.examly.io/api/verify-email?token={token}";

            var message = new MailMessage
            {
                From = new MailAddress(_config["Email:From"]),
                Subject = "Cooking Hub – Verify Your Email",
                Body = $"Please verify your email by clicking <a href='{verifyUrl}'>here</a>. If the link doesn't work, copy and paste this URL into your browser: {verifyUrl}",
                IsBodyHtml = true,
            };
            message.To.Add(toEmail);

            await smtpClient.SendMailAsync(message);
        }
    }
}
