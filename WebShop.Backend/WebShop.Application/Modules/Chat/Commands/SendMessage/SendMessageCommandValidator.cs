namespace WebShop.Application.Modules.Chat.Commands.SendMessage;

public sealed class SendMessageCommandValidator : AbstractValidator<SendMessageCommand>
{
    public SendMessageCommandValidator()
    {
        RuleFor(x => x.Message)
            .NotEmpty().WithMessage("Message is required.")
            .MaximumLength(2000).WithMessage("Message can be up to 2000 characters long.");

        RuleFor(x => x.History)
            .Must(h => h.Count <= 20).WithMessage("History can contain at most 20 turns.");
    }
}
