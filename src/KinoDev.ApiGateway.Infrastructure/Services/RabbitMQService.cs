using KinoDev.ApiGateway.Infrastructure.Models.ConfigurationSettings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace KinoDev.ApiGateway.Infrastructure.Services
{
    public class RabbitMQService : IMessageBrokerService, IAsyncDisposable
    {
        private IConnection _connection;

        private IChannel _channel;

        private readonly ILogger<RabbitMQService> _logger;

        private readonly RabbitMqSettings _settings;

        public RabbitMQService(IOptions<RabbitMqSettings> rabbitMqOptions, ILogger<RabbitMQService> logger)
        {
            _logger = logger;
            _settings = rabbitMqOptions.Value;
        }

        public async Task PublishAsync(object data, string subscription, string key = "")
        {
            _logger.LogInformation("Settings: {Settings}", _settings);
            await ValdiateConnectionState(subscription);

            var message = System.Text.Json.JsonSerializer.Serialize(data);
            var body = System.Text.Encoding.UTF8.GetBytes(message);

            await _channel.BasicPublishAsync(
                subscription,
                key,
                body: body
            );
        }

        public async ValueTask DisposeAsync()
        {
            if (_channel != null)
            {
                await _channel.DisposeAsync();
                _channel = null;
            }

            if (_connection != null)
            {
                await _connection.DisposeAsync();
                _connection = null;
            }
        }

        private async Task ValdiateConnectionState(string exchange)
        {
            var validateExchange = false;

            _logger.LogInformation("Validating connection state...");
            _logger.LogInformation("Connection: {Connection}", _connection);
            _logger.LogInformation("Channel: {Channel}", _channel);


            if (_connection == null || !_connection.IsOpen)
            {
                _logger.LogInformation("Creating new connection and channel...");
                try
                {
                    var factory = new ConnectionFactory
                    {
                        HostName = _settings.HostName,
                        Port = _settings.Port,
                        UserName = _settings.UserName,
                        Password = _settings.Password,
                    };

                    // Close the existing channel if it's open
                    if (_channel != null && _channel.IsOpen)
                    {
                        _logger.LogInformation("Closing existing channel...");
                        await _channel.CloseAsync();
                    }

                    _connection = await factory.CreateConnectionAsync();
                    _channel = await _connection.CreateChannelAsync();
                    validateExchange = true;

                    _logger.LogInformation("Successfully connected to RabbitMQ.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to connect to RabbitMQ.");
                    throw;
                }
            }
            else if (_channel == null || !_channel.IsOpen)
            {
                _logger.LogInformation("Creating new channel... _channel == null || !_channel.IsOpen");
                try
                {
                    _channel = await _connection.CreateChannelAsync();
                    validateExchange = true;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to create channel.");
                    throw;
                }
            }

            if (validateExchange)
            {
                _logger.LogInformation("Declaring exchange: ExchangeDeclareAsync: {Exchange}", exchange);
                // TODO: Move to deparate method as we can have multiple exchanges in the future
                await _channel.ExchangeDeclareAsync(
                    exchange: exchange,
                    type: ExchangeType.Fanout,
                    durable: true,
                    autoDelete: false,
                    arguments: null
                );
            }
        }

        public async Task SubscribeAsync(string subscription, Func<string, Task> callback, string key = "")
        {
            await ValdiateConnectionState(subscription);

            _logger.LogInformation("Subscribing to exchange !!!: {Exchange}", subscription);

            _logger.LogInformation("Creating queue: {Queue}", subscription);
            _logger.LogInformation("Channel: {Channel}", _channel);


            var queue = await _channel.QueueDeclareAsync("gateway-queue-q");
            await _channel.QueueBindAsync(
                queue: queue,
                exchange: "order-completed",
                routingKey: ""
            );
            _logger.LogInformation("Queue: {Queue}", queue);

            var consumer = new AsyncEventingBasicConsumer(_channel);
            _logger.LogInformation("Creating consumer: {Consumer}", consumer);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                System.Console.WriteLine("Received message **************************");
                var body = ea.Body.ToArray();
                var message = System.Text.Encoding.UTF8.GetString(body);
                await callback(message);
                // await _channel.BasicAckAsync(ea.DeliveryTag, false);
            };

            _logger.LogInformation("BasicConsumeAsync");


            await _channel.BasicConsumeAsync(
                queue: queue,
                autoAck: true,
                consumer: consumer
            );
        }
    }
}