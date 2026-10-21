namespace WebShop.Application.WebShopModules.CreditCard.Queries.GetById;

public class GetCreditCardByIdQuery : IRequest<GetCreditCardByIdQueryDto>
{
    public required int Id { get; set; }
}