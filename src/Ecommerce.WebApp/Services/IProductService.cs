using Ecommerce.WebApp.Models;

namespace Ecommerce.WebApp.Services
{
    public interface IProductService
    {
        Task<List<Product>?> GetProducts();
        Task<Product?> CreateProduct(Product product);
    }
}