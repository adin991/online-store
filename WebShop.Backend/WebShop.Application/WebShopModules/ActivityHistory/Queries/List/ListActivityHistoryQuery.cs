using WebShop.Domain.Entities.WebShopEntities;

namespace WebShop.Application.WebShopModules.ActivityHistory.Queries.List;

public class ListActivityHistoryQuery : BasePagedQuery<ListActivityHistoryQueryDto>
{
    public ActivityType? ActivityFilter { get; set; }
    public int? UserId { get; set; }
}