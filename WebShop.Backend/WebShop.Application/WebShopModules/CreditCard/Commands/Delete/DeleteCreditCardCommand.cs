namespace WebShop.Application.WebShopModules.CreditCard.Commands.Delete;

public class DeleteCreditCardCommand : IRequest<Unit>
{
    public required int Id { get; set; }
}