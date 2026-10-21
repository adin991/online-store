using WebShop.Domain.Entities.WebShopEntities;

namespace WebShop.Application.WebShopModules.AdStatus.Commands.Update;

public sealed class UpdateAdStatusCommand : IRequest<Unit>
{
    [JsonIgnore]
    public int Id { get; set; }
    public AdStatusType Status { get; set; }
}
