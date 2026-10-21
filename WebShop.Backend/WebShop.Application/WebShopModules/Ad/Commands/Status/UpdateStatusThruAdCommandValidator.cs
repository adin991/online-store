namespace WebShop.Application.WebShopModules.Ad.Commands.Status;

public sealed class UpdateStatusThruAdCommandValidator : AbstractValidator<UpdateStatusThruAdCommand>
{
    public UpdateStatusThruAdCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Ad id must be greater than 0.");

        RuleFor(x => x.AdStatusType)
            .IsInEnum().WithMessage("Unknown ad status.");
    }
}
