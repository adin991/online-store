namespace WebShop.Application.WebShopModules.Payment.Commands.CreateCartPaymentIntent;

public class CartItemDto
{
    public required int AdId { get; set; }
    public required int Quantity { get; set; }
}

public class CreateCartPaymentIntentCommand : IRequest<CreateCartPaymentIntentResultDto>
{
    public required List<CartItemDto> Items { get; set; }
}
