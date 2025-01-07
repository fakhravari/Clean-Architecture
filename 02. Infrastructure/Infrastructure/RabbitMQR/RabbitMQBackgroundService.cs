using Application.Contracts.Infrastructure;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.RabbitMQR
{
    public class RabbitMQBackgroundService : BackgroundService
    {
        private readonly IRabbitMQRepository _rabbitMQRepository;

        public RabbitMQBackgroundService(IRabbitMQRepository rabbitMQRepository)
        {
            _rabbitMQRepository = rabbitMQRepository;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _rabbitMQRepository.ReceiveMessagesAsync();
        }
    }
}
