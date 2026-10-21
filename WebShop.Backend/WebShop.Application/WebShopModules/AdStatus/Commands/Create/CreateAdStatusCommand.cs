using WebShop.Domain.Entities.WebShopEntities;

namespace WebShop.Application.WebShopModules.AdStatus.Commands.Create;

public class CreateAdStatusCommand : IRequest<int>
{
    public required int AdId { get; set; }
    public required AdStatusType Status { get; set; }
}
