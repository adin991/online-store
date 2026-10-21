using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebShop.Application.WebShopModules.Notifications.Queries.GetById
{
    public class GetNotificationsByIdQueryHandler(IAppDbContext db) : IRequestHandler<GetNotificationsByIdQuery, GetNotificationsByIdQueryDto>
    {
        public async Task<GetNotificationsByIdQueryDto> Handle(GetNotificationsByIdQuery request, CancellationToken ct)
        {
            var notification = await db.Notifications.FirstOrDefaultAsync(x => x.Id == request.Id, ct);
            if (notification == null)
            {
                throw new WebShopNotFoundException("Not found notification id");
            }
            return new GetNotificationsByIdQueryDto
            {
                Id = notification.Id,
                Title = notification.Title,
                Content = notification.Content,
                CreatedAt = notification.CreatedAtUtc
            };
        }
    }
}
