namespace WebShop.Application.Modules.Chat.Commands.SendMessage;

public sealed class SendMessageCommandHandler(IGeminiChatService gemini)
    : IStreamRequestHandler<SendMessageCommand, string>
{
    public IAsyncEnumerable<string> Handle(SendMessageCommand request, CancellationToken ct)
    {
        var history = new List<ChatTurn>(request.History) { new("user", request.Message) };
        return gemini.StreamReplyAsync(history, ct);
    }
}
