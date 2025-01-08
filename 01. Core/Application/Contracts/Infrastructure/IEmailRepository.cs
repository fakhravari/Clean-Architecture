namespace Application.Contracts.Infrastructure
{
    public interface IEmailRepository
    {
        Task<bool> SendEmailAsync(string toEmail, string subject, string message);
    }
}
