using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebShop.Domain.Common;

namespace WebShop.Domain.Entities.WebShopEntities
{
    public enum DeliveryStatusType
    {
        Delivered,
        Refunded,
        AwaitingFurtherInfo,
        InTransit,
        Canceled
    }
    public class DeliveryStatus : BaseEntity
    {

        public required int DeliveryId { get; set; }
        public Delivery Delivery { get; set; } = null!;
        public required DeliveryStatusType Stauts { get; set; }

    }
}
