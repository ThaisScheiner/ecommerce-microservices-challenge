namespace Ecommerce.WebApp.Models
{
    // Modelo para a requisição de criação de um pedido
    public class CreateOrderRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    // Modelo para a resposta (o pedido que foi criado)
    public class Order
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
    }
}