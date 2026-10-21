using WebShop.Application.Abstractions;
using WebShop.Domain.Entities.WebShopEntities;

namespace WebShop.Infrastructure.Database;

public partial class DatabaseContext : DbContext, IAppDbContext
{
    public DbSet<ProductCategoryEntity> ProductCategories => Set<ProductCategoryEntity>();
    public DbSet<ProductEntity> Products => Set<ProductEntity>();
    public DbSet<UserEntity> Users => Set<UserEntity>();
    public DbSet<RefreshTokenEntity> RefreshTokens => Set<RefreshTokenEntity>();
    public DbSet<ShoppingCart> ShoppingCarts => Set<ShoppingCart>();
    public DbSet<SearchHistory> SearchHistorys => Set<SearchHistory>();
    public DbSet<SCItem> SCItems => Set<SCItem>();
    public DbSet<Role> Role => Set<Role>();
    public DbSet<Refund> Refunds => Set<Refund>();
    public DbSet<PaymentInfo> PaymentInfos => Set<PaymentInfo>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<DeliveryStatus> DeliveryStatuss => Set<DeliveryStatus>();
    public DbSet<DeliveryCompany> DeliveryCompanys => Set<DeliveryCompany>();
    public DbSet<Delivery> Deliverys => Set<Delivery>();
    public DbSet<CreditCard> CreditCards => Set<CreditCard>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<AdStatus> AdStatuss => Set<AdStatus>();
    public DbSet<Ad> Ads => Set<Ad>();
    public DbSet<ActivityHistory> ActivityHistorys => Set<ActivityHistory>();
    public DbSet<Transaction> Transactions => Set<Transaction>();

    private readonly TimeProvider _clock;
    public DatabaseContext(DbContextOptions<DatabaseContext> options, TimeProvider clock) : base(options)
    {
        _clock = clock;
    }
    

}