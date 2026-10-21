namespace WebShop.Application.WebShopModules.Role.Commands.Update;

public class UpdateRoleCommand : IRequest<Unit>
{
    [JsonIgnore]
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
}