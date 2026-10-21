using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebShop.Application.WebShopModules.Notifications.Commands.Delete
{
    public class DeleteNotificationsHandler(IAppDbContext db) : IRequestHandler<DeleteNotificationsCommand, Unit>
    {
        public async Task<Unit> Handle(DeleteNotificationsCommand request, CancellationToken ct)
        {
            var notification = await db.Notifications.FirstOrDefaultAsync(x => x.Id == request.Id, ct);

            if (notification == null)
            {
                throw new WebShopNotFoundException("Not found notification");
            }

            db.Notifications.Remove(notification);
            await db.SaveChangesAsync(ct);

            return Unit.Value;
        }
    }
}
