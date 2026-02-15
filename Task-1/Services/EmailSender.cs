using System.Net.Mail;
using System.Net;
using Task_1.Interface;

namespace Task_1.Services
{
    public class EmailSender : IEmailSender
    { 
        public async Task SendDailyEmailAsync(string to, string subject, string body)
        {
            try
            {
                var message = new MailMessage("suganth@buson.in", to, subject, body)
                {
                    IsBodyHtml = true
                };

                using (var smtpClient = new SmtpClient("smtppro.zoho.in"))
                {
                    smtpClient.Port = 587;
                    smtpClient.EnableSsl = true;
                    smtpClient.Credentials = new NetworkCredential("suganth@buson.in", "vZLGKyJiz3uu");
                    smtpClient.UseDefaultCredentials = false;
                    await smtpClient.SendMailAsync(message);
                }

                Console.WriteLine("Daily email sent successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
            }
        }
    }
}
