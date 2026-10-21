namespace WebShop.Application.WebShopModules.User.Commands.Update;

public sealed class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("User id must be greater than 0.");
        RuleFor(x => x.Name).PersonName("Name");
        RuleFor(x => x.Surname).PersonName("Surname");
        RuleFor(x => x.Email).UserEmail();
        RuleFor(x => x.Gender).IsInEnum().WithMessage("Unknown gender.");
    }
}
