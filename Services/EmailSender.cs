using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;

namespace TradeManagementApp.Services
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(subject) || string.IsNullOrEmpty(htmlMessage))
            {
                // Log the issue or handle the error gracefully
                return Task.CompletedTask; // Do nothing if any parameter is null or empty
            }

            // Placeholder: pretend the email is sent
            return Task.CompletedTask;
        }
    }
}
