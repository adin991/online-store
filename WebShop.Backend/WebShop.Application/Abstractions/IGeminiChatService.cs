namespace WebShop.Application.Abstractions;

/// <summary>One turn of a chat conversation. <see cref="Role"/> is "user" or "model".</summary>
public sealed record ChatTurn(string Role, string Text);

/// <summary>
/// Thin abstraction over the Google Gemini API so the Application layer doesn't
/// depend directly on the HTTP client / SSE parsing details.
/// </summary>
public interface IGeminiChatService
{
    /// <summary>Streams the model's reply for the given conversation, one text chunk at a time.</summary>
    IAsyncEnumerable<string> StreamReplyAsync(IReadOnlyList<ChatTurn> history, CancellationToken ct);
}
