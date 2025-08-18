using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sales.API.Data;
using Sales.API.Dtos;
using Sales.API.Entities;
using Sales.API.Services;

namespace Sales.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMessageBusClient _messageBus;
        private readonly HttpClient _httpClient;

        public OrdersController(AppDbContext context, IMessageBusClient messageBus, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _messageBus = messageBus;
            _httpClient = httpClientFactory.CreateClient("StockService");
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(CreateOrderDto createOrderDto)
        {
            // Pega o token da requisição que chegou no Sales.API
            var authorizationHeader = Request.Headers.Authorization.FirstOrDefault();
            if (authorizationHeader == null || !authorizationHeader.StartsWith("Bearer "))
            {
                return Unauthorized("Token não fornecido ou mal formatado.");
            }
            var token = authorizationHeader.Substring("Bearer ".Length).Trim();

            // Adiciona o token na requisição que será feita para o Stock.API
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            // Chama o serviço de estoque para validar (agora com o token)
            StockValidationDto? product;
            try
            {
                product = await _httpClient.GetFromJsonAsync<StockValidationDto>($"api/products/{createOrderDto.ProductId}");
            }
            catch (HttpRequestException ex)
            {
                // Adicionando mais detalhes no log para depuração
                Console.WriteLine($"--> Erro ao chamar Stock.API: {ex.Message}");
                return BadRequest("Serviço de estoque indisponível.");
            }

            if (product == null)
            {
                return BadRequest("Produto não encontrado no estoque.");
            }

            if (product.QuantityInStock < createOrderDto.Quantity)
            {
                return BadRequest("Produto sem estoque suficiente.");
            }

            var order = new Order
            {
                ProductId = createOrderDto.ProductId,
                Quantity = createOrderDto.Quantity,
                OrderDate = DateTime.UtcNow,
                TotalPrice = product.Price * createOrderDto.Quantity
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            var orderMessage = new OrderMessage(order.ProductId, order.Quantity);
            _messageBus.PublishOrder(orderMessage);

            return Ok(order);
        }
    }
}