namespace WebShop.Application.WebShopModules.Payment.Commands.ConfirmPayment;

public class ConfirmPaymentResultDto
{
    public required bool Success { get; set; }
    public required string Status { get; set; }
}
