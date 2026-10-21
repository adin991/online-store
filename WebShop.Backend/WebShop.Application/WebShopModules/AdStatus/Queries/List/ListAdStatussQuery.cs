using WebShop.Application.WebShopModules.AdStatus.Queries.List;

namespace WebShop.Application.WebShopModules.AdStatuss.Queries.List;

public sealed class ListAdStautssQuery : BasePagedQuery<ListAdStatussQueryDto>
{
    public string? Search { get; init; } // searches the title of the Ad 
    public bool? IsDeleted { get; init; } // filters by whether the ad status is deleted
}
