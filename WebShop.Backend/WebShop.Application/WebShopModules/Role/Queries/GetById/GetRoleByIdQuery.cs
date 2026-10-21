namespace WebShop.Application.WebShopModules.Role.Queries.GetById;

public class GetRoleByIdQuery : IRequest<GetRoleByIdQueryDto>
{
    public required int Id { get; set; }
}