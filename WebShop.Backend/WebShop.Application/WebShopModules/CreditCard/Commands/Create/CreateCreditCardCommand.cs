namespace WebShop.Application.WebShopModules.CreditCard.Commands.Create;

public class CreateCreditCardCommand : IRequest<int>
{
    public required string Last4 { get; set; }
    public required string CardType { get; set; }
    public required int ExpMonth { get; set; }
    public required int ExpYear { get; set; }
}