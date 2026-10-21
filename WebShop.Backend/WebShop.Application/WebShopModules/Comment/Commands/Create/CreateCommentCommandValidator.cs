using CommentConstraints = WebShop.Domain.Entities.WebShopEntities.Comment.Constraints;

namespace WebShop.Application.WebShopModules.Comments.Commands.Create;

public sealed class CreateCommentCommandValidator : AbstractValidator<CreateCommentCommand>
{
    public CreateCommentCommandValidator()
    {
        RuleFor(x => x.AdId).GreaterThan(0).WithMessage("Ad id must be greater than 0.");

        RuleFor(x => x.Content)
            .Must(c => !string.IsNullOrWhiteSpace(c)).WithMessage("Comment cannot be empty.")
            .MaximumLength(CommentConstraints.ContentMaxLength)
                .WithMessage($"Comment can be at most {CommentConstraints.ContentMaxLength} characters long.");
    }
}
