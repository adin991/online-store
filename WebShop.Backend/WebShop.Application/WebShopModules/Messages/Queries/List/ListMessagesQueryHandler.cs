using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebShop.Application.WebShopModules.Messages.Queries.List
{
    public class ListMessagesQueryHandler(IAppDbContext context, IAppCurrentUser currentUser)
    : IRequestHandler<ListMessagesQuery, PageResult<ListMessagesQueryDto>>
    {
        public async Task<PageResult<ListMessagesQueryDto>> Handle(ListMessagesQuery request, CancellationToken ct)
        {
            var q = context.Messages.AsNoTracking();

            // a user can only see their own messages unless they are an admin
            if (!currentUser.IsAdmin)
                q = q.Where(m => m.SenderId == currentUser.UserId || m.ReceiverId == currentUser.UserId);

            if (!string.IsNullOrWhiteSpace(request.Search))
                q = q.Where(m => m.Content.Contains(request.Search));

            if (request.ReceiverId is not null)
                q = q.Where(m => m.ReceiverId == request.ReceiverId);

            if (request.SenderId is not null)
                q = q.Where(m => m.SenderId == request.SenderId);

            if (request.IsRead is not null)
                q = q.Where(m => m.IsRead == request.IsRead);

            var projectedQuery = q.OrderByDescending(m => m.RecivedDate)
                .Select(m => new ListMessagesQueryDto
                {
                    Id = m.Id,
                    Content = m.Content,
                    IsRead = m.IsRead,
                    RecivedDate = m.RecivedDate,
                    Sender = new ListMessagesQueryUserDto
                    {
                        Id = m.Sender.Id,
                        Name = m.Sender.Name,
                        Surname = m.Sender.Surname
                    },
                    Receiver = new ListMessagesQueryUserDto
                    {
                        Id = m.Receiver.Id,
                        Name = m.Receiver.Name,
                        Surname = m.Receiver.Surname
                    }
                });

            return await PageResult<ListMessagesQueryDto>.FromQueryableAsync(projectedQuery, request.Paging, ct);
        }
    }
}
