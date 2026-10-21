using WebShop.Domain.Entities.WebShopEntities;

namespace WebShop.Application.WebShopModules.Ad.Queries.List;

public sealed class ListAdsQueryDto
{
    public required int Id { get; init; }
    public required string Title { get; init; }
    public required decimal Price { get; init; }
    public required string Description { get; init; }
    public required int UserId { get; init; }
    public required ListAdsQueryUserDto UserDto { get; init; }
    public required ListAdsQueryCategoryDto? Category { get; init; }
    public List<string>? Images { get; init; }
    public string? Location { get; init; }
    // Primary (non-Boosted) status; null if the ad has no status yet.
    public AdStatusType? Status { get; init; }
    public required bool IsBoosted { get; init; }
    public required DateTime CreatedAtUtc { get; init; }
}

// Public seller info only - no private fields such as email.
public sealed class ListAdsQueryUserDto
{
    public required string Name { get; init; }
    public required string Surname { get; init; }
}

public sealed class ListAdsQueryCategoryDto
{
    public required int Id { get; init; }
    public required string Name { get; init; }
}
