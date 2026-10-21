using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebShop.Application.WebShopModules.SearchHistory.Commands.Update
{
    public class UpdateSearchHistoryCommandHandler(IAppDbContext db, IAppCurrentUser currentUser)
     : IRequestHandler<UpdateSearchHistoryCommand, Unit>
    {
        public async Task<Unit> Handle(UpdateSearchHistoryCommand request, CancellationToken ct)
        {
            var searchHistory = await db.SearchHistorys.FirstOrDefaultAsync(x => x.Id == request.Id, ct);
            if (searchHistory is null)
                throw new WebShopNotFoundException("Search history not found");

            if (!currentUser.IsAdmin && currentUser.UserId != searchHistory.UserId)
                throw new WebShopBusinessRuleException("403", "You cannot update this search history");

            searchHistory.Content = request.Content;
            searchHistory.Date = DateTime.UtcNow;

            await db.SaveChangesAsync(ct);
            return Unit.Value;
        }
    }
}
