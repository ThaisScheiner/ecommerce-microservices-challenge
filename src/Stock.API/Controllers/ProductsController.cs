using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stock.API.Data;
using Stock.API.Dtos;
using Stock.API.Entities;

namespace Stock.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProductsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
    {
        var products = await _context.Products
            .Select(p => new ProductDto(p.Id, p.Name, p.Description, p.Price, p.QuantityInStock))
            .ToListAsync();
        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetProduct(int id)
    {
        var product = await _context.Products.FindAsync(id);

        if (product == null)
        {
            return NotFound();
        }

        return Ok(new ProductDto(product.Id, product.Name, product.Description, product.Price, product.QuantityInStock));
    }

    [HttpPost]
    public async Task<ActionResult<ProductDto>> CreateProduct(CreateProductDto createProductDto)
    {
        var product = new Product
        {
            Name = createProductDto.Name,
            Description = createProductDto.Description,
            Price = createProductDto.Price,
            QuantityInStock = createProductDto.QuantityInStock
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        var productDto = new ProductDto(product.Id, product.Name, product.Description, product.Price, product.QuantityInStock);

        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, productDto);
    }
}