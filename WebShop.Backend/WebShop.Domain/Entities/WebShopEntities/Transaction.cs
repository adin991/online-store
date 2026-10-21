using WebShop.Domain.Common;
using WebShop.Domain.Entities.Identity;

namespace WebShop.Domain.Entities.WebShopEntities
{
    public enum TransactionStatus
    {
        Pending,
        Succeeded,
        Failed,
        Canceled
    }

    public class Transaction : BaseEntity
    {
        public required int AdId { get; set; }
        public Ad Ad { get; set; } = null!;
        public required int UserId { get; set; }
        public UserEntity User { get; set; } = null!;
        public required decimal AmountEur { get; set; }
        public required decimal AmountOriginalKm { get; set; }
        public required string StripePaymentIntentId { get; set; }
        public required TransactionStatus Status { get; set; }
        public DateTime? PaidAtUtc { get; set; }
    }
}
