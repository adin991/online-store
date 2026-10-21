namespace WebShop.Application.WebShopModules.Ad.Queries.GetById;

public class GetAdByIdQuery : IRequest<GetAdByIdQueryDto>
{
    public int Id { get; set; }
}