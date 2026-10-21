namespace WebShop.Application.WebShopModules.User.Commands.Create;

public sealed class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Name).PersonName("Name");
        RuleFor(x => x.Surname).PersonName("Surname");
        RuleFor(x => x.Email).UserEmail();

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(UserValidationRules.PasswordMinLength)
                .WithMessage($"Password must be at least {UserValidationRules.PasswordMinLength} characters long.")
            .MaximumLength(UserValidationRules.PasswordMaxLength)
                .WithMessage($"Password can be at most {UserValidationRules.PasswordMaxLength} characters long.");

        RuleFor(x => x.Gender).IsInEnum().WithMessage("Unknown gender.");
    }
}
