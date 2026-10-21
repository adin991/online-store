using WebShop.Domain.Entities.WebShopEntities;

namespace WebShop.Infrastructure.Database.Seeders;

/// <summary>
/// Dynamic seeder that runs at runtime, on application startup (see DatabaseInitializer).
/// Used for demo/test data that is not part of the migrations.
///
/// Documented test accounts (keep in sync with the project documentation):
///   Admin: admin@WebShop.local / Admin123!
///   Basic: user1@gmail.com     / User
/// </summary>
public static class DynamicDataSeeder
{
    public static async Task SeedAsync(DatabaseContext context)
    {
        // Make sure the database exists (without migrations)
        await context.Database.EnsureCreatedAsync();

        await SeedProductCategoriesAsync(context);
        await SeedUsersAsync(context);
    }

    private static async Task SeedProductCategoriesAsync(DatabaseContext context)
    {
        if (!await context.ProductCategories.AnyAsync())
        {
            context.ProductCategories.AddRange(
                new ProductCategoryEntity
                {
                    Name = "Electronics",
                    IsEnabled = true,
                    CreatedAtUtc = DateTime.UtcNow
                },
                new ProductCategoryEntity
                {
                    Name = "Fashion",
                    IsEnabled = true,
                    CreatedAtUtc = DateTime.UtcNow
                }
            );

            await context.SaveChangesAsync();
            Console.WriteLine("✅ Dynamic seed: product categories added.");
        }
    }

    /// <summary>
    /// Creates the roles and the documented demo users. Each account is added only if
    /// it does not exist yet, so an existing database also gets the documented accounts.
    /// </summary>
    private static async Task SeedUsersAsync(DatabaseContext context)
    {
        var basicRole = await EnsureRoleAsync(context, "Basic");
        var adminRole = await EnsureRoleAsync(context, "Admin");

        var hasher = new PasswordHasher<UserEntity>();
        var added = 0;

        added += await EnsureUserAsync(context, hasher, new UserEntity
        {
            Name = "Admin",
            Surname = "Admin",
            Email = "admin@WebShop.local",
            RoleId = adminRole.Id,
            IsAdmin = true,
            IsBasic = false,
            IsEnabled = true,
        }, "Admin123!");

        added += await EnsureUserAsync(context, hasher, new UserEntity
        {
            Name = "User",
            Surname = "One",
            Email = "user1@gmail.com",
            RoleId = basicRole.Id,
            IsBasic = true,
            IsEnabled = true,
        }, "User");

        if (added > 0)
        {
            await context.SaveChangesAsync();
            Console.WriteLine($"✅ Dynamic seed: {added} demo user(s) added.");
        }
    }

    private static async Task<Role> EnsureRoleAsync(DatabaseContext context, string name)
    {
        var role = await context.Role.FirstOrDefaultAsync(r => r.Name == name);
        if (role is not null)
            return role;

        role = new Role { Name = name, Description = "" };
        context.Role.Add(role);
        await context.SaveChangesAsync();
        return role;
    }

    private static async Task<int> EnsureUserAsync(
        DatabaseContext context,
        PasswordHasher<UserEntity> hasher,
        UserEntity user,
        string password)
    {
        if (await context.Users.AnyAsync(u => u.Email == user.Email))
            return 0;

        user.PasswordHash = hasher.HashPassword(user, password);
        context.Users.Add(user);
        return 1;
    }
}
