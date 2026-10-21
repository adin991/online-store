using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebShop.Application.WebShopModules.Comments.Commands.Delete
{
    public class DeleteCommentCommandHandler(IAppDbContext context, IAppCurrentUser currentUser)
    : IRequestHandler<DeleteCommentCommand, Unit>
    {
        public async Task<Unit> Handle(DeleteCommentCommand request, CancellationToken ct)
        {
            var comment = await context.Comments.FirstOrDefaultAsync(c => c.Id == request.Id, ct);
            if (comment is null)
                throw new WebShopNotFoundException("Comment with this ID doesn't exist");

            if (!currentUser.IsAdmin && currentUser.UserId != comment.UserId)
                throw new WebShopBusinessRuleException("403", "You cannot delete this comment");

            context.Comments.Remove(comment);
            await context.SaveChangesAsync(ct);
            return Unit.Value;
        }
    }
}
