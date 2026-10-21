using WebShop.Application.WebShopModules.Ad;
using WebShop.Domain.Entities.WebShopEntities;

namespace WebShop.Application.WebShopModules.AdStatus.Commands.Update;

public sealed class UpdateAdStatusCommandHandler(IAppDbContext context, IAppCurrentUser currentUser)
            : IRequestHandler<UpdateAdStatusCommand, Unit>
{
    public async Task<Unit> Handle(UpdateAdStatusCommand request, CancellationToken ct)
    {
        var adStatus = await context.AdStatuss
            .Include(x => x.Ad)
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (adStatus is null)
            throw new WebShopNotFoundException($"AdStatus (ID={request.Id}) was not found.");

        // The Boosted row and the primary row are separate - one cannot be turned into the other,
        // otherwise an ad could end up with two primary statuses or two Boosted statuses.
        var isBoostedRow = adStatus.Status == AdStatusType.Boosted;
        if (isBoostedRow != (request.Status == AdStatusType.Boosted))
            throw new WebShopBusinessRuleException("400", "A Boosted status cannot be changed into a normal status or vice versa.");

        if (isBoostedRow)
        {
            AdRules.EnsureCanBoost(currentUser, adStatus.Ad);
            return Unit.Value;
        }

        var isSold = await AdRules.IsSoldAsync(context, adStatus.AdId, ct);
        AdRules.EnsureCanChangePrimaryStatus(currentUser, adStatus.Ad, adStatus.Status, request.Status, isSold);

        adStatus.Status = request.Status;
        await context.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
