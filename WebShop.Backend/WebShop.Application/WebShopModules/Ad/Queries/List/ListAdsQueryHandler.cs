using WebShop.Domain.Entities.WebShopEntities;

namespace WebShop.Application.WebShopModules.Ad.Queries.List;

public sealed class ListAdsQueryHandler(IAppDbContext context, IAppCurrentUser currentUser)
    : IRequestHandler<ListAdsQuery, PageResult<ListAdsQueryDto>>
{
    public async Task<PageResult<ListAdsQueryDto>> Handle(ListAdsQuery request, CancellationToken ct)
    {
        var q = context.Ads.AsNoTracking();

        if (request.Mine == true)
        {
            if (!currentUser.IsAuthenticated || currentUser.UserId is null)
                throw new WebShopBusinessRuleException("401", "User must be logged in.");

            var userId = currentUser.UserId.Value;
            q = q.Where(x => x.UserId == userId);
        }
        else
        {
            // Public listing: only available ads (see AdRules).
            q = q.Where(AdRules.IsAvailable);
        }

        if (request.Status.HasValue)
            q = q.Where(x => x.AdStatuses.Any(s => s.Status == request.Status.Value));

        if (!string.IsNullOrWhiteSpace(request.Search))
            q = q.Where(x => x.Title.Contains(request.Search) || x.Description.Contains(request.Search));

        if (request.IsDeleted is not null)
            q = q.Where(x => x.IsDeleted == request.IsDeleted);

        if (!string.IsNullOrWhiteSpace(request.CategoryName))
            q = q.Where(x => x.CategoryEntity.Name == request.CategoryName);

        if (request.MinPrice.HasValue)
            q = q.Where(x => x.Price >= request.MinPrice.Value);

        if (request.MaxPrice.HasValue)
            q = q.Where(x => x.Price <= request.MaxPrice.Value);

        if (!string.IsNullOrWhiteSpace(request.Location))
            q = q.Where(x => x.Location == request.Location);

        var orderedQuery = request.SortBy switch
        {
            "higher" => q.OrderByDescending(x => x.Price),
            "lower"  => q.OrderBy(x => x.Price),
            "newest" => q.OrderByDescending(x => x.CreatedAtUtc),
            _        => q.OrderBy(x => x.Title)
        };

        var result = orderedQuery.Select(x => new ListAdsQueryDto
        {
            Id = x.Id,
            Title = x.Title,
            Description = x.Description,
            UserId = x.UserId,
            Price = x.Price,
            Location = x.Location,
            Category = new ListAdsQueryCategoryDto
            {
                Id = x.CategoryEntity.Id,
                Name = x.CategoryEntity.Name
            },
            Images = x.Images,
            Status = x.AdStatuses
                .Where(s => s.Status != AdStatusType.Boosted)
                .OrderByDescending(s => s.Id)
                .Select(s => (AdStatusType?)s.Status)
                .FirstOrDefault(),
            IsBoosted = x.AdStatuses.Any(s => s.Status == AdStatusType.Boosted),
            CreatedAtUtc = x.CreatedAtUtc,
            UserDto = new ListAdsQueryUserDto
            {
                Name = x.User.Name,
                Surname = x.User.Surname
            }
        });

        return await PageResult<ListAdsQueryDto>.FromQueryableAsync(result, request.Paging, ct);
    }
}
