namespace WebShop.Application.Abstractions;

public sealed record StripePaymentIntentResult(string PaymentIntentId, string ClientSecret);

/// <summary>
/// Thin abstraction over the Stripe API so the Application layer doesn't
/// depend directly on the Stripe.net SDK.
/// </summary>
public interface IStripePaymentGateway
{
    string PublishableKey { get; }

    Task<StripePaymentIntentResult> CreatePaymentIntentAsync(
        decimal amountEur,
        string description,
        IDictionary<string, string> metadata,
        CancellationToken ct);

    /// <summary>Returns the Stripe PaymentIntent status (e.g. "succeeded", "requires_payment_method").</summary>
    Task<string> GetPaymentIntentStatusAsync(string paymentIntentId, CancellationToken ct);
}
