using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebShop.Application.WebShopModules.Comment.Queries.GetById
{
    public class GetCommentByIdQueryHandler(IAppDbContext context)
    : IRequestHandler<GetCommentByIdQuery, GetCommentByIdQueryDto>
    {
        public async Task<GetCommentByIdQueryDto> Handle(GetCommentByIdQuery request, CancellationToken ct)
        {
            var comment = await context.Comments
                .AsNoTracking()
                .Where(c => c.Id == request.Id)
                .Select(c => new GetCommentByIdQueryDto
                {
                    Id = c.Id,
                    Content = c.Content,
                    AdId = c.AdId,
                    UserId = c.UserId,
                    User = new GetCommentByIdUserDto
                    {
                        Name = c.User.Name,
                        Surname = c.User.Surname
                    }
                })
                .FirstOrDefaultAsync(ct);

            if (comment is null)
                throw new WebShopNotFoundException("Comment with this ID doesn't exist");

            return comment;
        }
    }
}
