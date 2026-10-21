namespace WebShop.Application.WebShopModules.Notifications.Commands.Update;

public class UpdateNotificationsCommand : IRequest<Unit>
{
    [JsonIgnore]
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
    public bool? IsRead { get; set; }
}