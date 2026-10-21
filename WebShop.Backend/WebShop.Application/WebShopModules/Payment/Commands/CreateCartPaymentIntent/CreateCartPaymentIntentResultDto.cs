namespace WebShop.Application.WebShopModules.Payment.Commands.CreateCartPaymentIntent;

public class CreateCartPaymentIntentResultDto
{
    public required List<int> TransactionIds { get; set; }
    public required string ClientSecret { get; set; }
    public required string PublishableKey { get; set; }
    public required decimal AmountEur { get; set; }
}
