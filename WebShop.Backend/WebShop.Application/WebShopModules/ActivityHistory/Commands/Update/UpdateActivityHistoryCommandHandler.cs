using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebShop.Application.WebShopModules.ActivityHistory.Commands.Update
{

    public class UpdateActivityHistoryCommandHandler(IAppDbContext db, IAppCurrentUser currentUser)
        : IRequestHandler<UpdateActivityHistoryCommand, Unit>
    {
        public async Task<Unit> Handle(UpdateActivityHistoryCommand request, CancellationToken ct)
        {
            var activityHistory = await db.ActivityHistorys.FirstOrDefaultAsync(x => x.Id == request.Id, ct);
            if (activityHistory is null)
                throw new WebShopNotFoundException("Activity history not found");

            if (!currentUser.IsAdmin && currentUser.UserId != activityHistory.UserId)
                throw new WebShopBusinessRuleException("403", "You cannot update this activity history");

            activityHistory.Activity = request.Activity;

            await db.SaveChangesAsync(ct);
            return Unit.Value;
        }
    }
}
