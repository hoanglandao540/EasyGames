using System;
using System.Threading.Tasks;

namespace EasyGames.Web.Services
{
    // Simple email service interface for later use from Checkout/POS etc.
    public interface IEmailService
    {
        // We keep it async-shaped even though it just logs for now
        Task SendAsync(string to, string subject, string body);
    }

    // no SMTP; just writes to console so we can see it in tests.
    public class EmailService : IEmailService
    {
        public Task SendAsync(string to, string subject, string body)
        {
            //For this assignment we just "pretend" to send email by logging it
            Console.WriteLine($"[EmailService] To: {to} | Subject: {subject} | Body: {body}");
            return Task.CompletedTask;
        }
    }
}
