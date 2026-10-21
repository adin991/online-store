namespace WebShop.Application.WebShopModules.Role.Commands.Delete;

public class DeleteRoleCommand : IRequest<Unit>
{
    public required int Id { get; set; }
}