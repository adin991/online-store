namespace WebShop.Application.Modules.Catalog.Product.Commands.Create;

public class CreateProductCommand : IRequest<int>
{
    public  string Name { get; set; }
    public  string Description { get; set; }
    public  decimal Price { get; set; }
    public  int StockQuantity { get; set; }
    public  int CategoryId { get; set; }
}