namespace WebShop.Application.WebShopModules.Payment.Commands.ConfirmPayment;

public class ConfirmPaymentCommand : IRequest<ConfirmPaymentResultDto>
{
    public required List<int> TransactionIds { get; set; }
}
