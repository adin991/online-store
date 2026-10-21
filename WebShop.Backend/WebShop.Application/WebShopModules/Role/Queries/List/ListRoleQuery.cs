namespace WebShop.Application.WebShopModules.Role.Queries.List;

public class ListRoleQuery : BasePagedQuery<ListRoleQueryDto>
{
    public string? SearchName { get; set; }
}