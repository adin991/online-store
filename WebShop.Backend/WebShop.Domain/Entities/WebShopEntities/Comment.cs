using WebShop.Domain.Common;
using WebShop.Domain.Entities.Identity;

namespace WebShop.Domain.Entities.WebShopEntities
{
    public class Comment : BaseEntity
    {
        public required string Content { get; set; } = string.Empty;
        public required int UserId { get; set; }
        public UserEntity User { get; set; } = null!;
        public required int AdId { get; set; }
        public Ad Ad { get; set; } = null!;

        public static class Constraints
        {
            public const int ContentMaxLength = 1000;
        }
    }
}
