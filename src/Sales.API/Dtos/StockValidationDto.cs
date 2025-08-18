namespace Sales.API.Dtos
{
    public record StockValidationDto(int Id, string Name, decimal Price, int QuantityInStock);
}