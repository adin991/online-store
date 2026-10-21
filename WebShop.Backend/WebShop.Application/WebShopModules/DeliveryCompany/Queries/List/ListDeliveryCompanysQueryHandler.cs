namespace WebShop.Application.WebShopModules.DeliveryCompany.Queries.List;

public sealed class ListProductsQueryHandler(IAppDbContext ctx)
        : IRequestHandler<ListDeliveryCompanysQuery, PageResult<ListDeliveryCompanysQueryDto>>
{
    public async Task<PageResult<ListDeliveryCompanysQueryDto>> Handle(
        ListDeliveryCompanysQuery request, CancellationToken ct)
    {
        var q = ctx.DeliveryCompanys.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
             q = q.Where(x => x.Name.ToLower().Contains(request.Search.ToLower()));
        }

        var projectedQuery = q.OrderBy(x => x.Name)
            .Select(x => new ListDeliveryCompanysQueryDto
            {
                Id = x.Id,
                Name = x.Name,
            });

        return await PageResult<ListDeliveryCompanysQueryDto>.FromQueryableAsync(projectedQuery, request.Paging, ct);
    }
}
