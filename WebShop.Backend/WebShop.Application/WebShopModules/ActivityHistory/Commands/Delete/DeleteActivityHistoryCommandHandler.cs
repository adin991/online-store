using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebShop.Application.WebShopModules.ActivityHistory.Commands.Delete
{
    public class DeleteActivityHistoryCommandHandler(IAppDbContext db, IAppCurrentUser currentUser)
     : IRequestHandler<DeleteActivityHistoryCommand, Unit>
    {
        public async Task<Unit> Handle(DeleteActivityHistoryCommand request, CancellationToken ct)
        {
            var activityHistory = await db.ActivityHistorys.FirstOrDefaultAsync(x => x.Id == request.Id, ct);
            if (activityHistory is null)
                throw new WebShopNotFoundException("Activity history not found");

            if (!currentUser.IsAdmin && currentUser.UserId != activityHistory.UserId)
                throw new WebShopBusinessRuleException("403", "You cannot delete this activity history");

            db.ActivityHistorys.Remove(activityHistory);
            await db.SaveChangesAsync(ct);
            return Unit.Value;
        }
    }
}
