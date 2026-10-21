using WebShop.Domain.Entities.WebShopEntities;

namespace WebShop.Application.WebShopModules.ActivityHistory.Queries.GetById;

public class GetActivityHistoryByIdQueryDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public ActivityType Activity { get; set; }
    public GetActivityHistoryByIdUserDto User { get; set; } = null!;
}

public class GetActivityHistoryByIdUserDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Surname { get; set; } = null!;
}