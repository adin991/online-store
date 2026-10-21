using System.Linq.Expressions;
using WebShop.Domain.Entities.WebShopEntities;

namespace WebShop.Application.WebShopModules.Ad;

/// <summary>
/// Single place for the business rules around ad availability and ad status changes.
///
/// Status model: every ad has exactly one "primary" (non-Boosted) status
/// (Live, Closed, AwaitingReview, Removed, ShadowBanned) and optionally one extra Boosted status.
///
/// Availability rule: an ad is available (publicly listed, visible and purchasable)
/// only while its primary status is Live. A paid ad is moved to Closed, so it can no longer be bought.
/// </summary>
public static class AdRules
{
    /// <summary>EF-translatable filter: ad is publicly available.</summary>
    public static readonly Expression<Func<Domain.Entities.WebShopEntities.Ad, bool>> IsAvailable =
        ad => ad.AdStatuses.Any(s => s.Status == AdStatusType.Live);

    /// <summary>Statuses that only an administrator (moderator) may set or lift.</summary>
    private static readonly AdStatusType[] ModerationStatuses =
    {
        AdStatusType.AwaitingReview,
        AdStatusType.Removed,
        AdStatusType.ShadowBanned
    };

    public static bool IsOwnerOrAdmin(IAppCurrentUser currentUser, Domain.Entities.WebShopEntities.Ad ad)
        => currentUser.IsAdmin || (currentUser.UserId.HasValue && currentUser.UserId.Value == ad.UserId);

    /// <summary>Returns the primary (non-Boosted) status of an ad, or null if it has none.</summary>
    public static Domain.Entities.WebShopEntities.AdStatus? GetPrimaryStatus(IEnumerable<Domain.Entities.WebShopEntities.AdStatus> statuses)
        => statuses
            .Where(s => s.Status != AdStatusType.Boosted)
            .OrderByDescending(s => s.Id)
            .FirstOrDefault();

    /// <summary>
    /// Throws if the current user is not allowed to change the ad's primary status
    /// from <paramref name="current"/> to <paramref name="requested"/>.
    /// Admin: any change. Owner: only Live/Closed, never out of a moderation status,
    /// and a sold ad cannot be put back on sale.
    /// </summary>
    public static void EnsureCanChangePrimaryStatus(
        IAppCurrentUser currentUser,
        Domain.Entities.WebShopEntities.Ad ad,
        AdStatusType? current,
        AdStatusType requested,
        bool isSold)
    {
        if (!IsOwnerOrAdmin(currentUser, ad))
            throw new WebShopBusinessRuleException("403", "You are not allowed to change the status of this ad.");

        if (requested == AdStatusType.Boosted)
            throw new WebShopBusinessRuleException("400", "Boosted is not a primary status.");

        if (currentUser.IsAdmin)
            return;

        if (ModerationStatuses.Contains(requested))
            throw new WebShopBusinessRuleException("403", $"Only an administrator can set the status '{requested}'.");

        if (current.HasValue && ModerationStatuses.Contains(current.Value))
            throw new WebShopBusinessRuleException("403", $"The ad is in status '{current}' and can only be changed by an administrator.");

        if (isSold && requested == AdStatusType.Live)
            throw new WebShopBusinessRuleException("409", "This ad has already been sold and cannot be put back on sale.");
    }

    /// <summary>Throws if the current user may not add or remove the Boosted flag of the ad.</summary>
    public static void EnsureCanBoost(IAppCurrentUser currentUser, Domain.Entities.WebShopEntities.Ad ad)
    {
        if (!IsOwnerOrAdmin(currentUser, ad))
            throw new WebShopBusinessRuleException("403", "You are not allowed to change the status of this ad.");
    }

    /// <summary>
    /// Sets the primary (non-Boosted) status explicitly; creates it if the ad has none.
    /// The ad must be loaded with its <see cref="Domain.Entities.WebShopEntities.Ad.AdStatuses"/>.
    /// </summary>
    public static void SetPrimaryStatus(Domain.Entities.WebShopEntities.Ad ad, AdStatusType status)
    {
        var primary = GetPrimaryStatus(ad.AdStatuses);
        if (primary is null)
            ad.AdStatuses.Add(new Domain.Entities.WebShopEntities.AdStatus { AdId = ad.Id, Status = status });
        else
            primary.Status = status;
    }

    /// <summary>True if at least one payment for the ad has succeeded.</summary>
    public static Task<bool> IsSoldAsync(IAppDbContext db, int adId, CancellationToken ct)
        => db.Transactions.AnyAsync(t => t.AdId == adId && t.Status == TransactionStatus.Succeeded, ct);
}
