using WebShop.Application.WebShopModules.Ad;

namespace WebShop.Application.WebShopModules.User.Queries.GetPublicProfile;

/// <summary>Minimal public seller profile - safe to expose without login (no email or other private fields).</summary>
public sealed class GetPublicUserProfileQuery : IRequest<GetPublicUserProfileQueryDto>
{
    public int Id { get; set; }
}

public sealed class GetPublicUserProfileQueryDto
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public required string Surname { get; init; }
    public string? Pfp { get; init; }
    public required int ActiveAdsCount { get; init; }
}

public sealed class GetPublicUserProfileQueryHandler(IAppDbContext context)
    : IRequestHandler<GetPublicUserProfileQuery, GetPublicUserProfileQueryDto>
{
    public async Task<GetPublicUserProfileQueryDto> Handle(GetPublicUserProfileQuery request, CancellationToken ct)
    {
        var user = await context.Users
            .AsNoTracking()
            .Where(u => u.Id == request.Id && u.IsEnabled)
            .Select(u => new { u.Id, u.Name, u.Surname, u.Pfp })
            .FirstOrDefaultAsync(ct)
            ?? throw new WebShopNotFoundException("The user with this ID doesn't exist");

        var activeAds = await context.Ads
            .Where(AdRules.IsAvailable)
            .CountAsync(a => a.UserId == user.Id, ct);

        return new GetPublicUserProfileQueryDto
        {
            Id = user.Id,
            Name = user.Name,
            Surname = user.Surname,
            Pfp = user.Pfp,
            ActiveAdsCount = activeAds
        };
    }
}
