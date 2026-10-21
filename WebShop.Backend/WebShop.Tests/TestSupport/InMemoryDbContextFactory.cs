using Microsoft.Extensions.Time.Testing;

namespace WebShop.Tests.TestSupport;

/// <summary>
/// Creates a new, isolated EF Core InMemory database for every test (same approach as
/// ProductCategoryUnitTests), so tests do not share state.
/// </summary>
internal static class InMemoryDbContextFactory
{
    public static DatabaseContext Create(FakeTimeProvider? clock = null)
    {
        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new DatabaseContext(options, clock ?? new FakeTimeProvider());
    }
}
