using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebShop.Application.WebShopModules.SearchHistory.Commands.Create
{
    public class CreateSearchHistoryCommandHandler(IAppDbContext db, IAppCurrentUser currentUser)
    : IRequestHandler<CreateSearchHistoryCommand, int>
    {
        public async Task<int> Handle(CreateSearchHistoryCommand request, CancellationToken ct)
        {
            var userExists = await db.Users.AnyAsync(u => u.Id == currentUser.UserId, ct);
            if (!userExists)
                throw new WebShopNotFoundException("User not found");

            var searchHistory = new WebShop.Domain.Entities.WebShopEntities.SearchHistory
            {
                UserId = (int)currentUser.UserId,
                Content = request.Content,
                Date = DateTime.UtcNow
            };

            await db.SearchHistorys.AddAsync(searchHistory, ct);
            await db.SaveChangesAsync(ct);
            return searchHistory.Id;
        }
    }
}
