namespace Sales.API.Services
{
    public record OrderMessage(int ProductId, int Quantity);

    public interface IMessageBusClient
    {
        void PublishOrder(OrderMessage orderMessage);
    }
}