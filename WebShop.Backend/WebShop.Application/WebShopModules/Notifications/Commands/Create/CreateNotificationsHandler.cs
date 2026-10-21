using Microsoft.EntityFrameworkCore.Storage;
using Org.BouncyCastle.Asn1.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebShop.Application.WebShopModules.Notifications.Commands.Create
{
    public class CreateNotificationsHandler(IAppDbContext db, IAppCurrentUser currentUser) : IRequestHandler<CreateNotificationsCommand, int>
    {
        public async Task<int> Handle(CreateNotificationsCommand request, CancellationToken ct)
        {
            var userExists = await db.Users.AnyAsync(u => u.Id == currentUser.UserId, ct);

            var notification = new WebShop.Domain.Entities.WebShopEntities.Notification
            {
                Title = request.Title,
                Content = request.Content,
                IsRead = false,
                UserId = (int)currentUser.UserId,
                DateOfRead = default
            };

            await db.Notifications.AddAsync(notification, ct);
            await db.SaveChangesAsync(ct);

            return notification.Id;
        }
    }
}
