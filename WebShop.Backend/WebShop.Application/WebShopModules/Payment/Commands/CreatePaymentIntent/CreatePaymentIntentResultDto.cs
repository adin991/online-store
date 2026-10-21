namespace WebShop.Application.WebShopModules.Payment.Commands.CreatePaymentIntent;

public class CreatePaymentIntentResultDto
{
    public required int TransactionId { get; set; }
    public required string ClientSecret { get; set; }
    public required string PublishableKey { get; set; }
    public required decimal AmountEur { get; set; }
}
