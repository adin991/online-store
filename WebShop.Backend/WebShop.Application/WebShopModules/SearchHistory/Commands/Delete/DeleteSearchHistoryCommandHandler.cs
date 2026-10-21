using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebShop.Application.WebShopModules.SearchHistory.Commands.Delete
{
    public class DeleteSearchHistoryCommandHandler(IAppDbContext db, IAppCurrentUser currentUser)
    : IRequestHandler<DeleteSearchHistoryCommand, Unit>
    {
        public async Task<Unit> Handle(DeleteSearchHistoryCommand request, CancellationToken ct)
        {
            var searchHistory = await db.SearchHistorys.FirstOrDefaultAsync(x => x.Id == request.Id, ct);
            if (searchHistory is null)
                throw new WebShopNotFoundException("Search history not found");

            if (!currentUser.IsAdmin && currentUser.UserId != searchHistory.UserId)
                throw new WebShopBusinessRuleException("403", "You cannot delete this search history");

            db.SearchHistorys.Remove(searchHistory);
            await db.SaveChangesAsync(ct);
            return Unit.Value;
        }
    }
}
