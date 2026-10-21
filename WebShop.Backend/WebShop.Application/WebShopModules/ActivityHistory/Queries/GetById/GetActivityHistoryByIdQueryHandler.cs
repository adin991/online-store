namespace WebShop.Application.WebShopModules.ActivityHistory.Queries.GetById;

public class GetActivityHistoryByIdQueryHandler(IAppDbContext context)
    : IRequestHandler<GetActivityHistoryByIdQuery, GetActivityHistoryByIdQueryDto>
{
    public async Task<GetActivityHistoryByIdQueryDto> Handle(GetActivityHistoryByIdQuery request, CancellationToken ct)
    {
        var activityHistory = await context.ActivityHistorys
            .AsNoTracking()
            .Where(a => a.Id == request.Id)
            .Select(a => new GetActivityHistoryByIdQueryDto
            {
                Id = a.Id,
                UserId = a.UserId,
                Activity = a.Activity,
                User = new GetActivityHistoryByIdUserDto
                {
                    Id = a.User.Id,
                    Name = a.User.Name,
                    Surname = a.User.Surname
                }
            })
            .FirstOrDefaultAsync(ct);

        if (activityHistory is null)
            throw new WebShopNotFoundException("Activity history with this ID doesn't exist");

        return activityHistory;
    }
}