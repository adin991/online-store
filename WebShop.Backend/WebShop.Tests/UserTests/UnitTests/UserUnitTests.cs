using WebShop.Application.WebShopModules.User.Commands.Create;
using WebShop.Application.WebShopModules.User.Commands.Delete;
using WebShop.Application.WebShopModules.User.Commands.Update;
using WebShop.Application.WebShopModules.User.Queries.GetById;
using WebShop.Application.WebShopModules.User.Queries.List;

namespace WebShop.Tests.UserTests.UnitTests;

public class UserUnitTests
{
    private static UserEntity SeedUser(DatabaseContext context, string email, bool isAdmin = false)
    {
        var user = new UserEntity
        {
            Name = "Test",
            Surname = "User",
            Email = email,
            PasswordHash = "hashed",
            IsAdmin = isAdmin,
            IsEnabled = true
        };

        context.Users.Add(user);
        context.SaveChanges();
        return user;
    }

    // ---------- CreateUserCommandHandler ----------

    [Fact]
    public async Task Create_Handle_ShouldCreateUser_WithHashedPassword()
    {
        using var context = InMemoryDbContextFactory.Create();
        var handler = new CreateUserCommandHandler(context, new PasswordHasher<UserEntity>());
        var command = new CreateUserCommand
        {
            Name = "Ana",
            Surname = "Anić",
            Email = "ana@example.com",
            Password = "Sifra123!",
            Gender = Gender.Female
        };

        var result = await handler.Handle(command, CancellationToken.None);

        var user = await context.Users.FindAsync(result.UserId);
        Assert.NotNull(user);
        Assert.Equal("ana@example.com", user!.Email);
        Assert.True(user.IsEnabled);
        Assert.NotEqual(command.Password, user.PasswordHash);
    }

    [Fact]
    public async Task Create_Handle_ShouldThrowConflict_WhenEmailAlreadyExists()
    {
        using var context = InMemoryDbContextFactory.Create();
        SeedUser(context, "postoji@example.com");
        var handler = new CreateUserCommandHandler(context, new PasswordHasher<UserEntity>());
        var command = new CreateUserCommand
        {
            Name = "Marko",
            Surname = "Marković",
            Email = "postoji@example.com",
            Password = "Sifra123!",
            Gender = Gender.Male
        };

        await Assert.ThrowsAsync<WebShopConflictException>(
            () => handler.Handle(command, CancellationToken.None));
    }

    // ---------- UpdateUserCommandHandler ----------

    [Fact]
    public async Task Update_Handle_ShouldUpdateUser_WhenOwnerUpdatesOwnAccount()
    {
        using var context = InMemoryDbContextFactory.Create();
        var user = SeedUser(context, "vlasnik@example.com");
        var currentUser = new FakeCurrentUser { UserId = user.Id, IsAdmin = false };
        var handler = new UpdateUserCommandHandler(context, currentUser);
        var command = new UpdateUserCommand
        {
            Id = user.Id,
            Name = "Changed",
            Surname = user.Surname,
            Email = user.Email,
            Gender = user.Gender
        };

        await handler.Handle(command, CancellationToken.None);

        var updated = await context.Users.FindAsync(user.Id);
        Assert.Equal("Changed", updated!.Name);
    }

    [Fact]
    public async Task Update_Handle_ShouldThrowBusinessRule_WhenNonAdminUpdatesAnotherUser()
    {
        using var context = InMemoryDbContextFactory.Create();
        var owner = SeedUser(context, "vlasnik2@example.com");
        var currentUser = new FakeCurrentUser { UserId = owner.Id + 999, IsAdmin = false };
        var handler = new UpdateUserCommandHandler(context, currentUser);
        var command = new UpdateUserCommand
        {
            Id = owner.Id,
            Name = "Hacked",
            Surname = owner.Surname,
            Email = owner.Email,
            Gender = owner.Gender
        };

        await Assert.ThrowsAsync<WebShopBusinessRuleException>(
            () => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Update_Handle_ShouldThrowBusinessRule_WhenEmailAlreadyUsedByAnotherUser()
    {
        using var context = InMemoryDbContextFactory.Create();
        var user1 = SeedUser(context, "prvi@example.com");
        var user2 = SeedUser(context, "drugi@example.com");
        var currentUser = new FakeCurrentUser { UserId = user1.Id, IsAdmin = false };
        var handler = new UpdateUserCommandHandler(context, currentUser);
        var command = new UpdateUserCommand
        {
            Id = user1.Id,
            Name = user1.Name,
            Surname = user1.Surname,
            Email = user2.Email,
            Gender = user1.Gender
        };

        await Assert.ThrowsAsync<WebShopBusinessRuleException>(
            () => handler.Handle(command, CancellationToken.None));
    }

    // ---------- DeleteUserCommandHandler ----------

    [Fact]
    public async Task Delete_Handle_ShouldDeleteUser_WhenAdminDeletesAnyAccount()
    {
        using var context = InMemoryDbContextFactory.Create();
        var user = SeedUser(context, "zabrisati@example.com");
        var admin = new FakeCurrentUser { UserId = user.Id + 999, IsAdmin = true };
        var handler = new DeleteUserCommandHandler(context, admin);

        await handler.Handle(new DeleteUserCommand { Id = user.Id }, CancellationToken.None);

        // DatabaseContext does a soft delete (SaveChanges turns Remove into IsDeleted = true),
        // so the row stays in the database, only marked as deleted.
        var deleted = await context.Users.FindAsync(user.Id);
        Assert.NotNull(deleted);
        Assert.True(deleted!.IsDeleted);
    }

    [Fact]
    public async Task Delete_Handle_ShouldThrowBusinessRule_WhenNonAdminDeletesAnotherUser()
    {
        using var context = InMemoryDbContextFactory.Create();
        var user = SeedUser(context, "tudji@example.com");
        var currentUser = new FakeCurrentUser { UserId = user.Id + 999, IsAdmin = false };
        var handler = new DeleteUserCommandHandler(context, currentUser);

        await Assert.ThrowsAsync<WebShopBusinessRuleException>(
            () => handler.Handle(new DeleteUserCommand { Id = user.Id }, CancellationToken.None));
    }

    // ---------- GetUserByIdQueryHandler / ListUserQueryHandler (private data) ----------

    [Fact]
    public async Task GetById_Handle_ShouldReturnProfile_WhenUserReadsOwnProfile()
    {
        using var context = InMemoryDbContextFactory.Create();
        var user = SeedUser(context, "ja@example.com");
        var handler = new GetUserByIdQueryHandler(context, new FakeCurrentUser { IsAuthenticated = true, UserId = user.Id });

        var result = await handler.Handle(new GetUserByIdQuery { Id = user.Id }, CancellationToken.None);

        Assert.Equal("ja@example.com", result.Email);
    }

    [Fact]
    public async Task GetById_Handle_ShouldThrowBusinessRule_WhenReadingAnotherUsersProfile()
    {
        using var context = InMemoryDbContextFactory.Create();
        var user = SeedUser(context, "neko@example.com");
        var handler = new GetUserByIdQueryHandler(context, new FakeCurrentUser { IsAuthenticated = true, UserId = user.Id + 1 });

        await Assert.ThrowsAsync<WebShopBusinessRuleException>(
            () => handler.Handle(new GetUserByIdQuery { Id = user.Id }, CancellationToken.None));
    }

    [Fact]
    public async Task List_Handle_ShouldThrowBusinessRule_WhenNotAdmin()
    {
        using var context = InMemoryDbContextFactory.Create();
        var handler = new ListUserQueryHandler(context, new FakeCurrentUser { IsAuthenticated = true, UserId = 1 });

        await Assert.ThrowsAsync<WebShopBusinessRuleException>(
            () => handler.Handle(new ListUserQuery(), CancellationToken.None));
    }

    [Fact]
    public async Task List_Handle_ShouldFilterBySearch_WhenAdmin()
    {
        using var context = InMemoryDbContextFactory.Create();
        SeedUser(context, "amra@example.com");
        SeedUser(context, "kenan@example.com");
        var handler = new ListUserQueryHandler(context, new FakeCurrentUser { IsAuthenticated = true, UserId = 1, IsAdmin = true });

        var result = await handler.Handle(new ListUserQuery { Search = "kenan" }, CancellationToken.None);

        Assert.Equal(1, result.Total);
        Assert.Equal("kenan@example.com", result.Items[0].Email);
    }
}
