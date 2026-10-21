namespace WebShop.Application.WebShopModules.ActivityHistory.Queries.List;

public class ListActivityHistoryQueryHandler(IAppDbContext context, IAppCurrentUser currentUser)
    : IRequestHandler<ListActivityHistoryQuery, PageResult<ListActivityHistoryQueryDto>>
{
    public async Task<PageResult<ListActivityHistoryQueryDto>> Handle(ListActivityHistoryQuery request, CancellationToken ct)
    {
        var q = context.ActivityHistorys.AsNoTracking();

        // A user can only see their own activity unless they are an admin
        if (!currentUser.IsAdmin)
            q = q.Where(a => a.UserId == currentUser.UserId);

        if (request.ActivityFilter is not null)
            q = q.Where(a => a.Activity == request.ActivityFilter);

        if (request.UserId is not null)
            q = q.Where(a => a.UserId == request.UserId);

        var projectedQuery = q.OrderByDescending(a => a.CreatedAtUtc)
            .Select(a => new ListActivityHistoryQueryDto
            {
                Id = a.Id,
                UserId = a.UserId,
                Activity = a.Activity,
                User = new ListActivityHistoryQueryUserDto
                {
                    Id = a.User.Id,
                    Name = a.User.Name,
                    Surname = a.User.Surname
                }
            });

        return await PageResult<ListActivityHistoryQueryDto>.FromQueryableAsync(projectedQuery, request.Paging, ct);
    }
}