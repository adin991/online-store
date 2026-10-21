using WebShop.Domain.Entities.WebShopEntities;

namespace WebShop.Application.WebShopModules.Ad.Commands.Status;

public sealed class UpdateStatusThruAdCommandHandler(IAppDbContext context, IAppCurrentUser currentUser)
            : IRequestHandler<UpdateStatusThruAdCommand, Unit>
{
    public async Task<Unit> Handle(UpdateStatusThruAdCommand request, CancellationToken ct)
    {
        var ad = await context.Ads
            .Include(x => x.AdStatuses)
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (ad is null)
            throw new WebShopNotFoundException($"Ad (ID={request.Id}) was not found.");

        if (request.AdStatusType == AdStatusType.Boosted)
        {
            // Boosted is an extra flag next to the primary status - add it once.
            AdRules.EnsureCanBoost(currentUser, ad);
            if (!ad.AdStatuses.Any(s => s.Status == AdStatusType.Boosted))
                ad.AdStatuses.Add(new Domain.Entities.WebShopEntities.AdStatus { AdId = ad.Id, Status = AdStatusType.Boosted });
        }
        else
        {
            // Always change the primary (non-Boosted) status explicitly.
            var current = AdRules.GetPrimaryStatus(ad.AdStatuses)?.Status;
            var isSold = await AdRules.IsSoldAsync(context, ad.Id, ct);
            AdRules.EnsureCanChangePrimaryStatus(currentUser, ad, current, request.AdStatusType, isSold);
            AdRules.SetPrimaryStatus(ad, request.AdStatusType);
        }

        await context.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
