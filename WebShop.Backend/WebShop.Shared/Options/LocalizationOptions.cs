using System.ComponentModel.DataAnnotations;

namespace WebShop.Shared.Options;

/// <summary>
/// Typed regional settings from the "Localization" section.
/// The request culture is picked from the Accept-Language header sent by the SPA
/// (falls back to <see cref="DefaultCulture"/>).
/// </summary>
public sealed class AppLocalizationOptions
{
    public const string SectionName = "Localization";

    [Required] public string DefaultCulture { get; init; } = "bs-BA";
    [Required, MinLength(1)] public string[] SupportedCultures { get; init; } = { "bs-BA", "en-US" };

    /// <summary>ISO 4217 currency code of ad prices (KM = BAM).</summary>
    [Required] public string Currency { get; init; } = "BAM";
}
