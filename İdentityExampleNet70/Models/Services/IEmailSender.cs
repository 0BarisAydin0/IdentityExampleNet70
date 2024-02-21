namespace İdentityExampleNet70.Models.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
    }
}
