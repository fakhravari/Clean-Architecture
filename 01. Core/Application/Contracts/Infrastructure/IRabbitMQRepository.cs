namespace Application.Contracts.Infrastructure
{
    public interface IRabbitMQRepository
    {
        Task SendMessageAsync(string ToEmail, string Subject, string Message);
        Task ReceiveMessagesAsync();
    }
}
