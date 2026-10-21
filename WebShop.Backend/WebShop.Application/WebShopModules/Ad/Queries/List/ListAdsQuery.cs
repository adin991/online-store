using WebShop.Domain.Entities.WebShopEntities;

namespace WebShop.Application.WebShopModules.Ad.Queries.List;

public sealed class ListAdsQuery : BasePagedQuery<ListAdsQueryDto>
{
    public string? Search { get; init; }
    public bool? IsDeleted { get; init; }
    public string? CategoryName { get; init; }
    public decimal? MinPrice { get; init; }
    public decimal? MaxPrice { get; init; }
    public string? Location { get; init; }
    public string? SortBy { get; init; }

    /// <summary>
    /// true = only the current user's own ads, in every status (used by the dashboard).
    /// Otherwise only publicly available (Live) ads are returned.
    /// </summary>
    public bool? Mine { get; init; }

    /// <summary>Filter by primary status (useful together with Mine).</summary>
    public AdStatusType? Status { get; init; }
}
