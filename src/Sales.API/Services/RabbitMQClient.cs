using Microsoft.EntityFrameworkCore.Metadata;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Sales.API.Services
{
    public class RabbitMQClient : IMessageBusClient
    {
        private readonly IConnection _connection;
        private readonly RabbitMQ.Client.IModel _channel;
        private const string QueueName = "order-created-queue";

        public RabbitMQClient()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: QueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
        }

        public void PublishOrder(OrderMessage orderMessage)
        {
            var message = JsonSerializer.Serialize(orderMessage);
            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(exchange: "",
                                  routingKey: QueueName,
                                  basicProperties: null,
                                  body: body);

            Console.WriteLine($"--> Mensagem publicada no RabbitMQ: {message}");
        }
    }
}