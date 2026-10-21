namespace WebShop.Application.WebShopModules.Role.Queries.List;

public class ListRoleQueryHandler(IAppDbContext context)
    : IRequestHandler<ListRoleQuery, PageResult<ListRoleQueryDto>>
{
    public async Task<PageResult<ListRoleQueryDto>> Handle(ListRoleQuery request, CancellationToken ct)
    {
        var q = context.Role.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.SearchName))
            q = q.Where(r => r.Name.ToLower().Contains(request.SearchName.ToLower()));

        var projectedQuery = q.OrderBy(r => r.Name)
            .Select(r => new ListRoleQueryDto
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description
            });

        return await PageResult<ListRoleQueryDto>.FromQueryableAsync(projectedQuery, request.Paging, ct);
    }
}