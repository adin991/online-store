using WebShop.Application.Abstractions;
using WebShop.Shared.Options;
using Microsoft.Extensions.Options;
using Stripe;

namespace WebShop.Infrastructure.Common;

public sealed class StripePaymentGateway : IStripePaymentGateway
{
    private readonly StripeClient _client;
    private readonly StripeOptions _options;

    public StripePaymentGateway(IOptions<StripeOptions> options)
    {
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _client = new StripeClient(_options.SecretKey);
    }

    public string PublishableKey => _options.PublishableKey;

    public async Task<StripePaymentIntentResult> CreatePaymentIntentAsync(
        decimal amountEur,
        string description,
        IDictionary<string, string> metadata,
        CancellationToken ct)
    {
        var amountInCents = (long)Math.Round(amountEur * 100m, MidpointRounding.AwayFromZero);

        var service = new PaymentIntentService(_client);
        var intent = await service.CreateAsync(new PaymentIntentCreateOptions
        {
            Amount = amountInCents,
            Currency = "eur",
            Description = description,
            Metadata = new Dictionary<string, string>(metadata),
            AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
            {
                Enabled = true
            }
        }, cancellationToken: ct);

        return new StripePaymentIntentResult(intent.Id, intent.ClientSecret);
    }

    public async Task<string> GetPaymentIntentStatusAsync(string paymentIntentId, CancellationToken ct)
    {
        var service = new PaymentIntentService(_client);
        var intent = await service.GetAsync(paymentIntentId, cancellationToken: ct);
        return intent.Status;
    }
}
