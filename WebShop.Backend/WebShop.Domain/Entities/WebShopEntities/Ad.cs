using WebShop.Domain.Common;
using WebShop.Domain.Entities.Catalog;
using WebShop.Domain.Entities.Identity;

namespace WebShop.Domain.Entities.WebShopEntities
{
    public class Ad : BaseEntity
    {
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required decimal Price { get; set; }
        public required ProductCategoryEntity CategoryEntity { get; set; }
        public required int UserId { get; set; }
        public UserEntity User { get; set; } = null!;
        public List<string>? Images { get; set; } = new();
        public string? Location { get; set; }

        // Product options the buyer can choose from (e.g. "Red", "Blue" / "S", "M", "L").
        public List<string>? Colors { get; set; } = new();
        public List<string>? Sizes { get; set; } = new();

        public List<Comment> Comments { get; set; } = new();
        public List<SCItem> SCItems { get; set; } = new();
        public List<AdStatus> AdStatuses { get; set; } = new();

        public static class Constraints
        {
            public const int TitleMaxLength = 100;
            public const int DescriptionMaxLength = 2000;
            public const int LocationMaxLength = 100;
            public const decimal PriceMax = 1_000_000m;
            public const int MaxImages = 10;
            public const int ImageUrlMaxLength = 500;
            public const int MaxOptions = 10;
            public const int OptionMaxLength = 30;
        }
    }
}
