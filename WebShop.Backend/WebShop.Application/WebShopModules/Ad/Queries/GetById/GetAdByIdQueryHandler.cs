using WebShop.Domain.Entities.WebShopEntities;

namespace WebShop.Application.WebShopModules.Ad.Queries.GetById;

public class GetAdByIdQueryHandler(IAppDbContext context, IAppCurrentUser currentUser)
    : IRequestHandler<GetAdByIdQuery, GetAdByIdQueryDto>
{
    public async Task<GetAdByIdQueryDto> Handle(GetAdByIdQuery request, CancellationToken cancellationToken)
    {
        var ad = await context.Ads
            .AsNoTracking()
            .Where(c => c.Id == request.Id)
            .Select(x => new GetAdByIdQueryDto
            {
                Id = x.Id,
                Title = x.Title,
                Description = x.Description,
                Price = x.Price,
                UserId = x.UserId,
                Images = x.Images ?? new List<string>(),
                Location = x.Location,
                Colors = x.Colors ?? new List<string>(),
                Sizes = x.Sizes ?? new List<string>(),
                Status = x.AdStatuses
                    .Where(s => s.Status != AdStatusType.Boosted)
                    .OrderByDescending(s => s.Id)
                    .Select(s => (AdStatusType?)s.Status)
                    .FirstOrDefault(),
                IsBoosted = x.AdStatuses.Any(s => s.Status == AdStatusType.Boosted),
                IsAvailable = x.AdStatuses.Any(s => s.Status == AdStatusType.Live),
                CreatedAtUtc = x.CreatedAtUtc,
                Category = x.CategoryEntity == null ? null : new GetAdByIdQueryCategoryDto
                {
                    Id = x.CategoryEntity.Id,
                    Name = x.CategoryEntity.Name
                },
                UserDto = new GetAdByIdQueryUserDto
                {
                    Name = x.User.Name,
                    Surname = x.User.Surname
                }
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (ad == null)
            throw new WebShopNotFoundException($"Ad with Id {request.Id} not found.");

        // Unavailable ads (closed/sold/removed/...) are visible only to their owner and admins.
        var isOwnerOrAdmin = currentUser.IsAdmin
            || (currentUser.UserId.HasValue && currentUser.UserId.Value == ad.UserId);

        if (!ad.IsAvailable && !isOwnerOrAdmin)
            throw new WebShopNotFoundException($"Ad with Id {request.Id} not found.");

        return ad;
    }
}
