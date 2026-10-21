using WebShop.Application.WebShopModules.Ad;

namespace WebShop.Application.WebShopModules.Payment;

/// <summary>Purchase rules shared by the single-ad and the cart payment flow.</summary>
internal static class PaymentRules
{
    // Fixed currency-board rate BAM -> EUR (Stripe does not support charging in BAM).
    public const decimal BamToEurRate = 1.95583m;

    /// <summary>
    /// Loads the ads with their statuses and throws if any of them does not exist,
    /// is not available for sale (see AdRules) or belongs to the buyer.
    /// </summary>
    public static async Task<List<Domain.Entities.WebShopEntities.Ad>> LoadPurchasableAdsAsync(
        IAppDbContext db,
        IReadOnlyCollection<int> adIds,
        int buyerUserId,
        CancellationToken ct)
    {
        var ads = await db.Ads
            .Include(a => a.AdStatuses)
            .Where(a => adIds.Contains(a.Id))
            .ToListAsync(ct);

        if (ads.Count != adIds.Count)
            throw new WebShopNotFoundException("One or more ads were not found.");

        var isAvailable = AdRules.IsAvailable.Compile();

        foreach (var ad in ads)
        {
            if (ad.UserId == buyerUserId)
                throw new WebShopBusinessRuleException("400", $"You cannot buy your own ad '{ad.Title}'.");

            if (!isAvailable(ad))
                throw new WebShopBusinessRuleException("409", $"The ad '{ad.Title}' is no longer available.");
        }

        return ads;
    }

    public static decimal ToEur(decimal amountKm)
        => Math.Round(amountKm / BamToEurRate, 2, MidpointRounding.AwayFromZero);
}
