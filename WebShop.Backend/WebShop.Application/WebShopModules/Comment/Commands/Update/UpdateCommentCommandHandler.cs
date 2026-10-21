namespace WebShop.Application.WebShopModules.Comment.Commands.Update;

public class UpdateCommentCommandHandler(IAppDbContext db, IAppCurrentUser currentUser)
    : IRequestHandler<UpdateCommentCommand, Unit>
{
    public async Task<Unit> Handle(UpdateCommentCommand request, CancellationToken ct)
    {
        var comment = await db.Comments.FirstOrDefaultAsync(x => x.Id == request.Id, ct);
        if (comment is null)
            throw new WebShopNotFoundException("Comment not found");

        // Only the comment owner or an admin can update the comment
        if (!currentUser.IsAdmin && currentUser.UserId != comment.UserId)
            throw new WebShopBusinessRuleException("403", "You cannot update this comment");

        comment.Content = request.Content.Trim();

        await db.SaveChangesAsync(ct);
        return Unit.Value;
    }
}