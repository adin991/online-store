namespace WebShop.Application.WebShopModules.Messages.Commands.Update;

public class UpdateMessageCommandHandler(IAppDbContext db, IAppCurrentUser currentUser)
    : IRequestHandler<UpdateMessageCommand, Unit>
{
    public async Task<Unit> Handle(UpdateMessageCommand request, CancellationToken ct)
    {
        var message = await db.Messages.FirstOrDefaultAsync(x => x.Id == request.Id, ct);
        if (message is null)
            throw new WebShopNotFoundException("Message not found");

        // Only the sender, the receiver or an admin can update the message
        if (!currentUser.IsAdmin && 
            currentUser.UserId != message.SenderId && 
            currentUser.UserId != message.ReceiverId)
            throw new WebShopBusinessRuleException("403", "You cannot update this message");

        message.Content = request.Content;

        // If IsRead is set, update the read status
        if (request.IsRead.HasValue)
            message.IsRead = request.IsRead.Value;

        await db.SaveChangesAsync(ct);
        return Unit.Value;
    }
}