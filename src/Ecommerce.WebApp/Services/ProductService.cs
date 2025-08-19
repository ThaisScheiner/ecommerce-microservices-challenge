using Ecommerce.WebApp.Models;
using System.Net.Http.Json;

namespace Ecommerce.WebApp.Services
{
    public class ProductService : IProductService
    {
        private readonly HttpClient _httpClient;

        public ProductService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Product>?> GetProducts()
        {
            // Faz a chamada para GET /gateway/products/0 (o ID é ignorado no backend para listar todos)
            // Uma API mais robusta teria um endpoint /gateway/products
            return await _httpClient.GetFromJsonAsync<List<Product>>("/gateway/products");
        }

        public async Task<Product?> CreateProduct(Product product)
        {
            var response = await _httpClient.PostAsJsonAsync("/gateway/products", product);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Product>();
            }
            return null;
        }
    }
}