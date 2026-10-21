namespace WebShop.Application.WebShopModules.Ad;

/// <summary>Helpers for product options (colors / sizes) on an ad.</summary>
public static class AdOptions
{
    /// <summary>Trims values, drops empty entries and removes case-insensitive duplicates.</summary>
    public static List<string> Normalize(IEnumerable<string>? values)
        => (values ?? Enumerable.Empty<string>())
            .Select(v => v?.Trim() ?? string.Empty)
            .Where(v => v.Length > 0)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
}
