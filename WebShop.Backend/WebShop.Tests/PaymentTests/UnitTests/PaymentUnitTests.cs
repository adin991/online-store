using Microsoft.Extensions.Time.Testing;
using WebShop.Application.WebShopModules.Payment.Commands.ConfirmPayment;
using WebShop.Application.WebShopModules.Payment.Commands.CreateCartPaymentIntent;
using WebShop.Application.WebShopModules.Payment.Commands.CreatePaymentIntent;

namespace WebShop.Tests.PaymentTests.UnitTests;

public class PaymentUnitTests
{
    private static ProductCategoryEntity SeedCategory(DatabaseContext context)
    {
        var category = new ProductCategoryEntity { Name = "Category", IsEnabled = true };
        context.ProductCategories.Add(category);
        context.SaveChanges();
        return category;
    }

    private static Ad SeedAd(
        DatabaseContext context,
        ProductCategoryEntity category,
        decimal price,
        AdStatusType status = AdStatusType.Live,
        int ownerUserId = 1)
    {
        var ad = new Ad
        {
            Title = "Ad",
            Description = "Description",
            Price = price,
            CategoryEntity = category,
            UserId = ownerUserId,
            AdStatuses = new List<AdStatus> { new() { Status = status } }
        };
        context.Ads.Add(ad);
        context.SaveChanges();
        return ad;
    }

    // ---------- CreateCartPaymentIntentCommandHandler ----------

    [Fact]
    public async Task CreateCartIntent_Handle_ShouldCreatePendingTransactions_WhenCartIsValid()
    {
        using var context = InMemoryDbContextFactory.Create();
        var category = SeedCategory(context);
        var ad = SeedAd(context, category, price: 100m);
        var currentUser = new FakeCurrentUser { IsAuthenticated = true, UserId = 42 };
        var gateway = new FakeStripePaymentGateway();
        var handler = new CreateCartPaymentIntentCommandHandler(context, currentUser, gateway);
        var command = new CreateCartPaymentIntentCommand
        {
            Items = new List<CartItemDto> { new() { AdId = ad.Id, Quantity = 2 } }
        };

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.Single(result.TransactionIds);
        Assert.Equal(gateway.ClientSecretToReturn, result.ClientSecret);
        var transaction = await context.Transactions.FindAsync(result.TransactionIds[0]);
        Assert.NotNull(transaction);
        Assert.Equal(TransactionStatus.Pending, transaction!.Status);
        Assert.Equal(200m, transaction.AmountOriginalKm);
        Assert.Equal(42, transaction.UserId);
    }

    [Fact]
    public async Task CreateCartIntent_Handle_ShouldThrowBusinessRule_WhenUserNotAuthenticated()
    {
        using var context = InMemoryDbContextFactory.Create();
        var currentUser = new FakeCurrentUser { IsAuthenticated = false };
        var handler = new CreateCartPaymentIntentCommandHandler(context, currentUser, new FakeStripePaymentGateway());
        var command = new CreateCartPaymentIntentCommand { Items = new List<CartItemDto>() };

        await Assert.ThrowsAsync<WebShopBusinessRuleException>(
            () => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task CreateCartIntent_Handle_ShouldThrowBusinessRule_WhenCartIsEmpty()
    {
        using var context = InMemoryDbContextFactory.Create();
        var currentUser = new FakeCurrentUser { IsAuthenticated = true, UserId = 1 };
        var handler = new CreateCartPaymentIntentCommandHandler(context, currentUser, new FakeStripePaymentGateway());
        var command = new CreateCartPaymentIntentCommand { Items = new List<CartItemDto>() };

        await Assert.ThrowsAsync<WebShopBusinessRuleException>(
            () => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task CreateCartIntent_Handle_ShouldThrowNotFound_WhenAdDoesNotExist()
    {
        using var context = InMemoryDbContextFactory.Create();
        var currentUser = new FakeCurrentUser { IsAuthenticated = true, UserId = 1 };
        var handler = new CreateCartPaymentIntentCommandHandler(context, currentUser, new FakeStripePaymentGateway());
        var command = new CreateCartPaymentIntentCommand
        {
            Items = new List<CartItemDto> { new() { AdId = 987654, Quantity = 1 } }
        };

        await Assert.ThrowsAsync<WebShopNotFoundException>(
            () => handler.Handle(command, CancellationToken.None));
    }

    // ---------- ConfirmPaymentCommandHandler ----------

    private static Transaction SeedTransaction(DatabaseContext context, int userId, string paymentIntentId, TransactionStatus status = TransactionStatus.Pending)
    {
        var transaction = new Transaction
        {
            AdId = 0,
            UserId = userId,
            AmountEur = 50m,
            AmountOriginalKm = 100m,
            StripePaymentIntentId = paymentIntentId,
            Status = status
        };
        context.Transactions.Add(transaction);
        context.SaveChanges();
        return transaction;
    }

    [Fact]
    public async Task Confirm_Handle_ShouldMarkTransactionSucceeded_WhenStripeReportsSucceeded()
    {
        using var context = InMemoryDbContextFactory.Create();
        var transaction = SeedTransaction(context, userId: 10, paymentIntentId: "pi_1");
        var currentUser = new FakeCurrentUser { IsAuthenticated = true, UserId = 10 };
        var gateway = new FakeStripePaymentGateway { StatusToReturn = "succeeded" };
        var handler = new ConfirmPaymentCommandHandler(context, currentUser, gateway, new FakeTimeProvider());

        var result = await handler.Handle(
            new ConfirmPaymentCommand { TransactionIds = new List<int> { transaction.Id } },
            CancellationToken.None);

        Assert.True(result.Success);
        var updated = await context.Transactions.FindAsync(transaction.Id);
        Assert.Equal(TransactionStatus.Succeeded, updated!.Status);
        Assert.NotNull(updated.PaidAtUtc);
    }

    [Fact]
    public async Task Confirm_Handle_ShouldThrowBusinessRule_WhenTransactionBelongsToAnotherUser()
    {
        using var context = InMemoryDbContextFactory.Create();
        var transaction = SeedTransaction(context, userId: 10, paymentIntentId: "pi_2");
        var currentUser = new FakeCurrentUser { IsAuthenticated = true, UserId = 999 };
        var handler = new ConfirmPaymentCommandHandler(context, currentUser, new FakeStripePaymentGateway(), new FakeTimeProvider());

        await Assert.ThrowsAsync<WebShopBusinessRuleException>(
            () => handler.Handle(
                new ConfirmPaymentCommand { TransactionIds = new List<int> { transaction.Id } },
                CancellationToken.None));
    }

    [Fact]
    public async Task Confirm_Handle_ShouldThrowBusinessRule_WhenTransactionIdsEmpty()
    {
        using var context = InMemoryDbContextFactory.Create();
        var currentUser = new FakeCurrentUser { IsAuthenticated = true, UserId = 1 };
        var handler = new ConfirmPaymentCommandHandler(context, currentUser, new FakeStripePaymentGateway(), new FakeTimeProvider());

        await Assert.ThrowsAsync<WebShopBusinessRuleException>(
            () => handler.Handle(
                new ConfirmPaymentCommand { TransactionIds = new List<int>() },
                CancellationToken.None));
    }

    // ---------- Ad availability rules (list / payment lifecycle) ----------

    [Fact]
    public async Task CreateCartIntent_Handle_ShouldThrowBusinessRule_WhenBuyingOwnAd()
    {
        using var context = InMemoryDbContextFactory.Create();
        var category = SeedCategory(context);
        var ad = SeedAd(context, category, price: 100m, ownerUserId: 42);
        var currentUser = new FakeCurrentUser { IsAuthenticated = true, UserId = 42 };
        var handler = new CreateCartPaymentIntentCommandHandler(context, currentUser, new FakeStripePaymentGateway());
        var command = new CreateCartPaymentIntentCommand
        {
            Items = new List<CartItemDto> { new() { AdId = ad.Id, Quantity = 1 } }
        };

        await Assert.ThrowsAsync<WebShopBusinessRuleException>(
            () => handler.Handle(command, CancellationToken.None));
    }

    [Theory]
    [InlineData(AdStatusType.Closed)]
    [InlineData(AdStatusType.Removed)]
    [InlineData(AdStatusType.AwaitingReview)]
    public async Task CreateIntent_Handle_ShouldThrowBusinessRule_WhenAdIsNotAvailable(AdStatusType status)
    {
        using var context = InMemoryDbContextFactory.Create();
        var category = SeedCategory(context);
        var ad = SeedAd(context, category, price: 100m, status: status);
        var currentUser = new FakeCurrentUser { IsAuthenticated = true, UserId = 42 };
        var handler = new CreatePaymentIntentCommandHandler(context, currentUser, new FakeStripePaymentGateway());

        await Assert.ThrowsAsync<WebShopBusinessRuleException>(
            () => handler.Handle(new CreatePaymentIntentCommand { AdId = ad.Id }, CancellationToken.None));
    }

    [Fact]
    public async Task Confirm_Handle_ShouldCloseAd_WhenPaymentSucceeded()
    {
        using var context = InMemoryDbContextFactory.Create();
        var category = SeedCategory(context);
        var ad = SeedAd(context, category, price: 100m);
        var transaction = new Transaction
        {
            AdId = ad.Id,
            UserId = 10,
            AmountEur = 51.13m,
            AmountOriginalKm = 100m,
            StripePaymentIntentId = "pi_sold",
            Status = TransactionStatus.Pending
        };
        context.Transactions.Add(transaction);
        context.SaveChanges();

        var currentUser = new FakeCurrentUser { IsAuthenticated = true, UserId = 10 };
        var handler = new ConfirmPaymentCommandHandler(
            context, currentUser, new FakeStripePaymentGateway { StatusToReturn = "succeeded" }, new FakeTimeProvider());

        await handler.Handle(new ConfirmPaymentCommand { TransactionIds = new List<int> { transaction.Id } }, CancellationToken.None);

        var statuses = await context.AdStatuss.Where(s => s.AdId == ad.Id).ToListAsync();
        Assert.Single(statuses);
        Assert.Equal(AdStatusType.Closed, statuses[0].Status);

        // A sold ad cannot be bought again.
        var buyAgain = new CreatePaymentIntentCommandHandler(
            context, new FakeCurrentUser { IsAuthenticated = true, UserId = 11 }, new FakeStripePaymentGateway());
        await Assert.ThrowsAsync<WebShopBusinessRuleException>(
            () => buyAgain.Handle(new CreatePaymentIntentCommand { AdId = ad.Id }, CancellationToken.None));
    }
}
