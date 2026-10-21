namespace WebShop.Application.WebShopModules.AdStatus.Commands.Create;

public sealed class CreateAdStatusCommandValidator : AbstractValidator<CreateAdStatusCommand>
{
    public CreateAdStatusCommandValidator()
    {
        RuleFor(x => x.AdId).GreaterThan(0).WithMessage("Ad id must be greater than 0.");
        RuleFor(x => x.Status).IsInEnum().WithMessage("Unknown ad status.");
    }
}
