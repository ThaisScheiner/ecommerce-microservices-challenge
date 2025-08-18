namespace Stock.API.Dtos
{
    public record ProductDto(int Id, string Name, string Description, decimal Price, int QuantityInStock);
}
