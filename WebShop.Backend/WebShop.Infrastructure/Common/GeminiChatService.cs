using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using WebShop.Application.Abstractions;
using WebShop.Shared.Options;

namespace WebShop.Infrastructure.Common;

/// <summary>
/// Calls the Google Gemini API (generativelanguage.googleapis.com) directly over HTTP
/// and streams back the model's reply, one text chunk at a time.
/// </summary>
public sealed class GeminiChatService(HttpClient http, IOptions<GeminiOptions> options) : IGeminiChatService
{
    private const string SystemInstruction =
        "You are an assistant for the WebShop online store. You help users with questions about " +
        "using the site, delivery, returns and general shopping questions. Do not invent " +
        "specific product or order data that you do not have. Answer briefly, in the same " +
        "language the user writes in.";

    private readonly GeminiOptions _options = options.Value;

    public async IAsyncEnumerable<string> StreamReplyAsync(
        IReadOnlyList<ChatTurn> history,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken ct)
    {
        var requestBody = new GenerateContentRequest
        {
            Contents = history.Select(t => new ContentDto
            {
                Role = t.Role,
                Parts = [new PartDto { Text = t.Text }]
            }).ToList(),
            SystemInstruction = new SystemInstructionDto { Parts = [new PartDto { Text = SystemInstruction }] },
            GenerationConfig = new GenerationConfigDto { MaxOutputTokens = _options.MaxOutputTokens }
        };

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            $"v1beta/models/{_options.Model}:streamGenerateContent?alt=sse")
        {
            Content = JsonContent.Create(requestBody)
        };
        request.Headers.Add("x-goog-api-key", _options.ApiKey);

        using var response = await http.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct);
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync(ct);
        using var reader = new StreamReader(stream);

        while (await reader.ReadLineAsync(ct) is { } line)
        {
            if (!line.StartsWith("data: ", StringComparison.Ordinal))
                continue;

            var json = line["data: ".Length..];
            var chunk = JsonSerializer.Deserialize<GeminiStreamChunk>(json, JsonOptions);
            var text = chunk?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text;

            if (!string.IsNullOrEmpty(text))
                yield return text;
        }
    }

    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    private sealed class GenerateContentRequest
    {
        [JsonPropertyName("contents")]
        public required List<ContentDto> Contents { get; init; }

        [JsonPropertyName("systemInstruction")]
        public SystemInstructionDto? SystemInstruction { get; init; }

        [JsonPropertyName("generationConfig")]
        public GenerationConfigDto? GenerationConfig { get; init; }
    }

    private sealed class ContentDto
    {
        [JsonPropertyName("role")]
        public required string Role { get; init; }

        [JsonPropertyName("parts")]
        public required List<PartDto> Parts { get; init; }
    }

    private sealed class SystemInstructionDto
    {
        [JsonPropertyName("parts")]
        public required List<PartDto> Parts { get; init; }
    }

    private sealed class GenerationConfigDto
    {
        [JsonPropertyName("maxOutputTokens")]
        public int MaxOutputTokens { get; init; }
    }

    private sealed class PartDto
    {
        [JsonPropertyName("text")]
        public required string Text { get; init; }
    }

    private sealed class GeminiStreamChunk
    {
        [JsonPropertyName("candidates")]
        public List<CandidateDto>? Candidates { get; set; }
    }

    private sealed class CandidateDto
    {
        [JsonPropertyName("content")]
        public ContentPartsDto? Content { get; set; }
    }

    private sealed class ContentPartsDto
    {
        [JsonPropertyName("parts")]
        public List<PartDto>? Parts { get; set; }
    }
}
