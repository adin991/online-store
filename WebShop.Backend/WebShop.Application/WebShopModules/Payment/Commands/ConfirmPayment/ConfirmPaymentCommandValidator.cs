namespace WebShop.Application.WebShopModules.Payment.Commands.ConfirmPayment;

public sealed class ConfirmPaymentCommandValidator : AbstractValidator<ConfirmPaymentCommand>
{
    public ConfirmPaymentCommandValidator()
    {
        RuleFor(x => x.TransactionIds)
            .NotEmpty().WithMessage("No transactions to confirm.");

        RuleForEach(x => x.TransactionIds)
            .GreaterThan(0).WithMessage("Transaction id must be greater than 0.");
    }
}
