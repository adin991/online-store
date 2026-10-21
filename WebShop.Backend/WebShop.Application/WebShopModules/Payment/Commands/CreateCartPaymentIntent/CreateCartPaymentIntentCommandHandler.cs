using WebShop.Domain.Entities.WebShopEntities;

namespace WebShop.Application.WebShopModules.Payment.Commands.CreateCartPaymentIntent;

public class CreateCartPaymentIntentCommandHandler(
    IAppDbContext db,
    IAppCurrentUser currentUser,
    IStripePaymentGateway stripeGateway)
    : IRequestHandler<CreateCartPaymentIntentCommand, CreateCartPaymentIntentResultDto>
{
    public async Task<CreateCartPaymentIntentResultDto> Handle(CreateCartPaymentIntentCommand request, CancellationToken ct)
    {
        if (!currentUser.IsAuthenticated || currentUser.UserId is null)
            throw new WebShopBusinessRuleException("401", "User must be logged in");

        if (request.Items is null || request.Items.Count == 0)
            throw new WebShopBusinessRuleException("400", "Cart is empty");

        var buyerId = currentUser.UserId.Value;
        var adIds = request.Items.Select(i => i.AdId).Distinct().ToList();
        var ads = await PaymentRules.LoadPurchasableAdsAsync(db, adIds, buyerId, ct);

        var adsById = ads.ToDictionary(a => a.Id);

        var lines = request.Items
            .Select(i => (Item: i, AmountKm: adsById[i.AdId].Price * i.Quantity))
            .ToList();

        if (lines.Any(l => l.AmountKm <= 0))
            throw new WebShopBusinessRuleException("400", "Ad price must be greater than zero");

        var totalKm = lines.Sum(l => l.AmountKm);
        var totalEur = PaymentRules.ToEur(totalKm);

        var metadata = new Dictionary<string, string>
        {
            ["adIds"] = string.Join(",", adIds),
            ["userId"] = buyerId.ToString()
        };

        var intent = await stripeGateway.CreatePaymentIntentAsync(
            totalEur,
            $"Cart purchase ({request.Items.Count} items)",
            metadata,
            ct);

        var transactions = lines.Select(l => new Transaction
        {
            AdId = l.Item.AdId,
            UserId = buyerId,
            AmountEur = PaymentRules.ToEur(l.AmountKm),
            AmountOriginalKm = l.AmountKm,
            StripePaymentIntentId = intent.PaymentIntentId,
            Status = TransactionStatus.Pending
        }).ToList();

        await db.Transactions.AddRangeAsync(transactions, ct);
        await db.SaveChangesAsync(ct);

        return new CreateCartPaymentIntentResultDto
        {
            TransactionIds = transactions.Select(t => t.Id).ToList(),
            ClientSecret = intent.ClientSecret,
            PublishableKey = stripeGateway.PublishableKey,
            AmountEur = totalEur
        };
    }
}
