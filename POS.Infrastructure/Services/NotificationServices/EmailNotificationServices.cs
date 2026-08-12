using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace POS.Infrastructure.Services
{
    public interface IEmailNotificationService
    {
        Task SendActivationEmailAsync(string email, string username, string activationToken);
    }
    public class EmailNotificationService(IConfiguration configuration) : IEmailNotificationService
    {

        public async Task SendActivationEmailAsync(string email, string username, string activationToken)
        {
            // 1. Fetch values from your appsettings.json
            string smtpHost = configuration["EmailSettings:SmtpHost"] ?? throw new InvalidOperationException("SMTP Host not configured.");
            int smtpPort = int.Parse(configuration["EmailSettings:SmtpPort"] ?? "587");
            string fromEmail = configuration["EmailSettings:FromEmail"] ?? throw new InvalidOperationException("Sender email not configured.");
            string password = configuration["EmailSettings:Password"] ?? throw new InvalidOperationException("Email password not configured.");
            string webAppUrl = configuration["EmailSettings:WebAppUrl"] ?? "https://localhost:7001";

            // 2. Build the activation link pointing to your frontend or API endpoint
            string activationLink = $"{webAppUrl}/api/registration/activate?token={activationToken}&email={email}";

            // 3. Compose the HTML Body
            string emailBody = $"""
            <h1>Welcome to SAFAQATECH, {username}!</h1>
            <p>Thank you for registering your vendor profile.</p>
            <p>Please click the link below to verify your email address and activate your system access:</p>
            <p><a href="{activationLink}" style="padding:10px 20px; background-color:#007bff; color:white; text-decoration:none; border-radius:5px;">Activate My Account</a></p>
            <br/>
            <p>If the button doesn't work, copy and paste this URL into your browser:</p>
            <p>{activationLink}</p>
            """;

            // 4. Configure SMTP Client and dispatch message
            using var client = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(fromEmail, password),
                EnableSsl = true
            };

            using var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail, "NewWay Logistic Onboarding"),
                Subject = "Activate Your Vendor Account",
                Body = emailBody,
                IsBodyHtml = true
            };

            mailMessage.To.Add(email);

            await client.SendMailAsync(mailMessage);
        }
    }
}
