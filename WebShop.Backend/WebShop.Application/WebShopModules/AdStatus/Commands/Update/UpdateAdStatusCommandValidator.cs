namespace WebShop.Application.WebShopModules.AdStatus.Commands.Update;

public sealed class UpdateAdStatusCommandValidator : AbstractValidator<UpdateAdStatusCommand>
{
    public UpdateAdStatusCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("Ad status id must be greater than 0.");
        RuleFor(x => x.Status).IsInEnum().WithMessage("Unknown ad status.");
    }
}
