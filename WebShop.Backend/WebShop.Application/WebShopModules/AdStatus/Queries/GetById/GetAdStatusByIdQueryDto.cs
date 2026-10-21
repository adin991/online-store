
using WebShop.Domain.Entities.WebShopEntities;

namespace WebShop.Application.WebShopModules.AdStatus.Queries.GetById;

public class GetAdStatusByIdQueryDto
{
    public required int Id { get; init; }
    public required AdStatusType AdStatus { get; init; }
    public required string StatusName { get; set; } = string.Empty;
    public required int AdId { get; set; }
    public required GetAdStatusByIdAdDto AdDto { get; init; }
    
}

//ad id specific user 
public sealed class GetAdStatusByIdAdDto
{
    public required string Title { get; init; }

}
