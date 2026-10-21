using System.ComponentModel.DataAnnotations;

namespace WebShop.Shared.Options;

/// <summary>Typed Stripe settings from the "Stripe" section.</summary>
public sealed class StripeOptions
{
    public const string SectionName = "Stripe";

    [Required] public string SecretKey { get; init; } = default!;
    [Required] public string PublishableKey { get; init; } = default!;
}
