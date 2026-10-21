using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebShop.Application.WebShopModules.Comment.Queries.List
{
    public class ListCommentsQueryHandler(IAppDbContext context)
    : IRequestHandler<ListCommentsQuery, PageResult<ListCommentsQueryDto>>
    {
        public async Task<PageResult<ListCommentsQueryDto>> Handle(ListCommentsQuery request, CancellationToken ct)
        {
            var q = context.Comments.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
                q = q.Where(c => c.Content.Contains(request.Search));

            if (request.AdId is not null)
                q = q.Where(c => c.AdId == request.AdId);

            var projectedQuery = q.OrderBy(c => c.CreatedAtUtc)
                .Select(c => new ListCommentsQueryDto
                {
                    Id = c.Id,
                    Content = c.Content,
                    AdId = c.AdId,
                    UserId = c.UserId,
                    CreatedAtUtc = c.CreatedAtUtc,
                    User = new ListCommentsQueryUserDto
                    {
                        Name = c.User.Name,
                        Surname = c.User.Surname
                    }
                });

            return await PageResult<ListCommentsQueryDto>.FromQueryableAsync(projectedQuery, request.Paging, ct);
        }
    }
}
