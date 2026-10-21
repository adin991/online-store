namespace WebShop.Application.WebShopModules.Payment.Commands.CreatePaymentIntent;

public class CreatePaymentIntentCommand : IRequest<CreatePaymentIntentResultDto>
{
    public required int AdId { get; set; }
}
