namespace WebShop.Application.WebShopModules.ActivityHistory.Queries.GetById;

public class GetActivityHistoryByIdQuery : IRequest<GetActivityHistoryByIdQueryDto>
{
    public required int Id { get; set; }
}