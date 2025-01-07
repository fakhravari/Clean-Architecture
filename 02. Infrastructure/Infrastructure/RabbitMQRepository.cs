using Application.Contracts.Infrastructure;
using Domain.Model.RabiitMQ;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Infrastructure
{
    public class RabbitMQRepository : IRabbitMQRepository
    {
        private readonly ILogger<RabbitMQRepository> _logger;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly RabbitMQSettingModel _settings;

        public RabbitMQRepository(IOptions<RabbitMQSettingModel> settings, ILogger<RabbitMQRepository> logger)
        {
            _settings = settings.Value;
            _logger = logger;

            var factory = new ConnectionFactory()
            {
                HostName = _settings.HostName,
                UserName = _settings.UserName,
                Password = _settings.Password
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(queue: _settings.QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
            _logger.LogInformation("RabbitMQ connection established.");
        }

        public async Task<bool> SendMessageAsync(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(exchange: "", routingKey: _settings.QueueName, basicProperties: null, body: body);

            _logger.LogInformation("Message sent: {Message}", message);
            return true;
        }

        public async Task<bool> ReceiveMessagesAsync()
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                _logger.LogInformation("Message received: {Message}", message);
            };

            _channel.BasicConsume(queue: _settings.QueueName, autoAck: true, consumer: consumer);

            _logger.LogInformation("Waiting for messages...");
            return true;
        }

        public void Dispose()
        {
            _channel?.Close();
            _channel?.Dispose();
            _connection?.Close();
            _connection?.Dispose();
            Console.WriteLine("RabbitMQ resources disposed.");
        }
    }
}
