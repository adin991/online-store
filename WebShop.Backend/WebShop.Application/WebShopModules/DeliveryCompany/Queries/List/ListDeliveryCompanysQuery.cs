namespace WebShop.Application.WebShopModules.DeliveryCompany.Queries.List;
public sealed class ListDeliveryCompanysQuery : BasePagedQuery<ListDeliveryCompanysQueryDto>
{
    public string? Search { get; init; }
}
