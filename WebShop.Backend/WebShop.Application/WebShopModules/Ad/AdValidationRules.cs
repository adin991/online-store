using AdConstraints = WebShop.Domain.Entities.WebShopEntities.Ad.Constraints;

namespace WebShop.Application.WebShopModules.Ad;

/// <summary>Validation rules shared by CreateAdCommand and UpdateAdCommand.</summary>
internal static class AdValidationRules
{
    public static IRuleBuilderOptions<T, string> AdTitle<T>(this IRuleBuilder<T, string> rule)
        => rule
            .Must(v => !string.IsNullOrWhiteSpace(v)).WithMessage("Title is required.")
            .MaximumLength(AdConstraints.TitleMaxLength)
                .WithMessage($"Title can be at most {AdConstraints.TitleMaxLength} characters long.");

    public static IRuleBuilderOptions<T, string> AdDescription<T>(this IRuleBuilder<T, string> rule)
        => rule
            .Must(v => !string.IsNullOrWhiteSpace(v)).WithMessage("Description is required.")
            .MaximumLength(AdConstraints.DescriptionMaxLength)
                .WithMessage($"Description can be at most {AdConstraints.DescriptionMaxLength} characters long.");

    public static IRuleBuilderOptions<T, decimal> AdPrice<T>(this IRuleBuilder<T, decimal> rule)
        => rule
            .GreaterThan(0).WithMessage("Price must be greater than 0.")
            .LessThanOrEqualTo(AdConstraints.PriceMax)
                .WithMessage($"Price can be at most {AdConstraints.PriceMax}.")
            .PrecisionScale(18, 2, ignoreTrailingZeros: true)
                .WithMessage("Price can have at most 2 decimal places.");

    public static IRuleBuilderOptions<T, int> AdCategoryId<T>(this IRuleBuilder<T, int> rule)
        => rule.GreaterThan(0).WithMessage("Category is required.");

    public static IRuleBuilderOptions<T, string?> AdLocation<T>(this IRuleBuilder<T, string?> rule)
        => rule
            .MaximumLength(AdConstraints.LocationMaxLength)
                .WithMessage($"Location can be at most {AdConstraints.LocationMaxLength} characters long.");

    public static IRuleBuilderOptions<T, List<string>?> AdImages<T>(this IRuleBuilder<T, List<string>?> rule)
        => rule
            .Must(v => v is null || v.Count <= AdConstraints.MaxImages)
                .WithMessage($"An ad can have at most {AdConstraints.MaxImages} images.")
            .Must(v => v is null || v.All(i => !string.IsNullOrWhiteSpace(i) && i.Length <= AdConstraints.ImageUrlMaxLength))
                .WithMessage($"Image URLs must be non-empty and at most {AdConstraints.ImageUrlMaxLength} characters long.");

    public static IRuleBuilderOptions<T, List<string>?> AdOptionList<T>(this IRuleBuilder<T, List<string>?> rule, string name)
        => rule
            .Must(v => v is null || v.Count <= AdConstraints.MaxOptions)
                .WithMessage($"{name}: at most {AdConstraints.MaxOptions} options are allowed.")
            .Must(v => v is null || v.All(o => !string.IsNullOrWhiteSpace(o) && o.Trim().Length <= AdConstraints.OptionMaxLength))
                .WithMessage($"{name}: each option must be non-empty and at most {AdConstraints.OptionMaxLength} characters long.");
}
