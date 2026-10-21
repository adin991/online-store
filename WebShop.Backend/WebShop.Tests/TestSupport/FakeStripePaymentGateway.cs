namespace WebShop.Tests.TestSupport;

/// <summary>
/// Test double for IStripePaymentGateway - does not call the real Stripe API, it only returns
/// preset values so the Payment handlers can be tested in isolation.
/// </summary>
internal sealed class FakeStripePaymentGateway : IStripePaymentGateway
{
    public string PublishableKey { get; set; } = "pk_test_fake";
    public string PaymentIntentIdToReturn { get; set; } = "pi_fake_123";
    public string ClientSecretToReturn { get; set; } = "pi_fake_123_secret";
    public string StatusToReturn { get; set; } = "succeeded";

    public Task<StripePaymentIntentResult> CreatePaymentIntentAsync(
        decimal amountEur,
        string description,
        IDictionary<string, string> metadata,
        CancellationToken ct)
        => Task.FromResult(new StripePaymentIntentResult(PaymentIntentIdToReturn, ClientSecretToReturn));

    public Task<string> GetPaymentIntentStatusAsync(string paymentIntentId, CancellationToken ct)
        => Task.FromResult(StatusToReturn);
}
