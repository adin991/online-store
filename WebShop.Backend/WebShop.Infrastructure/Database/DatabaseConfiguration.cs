using System.Linq.Expressions;
using System.Runtime.Intrinsics.X86;
using WebShop.Domain.Common;
using WebShop.Domain.Entities.WebShopEntities;
using WebShop.Infrastructure.Database.Seeders;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
namespace WebShop.Infrastructure.Database;


public partial class DatabaseContext
{
    private DateTime UtcNow => _clock.GetUtcNow().UtcDateTime;

    private void ApplyAuditAndSoftDelete()
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAtUtc = UtcNow;
                    entry.Entity.ModifiedAtUtc = null; // ili = UtcNow
                    entry.Entity.IsDeleted = false;
                    break;

                case EntityState.Modified:
                    entry.Entity.ModifiedAtUtc = UtcNow;
                    break;

                case EntityState.Deleted:
                    // soft-delete: set is Modified and IsDeleted
                    entry.State = EntityState.Modified;
                    entry.Entity.IsDeleted = true;
                    entry.Entity.ModifiedAtUtc = UtcNow;
                    break;
            }
        }
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<decimal>().HavePrecision(18, 2);
        configurationBuilder.Properties<decimal?>().HavePrecision(18, 2);
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        //bugfix 27.10.2025.nakon nastave - učitaj sve konfiguracije iz Infrastructure.Database.Configurations
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DatabaseContext).Assembly);

        ApplyGlobalFielters(modelBuilder);

        StaticDataSeeder.Seed(modelBuilder); // static data

        // Messages
        modelBuilder.Entity<Message>()
            .HasOne(m => m.Sender)
            .WithMany(u => u.SentMessages)
            .HasForeignKey(m => m.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Message>()
            .HasOne(m => m.Receiver)
            .WithMany(u => u.ReceivedMessages)
            .HasForeignKey(m => m.ReceiverId)
            .OnDelete(DeleteBehavior.Restrict);

        // Comments
        modelBuilder.Entity<Comment>()
            .HasOne(c => c.User)
            .WithMany(u => u.Comments)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Comment>()
            .HasOne(c => c.Ad)
            .WithMany(a => a.Comments)
            .HasForeignKey(c => c.AdId)
            .OnDelete(DeleteBehavior.Cascade);

        //Ads
        modelBuilder.Entity<Ad>()
            .HasOne(a => a.User)
            .WithMany(u => u.Ads)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Refunds
        modelBuilder.Entity<Refund>()
            .HasOne(r => r.User)
            .WithMany(u => u.Refunds)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Refund>()
            .HasOne(r => r.PaymentInfo)
            .WithMany()
            .HasForeignKey(r => r.PaymentInfoId)
            .OnDelete(DeleteBehavior.Restrict);

        // CreditCards
        modelBuilder.Entity<CreditCard>()
            .HasOne(c => c.User)
            .WithMany(u => u.CreditCards)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // ShoppingCarts
        modelBuilder.Entity<ShoppingCart>()
            .HasOne(sc => sc.User)
            .WithMany(u => u.Carts)
            .HasForeignKey(sc => sc.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // SCItem
        modelBuilder.Entity<SCItem>()
            .HasOne(i => i.ShoppingCart)
            .WithMany(sc => sc.Items)
            .HasForeignKey(i => i.ShoppingCartId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<SCItem>()
            .HasOne(i => i.Ad)
            .WithMany(a => a.SCItems)
            .HasForeignKey(i => i.AdId)
            .OnDelete(DeleteBehavior.Restrict);

        // Delivery
        modelBuilder.Entity<Delivery>()
            .HasOne(d => d.PaymentInfo)
            .WithMany()
            .HasForeignKey(d => d.PaymentInfoId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Delivery>()
            .HasOne(d => d.DeliveryCompany)
            .WithMany(dc => dc.Deliveries)
            .HasForeignKey(d => d.DeliveryCompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        // Transactions
        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.Ad)
            .WithMany()
            .HasForeignKey(t => t.AdId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.User)
            .WithMany()
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Restrict);

    }


    private void ApplyGlobalFielters(ModelBuilder modelBuilder)
    {
        // Apply a global filter to all entities inheriting from BaseEntity
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var prop = Expression.Property(parameter, nameof(BaseEntity.IsDeleted));
                var compare = Expression.Equal(prop, Expression.Constant(false));
                var lambda = Expression.Lambda(compare, parameter);

                modelBuilder.Entity(entityType.ClrType)
                            .HasQueryFilter(lambda);
            }
        }
    }

    public override int SaveChanges()
    {
        ApplyAuditAndSoftDelete();

        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        ApplyAuditAndSoftDelete();

        return base.SaveChangesAsync(cancellationToken);
    }
}