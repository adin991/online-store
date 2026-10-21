namespace WebShop.Application.Modules.Chat.Commands.SendMessage;

/// <summary>
/// Sends a user message to the AI assistant and streams back the reply, one text
/// chunk at a time. <see cref="History"/> is the prior conversation (without the
/// new message) so the model has context - the client is responsible for keeping it.
/// </summary>
public sealed class SendMessageCommand : IStreamRequest<string>
{
    public required string Message { get; init; }
    public List<ChatTurn> History { get; init; } = [];
}
