using WebShop.Domain.Entities.WebShopEntities;

namespace WebShop.Application.WebShopModules.AdStatus.Queries.List;

public sealed class ListAdStatussQueryDto
{
    public required int Id { get; init; }
    public required AdStatusType AdStatus { get; init; }
    public required string StatusName { get; set; } = string.Empty;
    public required int AdId { get; set; }
    public required ListAdStatusQueryAdDto AdDto { get; init; }
    
}
//ad list specific user 
public sealed class ListAdStatusQueryAdDto
{
    public required string Title { get; init; }

}