using System.Text.Json;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using WebShop.Domain.Entities.WebShopEntities;

namespace WebShop.Infrastructure.Database.Configurations.Catelog;

public class AdConfiguration : IEntityTypeConfiguration<Ad>
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    // Compares string lists by content so EF detects in-place changes to the JSON columns.
    private static readonly ValueComparer<List<string>?> ListComparer = new(
        (a, b) => (a ?? new List<string>()).SequenceEqual(b ?? new List<string>()),
        v => (v ?? new List<string>()).Aggregate(0, (hash, s) => HashCode.Combine(hash, s.GetHashCode())),
        v => v == null ? null : v.ToList());

    public void Configure(EntityTypeBuilder<Ad> builder)
    {
        MapStringListAsJson(builder, x => x.Images);
        MapStringListAsJson(builder, x => x.Colors);
        MapStringListAsJson(builder, x => x.Sizes);
    }

    private static void MapStringListAsJson(
        EntityTypeBuilder<Ad> builder,
        System.Linq.Expressions.Expression<Func<Ad, List<string>?>> property)
    {
        builder.Property(property)
            .HasConversion(
                v => JsonSerializer.Serialize(v, JsonOptions),
                // null or empty column -> empty list
                v => string.IsNullOrWhiteSpace(v)
                    ? new List<string>()
                    : JsonSerializer.Deserialize<List<string>>(v, JsonOptions)!,
                ListComparer);
    }
}
