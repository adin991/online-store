namespace WebShop.Application.WebShopModules.Comments.Commands.Create
{
    public class CreateCommentCommandHandler(IAppDbContext context, IAppCurrentUser currentUser)
    : IRequestHandler<CreateCommentCommand, int>
    {
        public async Task<int> Handle(CreateCommentCommand request, CancellationToken ct)
        {
            if (!currentUser.IsAuthenticated || currentUser.UserId is null)
                throw new WebShopBusinessRuleException("401", "User must be logged in.");

            var adExists = await context.Ads.AnyAsync(a => a.Id == request.AdId, ct);
            if (!adExists)
                throw new WebShopNotFoundException("Ad with this ID doesn't exist");

            var comment = new WebShop.Domain.Entities.WebShopEntities.Comment
            {
                Content = request.Content.Trim(),
                AdId = request.AdId,
                UserId = currentUser.UserId.Value // currently logged-in user
            };

            await context.Comments.AddAsync(comment, ct);
            await context.SaveChangesAsync(ct);
            return comment.Id;
        }
    }
}
