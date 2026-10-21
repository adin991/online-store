namespace WebShop.Application.WebShopModules.Ad.Commands.Create;

public sealed class CreateAdCommandValidator : AbstractValidator<CreateAdCommand>
{
    public CreateAdCommandValidator()
    {
        RuleFor(x => x.Title).AdTitle();
        RuleFor(x => x.Description).AdDescription();
        RuleFor(x => x.Price).AdPrice();
        RuleFor(x => x.CategoryId).AdCategoryId();
        RuleFor(x => x.Location).AdLocation();
        RuleFor(x => x.Images).AdImages();
        RuleFor(x => x.Colors).AdOptionList("Colors");
        RuleFor(x => x.Sizes).AdOptionList("Sizes");
    }
}
