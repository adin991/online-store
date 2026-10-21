namespace WebShop.Application.WebShopModules.Role.Commands.Create;

public class CreateRoleCommand : IRequest<int>
{
    public string Name { get; set; }
    public required string Description { get; set; }
}