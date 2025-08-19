using Ecommerce.WebApp.Models;

namespace Ecommerce.WebApp.Services
{
    public interface IOrderService
    {
        Task<Order?> CreateOrder(CreateOrderRequest orderRequest);
        Task<List<Order>?> GetOrders();
    }
}