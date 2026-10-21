using WebShop.Application.WebShopModules.AdStatus.Queries.List;

namespace WebShop.Application.WebShopModules.AdStatuss.Queries.List;
public sealed class ListAdsQueryHandler(IAppDbContext context): IRequestHandler<ListAdStautssQuery, PageResult<ListAdStatussQueryDto>>
{
    public async Task<PageResult<ListAdStatussQueryDto>> Handle(ListAdStautssQuery request, CancellationToken ct)
    {
        var q = context.AdStatuss.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            q = q.Where(x => x.Ad.Title.Contains(request.Search) || x.Ad.Title.Contains(request.Search));
        }

        if (request.IsDeleted is not null)
            q = q.Where(x => x.IsDeleted == request.IsDeleted);

        var projectedQuery = q.OrderBy(x => x.Ad.Title)
            .Select(x => new ListAdStatussQueryDto
            {
                Id = x.Id,
                AdStatus=x.Status,
                StatusName = x.Status.ToString(),
                AdId =x.Ad.Id,
                
                AdDto = new ListAdStatusQueryAdDto
                {
                   Title=x.Ad.Title,
                }
            });

        return await PageResult<ListAdStatussQueryDto>.FromQueryableAsync(projectedQuery, request.Paging, ct);
    }
}