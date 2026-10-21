using WebShop.Domain.Entities.WebShopEntities;

namespace WebShop.Application.WebShopModules.Ad.Commands.Status;

public sealed class UpdateStatusThruAdCommand : IRequest<Unit>
{
    public required int Id { get; set; }
    public required AdStatusType AdStatusType { get; set; }
}
