using CommentConstraints = WebShop.Domain.Entities.WebShopEntities.Comment.Constraints;

namespace WebShop.Application.WebShopModules.Comment.Commands.Update;

public sealed class UpdateCommentCommandValidator : AbstractValidator<UpdateCommentCommand>
{
    public UpdateCommentCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("Comment id must be greater than 0.");

        RuleFor(x => x.Content)
            .Must(c => !string.IsNullOrWhiteSpace(c)).WithMessage("Comment cannot be empty.")
            .MaximumLength(CommentConstraints.ContentMaxLength)
                .WithMessage($"Comment can be at most {CommentConstraints.ContentMaxLength} characters long.");
    }
}
