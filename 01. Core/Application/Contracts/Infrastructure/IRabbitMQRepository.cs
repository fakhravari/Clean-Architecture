namespace Application.Contracts.Infrastructure
{
    public interface IRabbitMQRepository : IDisposable
    {
        Task<bool> SendMessageAsync(string message);
        Task<bool> ReceiveMessagesAsync();
    }
}
