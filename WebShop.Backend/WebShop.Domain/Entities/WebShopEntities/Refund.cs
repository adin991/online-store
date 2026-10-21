using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebShop.Domain.Common;
using WebShop.Domain.Entities.Identity;

namespace WebShop.Domain.Entities.WebShopEntities
{
    public enum RefundStatusType
    {
        Refunded,
        AwaitingFurtherInfo,
        Canceled,
        Processing
    }
    public class Refund : BaseEntity
    {
        public required int DeliveryId { get; set; }
        public Delivery Delivery { get; set; } = null!;
        public required int UserId { get; set; }
        public UserEntity User { get; set; } = null!;
        public required int Amount { get; set; }
        public required string Reason { get; set; } = string.Empty;
        public required DateTime DateOfRequest { get; set; }
        public required DateTime? DateOfRefund { get; set; }
        public required RefundStatusType RefundStatus { get; set; }
        public required int PaymentInfoId { get; set; }
        public PaymentInfo PaymentInfo { get; set; } = null!;

    }
}
