using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using Stock.API.Data;
using System.Text;
using System.Text.Json;

namespace Stock.API.Consumers
{
    public class OrderCreatedConsumer : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private IConnection? _connection;
        private IModel? _channel; 
        private const string QueueName = "order-created-queue";

        public OrderCreatedConsumer(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        private void InitializeRabbitMQ(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory { HostName = "localhost" };

            // Lógica de Retry para esperar o RabbitMQ iniciar
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _connection = factory.CreateConnection();
                    _channel = _connection.CreateModel();
                    _channel.QueueDeclare(queue: QueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
                    Console.WriteLine("--> Conectado ao RabbitMQ e pronto para consumir mensagens.");
                    break; 
                }
                catch (BrokerUnreachableException)
                {
                    Console.WriteLine("--> Não foi possível conectar ao RabbitMQ. Tentando novamente em 5 segundos...");
                    Task.Delay(5000, stoppingToken).Wait(stoppingToken); 
                }
            }
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Chama a inicialização com retry aqui, não no construtor
            InitializeRabbitMQ(stoppingToken);

            if (_channel == null)
            {
                // Se a conexão não foi estabelecida (ex: app foi cancelado), não faz nada.
                return Task.CompletedTask;
            }

            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                try
                {
                    var orderData = JsonSerializer.Deserialize<OrderMessage>(message);

                    if (orderData != null)
                    {
                        using var scope = _serviceProvider.CreateScope();
                        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                        var product = await dbContext.Products.FindAsync(new object[] { orderData.ProductId }, stoppingToken);

                        if (product != null && product.QuantityInStock >= orderData.Quantity)
                        {
                            product.QuantityInStock -= orderData.Quantity;
                            await dbContext.SaveChangesAsync(stoppingToken);
                            Console.WriteLine($"--> Estoque do produto ID {product.Id} atualizado para {product.QuantityInStock}.");
                        }
                        else
                        {
                            Console.WriteLine($"--> [AVISO] Produto não encontrado ou sem estoque. ID do Produto: {orderData.ProductId}");
                        }
                    }
                }
                catch (JsonException)
                {
                    Console.WriteLine($"--> [ERRO] Não foi possível deserializar a mensagem: {message}");
                }
            };

            _channel.BasicConsume(queue: QueueName, autoAck: true, consumer: consumer);

            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
            base.Dispose();
        }
    }

    public record OrderMessage(int ProductId, int Quantity);
}