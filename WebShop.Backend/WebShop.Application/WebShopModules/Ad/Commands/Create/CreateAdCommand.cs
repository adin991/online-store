namespace WebShop.Application.WebShopModules.Ad.Commands.Create;

public class CreateAdCommand : IRequest<int>
{
    public required string Title { get; set; }
    public required decimal Price { get; set; }
    public required string Description { get; set; }
    public List<string>? Images { get; set; }
    public required int CategoryId { get; set; }
    public string? Location { get; set; }
    public List<string>? Colors { get; set; }
    public List<string>? Sizes { get; set; }
}
