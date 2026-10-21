// User.cs
using WebShop.Domain.Common;
using WebShop.Domain.Entities.WebShopEntities;

namespace WebShop.Domain.Entities.Identity;

public enum Gender
{
     Male,
     Female
}

public sealed class UserEntity : BaseEntity
{
    public required string Name { get; set; } = string.Empty;
    public required string Surname { get; set; } = string.Empty;
    public string Email { get; set; }
    public Gender Gender { get; set; }
    public string PasswordHash { get; set; }
    public int RoleId { get; set; } = 1; // explicit foreign key for Role, so EF Core does not create a shadow RoleId
    public Role Role { get; set; } = null!; // many-to-one: every user has one role
    public bool IsAdmin { get; set; } = false;
    public bool IsBasic { get; set; } = true;
    public int TokenVersion { get; set; } = 0;// For global revocation
    public bool IsEnabled { get; set; } = true; // For soft deletion and account disabling
    public ICollection<RefreshTokenEntity> RefreshTokens { get; private set; } = new List<RefreshTokenEntity>();
    public string? Pfp { get; set; }//profile image of the user stored as a url to said image on a cloud service (will be turned in to byte[] if we skip file hosting)
    public List<Ad> Ads { get; set; } = new();
    public List<Message> SentMessages { get; set; } = new();
    public List<Message> ReceivedMessages { get; set; } = new();
    public List<ActivityHistory> Activities { get; set; } = new();
    public List<ShoppingCart> Carts { get; set; } = new();
    public List<Comment> Comments { get; set; } = new();
    public List<CreditCard> CreditCards { get; set; } = new();
    public List<Delivery> Deliveries { get; set; } = new();
    public List<Notification> Notifications { get; set; } = new();
    public List<Refund> Refunds { get; set; } = new();
    public List<SearchHistory> Searches { get; set; } = new();
}