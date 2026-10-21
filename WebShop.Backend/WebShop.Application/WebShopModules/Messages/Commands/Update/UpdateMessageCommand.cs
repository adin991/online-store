namespace WebShop.Application.WebShopModules.Messages.Commands.Update;

public class UpdateMessageCommand : IRequest<Unit>
{
    [JsonIgnore]
    public int Id { get; set; }
    public required string Content { get; set; }
    public bool? IsRead { get; set; }
}