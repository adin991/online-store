namespace WebShop.Application.WebShopModules.CreditCard.Commands.Update;

public class UpdateCreditCardCommand : IRequest<Unit>
{
    [JsonIgnore]
    public int Id { get; set; }
    public required string Last4 { get; set; }
    public required string CardType { get; set; }
    public required int ExpMonth { get; set; }
    public required int ExpYear { get; set; }
}