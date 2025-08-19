using Ecommerce.WebApp.Models;
using System.Net.Http.Json;

namespace Ecommerce.WebApp.Services
{
    public class OrderService : IOrderService
    {
        private readonly HttpClient _httpClient;

        public OrderService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Order?> CreateOrder(CreateOrderRequest orderRequest)
        {
            var response = await _httpClient.PostAsJsonAsync("/gateway/orders", orderRequest);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Order>();
            }
            
            return null;
        }

        public async Task<List<Order>?> GetOrders()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<Order>>("/gateway/orders");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar pedidos: {ex.Message}");
                return null;
            }
        }
    }
}