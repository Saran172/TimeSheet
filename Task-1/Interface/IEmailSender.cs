namespace Task_1.Interface
{
    public interface IEmailSender
    {
        Task SendDailyEmailAsync(string to, string subject, string body);
    }
}
