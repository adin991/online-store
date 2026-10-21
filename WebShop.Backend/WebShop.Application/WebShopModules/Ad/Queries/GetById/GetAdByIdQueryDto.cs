using WebShop.Domain.Entities.WebShopEntities;

namespace WebShop.Application.WebShopModules.Ad.Queries.GetById;

public class GetAdByIdQueryDto
{
    public required int Id { get; init; }
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required decimal Price { get; init; }
    public required int UserId { get; init; }
    public required GetAdByIdQueryUserDto UserDto { get; init; }
    public GetAdByIdQueryCategoryDto? Category { get; init; }
    public required List<string> Images { get; init; } = new();
    public string? Location { get; init; }
    public required List<string> Colors { get; init; } = new();
    public required List<string> Sizes { get; init; } = new();

    // Primary (non-Boosted) status; null if the ad has no status yet.
    public AdStatusType? Status { get; init; }
    public required bool IsBoosted { get; init; }

    /// <summary>True if the ad can currently be bought (see AdRules).</summary>
    public required bool IsAvailable { get; init; }
    public required DateTime CreatedAtUtc { get; init; }
}

// Public seller info only - no private fields such as email.
public sealed class GetAdByIdQueryUserDto
{
    public required string Name { get; init; }
    public required string Surname { get; init; }
}

public sealed class GetAdByIdQueryCategoryDto
{
    public required int Id { get; init; }
    public required string Name { get; init; }
}
