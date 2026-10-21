namespace WebShop.Application.WebShopModules.Ad.Commands.Update;

public sealed class UpdateAdCommandHandler(IAppDbContext context, IAppCurrentUser currentUser)
            : IRequestHandler<UpdateAdCommand, Unit>
{
    public async Task<Unit> Handle(UpdateAdCommand request, CancellationToken ct)
    {
        var ad = await context.Ads
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (ad is null)
            throw new WebShopNotFoundException($"Ad (ID={request.Id}) was not found.");

        if (!AdRules.IsOwnerOrAdmin(currentUser, ad))
            throw new WebShopBusinessRuleException("403", "You are not allowed to edit this ad.");

        var category = await context.ProductCategories
            .FindAsync(new object[] { request.CategoryId }, ct)
            ?? throw new ValidationException("Invalid category.");

        ad.Title = request.Title.Trim();
        ad.Description = request.Description.Trim();
        ad.Images = request.Images ?? new List<string>();
        ad.Price = request.Price;
        ad.CategoryEntity = category;
        ad.Location = request.Location?.Trim();
        ad.Colors = AdOptions.Normalize(request.Colors);
        ad.Sizes = AdOptions.Normalize(request.Sizes);

        await context.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
