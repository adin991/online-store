using WebShop.Domain.Entities.WebShopEntities;

namespace WebShop.Application.WebShopModules.ActivityHistory.Commands.Update;

public class UpdateActivityHistoryCommand : IRequest<Unit>
{
    [JsonIgnore]
    public int Id { get; set; }
    public required ActivityType Activity { get; set; }
}