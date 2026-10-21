
using WebShop.Domain.Entities.WebShopEntities;

namespace WebShop.Application.WebShopModules.AdStatus.Queries.GetById;
public class GetAdStatusByIdQuery : IRequest<GetAdStatusByIdQueryDto>
{
    public int Id { get; set; }
}