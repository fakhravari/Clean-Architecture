using Application.Contracts.Infrastructure;
using Domain.Model.RabiitMQ;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Infrastructure.RabbitMQR
{
    public class RabbitMQRepository : IRabbitMQRepository, IDisposable
    {
        private readonly ILogger<RabbitMQRepository> _logger;
        private readonly RabbitMQSettingModel _settings;
        private IConnection _connection;
        private IModel _channel;

        public RabbitMQRepository(IOptions<RabbitMQSettingModel> settings, ILogger<RabbitMQRepository> logger)
        {
            _settings = settings.Value;
            _logger = logger;

            var factory = new ConnectionFactory()
            {
                Uri = new Uri(_settings.Uri),
                HostName = _settings.HostName,
                UserName = _settings.UserName,
                Password = _settings.Password
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(queue: _settings.QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
            _logger.LogInformation("RabbitMQ connection established.");
        }

        public Task SendMessageAsync(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(exchange: "", routingKey: _settings.QueueName, basicProperties: null, body: body);
            _logger.LogInformation("Message sent: {Message}", message);

            return Task.CompletedTask;
        }

        public Task ReceiveMessagesAsync()
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

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _channel?.Close();
            _channel?.Dispose();
            _connection?.Close();
            _connection?.Dispose();
            _logger.LogInformation("RabbitMQ resources disposed.");
        }
    }
}
