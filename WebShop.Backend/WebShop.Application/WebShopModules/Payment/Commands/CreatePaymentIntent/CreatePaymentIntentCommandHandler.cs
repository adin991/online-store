using WebShop.Domain.Entities.WebShopEntities;

namespace WebShop.Application.WebShopModules.Payment.Commands.CreatePaymentIntent;

public class CreatePaymentIntentCommandHandler(
    IAppDbContext db,
    IAppCurrentUser currentUser,
    IStripePaymentGateway stripeGateway)
    : IRequestHandler<CreatePaymentIntentCommand, CreatePaymentIntentResultDto>
{
    public async Task<CreatePaymentIntentResultDto> Handle(CreatePaymentIntentCommand request, CancellationToken ct)
    {
        if (!currentUser.IsAuthenticated || currentUser.UserId is null)
            throw new WebShopBusinessRuleException("401", "User must be logged in");

        var buyerId = currentUser.UserId.Value;
        var ads = await PaymentRules.LoadPurchasableAdsAsync(db, new[] { request.AdId }, buyerId, ct);
        var ad = ads[0];

        var amountEur = PaymentRules.ToEur(ad.Price);
        if (amountEur <= 0)
            throw new WebShopBusinessRuleException("400", "Ad price must be greater than zero");

        var metadata = new Dictionary<string, string>
        {
            ["adId"] = ad.Id.ToString(),
            ["userId"] = buyerId.ToString()
        };

        var intent = await stripeGateway.CreatePaymentIntentAsync(
            amountEur,
            $"Ad purchase: {ad.Title}",
            metadata,
            ct);

        var transaction = new Transaction
        {
            AdId = ad.Id,
            UserId = buyerId,
            AmountEur = amountEur,
            AmountOriginalKm = ad.Price,
            StripePaymentIntentId = intent.PaymentIntentId,
            Status = TransactionStatus.Pending
        };

        await db.Transactions.AddAsync(transaction, ct);
        await db.SaveChangesAsync(ct);

        return new CreatePaymentIntentResultDto
        {
            TransactionId = transaction.Id,
            ClientSecret = intent.ClientSecret,
            PublishableKey = stripeGateway.PublishableKey,
            AmountEur = amountEur
        };
    }
}
