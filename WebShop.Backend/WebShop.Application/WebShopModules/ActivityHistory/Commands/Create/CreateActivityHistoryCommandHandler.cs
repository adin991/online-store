using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebShop.Domain.Entities.WebShopEntities;

namespace WebShop.Application.WebShopModules.ActivityHistory.Commands.Create
{
    public class CreateActivityHistoryCommandHandler(IAppDbContext db, IAppCurrentUser currentUser)
     : IRequestHandler<CreateActivityHistoryCommand, int>
    {
        public async Task<int> Handle(CreateActivityHistoryCommand request, CancellationToken ct)
        {
            if (!currentUser.IsAuthenticated)
                throw new WebShopBusinessRuleException("401", "User must be logged in");

            var userExists = await db.Users.AnyAsync(u => u.Id == currentUser.UserId, ct);
            if (!userExists)
                throw new WebShopNotFoundException("User not found");

            var activityHistory = new Domain.Entities.WebShopEntities.ActivityHistory
            {
                UserId = (int)currentUser.UserId,
                Activity = request.Activity
            };

            await db.ActivityHistorys.AddAsync(activityHistory, ct);
            await db.SaveChangesAsync(ct);
            return activityHistory.Id;
        }
    }
}
