using WebShop.Application.WebShopModules.Ad.Commands.Create;
using WebShop.Application.WebShopModules.Ad.Commands.Delete;
using WebShop.Application.WebShopModules.Ad.Commands.Update;
using WebShop.Application.WebShopModules.Ad.Commands.Status;
using WebShop.Application.WebShopModules.Ad.Queries.List;

namespace WebShop.Tests.AdTests.UnitTests;

public class AdUnitTests
{
    private static ProductCategoryEntity SeedCategory(DatabaseContext context, string name = "Electronics")
    {
        var category = new ProductCategoryEntity { Name = name, IsEnabled = true };
        context.ProductCategories.Add(category);
        context.SaveChanges();
        return category;
    }

    private static Ad SeedAd(
        DatabaseContext context,
        int ownerUserId,
        ProductCategoryEntity category,
        AdStatusType status = AdStatusType.Live)
    {
        var ad = new Ad
        {
            Title = "Existing ad",
            Description = "Description",
            Price = 100m,
            CategoryEntity = category,
            UserId = ownerUserId,
            AdStatuses = new List<AdStatus> { new() { Status = status } }
        };
        context.Ads.Add(ad);
        context.SaveChanges();
        return ad;
    }

    // ---------- CreateAdCommandHandler ----------

    [Fact]
    public async Task Create_Handle_ShouldCreateAd_WhenUserIsAuthenticated()
    {
        using var context = InMemoryDbContextFactory.Create();
        var category = SeedCategory(context);
        var currentUser = new FakeCurrentUser { IsAuthenticated = true, UserId = 5 };
        var handler = new CreateAdCommandHandler(context, currentUser);
        var command = new CreateAdCommand
        {
            Title = "New ad",
            Description = "Ad description",
            Price = 250m,
            CategoryId = category.Id
        };

        var adId = await handler.Handle(command, CancellationToken.None);

        var ad = await context.Ads.FindAsync(adId);
        Assert.NotNull(ad);
        Assert.Equal(5, ad!.UserId);
        Assert.Single(ad.AdStatuses);
        Assert.Equal(AdStatusType.Live, ad.AdStatuses[0].Status);
    }

    [Fact]
    public async Task Create_Handle_ShouldThrowUnauthorized_WhenUserNotAuthenticated()
    {
        using var context = InMemoryDbContextFactory.Create();
        var category = SeedCategory(context);
        var currentUser = new FakeCurrentUser { IsAuthenticated = false };
        var handler = new CreateAdCommandHandler(context, currentUser);
        var command = new CreateAdCommand
        {
            Title = "Ad",
            Description = "Description",
            Price = 10m,
            CategoryId = category.Id
        };

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Create_Handle_ShouldThrowValidation_WhenCategoryDoesNotExist()
    {
        using var context = InMemoryDbContextFactory.Create();
        var currentUser = new FakeCurrentUser { IsAuthenticated = true, UserId = 1 };
        var handler = new CreateAdCommandHandler(context, currentUser);
        var command = new CreateAdCommand
        {
            Title = "Ad",
            Description = "Description",
            Price = 10m,
            CategoryId = 12345
        };

        await Assert.ThrowsAsync<ValidationException>(
            () => handler.Handle(command, CancellationToken.None));
    }

    // ---------- UpdateAdCommandHandler ----------

    [Fact]
    public async Task Update_Handle_ShouldUpdateAd_WhenOwnerUpdatesOwnAd()
    {
        using var context = InMemoryDbContextFactory.Create();
        var category = SeedCategory(context);
        var ad = SeedAd(context, ownerUserId: 7, category);
        var currentUser = new FakeCurrentUser { UserId = 7, IsAdmin = false };
        var handler = new UpdateAdCommandHandler(context, currentUser);
        var command = new UpdateAdCommand
        {
            Id = ad.Id,
            Title = "Updated title",
            Description = "Updated description",
            Price = 150m,
            CategoryId = category.Id
        };

        await handler.Handle(command, CancellationToken.None);

        var updated = await context.Ads.FindAsync(ad.Id);
        Assert.Equal("Updated title", updated!.Title);
        Assert.Equal(150m, updated.Price);
    }

    [Fact]
    public async Task Update_Handle_ShouldThrowBusinessRule_WhenNonOwnerNonAdminUpdatesAd()
    {
        using var context = InMemoryDbContextFactory.Create();
        var category = SeedCategory(context);
        var ad = SeedAd(context, ownerUserId: 7, category);
        var currentUser = new FakeCurrentUser { UserId = 999, IsAdmin = false };
        var handler = new UpdateAdCommandHandler(context, currentUser);
        var command = new UpdateAdCommand
        {
            Id = ad.Id,
            Title = "Editing someone else's ad",
            Description = "Description",
            Price = 1m,
            CategoryId = category.Id
        };

        await Assert.ThrowsAsync<WebShopBusinessRuleException>(
            () => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Update_Handle_ShouldThrowNotFound_WhenAdDoesNotExist()
    {
        using var context = InMemoryDbContextFactory.Create();
        var category = SeedCategory(context);
        var currentUser = new FakeCurrentUser { UserId = 1, IsAdmin = true };
        var handler = new UpdateAdCommandHandler(context, currentUser);
        var command = new UpdateAdCommand
        {
            Id = 999999,
            Title = "Does not exist",
            Description = "Description",
            Price = 1m,
            CategoryId = category.Id
        };

        await Assert.ThrowsAsync<WebShopNotFoundException>(
            () => handler.Handle(command, CancellationToken.None));
    }

    // ---------- DeleteAdCommandHandler ----------

    [Fact]
    public async Task Delete_Handle_ShouldDeleteAd_WhenAdminDeletesAnyAd()
    {
        using var context = InMemoryDbContextFactory.Create();
        var category = SeedCategory(context);
        var ad = SeedAd(context, ownerUserId: 7, category);
        var admin = new FakeCurrentUser { UserId = 1, IsAdmin = true };
        var handler = new DeleteProductCommandHandler(context, admin);

        await handler.Handle(new DeleteAdCommand { Id = ad.Id }, CancellationToken.None);

        // DatabaseContext does a soft delete (SaveChanges turns Remove into IsDeleted = true),
        // so the row stays in the database, only marked as deleted.
        var deleted = await context.Ads.FindAsync(ad.Id);
        Assert.NotNull(deleted);
        Assert.True(deleted!.IsDeleted);
    }

    [Fact]
    public async Task Delete_Handle_ShouldThrowBusinessRule_WhenNonOwnerNonAdminDeletesAd()
    {
        using var context = InMemoryDbContextFactory.Create();
        var category = SeedCategory(context);
        var ad = SeedAd(context, ownerUserId: 7, category);
        var currentUser = new FakeCurrentUser { UserId = 999, IsAdmin = false };
        var handler = new DeleteProductCommandHandler(context, currentUser);

        await Assert.ThrowsAsync<WebShopBusinessRuleException>(
            () => handler.Handle(new DeleteAdCommand { Id = ad.Id }, CancellationToken.None));
    }

    // ---------- UpdateStatusThruAdCommandHandler ----------

    [Fact]
    public async Task ChangeStatus_Handle_ShouldThrowBusinessRule_WhenNonOwnerNonAdmin()
    {
        using var context = InMemoryDbContextFactory.Create();
        var ad = SeedAd(context, ownerUserId: 7, SeedCategory(context));
        var handler = new UpdateStatusThruAdCommandHandler(context, new FakeCurrentUser { IsAuthenticated = true, UserId = 999 });

        await Assert.ThrowsAsync<WebShopBusinessRuleException>(() => handler.Handle(
            new UpdateStatusThruAdCommand { Id = ad.Id, AdStatusType = AdStatusType.Closed }, CancellationToken.None));
    }

    [Fact]
    public async Task ChangeStatus_Handle_ShouldChangePrimaryStatus_AndKeepBoosted_WhenOwner()
    {
        using var context = InMemoryDbContextFactory.Create();
        var ad = SeedAd(context, ownerUserId: 7, SeedCategory(context));
        var owner = new FakeCurrentUser { IsAuthenticated = true, UserId = 7 };
        var handler = new UpdateStatusThruAdCommandHandler(context, owner);

        await handler.Handle(new UpdateStatusThruAdCommand { Id = ad.Id, AdStatusType = AdStatusType.Boosted }, CancellationToken.None);
        await handler.Handle(new UpdateStatusThruAdCommand { Id = ad.Id, AdStatusType = AdStatusType.Closed }, CancellationToken.None);

        var statuses = await context.AdStatuss.Where(s => s.AdId == ad.Id).Select(s => s.Status).ToListAsync();
        Assert.Equal(2, statuses.Count);
        Assert.Contains(AdStatusType.Boosted, statuses);
        Assert.Contains(AdStatusType.Closed, statuses);
    }

    [Fact]
    public async Task ChangeStatus_Handle_ShouldThrowBusinessRule_WhenOwnerSetsModerationStatus()
    {
        using var context = InMemoryDbContextFactory.Create();
        var ad = SeedAd(context, ownerUserId: 7, SeedCategory(context));
        var handler = new UpdateStatusThruAdCommandHandler(context, new FakeCurrentUser { IsAuthenticated = true, UserId = 7 });

        await Assert.ThrowsAsync<WebShopBusinessRuleException>(() => handler.Handle(
            new UpdateStatusThruAdCommand { Id = ad.Id, AdStatusType = AdStatusType.Removed }, CancellationToken.None));
    }

    // ---------- ListAdsQueryHandler ----------

    [Fact]
    public async Task List_Handle_ShouldReturnOnlyAvailableAds_ForPublicListing()
    {
        using var context = InMemoryDbContextFactory.Create();
        var category = SeedCategory(context);
        var owner = new UserEntity { Name = "Owner", Surname = "One", Email = "owner@example.com", PasswordHash = "x" };
        context.Users.Add(owner);
        context.SaveChanges();
        var live = SeedAd(context, owner.Id, category, AdStatusType.Live);
        SeedAd(context, owner.Id, category, AdStatusType.Closed);
        SeedAd(context, owner.Id, category, AdStatusType.Removed);

        var handler = new ListAdsQueryHandler(context, new FakeCurrentUser());
        var publicResult = await handler.Handle(new ListAdsQuery(), CancellationToken.None);

        Assert.Equal(1, publicResult.Total);
        Assert.Equal(live.Id, publicResult.Items[0].Id);

        // The owner's dashboard ("mine") shows all of their ads, whatever the status.
        var mineHandler = new ListAdsQueryHandler(context, new FakeCurrentUser { IsAuthenticated = true, UserId = owner.Id });
        var mine = await mineHandler.Handle(new ListAdsQuery { Mine = true }, CancellationToken.None);
        Assert.Equal(3, mine.Total);
    }
}
