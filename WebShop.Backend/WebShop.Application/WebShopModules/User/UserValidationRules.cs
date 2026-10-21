namespace WebShop.Application.WebShopModules.User;

/// <summary>Validation rules shared by CreateUserCommand and UpdateUserCommand.</summary>
internal static class UserValidationRules
{
    public const int NameMaxLength = 50;
    public const int EmailMaxLength = 200; // matches the Users.Email column
    public const int PasswordMinLength = 4;
    public const int PasswordMaxLength = 100;

    public static IRuleBuilderOptions<T, string> PersonName<T>(this IRuleBuilder<T, string> rule, string field)
        => rule
            .Must(v => !string.IsNullOrWhiteSpace(v)).WithMessage($"{field} is required.")
            .MaximumLength(NameMaxLength).WithMessage($"{field} can be at most {NameMaxLength} characters long.");

    public static IRuleBuilderOptions<T, string> UserEmail<T>(this IRuleBuilder<T, string> rule)
        => rule
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email is not valid.")
            .MaximumLength(EmailMaxLength).WithMessage($"Email can be at most {EmailMaxLength} characters long.");
}
