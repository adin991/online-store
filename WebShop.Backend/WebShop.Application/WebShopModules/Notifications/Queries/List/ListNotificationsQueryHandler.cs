using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebShop.Application.WebShopModules.Notifications.Queries.List
{
    public class ListNotificationsQueryHandler(IAppDbContext dbx) : IRequestHandler<ListNotificationsQuery, PageResult<ListNotificationsQueryDto>>
    {
        public async Task<PageResult<ListNotificationsQueryDto>> Handle(ListNotificationsQuery request, CancellationToken ct)
        {
            var q = dbx.Notifications.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.SearchTtile))
            {
                 q = q.Where(x => x.Title.ToLower().Contains(request.SearchTtile.ToLower()));
            }


            var notigQuery = q.OrderBy(x => x.Title)
                .Select(x => new ListNotificationsQueryDto
            {
                Title = x.Title,
                Content = x.Content
            }); 

            return await PageResult<ListNotificationsQueryDto>.FromQueryableAsync(notigQuery, request.Paging, ct);
        }
    }
}
