namespace WebShop.Application.WebShopModules.Payment.Commands.CreatePaymentIntent;

public sealed class CreatePaymentIntentCommandValidator : AbstractValidator<CreatePaymentIntentCommand>
{
    public CreatePaymentIntentCommandValidator()
    {
        RuleFor(x => x.AdId).GreaterThan(0).WithMessage("Ad id must be greater than 0.");
    }
}
