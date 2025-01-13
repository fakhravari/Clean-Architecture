using Application.Contracts.Infrastructure;
using Application.Contracts.Persistence.IRepository;
using Domain.Model.RabbitMQ;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Infrastructure.RabbitMQR;

public class RabbitMQRepository : IRabbitMQRepository, IDisposable
{
    private readonly ILogger<RabbitMQRepository> _logger;
    private readonly RabbitMQSettingModel _settings;
    private IConnection _connection;
    private IModel _channel;

    private readonly IServiceProvider _serviceProvider;

    private readonly string Queue_Email = "Queue_Email";
    public RabbitMQRepository(IOptions<RabbitMQSettingModel> settings, ILogger<RabbitMQRepository> logger, IServiceProvider serviceProvider)
    {
        _settings = settings.Value;
        _logger = logger;
        _serviceProvider = serviceProvider;


        var factory = new ConnectionFactory()
        {
            Uri = new Uri(_settings.Uri),
            HostName = _settings.HostName,
            UserName = _settings.UserName,
            Password = _settings.Password
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();



        _channel.QueueDeclare(queue: Queue_Email, durable: true, exclusive: false, autoDelete: false, arguments: null);
        _logger.LogInformation("RabbitMQ connection established.");
    }

    public async Task SendMessageAsync(string ToEmail, string Subject, string Message)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var _personelRepository = scope.ServiceProvider.GetRequiredService<IPersonelRepository>();
            var RetId = await _personelRepository.RabbitMQSend(Subject, Message, "fakhravary@gmail.com");
            if (RetId > 0)
            {
                var body = Encoding.UTF8.GetBytes(RetId.ToString());

                _channel.BasicPublish(exchange: "", routingKey: Queue_Email, basicProperties: null, body: body);
                _logger.LogInformation("Message sent: {Message}", Message);
            }
            else
            {
                _logger.LogInformation("Message sent RabbitMQSend: error {Message}", Message);
            }
        }
    }

    public Task ReceiveMessagesAsync()
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            using (var scope = _serviceProvider.CreateScope())
            {
                var _personelRepository = scope.ServiceProvider.GetRequiredService<IPersonelRepository>();
                _personelRepository.RabbitMQReceive();
            }

            _logger.LogInformation("Message received: {Message}", message);
        };

        _channel.BasicConsume(queue: Queue_Email, autoAck: true, consumer: consumer);
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