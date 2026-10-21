using System.ComponentModel.DataAnnotations;

namespace WebShop.Shared.Options;

/// <summary>Typed Google Gemini API settings from the "Gemini" section.</summary>
public sealed class GeminiOptions
{
    public const string SectionName = "Gemini";

    [Required] public string ApiKey { get; init; } = default!;
    public string Model { get; init; } = "gemini-3.6-flash";
    public int MaxOutputTokens { get; init; } = 1024;
}
