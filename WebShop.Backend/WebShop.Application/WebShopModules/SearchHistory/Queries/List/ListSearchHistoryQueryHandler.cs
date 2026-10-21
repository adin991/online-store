using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebShop.Application.WebShopModules.SearchHistory.Queries.List
{
    public class ListSearchHistoryQueryHandler(IAppDbContext context, IAppCurrentUser currentUser)
    : IRequestHandler<ListSearchHistoryQuery, PageResult<ListSearchHistoryQueryDto>>
    {
        public async Task<PageResult<ListSearchHistoryQueryDto>> Handle(ListSearchHistoryQuery request, CancellationToken ct)
        {
            var q = context.SearchHistorys.AsNoTracking();

            // A user can only see their own search history unless they are an admin
            if (!currentUser.IsAdmin)
                q = q.Where(s => s.UserId == currentUser.UserId);

            if (!string.IsNullOrWhiteSpace(request.SearchContent))
                q = q.Where(s => s.Content.ToLower().Contains(request.SearchContent.ToLower()));


            var projectedQuery = q.OrderByDescending(s => s.Date)
                .Select(s => new ListSearchHistoryQueryDto
                {
                    Id = s.Id,
                    UserId = s.UserId,
                    Content = s.Content,
                    Date = s.Date
                });

            return await PageResult<ListSearchHistoryQueryDto>.FromQueryableAsync(projectedQuery, request.Paging, ct);
        }
    }
}
