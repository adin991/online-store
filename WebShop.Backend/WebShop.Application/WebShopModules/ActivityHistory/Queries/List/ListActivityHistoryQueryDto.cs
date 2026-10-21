using WebShop.Domain.Entities.WebShopEntities;

namespace WebShop.Application.WebShopModules.ActivityHistory.Queries.List;

public class ListActivityHistoryQueryDto
{
    public int Id { get; init; }
    public int UserId { get; init; }
    public ActivityType Activity { get; init; }
    public ListActivityHistoryQueryUserDto User { get; init; } = null!;
}

public class ListActivityHistoryQueryUserDto
{
    public int Id { get; init; }
    public string Name { get; init; } = null!;
    public string Surname { get; init; } = null!;
}