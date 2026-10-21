namespace WebShop.Application.WebShopModules.Payment.Commands.CreateCartPaymentIntent;

public sealed class CreateCartPaymentIntentCommandValidator : AbstractValidator<CreateCartPaymentIntentCommand>
{
    public const int MaxItems = 50;
    public const int MaxQuantity = 999;

    public CreateCartPaymentIntentCommandValidator()
    {
        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("Cart is empty.")
            .Must(items => items.Count <= MaxItems).WithMessage($"A cart can contain at most {MaxItems} items.");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.AdId).GreaterThan(0).WithMessage("Ad id must be greater than 0.");
            item.RuleFor(i => i.Quantity)
                .InclusiveBetween(1, MaxQuantity).WithMessage($"Quantity must be between 1 and {MaxQuantity}.");
        });
    }
}
