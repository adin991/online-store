namespace WebShop.Application.WebShopModules.Ad.Commands.Update;

public sealed class UpdateAdCommandValidator : AbstractValidator<UpdateAdCommand>
{
    public UpdateAdCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("Ad id must be greater than 0.");
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
