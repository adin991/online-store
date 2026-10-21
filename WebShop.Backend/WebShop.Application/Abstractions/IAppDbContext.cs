using WebShop.Domain.Entities.WebShopEntities;

namespace WebShop.Application.Abstractions;

// Application layer
public interface IAppDbContext
{
    DbSet<ProductEntity> Products { get; }
    DbSet<ProductCategoryEntity> ProductCategories { get; }
    DbSet<UserEntity> Users { get; }
    DbSet<RefreshTokenEntity> RefreshTokens { get; }
    DbSet<ShoppingCart> ShoppingCarts { get; }
    DbSet<SearchHistory> SearchHistorys { get; }
    DbSet<SCItem> SCItems { get; }
    DbSet<Role> Role { get; }
    DbSet<Refund> Refunds { get; }
    DbSet<PaymentInfo> PaymentInfos { get; }
    DbSet<Notification> Notifications { get; }
    DbSet<Message> Messages { get; }
    DbSet<DeliveryStatus> DeliveryStatuss { get; }
    DbSet<DeliveryCompany> DeliveryCompanys { get; }
    DbSet<Delivery> Deliverys { get; }
    DbSet<CreditCard> CreditCards { get; }
    DbSet<Comment> Comments { get; }
    DbSet<AdStatus> AdStatuss { get; }
    DbSet<Ad> Ads { get; }
    DbSet<ActivityHistory> ActivityHistorys { get; }
    DbSet<Transaction> Transactions { get; }
    Task<int> SaveChangesAsync(CancellationToken ct);
}