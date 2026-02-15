using DevExpress.XtraPrinting.Export;
using Microsoft.AspNetCore.Cors.Infrastructure;
using System.Net.Mail;
using System.Net;
using Task_1.Interface;
using Microsoft.EntityFrameworkCore;
using static System.Formats.Asn1.AsnWriter;
using System.Security.Policy;
using Task_1.DbCon;

namespace Task_1.Services
{
    public class EmailBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _sendTime = new TimeSpan(20, 30, 0); // 8:30 PM on working days
   
        public EmailBackgroundService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var now = DateTime.Now;

                    if (now.DayOfWeek == DayOfWeek.Sunday)
                    {
                        Console.WriteLine("Emails are not sent on sundays");
                        await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
                        continue;
                    }

                    if (now.DayOfWeek == DayOfWeek.Saturday)
                    {
                        var currentMonth = now.Month;
                        var firstDayOfMonth = new DateTime(now.Year, currentMonth, 1);
                        var dayOfWeekOffset = (int)DayOfWeek.Saturday - (int)firstDayOfMonth.DayOfWeek;
                        dayOfWeekOffset = dayOfWeekOffset < 0 ? dayOfWeekOffset + 7 : dayOfWeekOffset;


                        var secondSaturday = firstDayOfMonth.AddDays(dayOfWeekOffset + 7);
                        var fourthSaturday = firstDayOfMonth.AddDays(dayOfWeekOffset + 21);

                        if(now.Date == secondSaturday.Date || now.Date == fourthSaturday.Date)
                        {
                            Console.WriteLine("Skipping mails for 2nd and 4th Saturday of the month");
                            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
                            continue;
                        }
                    }

                    var nextRun = now.Date.Add(_sendTime);
                    if (now > nextRun)
                    {
                        nextRun = nextRun.AddDays(1); 
                    }

                    var delay = nextRun - now;
                    Console.WriteLine($"Next email scheduled at: {nextRun}");

                    await Task.Delay(delay, stoppingToken);

                    await CheckAndNotifyWorkCompletion();

                    Console.WriteLine("Daily email check completed successfully.");
                }
                catch (TaskCanceledException)
                {
                    break; 
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in EmailBackgroundService: {ex.Message}");
                }
            }
        }

        private async Task CheckAndNotifyWorkCompletion()
        {
            // List of employee IDs to check
            var employeeIds = await GetEmployeeIds(); // Assume this fetches employee IDs (or you can pass them in)
                                                      // Set to track employees who have already been notified
            var notifiedEmails = new HashSet<string>();

            foreach (var employeeId in employeeIds)
            {
                // Resolve IWorkService and CustomerDbContext inside a scope
                using (var scope = _serviceProvider.CreateScope())
                {
                    var workService = scope.ServiceProvider.GetRequiredService<IWorkService>();
                    var customerDbContext = scope.ServiceProvider.GetRequiredService<CustomerDbContext>();

                    // Retrieve the list of today's work for the employee
                    var workinglist = await workService.GetWorkToday(employeeId);

                    // If no work is completed for today, send an email
                    if (workinglist.Count == 0)
                    {
                        string url = "http://dev.sellerkit.in:72";
                        string linkText = "You have not completed your TimeSheet for today. Please complete it as soon as possible.";
                        string subject = "Time Sheet Pending";
                        string body = $"<p>Dear Employee,</p><p>{linkText}: <a href=\"{url}\" target=\"_blank\">{url}</a></p><p>Best regards,</p><p>BUSON DIGITAL SERVICE SERVICES INDIA PVT. LTD</p>";

                        // Get the list of emails excluding specific employees
                        var employeeEmails = await GetEmployeeEmail(customerDbContext);

                        foreach (var employeeEmail in employeeEmails)
                        {
                            if (!string.IsNullOrEmpty(employeeEmail) && !notifiedEmails.Contains(employeeEmail))
                            {
                                // Send the email
                                await SendEmailAsync(employeeEmail, subject, body);
                                Console.WriteLine($"Sent email to {employeeEmail}.");

                                // Add employee to the notified list to avoid sending multiple emails
                                notifiedEmails.Add(employeeEmail);
                            }
                        }
                       
                    }
                    else
                    {
                        Console.WriteLine($"Employee {employeeId} has completed work for today.");
                    }
                }
            }
        }
        
        private async Task SendEmailAsync(string to, string subject, string body)
        {
            try
            {
                var sender = new MailAddress("suganth@buson.in", "BUSON DIGITAL SERVICE SERVICES INDIA PVT. LTD");
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
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
            }
        }

        private async Task<List<int>> GetEmployeeIds()
        {
            // Create a scope and resolve CustomerDbContext to get employee IDs
            using (var scope = _serviceProvider.CreateScope())
            {
                var customerDbContext = scope.ServiceProvider.GetRequiredService<CustomerDbContext>();
                return await customerDbContext.Employee
                    .Select(e => e.EMP_ID)
                    .ToListAsync(); // Return list of employee IDs
            }
        }

        private async Task<List<string?>> GetEmployeeEmail(CustomerDbContext customerDbContext)
        {
            var excludeEmpid = new List<int> { 5 };

            var employeeEmails = await customerDbContext.Employee
                .Where(e => excludeEmpid.Contains(e.EMP_ID))
                .Select(e => e.EMP_CONTACTMAIL)
                .ToListAsync();
            return employeeEmails;
            //var employee = await customerDbContext.Employee
            //    .FirstOrDefaultAsync(e => e.EMP_ID == employeeId);

            //return employee?.EMP_CONTACTMAIL; // Return email if found, otherwise null
        }
    }

}
