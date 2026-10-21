namespace WebShop.Application.WebShopModules.Comment.Commands.Update;

public class UpdateCommentCommand : IRequest<Unit>
{
    [JsonIgnore]
    public int Id { get; set; }
    public required string Content { get; set; }
}