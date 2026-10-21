namespace WebShop.Application.WebShopModules.Ad.Commands.Delete;

public class DeleteAdCommand : IRequest<Unit>
{
    public required int Id { get; set; }
}
