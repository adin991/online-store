namespace WebShop.Application.WebShopModules.CreditCard.Queries.List;

public class ListCreditCardQuery : IRequest<PageResult<ListCreditCardQueryDto>>
{
    public string? CardTypeFilter { get; set; }
    public int? UserId { get; set; }
    public PageRequest Paging { get; set; } = new();
}