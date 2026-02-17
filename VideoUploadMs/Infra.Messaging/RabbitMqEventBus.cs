using Core.Interfaces;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Infra.Messaging
{
    public class RabbitMqEventBus : IMessagingService
    {
        private readonly IConnection _connection;

        public RabbitMqEventBus(IConnection connection)
        {
            _connection = connection;
        }

        public Task PublishAsync<T>(string routingKey, T message)
        {
            using var channel = _connection.CreateModel();

            channel.QueueDeclare(
                queue: routingKey,
                durable: true,
                exclusive: false,
                autoDelete: false);

            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            channel.BasicPublish(
                exchange: "",
                routingKey: routingKey,
                basicProperties: properties,
                body: body);

            return Task.CompletedTask;
        }
    }

}
