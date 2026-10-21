namespace WebShop.Application.Modules.Catalog.Product.Queries.GetById;

public class GetProductByIdQueryDto
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public required bool IsEnabled { get; init; }
    public required string? Description { get; set; }
    public required decimal Price { get; set; }
    public required int StockQuantity { get; set; }
    public required string CategoryName { get; set; }
}
