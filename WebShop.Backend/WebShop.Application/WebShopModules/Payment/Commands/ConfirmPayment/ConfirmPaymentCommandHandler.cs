using WebShop.Application.WebShopModules.Ad;
using WebShop.Domain.Entities.WebShopEntities;

namespace WebShop.Application.WebShopModules.Payment.Commands.ConfirmPayment;

public class ConfirmPaymentCommandHandler(
    IAppDbContext db,
    IAppCurrentUser currentUser,
    IStripePaymentGateway stripeGateway,
    TimeProvider time)
    : IRequestHandler<ConfirmPaymentCommand, ConfirmPaymentResultDto>
{
    public async Task<ConfirmPaymentResultDto> Handle(ConfirmPaymentCommand request, CancellationToken ct)
    {
        if (!currentUser.IsAuthenticated)
            throw new WebShopBusinessRuleException("401", "User must be logged in");

        if (request.TransactionIds is null || request.TransactionIds.Count == 0)
            throw new WebShopBusinessRuleException("400", "No transactions to confirm");

        var transactions = await db.Transactions
            .Where(t => request.TransactionIds.Contains(t.Id))
            .ToListAsync(ct);

        if (transactions.Count != request.TransactionIds.Distinct().Count())
            throw new WebShopNotFoundException("Transaction not found");

        if (transactions.Any(t => t.UserId != currentUser.UserId))
            throw new WebShopBusinessRuleException("403", "This transaction does not belong to the current user");

        var paymentIntentId = transactions[0].StripePaymentIntentId;
        if (transactions.Any(t => t.StripePaymentIntentId != paymentIntentId))
            throw new WebShopBusinessRuleException("400", "Transactions do not belong to the same payment");

        // Never trust the client - the status is checked directly with Stripe.
        var stripeStatus = await stripeGateway.GetPaymentIntentStatusAsync(paymentIntentId, ct);

        var status = stripeStatus switch
        {
            "succeeded" => TransactionStatus.Succeeded,
            "canceled" => TransactionStatus.Canceled,
            _ => TransactionStatus.Failed
        };

        foreach (var transaction in transactions)
        {
            transaction.Status = status;
            if (status == TransactionStatus.Succeeded)
                transaction.PaidAtUtc = time.GetUtcNow().UtcDateTime;
        }

        if (status == TransactionStatus.Succeeded)
        {
            // A paid ad is sold: close it so it disappears from the public list and cannot be bought again.
            var adIds = transactions.Select(t => t.AdId).Distinct().ToList();
            var ads = await db.Ads
                .Include(a => a.AdStatuses)
                .Where(a => adIds.Contains(a.Id))
                .ToListAsync(ct);

            foreach (var ad in ads)
                AdRules.SetPrimaryStatus(ad, AdStatusType.Closed);
        }

        await db.SaveChangesAsync(ct);

        return new ConfirmPaymentResultDto
        {
            Success = status == TransactionStatus.Succeeded,
            Status = status.ToString()
        };
    }
}
