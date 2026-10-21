namespace WebShop.Application.WebShopModules.Notifications.Commands.Update;

public class UpdateNotificationsCommandHandler(IAppDbContext db, IAppCurrentUser currentUser)
    : IRequestHandler<UpdateNotificationsCommand, Unit>
{
    public async Task<Unit> Handle(UpdateNotificationsCommand request, CancellationToken ct)
    {
        var notification = await db.Notifications.FirstOrDefaultAsync(x => x.Id == request.Id, ct);
        if (notification is null)
            throw new WebShopNotFoundException("Notification not found");

        if (!currentUser.IsAdmin && currentUser.UserId != notification.UserId)
            throw new WebShopBusinessRuleException("403", "You cannot update this notification");

        notification.Title = request.Title;
        notification.Content = request.Content;

        // If IsRead is set, update the read status and DateOfRead
        if (request.IsRead.HasValue)
        {
            notification.IsRead = request.IsRead.Value;
            if (request.IsRead.Value)
                notification.DateOfRead = DateTime.UtcNow;
            else
                notification.DateOfRead = default;
        }

        await db.SaveChangesAsync(ct);
        return Unit.Value;
    }
}