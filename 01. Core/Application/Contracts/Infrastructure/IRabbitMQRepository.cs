namespace Application.Contracts.Infrastructure
{
    public interface IRabbitMQRepository
    {
        Task SendMessageAsync(string message);
        Task ReceiveMessagesAsync();
    }
}
