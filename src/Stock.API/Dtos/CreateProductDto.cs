namespace Stock.API.Dtos
{
    public record CreateProductDto(string Name, string Description, decimal Price, int QuantityInStock);
}
