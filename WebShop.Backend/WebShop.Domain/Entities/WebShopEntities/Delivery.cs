using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebShop.Domain.Common;

namespace WebShop.Domain.Entities.WebShopEntities
{
    public class Delivery : BaseEntity
    {
        public required DateTime DateOfOrder { get; set; }
        public DateTime? DateOfPickUp { get; set; }
        public DateTime? DateOfDelivery { get; set; }
        public required int ShoppingCartId { get; set; }
        public ShoppingCart ShoppingCart { get; set; } = null!;
        public required int PaymentInfoId { get; set; }
        public PaymentInfo PaymentInfo { get; set; } = null!;
        public required int DeliveryCompanyId { get; set; }
        public DeliveryCompany DeliveryCompany { get; set; } = null!;
        public List<DeliveryStatus> StatusHistory { get; set; } = new();
    }
}
