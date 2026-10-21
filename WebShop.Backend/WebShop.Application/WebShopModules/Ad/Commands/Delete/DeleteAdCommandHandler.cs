namespace WebShop.Application.WebShopModules.Ad.Commands.Delete;

public class DeleteProductCommandHandler(IAppDbContext context, IAppCurrentUser appCurrentUser)
      : IRequestHandler<DeleteAdCommand, Unit>
{
    public async Task<Unit> Handle(DeleteAdCommand request, CancellationToken cancellationToken)
    {
        var ad = await context.Ads
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (ad is null)
            throw new WebShopNotFoundException("Ad was not found.");

        if (!AdRules.IsOwnerOrAdmin(appCurrentUser, ad))
            throw new WebShopBusinessRuleException("403", "You are not allowed to delete this ad.");

        context.Ads.Remove(ad);
        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
